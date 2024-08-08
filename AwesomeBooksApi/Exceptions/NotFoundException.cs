
namespace AwesomeBooksApi.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
    public NotFoundException(string item, string identifier) : base( $"{item} : {identifier} not found " ) { }

}