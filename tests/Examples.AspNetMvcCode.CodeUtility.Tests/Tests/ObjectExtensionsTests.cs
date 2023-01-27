namespace Examples.AspNetMvcCode.CodeUtility.Tests;

public class ObjectExtensionsTests
{

    [Fact]
    public void TryParseToNumericTest()
    {
        bool success;
        decimal numericValue;
        object obj;

        obj = null;
        (success, numericValue) = obj.TryParseToNumericInvariant();
        Assert.False(success);
        Assert.True(decimal.MinValue == numericValue);

        (success, numericValue) = string.Empty.TryParseToNumericInvariant();
        Assert.False(success);
        Assert.True(decimal.MinValue == numericValue);

        long longvalue = 1234567890123;
        (success, numericValue) = longvalue.TryParseToNumericInvariant();
        Assert.True(success);
        Assert.True(longvalue == numericValue);

        double doubleValue = 12345.754654;
        (success, numericValue) = doubleValue.TryParseToNumericInvariant();
        Assert.True(success);
        Assert.True((decimal)doubleValue == numericValue);

        string stringNum = doubleValue.ToString(CultureInfo.InvariantCulture);
        (success, numericValue) = stringNum.TryParseToNumericInvariant();
        Assert.True(success);
        Assert.True((decimal)doubleValue == numericValue);
    }

    public enum TestResult
    {
        Ok,
        No,
    }
    [Fact]
    public void IsNumericStructTypeTest()
    {
        int intVal = 3422423;
        Assert.True(intVal.IsNumericStructType());

        double doubleVal = 34224.3224223;
        Assert.True(doubleVal.IsNumericStructType());

        Int64 int64Val = 132112421421412421;
        Assert.True(int64Val.IsNumericStructType());

        object objVal = null;
        Assert.False(objVal.IsNumericStructType());

        string strVal = doubleVal.ToString(CultureInfo.InvariantCulture);
        Assert.False(strVal.IsNumericStructType());

        TestResult testEnum = TestResult.Ok;
        Assert.False(testEnum.IsNumericStructType());
    }

    //[Fact]
    //public void Test()
    //{
    //  Assert.True();
    //
    //  Assert.False();
    //}
}