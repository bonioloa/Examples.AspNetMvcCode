namespace Examples.AspNetMvcCode.Common;

public static class StepStateGroupTypeExtensions
{
    public static bool IsOpen(this StepStateGroupType type)
    {
        return type == StepStateGroupType.Open;
    }

    public static bool IsClosed(this StepStateGroupType type)
    {
        return type == StepStateGroupType.Closed;
    }

    public static bool IsAborted(this StepStateGroupType type)
    {
        return type == StepStateGroupType.Aborted;
    }

    public static bool IsTerminated(this StepStateGroupType type)
    {
        return !type.IsOpen();
    }
}