using Serilog;
using Serilog.Context;

namespace Examples.AspNetMvcCode.Data;

internal static class CommandLogHelper
{
    internal static string AdoCommandAsSql(
        this SqlCommand sc
        , bool writeCommandLog
        , bool disableAllCommandLogging
        , bool enableAllCommandLogging
        )
    {
        if (disableAllCommandLogging)
        {
            return string.Empty;
        }

        bool writeCommand = enableAllCommandLogging || writeCommandLog;
        if (!writeCommand)
        {
            return string.Empty;
        }

        StringBuilder sql = new();

        sql.AppendLine("USE " + sc.Connection.Database + DbConstants.DbSqlQueryTermination);
        switch (sc.CommandType)
        {
            case CommandType.Text:
            case CommandType.StoredProcedure:
                sql.AppendLine(sc.Parameters.BuildAdoDeclarationsAndInitializations());
                sql.AppendLine(sc.CommandText.Clean());
                break;
        }
        return sql.ToString();
    }

    #region private helping methods

    private static void AdoParameterValueForSQL(
       this SqlParameter sp
       , out string formattedValue
       , out string sizeStr
       )
    {
        sizeStr = string.Empty;

        switch (sp.SqlDbType)
        {
            case SqlDbType.Char:
            case SqlDbType.NChar:
            case SqlDbType.NText:
            case SqlDbType.NVarChar:
            case SqlDbType.Text:
            case SqlDbType.VarChar:
            case SqlDbType.Xml:
                formattedValue = "'" + sp.Value.ToString().ReplaceInvariant("'", "''") + "'";
                sizeStr = sp.Size.Valid() ? sp.Size.ToString() : "MAX";
                sizeStr = $"({sizeStr})";
                break;
            case SqlDbType.Time:
            case SqlDbType.Date:
            case SqlDbType.DateTime:
            case SqlDbType.DateTime2:
            case SqlDbType.DateTimeOffset:
                formattedValue = "'" + ((DateTime)sp.Value).ToStringDateTimeInvariant().ReplaceInvariant("'", "''") + "'";
                break;

            case SqlDbType.Bit:
                formattedValue = sp.Value.ToBooleanOrDefault(false) ? "1" : "0";
                break;

            default:
                formattedValue = sp.Value.ToString().ReplaceInvariant("'", "''");
                break;
        }
    }


    private static bool ToBooleanOrDefault(this object objToBool, bool defaultVal)
    {
        using IDisposable logScopeCurrentClass =
            LogContext.PushProperty(AppLogPropertiesKeys.ClassName, nameof(CommandLogHelper));

        using IDisposable logScopeCurrentMethod =
            LogContext.PushProperty(AppLogPropertiesKeys.MethodName, nameof(ToBooleanOrDefault));



        bool outputVal = defaultVal;
        if (objToBool is null)
        {
            return outputVal;
        }
        try
        {
            switch (objToBool.ToString().ToUpperInvariant())
            {
                case "YES":
                case "TRUE":
                case "OK":
                case "Y":
                    outputVal = true;
                    break;
                case "NO":
                case "FALSE":
                case "N":
                    outputVal = false;
                    break;
            }
        }
        catch (Exception ex)
        {
            Log.Logger.Error(
                ex,
                "error while converting value '{ObjToBool}' "
                , objToBool
                );
        }
        return outputVal;
    }


    private static string BuildAdoDeclarationsAndInitializations(
        this SqlParameterCollection par
        )
    {
        StringBuilder sql = new();
        foreach (SqlParameter sp in par)
        {
            if (sp.Direction == ParameterDirection.Input)
            {
                sp.AdoParameterValueForSQL(
                    out string formattedValue
                    , out string sizeStr
                    );

                sql.Append($"DECLARE {sp.ParameterName} {sp.SqlDbType}{sizeStr}=");

                //IMPORTANT: crypt sensible data
                if (SqlParamsNames.ParamsToObfuscateInLogs.Any(p => p.EqualsInvariant(sp.ParameterName)))
                {
                    formattedValue = CryptString(formattedValue, method);
                }

                sql.AppendLine(formattedValue + DbConstants.DbSqlQueryTermination);
            }
        }
        return sql.ToString();
    }
    #endregion







    internal static string DapperCommandAsSql(
        string databaseName
        , string commandText
        , CommandType commandType
        , DynamicParameters dynamicParameters
        , bool writeCommandLog
        , bool disableAllCommandLogging
        , bool enableAllCommandLogging
        )
    {
        if (disableAllCommandLogging)
        {
            return string.Empty;
        }

        bool writeCommand = enableAllCommandLogging || writeCommandLog;
        if (!writeCommand)
        {
            return string.Empty;
        }


        StringBuilder sql = new();

        sql.AppendLine("USE " + databaseName + DbConstants.DbSqlQueryTermination);

        switch (commandType)
        {
            case CommandType.Text:
            case CommandType.StoredProcedure:

                sql.AppendLine(dynamicParameters.BuildDapperDeclarationsAndInitializations());
                sql.AppendLine(commandText.Clean());

                break;
        }

        return sql.ToString();
    }



    private static string BuildDapperDeclarationsAndInitializations(
        this DynamicParameters dynamicParameters
        )
    {
        Guard.Against.Null(dynamicParameters, nameof(dynamicParameters));

        StringBuilder sqlBuilder = new();
        string tmpParamName;

        foreach (string paramName in dynamicParameters.ParameterNames)
        {
            tmpParamName = paramName;

            //dapper (sometimes!?) removes sql prefix. To ensure correct command generation we need this code
            if (!tmpParamName.StartsWith(DbConstants.SqlParameterPrefix))
            {
                tmpParamName = DbConstants.SqlParameterPrefix + tmpParamName;
            }

            dynamic paramValue = dynamicParameters.Get<dynamic>(paramName);

            sqlBuilder.Append($"DECLARE {tmpParamName} ");

            AddDapperParamDynamicPart(sqlBuilder, tmpParamName, paramValue);

            sqlBuilder.AppendLine(DbConstants.DbSqlQueryTermination);
        }

        return
            sqlBuilder.ToString();
    }

    //https://stackoverflow.com/questions/18529965/is-there-any-way-to-trace-log-the-sql-using-dapper
    private static void AddDapperParamDynamicPart(StringBuilder sqlBuilder, string paramName, dynamic paramValue)
    {
        dynamic type = paramValue.GetType();


        if (type == typeof(DateTime))
        {
            sqlBuilder.Append(
                $"DATETIME2 = '{((DateTime)paramValue).ToStringDateTimeInvariant().ReplaceInvariant("'", "''")}'"
                );
            return;
        }

        if (type == typeof(bool))
        {
            sqlBuilder.Append($"BIT = {((bool)paramValue ? 1 : 0)}");
            return;
        }

        if (type == typeof(byte) || type == typeof(sbyte) || type == typeof(Byte))
        {

            sqlBuilder.Append($"TINYINT = {paramValue} ;");
            return;
        }

        if (type == typeof(short) || type == typeof(ushort) || type == typeof(Int16) || type == typeof(UInt16))
        {

            sqlBuilder.Append($"SMALLINT = {paramValue}");
            return;
        }

        if (type == typeof(int) || type == typeof(uint) || type == typeof(Int32) || type == typeof(UInt32))
        {

            sqlBuilder.Append($"INT = {paramValue}");
            return;
        }

        if (type == typeof(long) || type == typeof(ulong) || type == typeof(Int64) || type == typeof(UInt64))
        {
            sqlBuilder.Append($"BIGINT = {paramValue}");
            return;
        }

        if (type == typeof(decimal))
        {
            decimal number = (decimal)paramValue;

            //https://stackoverflow.com/questions/33490408/mathematically-determine-the-precision-and-scale-of-a-decimal-value
            //https://stackoverflow.com/questions/3281865/determine-the-decimal-precision-of-an-input-number
            sqlBuilder.Append($"DECIMAL({GetPrecision(number)},{GetScale(number)}) = {number.ToString(CultureInfo.InvariantCulture)}");
            return;
        }

        if (type == typeof(float) || type == typeof(Single))
        {
            //omit real type for now
            //sqlBuilder.AppendLine($"REAL = {((Single)paramValue).ToString(CultureInfo.InvariantCulture)}");
            //https://docs.microsoft.com/it-it/sql/t-sql/data-types/float-and-real-transact-sql?view=sql-server-ver16
            sqlBuilder.Append($"FLOAT(53) = {((float)paramValue).ToString(CultureInfo.InvariantCulture)}");
            return;
        }

        if (type == typeof(double))
        {
            //maybe find a way to map precision dynamically
            sqlBuilder.Append($"FLOAT(53) = {((double)paramValue).ToString(CultureInfo.InvariantCulture)}");
            return;
        }

        if (type == typeof(byte[]))
        {
            sqlBuilder.Append($"VARBINARY = binary command not repeatable");
            return;
        }

        if (type == typeof(string) || type == typeof(IHtmlContent) || type == typeof(HtmlString))
        {
            string paramValueString = string.Empty;
            if (type == typeof(string))
            {
                paramValueString = paramValue.ToString();
            }
            if (type == typeof(IHtmlContent) || type == typeof(HtmlString))
            {
                paramValueString = (paramValue as IHtmlContent).GetStringContent();
            }

            if (SqlParamsNames.ParamsToObfuscateInLogs.Any(p => p.EqualsInvariant(paramName)))
            {
                paramValueString = CryptString(paramValueString, method);
            }

            paramValueString = paramValueString.ReplaceInvariant("'", "''");

            int valueLength = paramValueString.Length;

            int nvarcharMaxLength = 4000;

            string paramLength =
                valueLength > nvarcharMaxLength
                ? "MAX"
                : valueLength.ToString();

            sqlBuilder.Append($"NVARCHAR({paramLength}) = '{paramValueString}'");

            return;
        }

        //for now don't handle parameter as list, maybe in future
        //else if (type == typeof(List<int>))
        //    sb.AppendFormat("-- REPLACE @{0} IN SQL: ({1})\n", paramName, string.Join(",", (List<int>)pValue));

        throw new PmDataException(
            $"unhandled type for parameter '{paramName}', type: '{type.ToString()}', value '{paramValue.ToString()}' "
            );

    }

    //more info on sql and c# mapping
    //https://stackoverflow.com/questions/425389/c-sharp-equivalent-of-sql-server-datatypes
    //https://docs.microsoft.com/en-us/sql/relational-databases/clr-integration-database-objects-types-net-framework/mapping-clr-parameter-data?view=sql-server-ver15&redirectedfrom=MSDN&viewFallbackFrom=sql-server-2014
    //https://stackoverflow.com/questions/1058322/anybody-got-a-c-sharp-function-that-maps-the-sql-datatype-of-a-column-to-its-clr
    //https://stackoverflow.com/questions/33490408/mathematically-determine-the-precision-and-scale-of-a-decimal-value
    public static int GetScale(decimal value)
    {
        if (value == 0)
        {
            return 0;
        }
        int[] bits = decimal.GetBits(value);
        return (bits[3] >> 16) & 0x7F;
    }

    public static int GetPrecision(decimal value)
    {
        if (value == 0)
        {
            return 0;
        }

        int[] bits = decimal.GetBits(value);
        //We will use false for the sign (false =  positive), because we don't care about it.
        //We will use 0 for the last argument instead of bits[3] to eliminate the fraction point.
        decimal d = new(bits[0], bits[1], bits[2], false, 0);
        return (int)Math.Floor(Math.Log10((double)d)) + 1;
    }
}