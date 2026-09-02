using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Fcg.Application.Games;
using Fcg.Application.Library;
using Fcg.Application.Promotions;
using Fcg.Application.Users;

namespace Fcg.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<RegisterUserRequestValidator>();
        services.AddScoped<AuthService>();
        services.AddScoped<UserService>();
        services.AddScoped<GameService>();
        services.AddScoped<LibraryService>();
        services.AddScoped<PromotionService>();
        return services;
    }
}
