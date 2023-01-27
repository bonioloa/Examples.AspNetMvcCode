namespace Examples.AspNetMvcCode.Web.Controllers;

[Authorize]
[Authorize(Policy = PoliciesKeys.UserShouldHaveCompleteProfile)]
[Authorize(Policy = PoliciesKeys.UserIsAdmin)]
[ServiceFilter(typeof(RedirectIfAccessWithLoginCodeFilter), Order = 1)]
[ServiceFilter(typeof(CheckPasswordFilter), Order = 2)]
public class AmministrazioneUtentiController : BaseContextController
{
    private readonly ISupervisorAdministrationLogic _logicSupervisorAdministration;

    private readonly IAuthorizationCustomWeb _webAuthorizationCustom;
    private readonly IMainLocalizer _localizer;
    private readonly IDataTablesNetBuilderWeb _webDataTablesNetBuilder;

    public AmministrazioneUtentiController(
        ISupervisorAdministrationLogic logicSupervisorAdministration
        , IHttpContextAccessorWeb webHttpContextAccessor
        , IAuthorizationCustomWeb webAuthorizationCustom
        , IMainLocalizer localizer
        , IDataTablesNetBuilderWeb webDataTablesNetBuilder
        ) : base(webHttpContextAccessor)
    {
        _logicSupervisorAdministration = logicSupervisorAdministration;
        _webAuthorizationCustom = webAuthorizationCustom;
        _localizer = localizer;
        _webDataTablesNetBuilder = webDataTablesNetBuilder;
    }


    [HttpGet]
    public IActionResult GestioneResponsabili(
        bool fareRicerca
        , [ValidateAsSearchNameSurnameFromQuery] string cognome
        , [ValidateAsSearchNameSurnameFromQuery] string nome
        , [ValidateAsSearchEmailFromQuery] string email
        , List<long> ruoli
        )
    {
        string filterSurname = cognome.Clean();
        string filterName = nome.Clean();
        string filterEmail = email.Clean();

        SupervisorSearchResultLgc supervisorSearchResult =
            _logicSupervisorAdministration.ConditionalSearch(
                performSearch: fareRicerca
                , filterSurname: filterSurname
                , filterName: filterName
                , filterEmail: filterEmail
                , filterRoles: ruoli
                );

        if (!supervisorSearchResult.Success)
        {
            OperationResultViewModel modelMessage =
                supervisorSearchResult.MapSearchSupervisorOperationResult();

            modelMessage.LocalizedMessage = _localizer[nameof(LocalizedStr.SearchErrorMessage)];


            _webHttpContextAccessor.SessionOperationResult = modelMessage;

            //DO NOT return on same page, a infinite redirect loop will happen
            return
                RedirectToRoute(
                    _webAuthorizationCustom.GetLandingPageByRole(null, null)
                    );
        }

        SupervisorSearchViewModel supervisorSearchViewModel =
            new(
                Surname: filterSurname,
                Name: filterName,
                Email: filterEmail,
                Roles: supervisorSearchResult.AvailableRolesWithSelected.MapIEnumerableFromLogicToWeb(),
                OrphanedRolesDescriptions: supervisorSearchResult.OrphanedRolesDescriptions,
                ShowResults: supervisorSearchResult.ShowResults,
                SearchResultsModel: null
                );


        if (!fareRicerca
            || !supervisorSearchResult.ShowResults
            || supervisorSearchResult.FoundResults.IsNullOrEmpty())
        {
            return
                View(
                    MvcComponents.ViewSupervisorManagement,
                    supervisorSearchViewModel with
                    {
                        ShowResults = true
                    });
        }

        //saving search make sense only when results are found and it's the search call, not the landing 
        _webHttpContextAccessor.SaveRouteForBackUserViewAndManage();

        return
            View(
                MvcComponents.ViewSupervisorManagement,
                supervisorSearchViewModel with
                {
                    SearchResultsModel =
                        _webDataTablesNetBuilder.BuildSupervisorSearchResultModel(
                            supervisorSearchResult.FoundResults.MapIEnumerableFromLogicToWeb()
                            )
                });
    }



    [HttpGet]
    public IActionResult NuovoResponsabile()
    {
        UserNewSupervisorStartViewModel model = LoadRolesAndRestoreEventualSubmittedDataForNewUser();

        return View(MvcComponents.ViewUserNewSupervisor, model);
    }


    [NonAction]
    private UserNewSupervisorStartViewModel LoadRolesAndRestoreEventualSubmittedDataForNewUser()
    {
        string selectedExclusiveRole = string.Empty;
        IEnumerable<long> selectedSupervisorRoles = Enumerable.Empty<long>();
        if (_webHttpContextAccessor.TempDataOnceUserNewSupervisorSave is not null)
        {
            selectedExclusiveRole = _webHttpContextAccessor.TempDataOnceUserNewSupervisorSave.ProfiloEsclusivo;
            selectedSupervisorRoles = _webHttpContextAccessor.TempDataOnceUserNewSupervisorSave.Profili;
        }


        RolesSelectionViewModel rolesSelection =
            _logicSupervisorAdministration.GetAvailableRolesForNewUser(
                selectedExclusiveRole: selectedExclusiveRole
                , selectedSupervisorRoles: selectedSupervisorRoles
                )
            .MapFromLogicToWebWithNullCheck();


        if (_webHttpContextAccessor.TempDataOnceUserNewSupervisorSave is not null)
        {
            return
                new UserNewSupervisorStartViewModel(
                    RolesForSelectionWithRestored: rolesSelection
                    , _webHttpContextAccessor.TempDataOnceUserNewSupervisorSave
                    );
        }

        return
            new UserNewSupervisorStartViewModel(
                RolesForSelectionWithRestored: rolesSelection
                , new UserNewSupervisorSaveViewModel(
                        Login: string.Empty,
                        Nome: string.Empty,
                        Cognome: string.Empty,
                        Email: string.Empty,
                        ProfiloEsclusivo: string.Empty,
                        Profili: new List<long>()
                        )
                );
    }






    [HttpGet]
    public IActionResult ModificaResponsabili(long idutente)
    {
        //when redirected from POST method with error, we will not handle previous submitted data restore
        //it's better to just reload current user data from db
        UserSupervisorDataLgc userEditSupervisor = _logicSupervisorAdministration.GetUserSupervisorData(userId: idutente);

        return View(MvcComponents.ViewSupervisorEdit, userEditSupervisor.MapFromLogicToWebCustomWithNullCheck());
    }
}