namespace Cooklyn.Server.Exceptions;

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

public sealed class DomainExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<DomainExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (status, title, detail, property) = exception switch
        {
            ValidationException ve => (StatusCodes.Status400BadRequest, "Validation failed", ve.Message, ve.Property),
            NotFoundException nfe => (StatusCodes.Status404NotFound, "Resource not found", nfe.Message, string.Empty),
            _ => (0, string.Empty, string.Empty, string.Empty)
        };

        if (status == 0)
            return false;

        logger.LogWarning(exception,
            "Handled {ExceptionType} for {Path}: {Message}",
            exception.GetType().Name,
            httpContext.Request.Path,
            exception.Message);

        httpContext.Response.StatusCode = status;

        var problemDetails = new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = detail,
            Type = $"https://httpstatuses.io/{status}",
            Instance = httpContext.Request.Path
        };

        if (!string.IsNullOrEmpty(property))
        {
            problemDetails.Extensions["property"] = property;
        }

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = problemDetails
        });
    }
}
