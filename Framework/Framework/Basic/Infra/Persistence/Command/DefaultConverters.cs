namespace Framework;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

public sealed class CodeConverter()
    : ValueConverter<Code, Guid>(code => code.Value, value => Code.New(value));