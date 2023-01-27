namespace Examples.AspNetMvcCode.Web.Models;

public class RoleInclusionViewModel
{
    public long RoleGroupIdForAssignment { get; set; }
    public IList<string> RolesToInclude { get; set; }
}