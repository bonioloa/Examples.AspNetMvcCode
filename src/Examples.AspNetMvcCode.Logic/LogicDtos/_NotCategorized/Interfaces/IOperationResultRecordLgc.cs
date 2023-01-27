namespace Examples.AspNetMvcCode.Logic;

internal interface IOperationResultRecordLgc
{
    [JsonIgnore]
    bool Success { get; init; }

    /// <summary>
    /// this is for error messages ad hoc and used only once, not worth to be mapped automatically with a warning type
    /// </summary>
    [JsonIgnore]
    string ErrorMessage { get; init; }
}