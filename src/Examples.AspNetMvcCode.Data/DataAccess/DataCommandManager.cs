namespace Examples.AspNetMvcCode.Data;

/// <summary>
/// abstract class for databases interaction. </br>
/// Uses ADO.NET and also DAPPER (introduced recently, to be used in new query methods and to replace older when modified)
/// </summary>
/// <remarks>
/// Must be inherited by a class that must be configured as scoped to keep alive command queue<br/>
/// Methods of this class should NEVER be used outside DATA LAYER<br/>
/// Everything is public for the sake of testing and service mapping/initialization
/// </remarks>
public abstract class DataCommandManager : IDataCommandManager
{
    private readonly ILogger _logger;

    public DataCommandManager(
        ILogger logger
        )
    {
        _logger = logger;
    }



    /// <summary>
    /// necessary in case derived class is used in Unit of Work classes</br>
    /// and there is the need to process multiple commands in the same web call 
    /// </summary>
    /// <remarks>to leverage multiple commands queue, derived class must be configured in DI as Scoped</remarks>
    private Queue<CommandExecutionDb> _batchCommands;

    /// <summary>
    /// by construction this should be filled after derived class DI instantiation
    /// </summary>
    protected private DataAccessProperties _dataAccessProperties;

    private T ExecuteCommand<T>(CommandExecutionDb commandExecution)
    {
        ValidateState();
        ValidateCommand(commandExecution);


        using SqlConnection connection = new(_dataAccessProperties.SqlConnectionStringBuilder.ConnectionString);
        connection.Open();

        using SqlCommand cmd = connection.CreateCommand();


        try//try is used only as wrapping to ensure the execution of finally block, eventual exception will be rethrow
        {
            commandExecution = MapParametersAndNormalizeExecutionForAdo(commandExecution, cmd);

            cmd.Connection = connection;
            cmd.CommandTimeout = _dataAccessProperties.SqlCommandTimeoutInSeconds;
            cmd.CommandType = commandExecution.CommandType;
            cmd.CommandText = commandExecution.CommandText;


            LogAdoCommand(
                cmd
                , forceWriteCommandLog: commandExecution.WriteCommandLog
                , enableAllCommandLogging: _dataAccessProperties.EnableAllCommandLogging
                , disableAllCommandLogging: _dataAccessProperties.DisableAllCommandLogging
                );


            if (typeof(T) == typeof(DataTable))
            {
                using IDataReader dataReader = cmd.ExecuteReader();
                DataTable table = new();
                table.Load(dataReader);
                return (T)(object)table;
            }
            if (typeof(T) == typeof(List<DataTable>))
            {
                using SqlDataAdapter dataAdapter = new(cmd);
                using DataSet dataSet = new();
                dataAdapter.Fill(dataSet);

                List<DataTable> output = new();

                if (dataSet is not null
                    && dataSet.Tables is not null
                    && dataSet.Tables.Count.Valid())
                {
                    foreach (DataTable table in dataSet.Tables)
                    {
                        output.Add(table);
                    }
                }

                return (T)(object)output;
            }

            object result = cmd.ExecuteScalar();
            return (T)result;
        }
#pragma warning disable CS0168 // Variable is declared but never used  (debug reasons)
        catch (Exception ex)
        {
            throw;
        }
#pragma warning restore CS0168 // Variable is declared but never used
    }



    public List<DataTable> ReadMultipleData(CommandExecutionDb commandExecution)
    {
        Guard.Against.Null(commandExecution, nameof(commandExecution));

        return ExecuteCommand<List<DataTable>>(commandExecution);
    }


    public DataTable ReadData(CommandExecutionDb commandExecution)
    {
        Guard.Against.Null(commandExecution, nameof(commandExecution));

        return ExecuteCommand<DataTable>(commandExecution);
    }


    private object ExecuteToGetScalar(CommandExecutionDb commandExecution)
    {
        return ExecuteCommand<object>(commandExecution);
    }


    public string ExecuteToGetCleanString(CommandExecutionDb commandExecution)
    {
        object obj = ExecuteToGetScalar(commandExecution);

        return
            obj != null
            ? obj.ToString().Clean()
            : string.Empty;
    }




    public void EnqueueNonQueryCommand(
        CommandExecutionDb commandExecution
        )
    {
        ValidateCommand(commandExecution);

        _batchCommands ??= new Queue<CommandExecutionDb>();

        _batchCommands.Enqueue(commandExecution);
    }


    //this is how much zeros we add before numeric suffix,
    //used to rename and differentiate parameters for query batch execution
    private const int ParametersPaddingLength = 6;

    /// <summary>
    /// commit all commands in current queue<br/>
    /// execution: <br/>
    /// cycle and concatenate all commands <br/>
    /// a suffix is added to parameters names using a padded current cycle index, ensuring no name conflicts<br/>
    /// </summary>
    /// <remarks>
    /// IMPORTANT: this method is public only because of convenience and testing.<br/>
    /// For new classes, this method should be used ONLY in Unit of Work services and NEVER outside of Data layer
    /// </remarks>
    public void CommitCommandsInCurrentBatch()
    {
        if (_batchCommands is null || _batchCommands.Count == 0)
        {
            return;
        }

        ValidateState();


        using SqlConnection connection = new(_dataAccessProperties.SqlConnectionStringBuilder.ConnectionString);
        connection.Open();

        using SqlCommand cmd = connection.CreateCommand();

        bool needsRollback = true;


        try
        {
            cmd.Transaction = connection.BeginTransaction(_dataAccessProperties.IsolationLevel);
            cmd.CommandTimeout = _dataAccessProperties.SqlCommandTimeoutInSeconds;

            CommandExecutionDb tmpExec = null;
            SqlParameter tmpParameter = null;
            int index = 0;
            string parameterNameWithIndex = string.Empty;
            string tmpCommand = string.Empty;
            string formatIndex = string.Empty;
            StringBuilder commandsBuilder = new();
            while (_batchCommands.Count.Valid())
            {
                tmpExec = _batchCommands.Dequeue();
                if (tmpExec is null)
                {
                    continue;
                }

                #region code to prevent parameters conflict at execution

                //commands batch can have queries with same name parameters
                //this is solved renaming parameters 
                //and parameter in query string command
                //a suffix with increment is added to the end of parameters

                tmpCommand = tmpExec.CommandText;
                //padzeroes to differentiate parameters between queries
                formatIndex =
                    index.ToString()
                         .PadLeft(ParametersPaddingLength, DbConstants.NumericPadderChar);

                if (tmpExec.Parameters.HasValues())
                {
                    foreach (CommandParameterDb parameter in tmpExec.Parameters.OrderBy(p => p.Progressive))
                    {
                        parameterNameWithIndex = $"{parameter.Name}{formatIndex}";
                        tmpCommand = tmpCommand.ReplaceInvariant(parameter.Name, parameterNameWithIndex);

                        tmpParameter = cmd.CreateParameter();
                        tmpParameter.ParameterName = parameterNameWithIndex;
                        tmpParameter.Value = parameter.Value;

                        tmpParameter.Direction =
                            parameter.Direction is null
                            ? ParameterDirection.Input
                            : (ParameterDirection)parameter.Direction;

                        if (parameter.Type != null)
                        {
                            tmpParameter.SqlDbType = (SqlDbType)parameter.Type;
                        }
                        if (parameter.TypeName.StringHasValue())
                        {
                            tmpParameter.TypeName = parameter.TypeName;
                        }

                        cmd.Parameters.Add(tmpParameter);
                    }
                }
                #endregion

                commandsBuilder.Append(tmpCommand);
                commandsBuilder.AppendLine(DbConstants.DbSqlQueryTermination);
                index++;
            }
            cmd.CommandText = commandsBuilder.ToString();

            LogAdoCommandAlways(cmd);//always log inserts

            cmd.ExecuteNonQuery();

            cmd.Transaction?.Commit();
            cmd.Transaction?.Dispose();

            needsRollback = false;//flag as executed correctly
        }
#pragma warning disable CS0168 // Variable is declared but never used (debug reasons)
        catch (Exception ex)
        {
            if (_batchCommands.Count.Valid())
            {
                _batchCommands.Clear();
            }

            throw;
        }
#pragma warning restore CS0168 // Variable is declared but never used
        finally
        {
            FinallyTransactionCleanup(cmd?.Transaction, needsRollback);
        }
    }


    /// <summary>
    /// To be used in try/catch/finally block, in finally part<br/>
    /// performs rollback if <paramref name="needsRollback"/> is true and dispose transaction 
    /// </summary>
    /// <param name="transaction"></param>
    /// <param name="needsRollback"></param>
    private void FinallyTransactionCleanup(SqlTransaction transaction, bool needsRollback)
    {
        if (transaction is null)
        {
            return;
        }

        if (needsRollback)
        {
            try
            {
                transaction.Rollback();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "cleanup - nothing to rollback for transaction");
            }
        }

        try
        {
#pragma warning disable IDISP007 // Don't dispose injected
            transaction.Dispose();
#pragma warning restore IDISP007 // Don't dispose injected
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "cleanup - nothing to dispose for transaction");
        }

    }


    private static CommandExecutionDb MapParametersAndNormalizeExecutionForAdo(CommandExecutionDb commandExecution, SqlCommand cmd)
    {
        if (commandExecution.Parameters.HasValues())
        {
            SqlParameter tmpParameter = null;
            foreach (CommandParameterDb parameter in commandExecution.Parameters.OrderBy(p => p.Progressive))
            {
                tmpParameter = cmd.CreateParameter();
                tmpParameter.ParameterName = parameter.Name;
                tmpParameter.Value = parameter.Value;

                tmpParameter.Direction =
                    parameter.Direction is null
                    ? ParameterDirection.Input
                    : (ParameterDirection)parameter.Direction;

                if (parameter.Type != null)
                {
                    tmpParameter.SqlDbType = (SqlDbType)parameter.Type;
                }
                if (parameter.TypeName.StringHasValue())
                {
                    tmpParameter.TypeName = parameter.TypeName;
                }

                cmd.Parameters.Add(tmpParameter);
            }
        }

        return commandExecution;
    }


    private void LogAdoCommandAlways(SqlCommand cmd)
    {
        LogAdoCommand(
            cmd
            , forceWriteCommandLog: true
            , enableAllCommandLogging: true
            , disableAllCommandLogging: false
            );
    }

    private void LogAdoCommand(
        SqlCommand cmd
        , bool forceWriteCommandLog
        , bool enableAllCommandLogging
        , bool disableAllCommandLogging
        )
    {
        string commandToLog =
            cmd.AdoCommandAsSql(
                writeCommandLog: forceWriteCommandLog
                , disableAllCommandLogging: disableAllCommandLogging
                , enableAllCommandLogging: enableAllCommandLogging
                );

        if (commandToLog.StringHasValue())
        {
            _logger.LogInformation("Command to be executed: {Command}", Environment.NewLine + commandToLog);
        }
    }



    /*-----------------------------------------------------------------------------------------
     * start of dapper code part
     * (for each method remember to add the needed custom type handlers)
     * ----------------------------------------------------------------------------------
     */

    [SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "Leave exception var for debug reasons")]
    public IEnumerable<T> Query<T>(CommandExecutionDb commandExecution)
    {
        ValidateState();
        ValidateCommand(commandExecution);


        try
        {
            using SqlConnection connection = new(_dataAccessProperties.SqlConnectionStringBuilder.ConnectionString);

            DynamicParameters parameters = MapParametersAndNormalizeExecutionForDapper(ref commandExecution);

            LogDapperCommand(
                commandExecution
                , connection.Database
                , parameters
                , enableAllCommandLogging: _dataAccessProperties.EnableAllCommandLogging
                , disableAllCommandLogging: _dataAccessProperties.DisableAllCommandLogging
                );

            //custom handlers
            SqlMapper.AddTypeHandler(new IHtmlContentTypeHandler());

            return
                connection.Query<T>(
                    sql: commandExecution.CommandText
                    , parameters
                    , commandType: commandExecution.CommandType
                    , commandTimeout: _dataAccessProperties.SqlCommandTimeoutInSeconds
                    );
        }
#pragma warning disable CS0168 // Variable is declared but never used  (debug reasons)
        catch (Exception ex)
        {
            throw;
        }
#pragma warning restore CS0168 // Variable is declared but never used
    }


    private object ExecuteWithTransactionAndReturnResult(CommandExecutionDb commandExecution)
    {
        //https://dapper-tutorial.net/transaction
        //https://github.com/zzzprojects/Dapper.Transaction
        ValidateState();
        ValidateCommand(commandExecution);

        object result = null;


        try
        {
            DynamicParameters parameters = MapParametersAndNormalizeExecutionForDapper(ref commandExecution);

            using SqlConnection connection = new(_dataAccessProperties.SqlConnectionStringBuilder.ConnectionString);
            connection.Open();

            LogDapperCommandAlways(
                commandExecution
                , connection.Database
                , parameters
                );

            using SqlTransaction transaction = connection.BeginTransaction(_dataAccessProperties.IsolationLevel);

            //custom handlers
            SqlMapper.AddTypeHandler(new IHtmlContentTypeHandler());

            result =
                transaction.ExecuteScalar(
                    sql: commandExecution.CommandText
                    , parameters
                    , commandType: commandExecution.CommandType
                    );

            transaction.Commit();
        }
#pragma warning disable CS0168 // Variable is declared but never used (debug reasons)
#pragma warning disable IDE0059 // Unnecessary assignment of a value 
        catch (Exception ex)
        {
            throw;
        }
#pragma warning restore CS0168 // Variable is declared but never used
#pragma warning restore IDE0059 // Unnecessary assignment of a value

        return result;
    }


    public long ExecuteCommitWithLongResult(CommandExecutionDb commandExecution)
    {
        object result = ExecuteWithTransactionAndReturnResult(commandExecution);

        if (result is null)
        {
            throw new PmDataException("command with transaction returned null, expected long result");
        }

        return (long)result;
    }


    private static DynamicParameters MapParametersAndNormalizeExecutionForDapper(ref CommandExecutionDb commandExecution)
    {
        Guard.Against.Null(commandExecution, nameof(commandExecution));


        if (commandExecution.Parameters.IsNullOrEmpty())
        {
            return new DynamicParameters();
        }


        DynamicParameters parameters = new();

        foreach (CommandParameterDb parameter in commandExecution.Parameters.OrderBy(p => p.Progressive))
        {
            if (parameter.Type != null)
            {
                throw new PmDataException("Dapper call does not support structured parameters");
            }

            parameters.Add(
                parameter.Name
                , parameter.Value
                );
        }
        return parameters;
    }


    /// <summary>
    /// writes command log regardless of appsettings configuration
    /// </summary>
    /// <param name="commandExecution"></param>
    /// <param name="databaseName"></param>
    /// <param name="dynamicParameters"></param>
    private void LogDapperCommandAlways(
        CommandExecutionDb commandExecution
        , string databaseName
        , DynamicParameters dynamicParameters
        )
    {
        LogDapperCommand(
            commandExecution
            , databaseName
            , dynamicParameters
            , enableAllCommandLogging: true
            , disableAllCommandLogging: false
            );
    }


    private void LogDapperCommand(
        CommandExecutionDb commandExecution
        , string databaseName
        , DynamicParameters dynamicParameters
        , bool enableAllCommandLogging
        , bool disableAllCommandLogging
        )
    {
        string commandToLog =
            CommandLogHelper.DapperCommandAsSql(
                databaseName
                , commandExecution.CommandText
                , commandExecution.CommandType
                , dynamicParameters
                , writeCommandLog: commandExecution is null || commandExecution.WriteCommandLog
                , disableAllCommandLogging: disableAllCommandLogging
                , enableAllCommandLogging: enableAllCommandLogging
                );

        if (commandToLog.StringHasValue())
        {
            _logger.LogInformation("Command to be executed: {Command}", Environment.NewLine + commandToLog);
        }
    }





    private void ValidateState()
    {
        Guard.Against.Null(_dataAccessProperties, nameof(DataAccessProperties));

        Guard.Against.Null(
            _dataAccessProperties.SqlConnectionStringBuilder
            , nameof(_dataAccessProperties.SqlConnectionStringBuilder)
            );

        Guard.Against.NullOrWhiteSpace(
            _dataAccessProperties.SqlConnectionStringBuilder.ConnectionString
            , nameof(_dataAccessProperties.SqlConnectionStringBuilder.ConnectionString)
            );

        Guard.Against.InvalidInput(
            _dataAccessProperties.IsolationLevel
            , nameof(_dataAccessProperties.IsolationLevel)
            , (input) => input != IsolationLevel.Unspecified
            , $"value '{_dataAccessProperties.IsolationLevel}' is not allowed. Provide a specific value of {nameof(IsolationLevel)}"
            );

        Guard.Against.NegativeOrZero(
            _dataAccessProperties.SqlCommandTimeoutInSeconds
            , nameof(_dataAccessProperties.SqlCommandTimeoutInSeconds)
            );

        if (_dataAccessProperties.EnableAllCommandLogging && _dataAccessProperties.DisableAllCommandLogging)
        {
            throw new PmDataException("");
        }
    }


    private static void ValidateCommand(CommandExecutionDb commandExecution)
    {
        Guard.Against.Null(commandExecution, nameof(commandExecution));

        Guard.Against.NullOrWhiteSpace(commandExecution.CommandText, nameof(commandExecution.CommandText));
    }
}