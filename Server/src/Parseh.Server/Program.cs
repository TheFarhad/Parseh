using System.Text;
using Carter;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Parseh.Server.Core.Domain.Aggregates.User.Entity;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.Configure<JwtOption>(builder.Configuration.GetSection("JwtOptions"));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true; // to access by HttpContext
    options.RequireHttpsMetadata = true;

    var rsaPublic = RsaKeyProvider.LoadPublicKey("Endpoint/Keys/Public/public.key");

    options.TokenValidationParameters = new TokenValidationParameters
    {


        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,

        ValidIssuer = builder.Configuration["JwtOptions:Issuer"],
        ValidateIssuer = true,

        ValidAudience = builder.Configuration["JwtOptions:Audience"],
        ValidateAudience = true,

        IssuerSigningKey = new RsaSecurityKey(rsaPublic),
        ValidateIssuerSigningKey = true
    };

});

builder.Services.AddCors(options =>
{
    options.AddPolicy("WindowsApp", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });

});
builder.Services.AddCarter();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseCors("WindowsApp");

app.MapCarter();

app.Run();
