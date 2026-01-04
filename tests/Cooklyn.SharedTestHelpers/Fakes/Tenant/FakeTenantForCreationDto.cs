namespace Cooklyn.SharedTestHelpers.Fakes.Tenant;

using AutoBogus;
using Cooklyn.Server.Domain.Tenants.Dtos;

public sealed class FakeTenantForCreationDto : AutoFaker<TenantForCreationDto>
{
    public FakeTenantForCreationDto()
    {
        RuleFor(x => x.Name, f => f.Company.CompanyName());
    }
}
