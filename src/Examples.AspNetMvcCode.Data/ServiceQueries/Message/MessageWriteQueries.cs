namespace Examples.AspNetMvcCode.Data;

public class MessageWriteQueries : IMessageWriteQueries
{
    private readonly ILogger<MessageWriteQueries> _logger;
    private readonly ContextTenant _contextTenant;
    private readonly IDataCommandManagerTenant _dataCommandManagerTenant;
    private readonly ITenantCryptManager _cryptManagerTenant;

    public MessageWriteQueries(
        ILogger<MessageWriteQueries> logger
        , ContextTenant contextTenant
        , IDataCommandManagerTenant dataCommandManagerTenant
        , ITenantCryptManager cryptManager
        )
    {
        _logger = logger;
        _contextTenant = contextTenant;
        _dataCommandManagerTenant = dataCommandManagerTenant;
        _cryptManagerTenant = cryptManager;
    }



    public void EnqueueInsertItemUserChatMessage(
        ItemUserMessageWriteQr itemUserMessageNewToSave
        )
    {
        using IDisposable logScopeCurrentMethod =
            _logger.BeginScope(
                new Dictionary<string, object>
                {
                    { AppLogPropertiesKeys.MethodName, nameof(EnqueueInsertItemUserChatMessage) }
                });



        _logger.LogDebug("CALL");

        Guard.Against.Null(itemUserMessageNewToSave, nameof(itemUserMessageNewToSave));
        Guard.Against.NegativeOrZero(itemUserMessageNewToSave.UserId, nameof(itemUserMessageNewToSave.UserId));
        Guard.Against.NegativeOrZero(itemUserMessageNewToSave.ItemId, nameof(itemUserMessageNewToSave.ItemId));
        Guard.Against.NegativeOrZero(itemUserMessageNewToSave.ProcessId, nameof(itemUserMessageNewToSave.ProcessId));
        Guard.Against.NullOrWhiteSpace(itemUserMessageNewToSave.Phase, nameof(itemUserMessageNewToSave.Phase));
        Guard.Against.NullOrWhiteSpace(itemUserMessageNewToSave.State, nameof(itemUserMessageNewToSave.State));
        Guard.Against.NegativeOrZero(itemUserMessageNewToSave.NewIdMessage, nameof(itemUserMessageNewToSave.NewIdMessage));

        Guard.Against.InvalidInput(
            itemUserMessageNewToSave.OperationDateTime
            , nameof(itemUserMessageNewToSave.OperationDateTime)
            , (input) => input != DateTime.MinValue
            );

        _dataCommandManagerTenant.EnqueueNonQueryCommand(
            new CommandExecutionDb()
            {
                Parameters =
                    new HashSet<CommandParameterDb>
                    {
                        new () { Name = SqlParamsNames.CompanyGroupId, Value = _contextTenant.CompanyGroupId},//not used in query for now
                        new () { Name = SqlParamsNames.MessageId, Value = itemUserMessageNewToSave.NewIdMessage },
                        new () { Name = SqlParamsNames.ProcessId, Value = itemUserMessageNewToSave.ProcessId},
                        new () { Name = SqlParamsNames.Phase, Value = itemUserMessageNewToSave.Phase },
                        new () { Name = SqlParamsNames.State, Value = itemUserMessageNewToSave.State },
                        new () { Name = SqlParamsNames.ItemId, Value = itemUserMessageNewToSave.ItemId},
                        new () { Name = SqlParamsNames.UserId, Value = itemUserMessageNewToSave.UserId},
                        new () { Name = SqlParamsNames.CryptSubject, Value = _cryptManagerTenant.CleanAndEncryptString(itemUserMessageNewToSave.Subject) },
                        new () { Name = SqlParamsNames.CryptMessage, Value = _cryptManagerTenant.CleanAndEncryptString(itemUserMessageNewToSave.Text) },
                        new () { Name = SqlParamsNames.MessageDateOperation, Value = itemUserMessageNewToSave.OperationDateTime.ToDbStringDateInvariant()},
                        new () { Name = SqlParamsNames.MessageTimeOperation, Value = itemUserMessageNewToSave.OperationDateTime.ToDbStringTimeInvariant()},
                    },
                CommandText = $@"
                    INSERT INTO Z_WBL_AEMAIL(
                        n_prog
                        , n_idprocesso
                        , s_idfase
                        , s_stato
                        , n_cod_adempimento
                        , s_from
                        , s_oggetto
                        , s_testo
                        , s_nome_utente
                        , s_password
                        , d_sys
                        , o_sys
                        )
                    SELECT
                        {SqlParamsNames.MessageId} AS n_prog
                        , {SqlParamsNames.ProcessId} AS n_idprocesso
                        , {SqlParamsNames.Phase} AS s_idfase
                        , {SqlParamsNames.State} AS s_stato
                        , {SqlParamsNames.ItemId} AS n_cod_adempimento
                        , {SqlParamsNames.UserId} AS s_from
                        , {SqlParamsNames.CryptSubject} AS s_oggetto
                        , {SqlParamsNames.CryptMessage} AS s_testo
                        , '' AS s_nome_utente
                        , '' AS s_password
                        , {SqlParamsNames.MessageDateOperation} AS d_sys
                        , {SqlParamsNames.MessageTimeOperation} AS o_sys
                    ",
            });//in future to get user name and surname join on accessi table
    }

}
