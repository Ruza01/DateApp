using System;
using API.Data;
using API.Services;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddControllers();
        services.AddDbContext<DataContext>(opt => 
        {
            opt.UseSqlite(config.GetConnectionString("DefaultConnection"));
        });
        services.AddCors();
        services.AddScoped<ITokenService, TokenService>(); //ovo je dependency injection, on sam raspolaze kada i koliko puta ce injectovati ovaj servis u kontroler

        return services;
    }
}
