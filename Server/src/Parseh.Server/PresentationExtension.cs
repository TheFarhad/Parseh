namespace Parseh.Server;

using System.Text;
using System.Text.Json.Serialization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Infra.Persistence.EF.Command;
using Framework.Presentation;

internal static class PresentationExtension
{
    internal static IServiceCollection PresentationRegistery(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .FrameworkEndpointRegistery(configuration, false)
            .JwtConfigurations(configuration)
            .AddOpenApi()
            .AddEndpoints(typeof(PresentationExtension).Assembly)
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

    private static IServiceCollection JwtConfigurations(this IServiceCollection services, IConfiguration configuration)
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
