using FluentAssertions;
using Fcg.Domain.Common;
using Fcg.Domain.Users;

namespace Fcg.Domain.Tests.Users;

public class PasswordPolicyTests
{
    [Theory]
    [InlineData("Senha@123")]
    [InlineData("Abcdef1!")]
    [InlineData("P@ssw0rd")]
    public void EnsureValid_Should_Accept_Secure_Password(string password)
    {
        var act = () => PasswordPolicy.EnsureValid(password);

        act.Should().NotThrow();
    }

    [Theory]
    [InlineData("")]
    [InlineData("curta1!")]
    [InlineData("semnumero!")]
    [InlineData("SemEspecial1")]
    [InlineData("12345678!")]
    public void EnsureValid_Should_Reject_Weak_Password(string password)
    {
        var act = () => PasswordPolicy.EnsureValid(password);

        act.Should().Throw<DomainException>();
    }
}
