namespace AwesomeBooksApi.Exceptions;

public class AlreadyExistsException : Exception
{
    public AlreadyExistsException(string message) : base(message) { }

    public AlreadyExistsException(string item, string identifier) : base($"{item} : {identifier} already exists") { }
}

