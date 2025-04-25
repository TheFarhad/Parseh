namespace Framework;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

public interface IUploadService
{
    Task<string> UploadAsync(IFormFile source, string path, string prefixName, bool IsEncrypt = false);
    string FileUrl(string path, string fileName);
}

public sealed class UploadService : IUploadService
{
    private readonly IFileService _filer;
    private readonly IWebHostEnvironment _host;
    private readonly IHttpContextAccessor _contextAccessor;

    public UploadService(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var provider = scope.ServiceProvider;
        _host = provider.GetRequiredService<IWebHostEnvironment>();
        _contextAccessor = provider.GetRequiredService<IHttpContextAccessor>();
        _filer = provider.GetRequiredService<IFileService>();
    }

    public async Task<string> UploadAsync(IFormFile source, string path, string prefixName, bool IsEncrypt = false)
    {
        var result = String.Empty;
        if (source is { })
        {
            var (fileName, finalPath) = UploadConfigs(source, path, prefixName);
            await _filer.CopyToAsync(source, finalPath);
            result = fileName;
        }
        return result;
    }

    public string FileUrl(string path, string fileName)
    {
        // usfull for web api project to send file url to client
        // چون نباید مسیر فیزیکی محل فایل ها را در سمت کلاینت نشان هیم. مثلا c:\\Portfolio\Files\Images\...
        var host = _contextAccessor.HttpContext.Request.Host.Value;
        var result = Path.Combine(host, path, fileName);
        return result;
    }

    private (string fileName, string finalPath) UploadConfigs(IFormFile source, string path, string prefixName)
    {
        var dirctory = @$"{_host.WebRootPath}{path}";
        _filer.CreateDirectory(dirctory);
        var fileName = $"{prefixName}{UniqueValue()}{_filer.FileExtension(source.FileName)}";
        var finalPath = @$"{dirctory}/{fileName}";
        var result = (fileName, finalPath);
        return result;
    }

    private string UniqueValue()
    {
        var date = DateTime.Now;
        var result = $"{date.Year}-{date.Month}-{date.Day}_{date.Hour}-{date.Minute}-{date.Second}";
        return result;
    }
}


