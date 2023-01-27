namespace Examples.AspNetMvcCode.Data;

public interface IProgressiveWriteQueries
{
    long CreateAndCommitNewIdForDataGridState();
    long CreateAndCommitNewIdForItem();
    long CreateAndCommitNewIdForAttachment();
    long CreateAndCommitNewIdForForm();
    long CreateAndCommitNewIdForMessage();
    long CreateAndCommitNewProgressiveItemByCurrentProcess(long processId);
}