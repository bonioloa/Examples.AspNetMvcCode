namespace Examples.AspNetMvcCode.Web.Code;

/// <summary>
/// Exception for code in Web project
/// </summary>
/// <remarks>Do not use this exception in other project</remarks>
[SuppressMessage(
    "Design"
    , "CA1032:Implement standard exception constructors"
    , Justification = "We want to force user to only set up a message when throwing this exception. If needed implement other constructor but not use the parameterless constructor"
    )]
[SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "Keep this suppression in case rule will be enabled")]
public class PmWebException : Exception
{
    //DO not implement default parameterless constructor, devs must be forced to provide a message
    public PmWebException(string message) : base(message)
    {
    }
}