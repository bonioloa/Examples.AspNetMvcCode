namespace Examples.AspNetMvcCode.Web.Models;

public record UserEditSupervisorSaveViewModel(
     long IdUtente
    , string Nome
    , string Cognome
    , string Email
    , string ProfiloEsclusivo
    , List<long> Profili
    );