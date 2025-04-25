namespace Framework;

using System.IO;
using Microsoft.AspNetCore.Http;

public interface IFileService
{
    void CopyTo(IFormFile source, string path);
    Task CopyToAsync(IFormFile source, string path);
    void CreateDirectory(string path);
    bool ExistDirectory(string path);
    bool ExistFile(string path);
    string FileExtension(string fileName);
    byte[] Bytes(string path, bool IsEncrypt = false);
    string ToBase64(byte[] source);
    byte[] FromBase64(string source);
    byte[] Bytes(IFormFile source);
    byte[] ObjectToBytes(object source);
    T BytesToObject<T>(byte[] source);
    Task<byte[]> BytesAsync(IFormFile source);
}

public sealed class FileService : IFileService
{
    public void CopyTo(IFormFile source, string path)
    {
        using var stream = new FileStream(path, FileMode.Create);
        source.CopyTo(stream);
    }

    public async Task CopyToAsync(IFormFile source, string path)
    {
        using var stream = new FileStream(path, FileMode.Create);
        await source.CopyToAsync(stream);
    }

    public void CopyTo(IFormFile source, string path, bool IsEncrypt)
    {
        var bytes = Bytes(source);
        var data = !IsEncrypt ? bytes : FileCryptor(bytes, CryptoFileMode.Encrypt);
        using var writer = new BinaryWriter(File.OpenWrite(path));
        writer.Write(data);
    }

    public void CreateDirectory(string path)
    {
        if (!ExistDirectory(path)) Directory.CreateDirectory(path);
    }

    public bool ExistDirectory(string path) => Directory.Exists(path);

    public bool ExistFile(string path) => File.Exists(path);

    public string FileExtension(string fileName) => new FileInfo(fileName).Extension;

    public byte[] Bytes(string path, bool IsEncrypt = false)
    {
        var result = default(byte[]);
        if (ExistFile(path)) result = File.ReadAllBytes(path);
        if (result is not null && IsEncrypt) result = FileCryptor(result, CryptoFileMode.Encrypt);
        return result;
    }

    public string? ToBase64(byte[] source)
        => source is not null ? Convert.ToBase64String(source) : null;

    public byte[] FromBase64(string source)
        => Convert.FromBase64String(source);

    public byte[] Bytes(IFormFile source)
    {
        byte[] result = default!;
        if (source is { })
        {
            using var stream = new MemoryStream();
            source.CopyTo(stream);
            result = stream.ToArray();
        }
        return result;
    }

    public async Task<byte[]> BytesAsync(IFormFile source)
    {
        byte[] result = default!;
        if (source is not null)
        {
            using var stream = new MemoryStream();
            await source.CopyToAsync(stream);
            result = stream.ToArray();
        }
        return result;
    }

    public byte[] FileCryptor(byte[] source, CryptoFileMode cryptMode)
    {
        var encryptKey = "";

        return null;
    }

    public byte[] ObjectToBytes(object source)
    {
        throw new NotImplementedException();
    }

    public T BytesToObject<T>(byte[] source)
    {
        throw new NotImplementedException();
    }
}

public enum CryptoFileMode : byte { Encrypt, Decrypt }


