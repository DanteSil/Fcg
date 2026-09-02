using FluentAssertions;
using Fcg.Domain.Common;
using Fcg.Domain.Users;

namespace Fcg.Domain.Tests.Users;

public class EmailTests
{
    [Theory]
    [InlineData("aluno@fiap.com.br")]
    [InlineData("User.Name+tag@Alura.COM")]
    public void Create_Should_Accept_Valid_Email(string value)
    {
        var email = Email.Create(value);

        email.Value.Should().Be(value.Trim().ToLowerInvariant());
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("sem-arroba")]
    [InlineData("a@")]
    [InlineData("@fiap.com")]
    public void Create_Should_Reject_Invalid_Email(string value)
    {
        var act = () => Email.Create(value);

        act.Should().Throw<DomainException>();
    }
}
