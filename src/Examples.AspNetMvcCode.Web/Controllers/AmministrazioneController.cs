namespace Examples.AspNetMvcCode.Web.Controllers;

[Authorize]
[Authorize(Policy = PoliciesKeys.UserShouldHaveCompleteProfile)]
[Authorize(Policy = PoliciesKeys.UserIsAdmin)]
[ServiceFilter(typeof(RedirectIfAccessWithLoginCodeFilter), Order = 1)]
[ServiceFilter(typeof(CheckPasswordFilter), Order = 2)]
public class AmministrazioneController : BaseContextController
{
    public AmministrazioneController(
        IHttpContextAccessorWeb webHttpContextAccessor
        ) : base(webHttpContextAccessor)
    {
    }


    [HttpGet]
    public IActionResult AzioniPossibili(
        )
    {
        return
            View(
                MvcComponents.ViewAvailableActions
                );
    }
}