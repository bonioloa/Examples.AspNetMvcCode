namespace Examples.AspNetMvcCode.Web.Models;

public record UserNewSupervisorSaveViewModel(
    string Login
    , string Nome
    , string Cognome
    , string Email
    , string ProfiloEsclusivo
    , List<long> Profili
    );