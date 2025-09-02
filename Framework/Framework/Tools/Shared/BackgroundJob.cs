using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;

namespace Framework;

public abstract class BackgroundJob : BackgroundService { }

public interface IHostedLifespanService : IHostedLifecycleService
{
    async Task IHostedLifecycleService.StartingAsync(CancellationToken token)
         => await Task.CompletedTask;

    async Task IHostedService.StartAsync(CancellationToken token)
       => await Task.CompletedTask;

    async Task IHostedLifecycleService.StartedAsync(CancellationToken token)
         => await Task.CompletedTask;

    async Task IHostedLifecycleService.StoppingAsync(CancellationToken token)
        => await Task.CompletedTask;

    async Task IHostedService.StopAsync(CancellationToken token)
      => await Task.CompletedTask;

    async Task IHostedLifecycleService.StoppedAsync(CancellationToken token)
         => await Task.CompletedTask;
}

public static class Registerer
{
    public static IServiceCollection Dependencies(this IServiceCollection services, Assembly assembly, Type assignableTo, ServiceLifetime lifetime)
         => services
            .Dependencies([assembly], [assignableTo], lifetime);

    public static IServiceCollection Dependencies(this IServiceCollection services, List<Assembly> assemblies, List<Type> assignableTo, ServiceLifetime lifetime)
         => services
            .ScrutorRegisterer(assemblies, assignableTo, lifetime);

    private static IServiceCollection ScrutorRegisterer(this IServiceCollection source, List<Assembly> assemblies, List<Type> assignableTo, ServiceLifetime lifeTime)
        => source
             .Scan(typeSelector =>
                 typeSelector.FromAssemblies(assemblies)
                 .AddClasses(typeFilter => typeFilter.AssignableToAny(assignableTo), publicOnly: false)
                 .AsImplementedInterfaces()
                 .WithLifetime(lifeTime)
             );
}


public record Regexes(string Email, string Mobile, string NationalCode, string Password, string Uri);

public class RegexValidator<T>
{
    private readonly Regexes _regexes;
    private readonly List<(Expression<Func<T, bool>> Rule, string ErrorMessage)> _rules
        = new List<(Expression<Func<T, bool>>, string)>();

    public RegexValidator(IOptionsMonitor<Regexes> regexOptions)
        => _regexes = regexOptions.CurrentValue;

    public RegexValidator<T> AddRule(Expression<Func<T, bool>> rule, string errorMessage)
    {
        _rules.Add((rule, errorMessage));
        return this;
    }

    public RegexValidateResult Validate(T input)
    {
        var errors = _rules
            .Where(r => !r.Rule.Compile().Invoke(input))
            .Select(r => r.ErrorMessage)
            .ToList();

        return new RegexValidateResult(errors);
    }
}
public class RegexValidateResult
{
    public IReadOnlyList<string> Errors;
    public bool IsValid => !Errors.Any();

    public RegexValidateResult(List<string> errors)
        => Errors = errors;
}

public class UserDto
{
    public string Email { get; set; }
    public string Password { get; set; }
}

//var validator = new GenericValidator<UserDto>()
//    .AddRule(u => Regex.IsMatch(u.Email, @"^[\w\.-]+@[\w\.-]+\.\w{2,}$"), "ایمیل نامعتبر است")
//    .AddRule(u => Regex.IsMatch(u.Password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$"), "رمز عبور ضعیف است");

//    var user = new UserDto { Email = "test@site.com", Password = "Weak123" };
//    var result = validator.Validate(user);

//if (!result.IsValid)
//{
//    foreach (var error in result.Errors)
//        Console.WriteLine(error);
//}

