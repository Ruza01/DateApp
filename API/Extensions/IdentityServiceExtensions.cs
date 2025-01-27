using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace API.Extensions;

public static class IdentityServiceExtensions
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>                                             //Bearer Token se koristi za slanje JWT tokena sa svakim zahtevom kako bi server mogao da validira korisnika.Format Bearer <token>
        {
            var tokenKey = config["TokenKey"] ?? throw new Exception("TokenKey not found");
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey)),
                ValidateIssuer = false,
                ValidateAudience = false
            };
        });

        return services;
    }
}
