namespace Examples.AspNetMvcCode.CodeUtility.Tests;

public class StringNumericsExtensionsTests
{
    private const string MinValueDecimal = "MinValue";
    private const string NullStr = null;



    [Theory]
    [InlineData("-1234567.678", null, true, "-1234567.678")]
    [InlineData("1,234,567.678", null, true, "1234567.678")]
    [InlineData("-1234567,678", "it", true, "-1234567.678")]
    [InlineData("1.234.567,678", "it", true, "1234567.678")]

    [InlineData("-1,234", null, true, "-1234")]//this can be a cause of many problems
    [InlineData("567.678", "it", true, "567678")]

    [InlineData(NullStr, null, false, MinValueDecimal)]
    [InlineData(NullStr, "it", false, MinValueDecimal)]

    [InlineData("1.234.567,678", null, false, MinValueDecimal)]//fail because of double decimal separator (dot)
    [InlineData("-1.234.567,678", null, false, MinValueDecimal)]

    [InlineData("1,234,567.678", "it", false, MinValueDecimal)]//fail because of double decimal separator (comma)
    [InlineData("-1,234,567.678", "it", false, MinValueDecimal)]
    public void TryParseSafeTest(
        string input
        , string cultureIsoCode
        , bool expectedOutput
        , string expectedOutResultCultureInvariant //decimals can't be constants, neither minvalue
        )
    {
        bool success = cultureIsoCode.Empty()
            ? input.TryParseSafe(out decimal number)
            : input.TryParseSafe(out number, new CultureInfo(cultureIsoCode));

        decimal expectedOutResult = decimal.MinValue;
        if (!expectedOutResultCultureInvariant.EqualsInvariant(MinValueDecimal))
        {
            expectedOutResult = decimal.Parse(expectedOutResultCultureInvariant, CultureInfo.InvariantCulture);
        }

        Assert.Equal(success, expectedOutput);
        Assert.Equal(number, expectedOutResult);
    }



    [Theory]
    [InlineData(NullStr, null, false, MinValueDecimal)]
    [InlineData(NullStr, "it", false, MinValueDecimal)]

    [InlineData("-1234567.678", null, true, "-1234567.678")]
    [InlineData("-1234567,678", "it", true, "-1234567.678")]

    [InlineData("1,234,567.678", null, false, MinValueDecimal)]
    [InlineData("1,234,567.678", "it", false, MinValueDecimal)]

    [InlineData("1.234.567,678", null, false, MinValueDecimal)]
    [InlineData("1.234.567,678", "it", false, MinValueDecimal)]

    public void TryParseSafeNoThousandsSeparatorTest(
        string input
        , string cultureIsoCode
        , bool expectedOutput
        , string expectedOutResultCultureInvariant //decimals can't be constants, neither minvalue
        )
    {
        bool success = cultureIsoCode.Empty()
            ? input.TryParseSafeNoThousandsSeparator(out decimal number)
            : input.TryParseSafeNoThousandsSeparator(out number, new CultureInfo(cultureIsoCode));

        decimal expectedOutResult = decimal.MinValue;
        if (!expectedOutResultCultureInvariant.EqualsInvariant(MinValueDecimal))
        {
            expectedOutResult = decimal.Parse(expectedOutResultCultureInvariant, CultureInfo.InvariantCulture);
        }

        Assert.Equal(success, expectedOutput);
        Assert.Equal(number, expectedOutResult);
    }
}