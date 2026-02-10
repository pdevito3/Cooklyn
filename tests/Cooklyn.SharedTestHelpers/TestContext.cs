namespace Cooklyn.SharedTestHelpers;

/// <summary>
/// Provides shared test context that fakers can access to use real database entities.
/// Integration tests should override DefaultTenantId after creating a real tenant.
/// </summary>
public static class TestContext
{
    /// <summary>
    /// The default tenant ID to use for entities that require a tenant.
    /// Initialized with a placeholder GUID for unit tests.
    /// Integration tests override this after creating a real tenant in the database.
    /// </summary>
    public static string DefaultTenantId { get; set; } = "tenant_11111111111111111111111111111111";
}
