namespace Comunica.ProcessManager.Web.Code;

/// <summary>
/// this exception should not be used in other layers
/// </summary>
public class WebAppException : Exception
{
    public WebAppException() : base()
    {

    }
    public WebAppException(string message) : base(message)
    {
    }

    public WebAppException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
