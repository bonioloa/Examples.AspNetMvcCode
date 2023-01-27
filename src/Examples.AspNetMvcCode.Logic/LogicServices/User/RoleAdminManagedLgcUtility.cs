namespace Examples.AspNetMvcCode.Logic.LogicServices.User;

internal static class RoleAdminManagedLgcUtility
{
    //internal const string ErrorSaveUserInfoGeneric =
    //    "E' avvenuto un problema tecnico durante il salvataggio. Segnala il problema al tuo referente applicativo.";

    internal static readonly HtmlString DescriptionExclusiveRoleDefaultNone = new("Responsabile");
    internal static readonly HtmlString DescriptionExclusiveRoleDefaultBasicRole = new("Utente semplice");
    internal static readonly HtmlString DescriptionExclusiveRoleDefaultAdminApplication = new("Amministratore applicativo");


    internal const string ErrorNewUserSubmitExclusiveRoleEmpty = "Il valore per Profilo Esclusivo è vuoto";
    internal const string ErrorRolesEmpty = "Non è stato indicato alcun profilo da assegnare all'utente";
}