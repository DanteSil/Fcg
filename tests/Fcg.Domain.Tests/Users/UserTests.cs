using FluentAssertions;
using Fcg.Domain.Common;
using Fcg.Domain.Users;
using Fcg.Domain.Users.Events;

namespace Fcg.Domain.Tests.Users;

public class UserTests
{
    [Fact]
    public void Register_Should_Create_User_And_Raise_Event()
    {
        var email = Email.Create("aluno@fiap.com.br");

        var user = User.Register("Aluno FIAP", email, "hash");

        user.Name.Should().Be("Aluno FIAP");
        user.Email.Value.Should().Be("aluno@fiap.com.br");
        user.Role.Should().Be(UserRole.User);
        user.DomainEvents.Should().ContainSingle(e => e is UserRegistered);
    }

    [Fact]
    public void Register_Should_Reject_Empty_Name()
    {
        var email = Email.Create("aluno@fiap.com.br");

        var act = () => User.Register("  ", email, "hash");

        act.Should().Throw<DomainException>();
    }
}
