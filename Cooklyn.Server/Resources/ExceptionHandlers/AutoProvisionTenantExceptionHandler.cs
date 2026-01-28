namespace Cooklyn.Server.Resources.ExceptionHandlers;

using System.Security.Claims;
using Databases;
using Domain.Tenants;
using Domain.Tenants.Models;
using Domain.Users;
using Domain.Users.Models;
using Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services;

/// <summary>
/// Exception handler that auto-provisions a tenant and user when an authenticated user
/// doesn't have a corresponding record in the database.
/// </summary>
public sealed class AutoProvisionTenantExceptionHandler(
    IServiceScopeFactory scopeFactory,
    ILogger<AutoProvisionTenantExceptionHandler> logger) : IExceptionHandler
{
    private const string UnableToDetermineTenantMessage = "Unable to determine tenant";

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        // Only handle ValidationException with "Unable to determine tenant" message
        if (exception is not ValidationException validationException ||
            !validationException.Message.Contains(UnableToDetermineTenantMessage, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var user = httpContext.User;
        var isAuthenticated = user.Identity?.IsAuthenticated ?? false;
        var userIdentifier = user.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? user.FindFirstValue("sub")
            ?? user.FindFirstValue("preferred_username"); // Keycloak fallback

        // Must have an authenticated user with an identifier
        if (!isAuthenticated || string.IsNullOrEmpty(userIdentifier))
        {
            // Log all claims for debugging
            var allClaims = user.Claims.Select(c => $"{c.Type}={c.Value}").ToList();
            logger.LogWarning(
                "Cannot auto-provision tenant: IsAuthenticated={IsAuthenticated}, UserIdentifier={UserIdentifier}, Claims=[{Claims}]",
                isAuthenticated,
                userIdentifier ?? "(null)",
                string.Join(", ", allClaims));
            return false;
        }

        // Create a new scope to get a fresh DbContext and TenantIdProvider
        await using var scope = scopeFactory.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var tenantIdProvider = scope.ServiceProvider.GetRequiredService<ITenantIdProvider>();

        // Double-check that the user doesn't already exist (race condition protection)
        var existingUser = await dbContext.Users
            .IgnoreQueryFilters([QueryFilterNames.Tenant])
            .FirstOrDefaultAsync(u => u.Identifier == userIdentifier, cancellationToken);

        if (existingUser != null)
        {
            logger.LogWarning(
                "User {UserIdentifier} already exists but tenant lookup failed - invalidating cache",
                userIdentifier);

            await tenantIdProvider.InvalidateCacheAsync(userIdentifier, cancellationToken);

            // Return a retry response
            httpContext.Response.StatusCode = StatusCodes.Status409Conflict;
            await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Please retry your request",
                Detail = "Your account was recently provisioned. Please retry your request.",
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.10"
            }, cancellationToken);

            return true;
        }

        // Extract user info from claims (check both mapped and unmapped claim types)
        var claimFirstName = user.FindFirstValue(ClaimTypes.GivenName) ?? user.FindFirstValue("given_name");
        var claimLastName = user.FindFirstValue(ClaimTypes.Surname) ?? user.FindFirstValue("family_name");
        var claimName = user.Identity?.Name ?? user.FindFirstValue("name") ?? user.FindFirstValue(ClaimTypes.Name);
        var claimEmail = user.FindFirstValue(ClaimTypes.Email)
            ?? user.FindFirstValue("email")
            ?? user.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");
        var claimUsername = user.FindFirstValue("preferred_username") ?? user.FindFirstValue("username");

        var firstName = claimFirstName ?? claimName?.Split(' ').FirstOrDefault() ?? "User";
        var lastName = claimLastName ?? claimName?.Split(' ').LastOrDefault() ?? userIdentifier;
        var email = claimEmail ?? $"{userIdentifier}@unknown.local";
        var username = claimUsername ?? claimName ?? userIdentifier;

        // Determine role - default to "User" unless they have an admin role claim
        var hasAdminRole = user.IsInRole("admin") ||
                           user.Claims.Any(c => c.Type is "role" or "roles" or ClaimTypes.Role &&
                                                c.Value.Equals("admin", StringComparison.OrdinalIgnoreCase));
        var role = hasAdminRole ? "Admin" : "User";

        // Generate tenant name from user info
        var tenantName = $"{firstName} {lastName}'s Workspace";

        logger.LogInformation(
            "Auto-provisioning tenant and user for {UserIdentifier} ({Email})",
            userIdentifier,
            email);

        try
        {
            // Create tenant
            var tenantForCreation = new TenantForCreation { Name = tenantName };
            var tenant = Tenant.Create(tenantForCreation);
            await dbContext.Tenants.AddAsync(tenant, cancellationToken);

            // Create user with tenant reference
            var userForCreation = new UserForCreation
            {
                TenantId = tenant.Id,
                FirstName = firstName,
                LastName = lastName,
                Identifier = userIdentifier,
                Email = email,
                Username = username,
                Role = role
            };
            var newUser = User.Create(userForCreation);
            await dbContext.Users.AddAsync(newUser, cancellationToken);

            // Save both in a single transaction
            await dbContext.SaveChangesAsync(cancellationToken);

            logger.LogInformation(
                "Successfully provisioned tenant {TenantId} and user {UserId} for {UserIdentifier}",
                tenant.Id,
                newUser.Id,
                userIdentifier);

            // Invalidate the tenant cache so subsequent requests get the new tenant ID
            await tenantIdProvider.InvalidateCacheAsync(userIdentifier, cancellationToken);

            // Return a response indicating the user was provisioned and should retry
            httpContext.Response.StatusCode = StatusCodes.Status409Conflict;
            await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Account provisioned",
                Detail = "Your account has been automatically provisioned. Please retry your request.",
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.10",
                Extensions = { ["tenantId"] = tenant.Id, ["userId"] = newUser.Id }
            }, cancellationToken);

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Failed to auto-provision tenant and user for {UserIdentifier}",
                userIdentifier);

            // Let the default exception handler deal with this
            return false;
        }
    }
}
