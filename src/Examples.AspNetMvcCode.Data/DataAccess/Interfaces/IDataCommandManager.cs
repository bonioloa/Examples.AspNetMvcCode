namespace Examples.AspNetMvcCode.Data;

public interface IDataCommandManager
{
    void CommitCommandsInCurrentBatch();
    void EnqueueNonQueryCommand(CommandExecutionDb commandExecution);
    string ExecuteToGetCleanString(CommandExecutionDb commandExecution);
    long ExecuteCommitWithLongResult(CommandExecutionDb commandExecution);
    IEnumerable<T> Query<T>(CommandExecutionDb commandExecution);
    DataTable ReadData(CommandExecutionDb commandExecution);
    List<DataTable> ReadMultipleData(CommandExecutionDb commandExecution);
}