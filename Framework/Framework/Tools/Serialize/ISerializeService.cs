namespace Framework;

public interface ISerializeService : IDisposable
{
    string Serialize(object source);
    string Serialize<TSource>(TSource source);
    object? Deserialize(string source, Type type);
    TOutput? Deserialize<TOutput>(string source);
}

public interface IExcelSerializeService
{
    byte[] ToExcelBytes<T>(List<T> source, string sheet = "default");
    DataTable ExcelToDataTable(byte[] source);
    List<T> ExcelBytesToList<T>(byte[] source);
}

public enum SerializeFlag : byte
{
    ///<summary>Json Serializer</summary>
    Json = 0,
    NewtonSoft = 1,
    NetJson = 2
}