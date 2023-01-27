namespace Examples.AspNetMvcCode.Logic;

public interface IOperationResultLgc
{
    [JsonIgnore]
    bool Success { get; set; }

    [JsonIgnore]
    WarningType WarningType { get; set; }

    [JsonIgnore]
    IList<MessageField> FieldToWarnList { get; set; }

    [JsonIgnore]
    string ValuesAllowed { get; set; }
}