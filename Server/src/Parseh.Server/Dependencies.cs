namespace Parseh.Server;

using Carter;
using Infra.Persistence.EF.Command;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

internal static class Dependencies
{
    internal static IServiceCollection EndpointDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.FrameworkEndpointDependencies(configuration, false);

        services
            .JwtConfigurations(configuration)
            .AddOpenApi()
            .AddCarter()
            .AddCors(_ =>
            {
                // TODO: فقط برای کلاینت ست شود
                _.AddPolicy("WindowsApp", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });

            });

        return services;
    }

    static IServiceCollection JwtConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            var jwtOptions = configuration.GetSection("JwtOptions").Get<JwtOption>()!;

            SecurityKey key;

            // public key only for validating token
            var rsa = RsaKeyProvider.LoadPublicKey("Security/Keys/Public/public.key");
            if (rsa is { })
            {
                key = new RsaSecurityKey(rsa);
            }
            else
            {
                var secret = Encoding.UTF8.GetBytes(jwtOptions.SecretKey);
                key = new SymmetricSecurityKey(secret);
            }

            options.SaveToken = true; // to access by HttpContext
            options.RequireHttpsMetadata = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,

                ValidIssuer = jwtOptions.Issuer,
                ValidateIssuer = true,

                ValidAudience = jwtOptions.Audience,
                ValidateAudience = true,

                IssuerSigningKey = key,
                ValidateIssuerSigningKey = true
            };
        });
        // TODO: define policies
        services.AddAuthorization();

        return services;
    }
}
