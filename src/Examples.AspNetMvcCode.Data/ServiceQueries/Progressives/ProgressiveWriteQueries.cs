namespace Examples.AspNetMvcCode.Data;

/// <summary>
/// this service keeps track of progressives of rows for some tables and also some other functional progressives
/// </summary>
/// <remarks>
/// refrain to reference this service outside of unit of work services and services of Logic layes
/// </remarks>
public class ProgressiveWriteQueries : IProgressiveWriteQueries
{
    private readonly ILogger<ProgressiveWriteQueries> _logger;
    private readonly ContextTenant _contextTenant;
    private readonly IDataCommandManagerTenant _dataCommandManagerTenant;

    public ProgressiveWriteQueries(
        ILogger<ProgressiveWriteQueries> logger
        , ContextTenant contextTenant
        , IDataCommandManagerTenant dataCommandManagerTenant
        )
    {
        _logger = logger;
        _contextTenant = contextTenant;
        _dataCommandManagerTenant = dataCommandManagerTenant;
    }




    private enum ProgressiveType
    {
        None,
        ItemId,
        ItemsByProcess,
        DataGridStateId,
        AttachmentId,
        FormId,
        MessageId,
    }


    /// <summary>
    /// create new progressive for table Z_WBL_STO_ADEMPIMENTI and save it to progressives table. (new n_cod_adempimento)
    /// </summary>
    /// <returns></returns>
    public long CreateAndCommitNewIdForItem()
    {
        using IDisposable logScopeCurrentMethod =
            _logger.BeginScope(
                new Dictionary<string, object>
                {
                    { AppLogPropertiesKeys.MethodName, nameof(CreateAndCommitNewIdForItem) }
                });

        _logger.LogDebug("CALL");


        return
            CreateAndCommitIncrementedProgressive(
               null
                , ProgressiveType.ItemId
                );
    }


    /// <summary>
    /// create new progressive using context processId and save it to progressives table.
    /// This progressive can be used for item descriptive code creation
    /// </summary>
    /// <returns></returns>
    public long CreateAndCommitNewProgressiveItemByCurrentProcess(long processId)
    {
        using IDisposable logScopeCurrentMethod =
            _logger.BeginScope(
                new Dictionary<string, object>
                {
                    { AppLogPropertiesKeys.MethodName, nameof(CreateAndCommitNewProgressiveItemByCurrentProcess) }
                });

        _logger.LogDebug("CALL");


        return
            CreateAndCommitIncrementedProgressive(
                processId
                , ProgressiveType.ItemsByProcess
                );
    }



    /// <summary>
    /// increment to get new progressive for Z_SYS_DATAGRID_STATE table
    /// </summary>
    /// <returns></returns>
    public long CreateAndCommitNewIdForDataGridState()
    {
        using IDisposable logScopeCurrentMethod =
            _logger.BeginScope(
                new Dictionary<string, object>
                {
                    { AppLogPropertiesKeys.MethodName, nameof(CreateAndCommitNewIdForDataGridState) }
                });

        _logger.LogDebug("CALL");


        return
            CreateAndCommitIncrementedProgressive(
                null
                , ProgressiveType.DataGridStateId
                );
    }


    /// <summary>
    /// increment to get new progressive for Z_WBL_ADOCS table
    /// </summary>
    /// <returns></returns>
    public long CreateAndCommitNewIdForAttachment()
    {
        using IDisposable logScopeCurrentMethod =
            _logger.BeginScope(
                new Dictionary<string, object>
                {
                    { AppLogPropertiesKeys.MethodName, nameof(CreateAndCommitNewIdForAttachment) }
                });

        _logger.LogDebug("CALL");


        return
            CreateAndCommitIncrementedProgressive(
                null
                , ProgressiveType.AttachmentId
                );
    }



    /// <summary>
    /// increment to get new progressive for Z_WBL_ASCHEDE table
    /// </summary>
    /// <returns></returns>
    public long CreateAndCommitNewIdForForm()
    {
        using IDisposable logScopeCurrentMethod =
            _logger.BeginScope(
                new Dictionary<string, object>
                {
                    { AppLogPropertiesKeys.MethodName, nameof(CreateAndCommitNewIdForForm) }
                });

        _logger.LogDebug("CALL");


        return
            CreateAndCommitIncrementedProgressive(
                null
                , ProgressiveType.FormId
                );
    }



    /// <summary>
    /// increment to get new progressive for Z_WBL_ASCHEDE table
    /// </summary>
    /// <returns></returns>
    public long CreateAndCommitNewIdForMessage()
    {
        using IDisposable logScopeCurrentMethod =
            _logger.BeginScope(
                new Dictionary<string, object>
                {
                    { AppLogPropertiesKeys.MethodName, nameof(CreateAndCommitNewIdForMessage) }
                });

        _logger.LogDebug("CALL");


        return
            CreateAndCommitIncrementedProgressive(
                null
                , ProgressiveType.MessageId
                );
    }





    private long CreateAndCommitIncrementedProgressive(
        long? processId
        , ProgressiveType progressiveType
        )
    {
        Guard.Against.InvalidInput(
            progressiveType
            , nameof(progressiveType)
            , (input) => input != ProgressiveType.None
            , $"value '{progressiveType}' is invalid."
            );

        HashSet<CommandParameterDb> parameters =
            new()
            {
                new() { Name = SqlParamsNames.ProgressiveType, Value = progressiveType.ToString() },
            };

        string processIdFilter = DbConstants.InvariantQueryCondition;
        if (processId.Valid())
        {
            parameters.Add(new CommandParameterDb { Name = SqlParamsNames.ProcessId, Value = (long)processId });
            processIdFilter = $" prgr.ProcessId = {SqlParamsNames.ProcessId} ";
        }
        processIdFilter = processIdFilter + " --" + nameof(processIdFilter);


        long newId =
            _dataCommandManagerTenant.ExecuteCommitWithLongResult(
                new CommandExecutionDb()
                {
                    Parameters = parameters,
                    CommandText = $@"
                        DECLARE @NewProgressive BIGINT
                        ;
                        SELECT @NewProgressive = prgr.Progressive + 1
                        FROM Z_WBL_APROGRESSIVI prgr
                        WITH(XLOCK, ROWLOCK)
                        WHERE prgr.ProgressiveType = {SqlParamsNames.ProgressiveType}
                            AND {processIdFilter}
                        ;
                        UPDATE prgr SET prgr.Progressive = @NewProgressive
                        FROM Z_WBL_APROGRESSIVI prgr
                        WHERE prgr.ProgressiveType = {SqlParamsNames.ProgressiveType}
                            AND {processIdFilter}
                        ;
                        SELECT @NewProgressive
                    ",
                });

        if (newId.Invalid())
        {
            throw new PmDataException($"new progressive is less or equal of 0");
        }

        return newId;
    }
}