namespace Framework;

public abstract class DomainException(string message)
    : ServiceException(MessageTemplate.Format(message))
{
    private const string MessageTemplate = "At Domain: \n\t[{0}]";
}

public sealed class PropertyNullException : DomainException
{
    private const string MessageTemplate = "{0} should not be null!";

    private PropertyNullException(string property)
        : base(MessageTemplate.Format(property))
    { }

    public static void Throw(string property) => new PropertyNullException(property);
}


public sealed class InvalidCharacterException : DomainException
{
    private const string MessageTemplate = "The number of {0} characters must be between {1} and {2}";

    private InvalidCharacterException(string property, int minChar = 3, int maxChar = 100)
        : base(MessageTemplate.Format(property, $"{minChar}", $"{maxChar}"))
    { }

    public static void Throw(string property, int minChar, int maxChar)
    {
        throw new InvalidCharacterException(property, minChar, maxChar);
    }
}