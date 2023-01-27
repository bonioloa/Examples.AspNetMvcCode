namespace Examples.AspNetMvcCode.Data;

internal static class DbUtility
{
    internal const string DbItalianLanguage = "ITA";
    internal const string DbItalianIsoLanguage = "it";
    internal const string DbEnglishLanguage = "ENG";
    internal const string DbEnglishIsoLanguage = "en";
    internal const string DbSpanishLanguage = "ESP";
    internal const string DbSpanishIsoLanguage = "es";

    /// <summary>
    /// build construct to use for a "IN" SQL condition. Format "('par1','par3')"
    /// This is done as constants, without defining parameters in sql command.
    /// Don't use with user input to prevent injection
    /// </summary>
    /// <param name="inPar"></param>
    /// <returns></returns>
    internal static string BuildInWithConstants(IEnumerable<string> inPar)
    {
        if (inPar.IsNullOrEmpty())
        {
            throw new PmDataException($"empty parameters list provided");
        }
        //empty parameters will be ignored
        return " ( '" + string.Join("','", inPar.Where(p => p.StringHasValue())) + "' ) ";
    }



    /// <summary>
    /// build construct to use for a "IN" SQL condition. Format "(123,456)"
    /// This is done as constants, without defining parameters in sql command
    /// Don't use with user input to prevent injection
    /// </summary>
    /// <param name="inPar"></param>
    /// <returns></returns>
    /// <exception cref="PmDataException"></exception>
    internal static string BuildInWithConstants(IEnumerable<long> inPar)
    {
        if (inPar.IsNullOrEmpty())
        {
            throw new PmDataException($"empty parameters list provided");
        }
        return " ( " + string.Join(",", inPar) + " ) ";
    }



    /// <summary>
    /// create the required parts to build a IN condition with parameters.
    /// To be used with user input, parameters will prevent in injection
    /// </summary>
    /// <param name="columnForIn"></param>
    /// <param name="paramsPrefix"></param>
    /// <param name="values"></param>
    /// <param name="tableAlias">OPTIONAL</param>
    /// <returns>in case <paramref name="values"/> is empty returns <see cref="DbConstants.InvariantQueryCondition"/></returns>
    /// <remarks>don't use this query in a batch, because there is a chance of parameter conflict if this method is used
    /// in other queries of batch
    /// </remarks>
    internal static (
        IDictionary<string, T> parameters
        , string inQueryPart
        ) BuildInParametrizedArgs<T>(
        string paramsPrefix
        , IEnumerable<T> values
        , string columnForIn
        , string tableAlias = ""
        )
    {
        IDictionary<string, T> parameters = new Dictionary<string, T>();

        if (values.IsNullOrEmpty())
        {
            return (parameters, DbConstants.InvariantQueryCondition);
        }

        IList<string> inQueryParams = new List<string>();

        string tmpParameterName;
        int index = 0;
        foreach (T value in values)
        {
            tmpParameterName = paramsPrefix + index.ToString().PadLeft(3, DbConstants.NumericPadderChar);
            parameters.Add(tmpParameterName, value);
            inQueryParams.Add(tmpParameterName);
            index++;
        }

        if (tableAlias.StringHasValue())
        {
            tableAlias += ".";
        }


        //empty parameters will be ignored
        return (
            parameters
            , $" {tableAlias}{columnForIn} IN ( {string.Join(",", inQueryParams)} ) "
            );
    }


    /// <summary>
    /// Convert a iso code to the conventional string suffix used in localized tables
    /// </summary>
    /// <param name="localizationIsoCode"></param>
    /// <returns></returns>
    /// <remarks>use this method only when you can't use current language, but arbitrary localization
    /// (<see cref="ContextAppExtensions.LanguageSuffix(ContextApp)"/>)</remarks>
    internal static string GetOldSchemaLangSuffix(string localizationIsoCode)
    {
        bool found = false;
        string dbSuffix = string.Empty;//default suffix for italiano

        if (!found
            && localizationIsoCode.EqualsInvariant(SupportedCulturesConstants.IsoCodeEnglish))
        {
            dbSuffix = "_" + DbEnglishLanguage;
            found = true;
        }

        if (!found
            && localizationIsoCode.EqualsInvariant(SupportedCulturesConstants.IsoCodeSpanish))
        {
            dbSuffix = "_" + DbSpanishLanguage;
            //found = true;
        }

        return dbSuffix;
    }



    /// <summary>
    /// converts db time string to standard time format.
    /// </summary>
    /// <param name="tableAlias"></param>
    /// <param name="timeColumnName"></param>
    /// <returns></returns>
    /// <remarks>milliseconds are not handled because they are not used in db columns</remarks>
    private static string GetSqlCodeToConvertDbTimeToSqlTime(string tableAlias, string timeColumnName)
    {
        if (tableAlias.StringHasValue() && !tableAlias.EndsWith("."))
        {
            tableAlias += ".";
        }

        return $" STUFF(STUFF({tableAlias + timeColumnName}, 3, 0, ':'), 6, 0, ':') ";
    }



    /// <summary>
    /// date and time in db sometimes are stored in two separate columns, this function builds sql code
    /// to format the two columns in a string convertible by sql
    /// </summary>
    /// <param name="tableAlias"></param>
    /// <param name="dateColumnName"></param>
    /// <param name="timeColumnName"></param>
    /// <returns></returns>
    internal static string GetSqlCodeToConvertDbDateTimeToSqlDateTime(
        string tableAlias
        , string dateColumnName
        , string timeColumnName
        )
    {
        return
            GetSqlCodeToConvertDbDateTimeToSqlDateTime(
                tableAlias: tableAlias
                , dateColumnName: dateColumnName
                , timeColumnName: timeColumnName
                , sqlDateTimeCoalescing: SqlDateTimeCoalescing.None
                );
    }



    /// <summary>
    /// date and time in db sometimes are stored in two separate columns, this function builds sql code
    /// to format the two columns in a string convertible by sql.
    /// </summary>
    /// <param name="tableAlias"></param>
    /// <param name="dateColumnName"></param>
    /// <param name="timeColumnName"></param>
    /// <param name="sqlDateTimeCoalescing">a condition that allows to specify what kind of default to use for time and date part</param>
    /// <returns></returns>
    internal static string GetSqlCodeToConvertDbDateTimeToSqlDateTime(
        string tableAlias
        , string dateColumnName
        , string timeColumnName
        , SqlDateTimeCoalescing sqlDateTimeCoalescing
        )
    {
        if (tableAlias.StringHasValue())
        {
            tableAlias += ".";
        }

        string defaultSqlDate;
        string defaultSqlTime;
        switch (sqlDateTimeCoalescing)
        {
            case SqlDateTimeCoalescing.MinDateTime:
                defaultSqlDate = DbConstants.SqlTypeDateMin;
                defaultSqlTime = DbConstants.SqlTypeTimePartMin;
                break;

            case SqlDateTimeCoalescing.MaxDateTime:
                defaultSqlDate = DbConstants.SqlTypeDateMax;
                defaultSqlTime = DbConstants.SqlTypeTimePartMax;
                break;

            default:
                return
                    $" CONVERT(DATETIME2, {tableAlias + dateColumnName} + ' ' + {GetSqlCodeToConvertDbTimeToSqlTime(tableAlias, timeColumnName)}) ";

        }

        return
            $@" CONVERT(DATETIME2,
                            COALESCE({tableAlias + dateColumnName}, '{defaultSqlDate}') 
                                + ' ' + COALESCE({GetSqlCodeToConvertDbTimeToSqlTime(tableAlias, timeColumnName)}, '{defaultSqlTime}')
                        ) ";
    }



    /// <summary>
    /// add trailing spaces to <paramref name="stringToMakeUnique"/> if this string already exists in <paramref name="alreadyUsedStrings"/>.<br/>
    /// Necessary in case <paramref name="stringToMakeUnique"/> is used for building objects that need this string to be unique 
    /// </summary>
    /// <param name="alreadyUsedStrings"></param>
    /// <param name="stringToMakeUnique"></param>
    /// <returns></returns>
    /// <exception cref="PmDataException"></exception>
    internal static string FormatStringUnique(
        ref HashSet<string> alreadyUsedStrings
        , string stringToMakeUnique
        )
    {
        //all this could be done with fancy recursion but with the while we can have a cycle safeguard, with recursion no
        Guard.Against.NullOrWhiteSpace(stringToMakeUnique, nameof(stringToMakeUnique), "This method is intended to be used for non empty strings");

        Guard.Against.Null(alreadyUsedStrings, nameof(alreadyUsedStrings), $"{nameof(alreadyUsedStrings)} must be initialized externally");


        string stringFormatted = stringToMakeUnique;

        bool isStringUnique = false;

        int maxCycles = 1000;
        int infiniteSafeguard = 0;

        while (!isStringUnique)
        {
            if (infiniteSafeguard >= maxCycles)
            {
                throw new PmDataException(
                    @$"reached max cycles, something went very wrong. 
                    {nameof(stringToMakeUnique)}; '{stringToMakeUnique}'; 
                    {nameof(alreadyUsedStrings)}: '{alreadyUsedStrings}' "
                    );
            }

            isStringUnique = alreadyUsedStrings.Add(stringFormatted);
            if (!isStringUnique)
            {
                //concatenate trailing spaces until string is different from others already used.
                //Spaces in html representation will be truncated but this logic
                //is crucial for report writing which needs unique columns names
                stringFormatted += " ";
            }

            infiniteSafeguard++;
        }


        return stringFormatted;
    }
}