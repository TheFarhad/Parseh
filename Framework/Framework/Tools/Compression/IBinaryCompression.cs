namespace Framework;

using System.IO.Compression;

public interface IBlobCompression
{
    byte[] Compress(byte[] source);
    byte[] Compress(string source);
    string Decompress(byte[] source);
}

public sealed class GZipBlobCompression : IBlobCompression
{
    public byte[] Compress(byte[] source)
    {
        using var msi = new MemoryStream(source);
        using var mso = new MemoryStream();
        using var gs = new GZipStream(mso, CompressionMode.Compress);
        msi.CopyTo(gs);

        return mso.ToArray();
    }

    public byte[] Compress(string source)
    {
        var bytes = Encoding.UTF8.GetBytes(source);
        return Compress(bytes);
    }

    public string Decompress(byte[] source)
    {
        using var msi = new MemoryStream(source);
        using var mso = new MemoryStream();
        using var gzipStream = new GZipStream(msi, CompressionMode.Decompress);
        gzipStream.CopyTo(mso);

        return Encoding.UTF8.GetString(mso.ToArray());
    }
}