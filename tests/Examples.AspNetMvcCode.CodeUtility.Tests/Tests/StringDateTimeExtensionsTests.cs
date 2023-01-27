namespace Examples.AspNetMvcCode.CodeUtility.Tests;

public class StringDateTimeExtensionsTests
{
    private static readonly string nullString = null;
    private static readonly string emptyString = string.Empty;

    [Fact]
    public void TestTryParseDbDateInvariant()
    {
        //null and empty are allowed values
        bool success = nullString.TryParseDbDateInvariant(out DateTime date);
        Assert.True(success);
        Assert.True(DateTime.MinValue.Date == date);

        success = emptyString.TryParseDbDateInvariant(out date);
        Assert.True(success);
        Assert.True(DateTime.MinValue.Date == date);


        success = "20201231".TryParseDbDateInvariant(out date);
        Assert.True(success);
        Assert.True(new DateTime(2020, 12, 31) == date);

        success = "99991231".TryParseDbDateInvariant(out date);
        Assert.True(success);
        Assert.True(DateTime.MaxValue.Date == date);

        success = "00010101".TryParseDbDateInvariant(out date);
        Assert.True(success);
        Assert.True(DateTime.MinValue.Date == date);

        //fail not a string date
        success = "vcweckwnvwc".TryParseDbDateInvariant(out date);
        Assert.False(success);
        Assert.True(DateTime.MinValue.Date == date);

        //fail wrong format
        success = "2121-09-12".TryParseDbDateInvariant(out date);
        Assert.False(success);
        Assert.True(DateTime.MinValue.Date == date);

        //fail invalid date
        success = "21213232".TryParseDbDateInvariant(out date);
        Assert.False(success);
        Assert.True(DateTime.MinValue.Date == date);
    }


    #region TryParseSortableDateTimeInvariantStandard
    [Fact]
    public void TestNullEmptyTryParseSortableDateTimeInvariantStandard()
    {
        bool success = nullString.TryParseSortableDateTimeInvariantStandard(out DateTime dateTime);
        Assert.True(success);
        Assert.True(DateTime.MinValue == dateTime);

        success = emptyString.TryParseSortableDateTimeInvariantStandard(out dateTime);
        Assert.True(success);
        Assert.True(DateTime.MinValue == dateTime);
    }


    [Fact]
    public void TestValidValuesTryParseSortableDateTimeInvariantStandard()
    {
        bool success = "2020-12-31T23:12:01".TryParseSortableDateTimeInvariantStandard(out DateTime dateTime);
        Assert.True(success);
        Assert.True(new DateTime(2020, 12, 31, 23, 12, 01) == dateTime);

        success = "9999-12-31T23:59:59".TryParseSortableDateTimeInvariantStandard(out dateTime);
        Assert.True(success);
        Assert.True(new DateTime(9999, 12, 31, 23, 59, 59) == dateTime);

        success = "0001-01-01T00:00:00".TryParseSortableDateTimeInvariantStandard(out dateTime);
        Assert.True(success);
        Assert.True(new DateTime(1, 1, 1, 0, 0, 0) == dateTime);
    }


    [Fact]
    public void TestInvalidValuesTryParseSortableDateTimeInvariantStandard()
    {
        //fail not a string date
        bool success = "vcweckwnvwc".TryParseSortableDateTimeInvariantStandard(out DateTime dateTime);
        Assert.False(success);
        Assert.True(DateTime.MinValue == dateTime);

        //fail wrong format
        success = "2121-09-12".TryParseSortableDateTimeInvariantStandard(out dateTime);
        Assert.False(success);
        Assert.True(DateTime.MinValue == dateTime);

        //valid format but invalid date
        success = "2121-32-32T99:80:70".TryParseSortableDateTimeInvariantStandard(out dateTime);
        Assert.False(success);
        Assert.True(DateTime.MinValue == dateTime);
    }
    #endregion



    [Fact]
    public void TestTryParseSortableDateInvariant()
    {
        //null and empty are allowed values
        bool success = nullString.TryParseSortableDateInvariant(out DateTime date);
        Assert.True(success);
        Assert.True(DateTime.MinValue.Date == date);

        success = emptyString.TryParseSortableDateInvariant(out date);
        Assert.True(success);
        Assert.True(DateTime.MinValue.Date == date);


        success = "2020-12-31".TryParseSortableDateInvariant(out date);
        Assert.True(success);
        Assert.True(new DateTime(2020, 12, 31) == date);

        success = "9999-12-31".TryParseSortableDateInvariant(out date);
        Assert.True(success);
        Assert.True(DateTime.MaxValue.Date == date);

        success = "0001-01-01".TryParseSortableDateInvariant(out date);
        Assert.True(success);
        Assert.True(DateTime.MinValue.Date == date);


        //fail not a string date
        success = "vcweckwnvwc".TryParseSortableDateInvariant(out date);
        Assert.False(success);
        Assert.True(DateTime.MinValue.Date == date);

        //fail wrong format
        success = "21210912".TryParseSortableDateInvariant(out date);
        Assert.False(success);
        Assert.True(DateTime.MinValue.Date == date);

        //fail invalid date
        success = "2121-32-32".TryParseSortableDateInvariant(out date);
        Assert.False(success);
        Assert.True(DateTime.MinValue.Date == date);
    }


    [Fact]
    public void TestTryParseDbDateTimeShortInvariant()
    {
        //null and empty are allowed values
        bool success = nullString.TryParseDbDateTimeShortInvariant(out DateTime dateTime);
        Assert.True(success);
        Assert.True(DateTime.MinValue == dateTime);

        success = emptyString.TryParseDbDateTimeShortInvariant(out dateTime);
        Assert.True(success);
        Assert.True(DateTime.MinValue == dateTime);


        success = "202012312312".TryParseDbDateTimeShortInvariant(out dateTime);
        Assert.True(success);
        Assert.True(new DateTime(2020, 12, 31, 23, 12, 00) == dateTime);

        success = "999912312359".TryParseDbDateTimeShortInvariant(out dateTime);
        Assert.True(success);
        Assert.True(new DateTime(9999, 12, 31, 23, 59, 00) == dateTime);

        success = "000101010000".TryParseDbDateTimeShortInvariant(out dateTime);
        Assert.True(success);
        Assert.True(new DateTime(1, 1, 1, 0, 0, 0) == dateTime);

        //fail not a string date
        success = "vcweckwnvwc".TryParseDbDateTimeShortInvariant(out dateTime);
        Assert.False(success);
        Assert.True(DateTime.MinValue == dateTime);

        //fail wrong format
        success = "2121-09-12".TryParseDbDateTimeShortInvariant(out dateTime);
        Assert.False(success);
        Assert.True(DateTime.MinValue == dateTime);

        //fail invalid date
        success = "212132329980".TryParseDbDateTimeShortInvariant(out dateTime);
        Assert.False(success);
        Assert.True(DateTime.MinValue == dateTime);
    }


    [Fact]
    public void TestTryParseDbDateTimeLongInvariant()
    {
        //null and empty are allowed values
        bool success = nullString.TryParseDbDateTimeLongInvariant(out DateTime dateTime);
        Assert.True(success);
        Assert.True(DateTime.MinValue == dateTime);

        success = emptyString.TryParseDbDateTimeLongInvariant(out dateTime);
        Assert.True(success);
        Assert.True(DateTime.MinValue == dateTime);


        success = "20201231231201".TryParseDbDateTimeLongInvariant(out dateTime);
        Assert.True(success);
        Assert.True(new DateTime(2020, 12, 31, 23, 12, 01) == dateTime);

        success = "99991231235959".TryParseDbDateTimeLongInvariant(out dateTime);
        Assert.True(success);
        Assert.True(new DateTime(9999, 12, 31, 23, 59, 59) == dateTime);

        success = "00010101000000".TryParseDbDateTimeLongInvariant(out dateTime);
        Assert.True(success);
        Assert.True(new DateTime(1, 1, 1, 0, 0, 0) == dateTime);

        //fail not a string date
        success = "vcweckwnvwc".TryParseDbDateTimeLongInvariant(out dateTime);
        Assert.False(success);
        Assert.True(DateTime.MinValue == dateTime);

        //fail wrong format
        success = "2121-09-12".TryParseDbDateTimeLongInvariant(out dateTime);
        Assert.False(success);
        Assert.True(DateTime.MinValue == dateTime);

        //fail invalid date
        success = "21213232998070".TryParseDbDateTimeLongInvariant(out dateTime);
        Assert.False(success);
        Assert.True(DateTime.MinValue == dateTime);
    }
}