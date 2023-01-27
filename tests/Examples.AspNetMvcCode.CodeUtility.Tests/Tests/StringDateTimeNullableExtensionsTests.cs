namespace Examples.AspNetMvcCode.CodeUtility.Tests;

public class StringDateTimeNullableExtensionsTests
{
    private static readonly string nullString = null;
    private static readonly string emptyString = string.Empty;

    [Fact]
    public void TestTryParseDbDateInvariantToNullable()
    {
        //null and empty are allowed values
        bool success = nullString.TryParseDbDateInvariantToNullable(out DateTime? date);
        Assert.True(success);
        Assert.True(null == date);

        success = emptyString.TryParseDbDateInvariantToNullable(out date);
        Assert.True(success);
        Assert.True(null == date);


        success = "20201231".TryParseDbDateInvariantToNullable(out date);
        Assert.True(success);
        Assert.True(new DateTime(2020, 12, 31) == date);

        success = "99991231".TryParseDbDateInvariantToNullable(out date);
        Assert.True(success);
        Assert.True(DateTime.MaxValue.Date == date);

        success = "00010101".TryParseDbDateInvariantToNullable(out date);
        Assert.True(success);
        Assert.True(DateTime.MinValue.Date == date);

        //fail not a string date
        success = "vcweckwnvwc".TryParseDbDateInvariantToNullable(out date);
        Assert.False(success);
        Assert.True(null == date);

        //fail wrong format
        success = "2121-09-12".TryParseDbDateInvariantToNullable(out date);
        Assert.False(success);
        Assert.True(null == date);

        //fail invalid date
        success = "21213232".TryParseDbDateInvariantToNullable(out date);
        Assert.False(success);
        Assert.True(null == date);
    }


    [Fact]
    public void TestTryParseSortableDateInvariantToNullable()
    {
        //null and empty are allowed values
        bool success = nullString.TryParseSortableDateInvariantToNullable(out DateTime? date);
        Assert.True(success);
        Assert.True(null == date);

        success = emptyString.TryParseSortableDateInvariantToNullable(out date);
        Assert.True(success);
        Assert.True(null == date);


        success = "2020-12-31".TryParseSortableDateInvariantToNullable(out date);
        Assert.True(success);
        Assert.True(new DateTime(2020, 12, 31) == date);

        success = "9999-12-31".TryParseSortableDateInvariantToNullable(out date);
        Assert.True(success);
        Assert.True(DateTime.MaxValue.Date == date);

        success = "0001-01-01".TryParseSortableDateInvariantToNullable(out date);
        Assert.True(success);
        Assert.True(DateTime.MinValue.Date == date);


        //fail not a string date
        success = "vcweckwnvwc".TryParseSortableDateInvariantToNullable(out date);
        Assert.False(success);
        Assert.True(null == date);

        //fail wrong format
        success = "21210912".TryParseSortableDateInvariantToNullable(out date);
        Assert.False(success);
        Assert.True(null == date);

        //fail invalid date
        success = "2121-32-32".TryParseSortableDateInvariantToNullable(out date);
        Assert.False(success);
        Assert.True(null == date);
    }


    [Fact]
    public void TestTryParseDbDateTimeShortInvariantToNullable()
    {
        //null and empty are allowed values
        bool success = nullString.TryParseDbDateTimeShortInvariantToNullable(out DateTime? date);
        Assert.True(success);
        Assert.True(null == date);

        success = emptyString.TryParseDbDateTimeShortInvariantToNullable(out date);
        Assert.True(success);
        Assert.True(null == date);


        success = "202012312312".TryParseDbDateTimeShortInvariantToNullable(out date);
        Assert.True(success);
        Assert.True(new DateTime(2020, 12, 31, 23, 12, 00) == date);

        success = "999912312359".TryParseDbDateTimeShortInvariantToNullable(out date);
        Assert.True(success);
        Assert.True(new DateTime(9999, 12, 31, 23, 59, 00) == date);

        success = "000101010000".TryParseDbDateTimeShortInvariantToNullable(out date);
        Assert.True(success);
        Assert.True(new DateTime(1, 1, 1, 0, 0, 0) == date);

        //fail not a string date
        success = "vcweckwnvwc".TryParseDbDateTimeShortInvariantToNullable(out date);
        Assert.False(success);
        Assert.True(null == date);

        //fail wrong format
        success = "2121-09-12".TryParseDbDateTimeShortInvariantToNullable(out date);
        Assert.False(success);
        Assert.True(null == date);

        //fail invalid date
        success = "212132329980".TryParseDbDateTimeShortInvariantToNullable(out date);
        Assert.False(success);
        Assert.True(null == date);
    }


    [Fact]
    public void TestTryParseDbDateTimeLongInvariantToNullable()
    {
        //null and empty are allowed values
        bool success = nullString.TryParseDbDateTimeLongInvariantToNullable(out DateTime? date);
        Assert.True(success);
        Assert.True(null == date);

        success = emptyString.TryParseDbDateTimeLongInvariantToNullable(out date);
        Assert.True(success);
        Assert.True(null == date);


        success = "20201231231201".TryParseDbDateTimeLongInvariantToNullable(out date);
        Assert.True(success);
        Assert.True(new DateTime(2020, 12, 31, 23, 12, 01) == date);

        success = "99991231235959".TryParseDbDateTimeLongInvariantToNullable(out date);
        Assert.True(success);
        Assert.True(new DateTime(9999, 12, 31, 23, 59, 59) == date);

        success = "00010101000000".TryParseDbDateTimeLongInvariantToNullable(out date);
        Assert.True(success);
        Assert.True(new DateTime(1, 1, 1, 0, 0, 0) == date);

        //fail not a string date
        success = "vcweckwnvwc".TryParseDbDateTimeLongInvariantToNullable(out date);
        Assert.False(success);
        Assert.True(null == date);

        //fail wrong format
        success = "2121-09-12".TryParseDbDateTimeShortInvariantToNullable(out date);
        Assert.False(success);
        Assert.True(null == date);

        //fail invalid date
        success = "21213232998070".TryParseDbDateTimeShortInvariantToNullable(out date);
        Assert.False(success);
        Assert.True(null == date);
    }



    #region TryParseSortableDateTimeInvariantStandardToNullable

    [Fact]
    public void TestNullEmptyTryParseSortableDateTimeInvariantStandardToNullable()
    {
        bool success = nullString.TryParseSortableDateTimeInvariantStandardToNullable(out DateTime? dateTime);
        Assert.True(success);
        Assert.True(null == dateTime);

        success = emptyString.TryParseSortableDateTimeInvariantStandardToNullable(out dateTime);
        Assert.True(success);
        Assert.True(null == dateTime);
    }


    [Fact]
    public void TestValidValuesTryParseSortableDateTimeInvariantStandardToNullable()
    {
        bool success = "2020-12-31T23:12:01".TryParseSortableDateTimeInvariantStandardToNullable(out DateTime? dateTime);
        Assert.True(success);
        Assert.True(new DateTime(2020, 12, 31, 23, 12, 01) == dateTime);

        success = "9999-12-31T23:59:59".TryParseSortableDateTimeInvariantStandardToNullable(out dateTime);
        Assert.True(success);
        Assert.True(new DateTime(9999, 12, 31, 23, 59, 59) == dateTime);

        success = "0001-01-01T00:00:00".TryParseSortableDateTimeInvariantStandardToNullable(out dateTime);
        Assert.True(success);
        Assert.True(new DateTime(1, 1, 1, 0, 0, 0) == dateTime);
    }


    [Fact]
    public void TestInvalidValuesTryParseSortableDateTimeInvariantStandardToNullable()
    {
        //fail not a string date
        bool success = "vcweckwnvwc".TryParseSortableDateTimeInvariantStandardToNullable(out DateTime? dateTime);
        Assert.False(success);
        Assert.True(null == dateTime);

        //fail wrong format
        success = "2121-09-12".TryParseSortableDateTimeInvariantStandardToNullable(out dateTime);
        Assert.False(success);
        Assert.True(null == dateTime);

        //valid format but invalid date
        success = "2121-32-32T99:80:70".TryParseSortableDateTimeInvariantStandardToNullable(out dateTime);
        Assert.False(success);
        Assert.True(null == dateTime);
    }
    #endregion
}