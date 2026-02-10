namespace Cooklyn.SharedTestHelpers.Fakes.User;

using AutoBogus;
using Cooklyn.Server.Domain.Users;
using Cooklyn.Server.Domain.Users.Dtos;

public sealed class FakeUserForCreationDto : AutoFaker<UserForCreationDto>
{
    public FakeUserForCreationDto()
    {
        RuleFor(x => x.TenantId, (_, _) => TestContext.DefaultTenantId);
        RuleFor(x => x.FirstName, f => f.Person.FirstName);
        RuleFor(x => x.LastName, f => f.Person.LastName);
        RuleFor(x => x.Identifier, f => f.Random.Guid().ToString());
        RuleFor(x => x.Email, f => f.Internet.Email());
        RuleFor(x => x.Username, f => f.Internet.UserName());
        RuleFor(x => x.Role, f => f.PickRandom(UserRole.ListNames()));
    }
}
