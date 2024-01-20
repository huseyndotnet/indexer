namespace Indexer.Api.Exceptions;

public class WebsiteAlreadyExistsException : BaseException
{
    public sealed override string Message => "Website Already Exists";
}