using FluentAssertions;
using NSubstitute;
using Fcg.Application.Abstractions;
using Fcg.Application.Common;
using Fcg.Application.Games;
using Fcg.Application.Library;
using Fcg.Application.Users;
using Fcg.Domain.Games;
using Fcg.Domain.Interfaces;
using Fcg.Domain.Library;
using Fcg.Domain.Users;

namespace Fcg.Application.Tests;

public class AuthServiceTests
{
    private readonly IUserRepository _users = Substitute.For<IUserRepository>();
    private readonly IUnitOfWork _uow = Substitute.For<IUnitOfWork>();
    private readonly IPasswordHasher _hasher = Substitute.For<IPasswordHasher>();
    private readonly IJwtTokenService _jwt = Substitute.For<IJwtTokenService>();

    private AuthService CreateSut() => new(_users, _uow, _hasher, _jwt);

    [Fact]
    public async Task Register_Should_Fail_When_Email_Already_Exists()
    {
        _users.EmailExistsAsync("aluno@fiap.com.br", Arg.Any<CancellationToken>()).Returns(true);
        var sut = CreateSut();

        var act = () => sut.RegisterAsync(new RegisterUserRequest("Aluno", "aluno@fiap.com.br", "Senha@123"));

        await act.Should().ThrowAsync<ConflictException>();
    }

    [Fact]
    public async Task Register_Should_Create_User_When_Valid()
    {
        _users.EmailExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(false);
        _hasher.Hash(Arg.Any<string>()).Returns("hash");
        _jwt.GenerateToken(Arg.Any<User>()).Returns("token");
        var sut = CreateSut();

        var result = await sut.RegisterAsync(new RegisterUserRequest("Aluno", "aluno@fiap.com.br", "Senha@123"));

        result.Token.Should().Be("token");
        result.User.Email.Should().Be("aluno@fiap.com.br");
        await _users.Received(1).AddAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
        await _uow.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Login_Should_Fail_With_Wrong_Password()
    {
        var user = User.Register("Aluno", Email.Create("aluno@fiap.com.br"), "hash");
        _users.GetByEmailAsync("aluno@fiap.com.br", Arg.Any<CancellationToken>()).Returns(user);
        _hasher.Verify("errada", "hash").Returns(false);
        var sut = CreateSut();

        var act = () => sut.LoginAsync(new LoginRequest("aluno@fiap.com.br", "errada"));

        await act.Should().ThrowAsync<UnauthorizedAppException>();
    }
}

public class GameServiceTests
{
    private readonly IGameRepository _games = Substitute.For<IGameRepository>();
    private readonly IUnitOfWork _uow = Substitute.For<IUnitOfWork>();

    [Fact]
    public async Task Create_Should_Fail_When_Actor_Is_Not_Admin()
    {
        var sut = new GameService(_games, _uow);

        var act = () => sut.CreateAsync(new CreateGameRequest("Jogo", "Desc", 10), UserRole.User);

        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task Create_Should_Persist_When_Actor_Is_Admin()
    {
        var sut = new GameService(_games, _uow);

        var result = await sut.CreateAsync(new CreateGameRequest("Jogo", "Desc", 10), UserRole.Admin);

        result.Title.Should().Be("Jogo");
        await _games.Received(1).AddAsync(Arg.Any<Game>(), Arg.Any<CancellationToken>());
        await _uow.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}

public class LibraryServiceTests
{
    private readonly ILibraryRepository _library = Substitute.For<ILibraryRepository>();
    private readonly IGameRepository _games = Substitute.For<IGameRepository>();
    private readonly IUnitOfWork _uow = Substitute.For<IUnitOfWork>();

    [Fact]
    public async Task Acquire_Should_Fail_When_Game_Already_Owned()
    {
        var userId = Guid.NewGuid();
        var game = Game.Create("Jogo", "Desc", 29.9m);
        _games.GetByIdAsync(game.Id, Arg.Any<CancellationToken>()).Returns(game);
        _library.ExistsAsync(userId, game.Id, Arg.Any<CancellationToken>()).Returns(true);
        var sut = new LibraryService(_library, _games, _uow);

        var act = () => sut.AcquireAsync(userId, game.Id, UserRole.User);

        await act.Should().ThrowAsync<ConflictException>();
    }

    [Fact]
    public async Task Acquire_Should_Add_Item_For_User()
    {
        var userId = Guid.NewGuid();
        var game = Game.Create("Jogo", "Desc", 29.9m);
        _games.GetByIdAsync(game.Id, Arg.Any<CancellationToken>()).Returns(game);
        _library.ExistsAsync(userId, game.Id, Arg.Any<CancellationToken>()).Returns(false);
        var sut = new LibraryService(_library, _games, _uow);

        var result = await sut.AcquireAsync(userId, game.Id, UserRole.User);

        result.GameId.Should().Be(game.Id);
        result.Title.Should().Be("Jogo");
        await _library.Received(1).AddAsync(Arg.Any<LibraryItem>(), Arg.Any<CancellationToken>());
    }
}
