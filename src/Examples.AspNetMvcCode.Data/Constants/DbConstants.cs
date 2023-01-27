namespace Examples.AspNetMvcCode.Data;

//shared constants for data namespace
public static class DbConstants
{
    /// <summary>
    /// when optional filters not provided by input substitute with this invariant condition  
    /// so we don't have to worry adding or removing 
    /// ADD or OR operator
    /// </summary>
    internal const string InvariantQueryCondition = " 1 = 1 ";

    /// <summary>
    /// standard SQL conversion format ( <see href="https://www.w3schools.com/sql/func_sqlserver_convert.asp"/>) 
    /// used in database for varchar dates
    /// </summary>
    internal const string SqlVarcharDateFormatForDb = "112";

   
    internal const string SqlCodeCurrentDateStandard =
        $" CONVERT(VARCHAR(8),GETDATE(), {SqlVarcharDateFormatForDb}) ";

    //MaxDate as defined on database (value convention)
    internal readonly static DateTime AppMaxDate = new(2100, 12, 31);
    internal readonly static string MaxDateStandard = AppMaxDate.ToDbStringDateInvariant();
    internal readonly static string SqlParamMaxDateStandard = $" '{MaxDateStandard}' ";

    //minDate as defined on database (value convention)
    internal readonly static DateTime AppMinDate = new(1990, 01, 01);
    internal readonly static string MinDateStandard = AppMinDate.ToDbStringDateInvariant();
    internal readonly static string SqlParamMinDateStandard = $" '{MinDateStandard}' ";

    //default values for sql type DATE
    //see https://www.w3schools.com/sql/sql_datatypes.asp

    /// <summary>
    /// default min value for sql type DATE
    /// </summary>
    internal const string SqlTypeDateMin = "10000101";
    /// <summary>
    /// default min value for sql type DATE with varchar syntax
    /// </summary>
    internal const string SqlTypeDateMinAsChars = $"'{SqlTypeDateMin}'";

    /// <summary>
    /// default max value for sql type DATE
    /// </summary>
    internal const string SqlTypeDateMax = "99991231";
    /// <summary>
    /// default max value for sql type DATE
    /// </summary>
    internal const string SqlTypeDateMaxAsChars = $"'{SqlTypeDateMax}'";

    /// <summary>
    /// default min value for time part
    /// </summary>
    internal const string SqlTypeTimePartMin = "00:00:00";
    /// <summary>
    /// default max value for time part (24 h)
    /// </summary>
    internal const string SqlTypeTimePartMax = "23:59:59";

    //empty date defined on database (value convention)
    internal const string DateEmptyZeros = "00000000";
    internal const string SqlParamEmptyDateStandard = $" '{DateEmptyZeros}' ";

    internal const char EmailSeparator = ';';

    internal static readonly string QueryColumnSeparator = ",";
    internal static readonly string DbSqlQueryTermination = ";";

    internal const char NumericPadderChar = '0';

    internal const char SqlParameterPrefix = '@';
}