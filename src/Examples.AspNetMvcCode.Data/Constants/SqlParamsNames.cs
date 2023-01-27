namespace Examples.AspNetMvcCode.Data;

/// <summary>
/// All SQL parameters. Never use outside data layer
/// VERY IMPORTANT: when new constant is created, check if it is contained in another
/// constant name ('@' must be included in comparison)
/// This is needed for correct working of batch command execution
/// </summary>
internal static class SqlParamsNames
{
    //per legacy nel database
    internal static readonly IList<string> ParamsToObfuscateInLogs =
        new List<string>()
    {
            CryptUserLogin,
            CryptPassword,
            CryptName,
            CryptSurname,
            CryptEmail,
            FileName,
            CryptName,
            CryptSurname,
    };

    //generici
    internal const string ModifyDateTime = "@DataOraModifica";

    //utenze (obfuscate also some with crypt prefix)
    internal const string UserId = "@UserId";
    internal const string CryptUserLogin = "@CryptUserLogin";
    internal const string CryptPassword = "@CryptPasswordHash";
    internal const string PasswordExpiringDate = "@DataScadenzaPassword";
    internal const string CryptName = "@CryptNome";//obfuscate
    internal const string CryptSurname = "@CryptCognome";//obfuscate
    internal const string CredentialsEndDate = "@DataFineValiditaCredenziali";
    internal const string Anonymous = "@Anonimo";
    internal const string UserStartDate = "@DataInizioUtente";
    internal const string UserEndDate = "@DataFineUtente";

    //root
    internal const string ConfigurationType = "@TipoConfigurazione";
    internal const string Language = "@Linguaggio";

    //email
    internal const string CryptEmail = "@CryptEmail";//obfuscate
    internal const string ParamsCode = "@CodiceParametro";

    //messaggi
    internal const string CryptMessage = "@CryptMessaggio";
    internal const string CryptSubject = "@CryptOggetto";
    internal const string MessageId = "@IdMessaggio";
    internal const string MessageDateOperation = "@DataOperazioneMessaggio";
    internal const string MessageTimeOperation = "@OrarioOperazioneMessaggio";

    //schede e decodifiche
    internal const string IdForm = "@IdScheda";
    internal const string FormCode = "@CodiceScheda";
    internal const string OptionGroupCode = "@GruppoScelte";

    //items/elementi
    internal const string ItemId = "@IdElemento";
    internal const string ItemIdMaster = "@ElementoMasterId";
    internal const string Phase = "@Fase";
    internal const string FromPhase = "@PartenzaFase";
    internal const string NextPhase = "@ProssimaFase";
    internal const string State = "@Stato";
    internal const string FromState = "@PartenzaStato";
    internal const string NextState = "@ProssimoStato";
    internal const string CryptItemDescriptiveCode = "@CryptCodiceDescrittivoElemento";
    internal const string ItemDateStart = "@DataInizioElemento";
    internal const string ItemDateEnd = "@DataFineElemento";
    internal const string ItemDateOperation = "@DataOperazioneElemento";
    internal const string ItemTimeOperation = "@OrarioOperazioneElemento";
    internal const string ItemDateTimeOperation = "@DataOraOperazioneElemento";

    //processi
    internal const string ProcessId = "@IdProcesso";
    internal const string ProcessIdMaster = "@ProcessoMasterId";
    internal const string ProcessLogicGroupId = "@LogicGroupId";
    internal const string MultipleProcessesPrefix = "@InIdProcesso_";

    //ricerca
    internal const string StateGroupType = "@TipoRaggruppamentoStato";
    internal const string DateSubmitFrom = "@DataInvioItemDa";
    internal const string DateSubmitTo = "@DataInvioItemA";
    internal const string DateExpirationFrom = "@DataScadenzaItemDa";
    internal const string DateExpirationTo = "@DataScadenzaItemA";

    //log (this parameters don't need obfuscation)
    internal const string LogActionType = "@LogTipoAzione";
    internal const string AccessType = "@TipoAccesso";
    internal const string ActionType = "@TipoAzione";
    internal const string Step = "@Step";
    internal const string ElementId = "@CodiceElementoLog";
    internal const string FileId = "@IdFile";
    internal const string FileName = "@NomeFile";
    internal const string LogDateChanged = "@DataModificata";
    internal const string LogDateOperation = "@LogDataOperazione";
    internal const string LogTimeOperation = "@LogOrarioOperazione";

    //relazioni-gruppi
    internal const string RoleToVerify = "@VerificaIdRuolo";
    internal const string HasSupervisorRole = "@UtenteIsResponsabile";
    internal const string SingleRole = "@RuoloSingolo";

    //gruppo società
    internal const string CompanyGroupId = "@IdGruppoSocieta";
    internal const string CompanyGroupCode = "@CodGruppoSocieta";

    //attachments
    internal const string FileAttachmentId = "@IdAllegato";
    internal const string CryptFileAttachmentName = "@AllegatoCryptNome";
    internal const string CryptFileAttachmentType = "@TipoAllegatoCrypt";
    internal const string CryptPhysicalPath = "@CryptPathFisico";
    internal const string CryptFileAttachmentAsByte = "@CriptAllegatoInByte";
    internal const string CryptFileAttachmentSimpleDescription = "@CryptAllegatoDescrizioneSintetica";
    internal const string CryptFileAttachmentExtendedDescription = "@CryptAllegatoDescrizioneEstesa";
    internal const string FileDateInsert = "@DataInserimentoAllegato";

    //permessi
    internal const string RoleGroupIdForAssignment = "@IdGruppoRuoliPerAssegnazionePermessi";

    //report 
    internal const string ReportDocumentIdCode = "@CodiceReport";

    //sso
    internal const string SsoConfigId = "@IdConfigSso";

    //calcoli
    internal const string ItemCalculatedFieldPrefix = "@CampoCalcolatoElemento_";
    internal const string PreviousCalculationOutputTable = "@OggettoOutputCalcoloPrecedente";

    //progressivi
    internal const string ProgressiveType = "@TipoProgressivo";

    //data grid state
    internal const string DataGridViewStateId = "@IdStatoDataGrid";
    internal const string DataGridViewStateUserProvidedDescription = "@StatoVistaDataGridDescrizioneStatoDaUtente";
    internal const string DataGridViewStateSerialized = "@StatoVistaDataGridSerializzto";
    internal const string DataGridStateUsageType = "@StatoDataGridTipologiaUtilizzo";
    internal const string DataGridDateTimeOperation = "@DataOraOperazioneDataGrid";

    //scadenze
    internal const string IdExpiration = "@IdScadenza";

    //rivelazione identità
    internal const string IdentityRevelationDate = "@DataRivelazioneIdentita";
    internal const string IdentityRevelationTime = "@OrarioRivelazioneIdentita";
}