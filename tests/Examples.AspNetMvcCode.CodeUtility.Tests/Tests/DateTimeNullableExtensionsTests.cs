namespace Examples.AspNetMvcCode.CodeUtility.Tests;

public class DateTimeNullableExtensionsTests
{
    private static readonly CultureInfo itCulture = new("it");
    private static readonly CultureInfo enUsCulture = new("en-us");
    private static readonly CultureInfo frCulture = new("fr");
    private static readonly CultureInfo esCulture = new("es");
    private static readonly CultureInfo enGbCulture = new("en-gb");
    private static readonly CultureInfo enGeneric = new("en");

    private static readonly DateTime? date1 = new(2020, 12, 1);
    private static readonly DateTime? dateTime1 = new(2023, 1, 31, 13, 20, 54);
    private static readonly DateTime? dateNull = null;

    /* It's really not much worth to test all localized methods because they are only used as display, 
     * apart from the invariant methods that are used to convert back
     */
    [Fact]
    public void TestToStringShortDate()
    {
        Assert.True("01/12/2020" == date1.ToStringShortDate(itCulture));
        Assert.True("12/1/2020" == date1.ToStringShortDate(enUsCulture));
        Assert.True("01/12/2020" == date1.ToStringShortDate(frCulture));
        Assert.True("01/12/2020" == date1.ToStringShortDate(esCulture));
        Assert.True("01/12/2020" == date1.ToStringShortDate(enGbCulture));
        Assert.True("12/1/2020" == date1.ToStringShortDate(enGeneric));

        Assert.True("31/01/2023" == dateTime1.ToStringShortDate(itCulture));
        Assert.True("1/31/2023" == dateTime1.ToStringShortDate(enUsCulture));
        Assert.True("31/01/2023" == dateTime1.ToStringShortDate(frCulture));
        Assert.True("31/01/2023" == dateTime1.ToStringShortDate(esCulture));
        Assert.True("31/01/2023" == dateTime1.ToStringShortDate(enGbCulture));
        Assert.True("1/31/2023" == dateTime1.ToStringShortDate(enGeneric));

        Assert.True(string.Empty == dateNull.ToStringShortDate(itCulture));
        Assert.True(string.Empty == dateNull.ToStringShortDate(enUsCulture));
        Assert.True(string.Empty == dateNull.ToStringShortDate(frCulture));
        Assert.True(string.Empty == dateNull.ToStringShortDate(esCulture));
        Assert.True(string.Empty == dateNull.ToStringShortDate(enGbCulture));
        Assert.True(string.Empty == dateNull.ToStringShortDate(enGeneric));

        Assert.False(null == dateNull.ToStringShortDate(itCulture));
        Assert.False(null == dateNull.ToStringShortDate(enUsCulture));
        Assert.False(null == dateNull.ToStringShortDate(frCulture));
        Assert.False(null == dateNull.ToStringShortDate(esCulture));
        Assert.False(null == dateNull.ToStringShortDate(enGbCulture));
        Assert.False(null == dateNull.ToStringShortDate(enGeneric));
    }


    [Fact]
    public void TestToStringDateSortableInvariant()
    {
        Assert.True("2020-12-01" == date1.ToStringDateSortableInvariant());
        Assert.True("2023-01-31" == dateTime1.ToStringDateSortableInvariant());
        Assert.True(string.Empty == dateNull.ToStringDateSortableInvariant());
        Assert.False(null == dateNull.ToStringDateSortableInvariant());

    }


    [Fact]
    public void TestToStringDateSortableInvariantStandard()
    {
        Assert.True("2020-12-01T00:00:00" == date1.ToStringDateTimeSortableInvariantStandard());
        Assert.True("2023-01-31T13:20:54" == dateTime1.ToStringDateTimeSortableInvariantStandard());
        Assert.True(string.Empty == dateNull.ToStringDateTimeSortableInvariantStandard());
        Assert.False(null == dateNull.ToStringDateTimeSortableInvariantStandard());
    }


    [Fact]
    public void TestToStringDateTimeInvariant()
    {
        Assert.True("2020-12-01 00:00:00" == date1.ToStringDateTimeInvariant());
        Assert.True("2023-01-31 13:20:54" == dateTime1.ToStringDateTimeInvariant());
        Assert.True(string.Empty == dateNull.ToStringDateTimeInvariant());
        Assert.False(null == dateNull.ToStringDateTimeInvariant());
    }


    [Fact]
    public void TestToDbStringDateInvariant()
    {
        Assert.True("20201201" == date1.ToDbStringDateInvariant());
        Assert.True("20230131" == dateTime1.ToDbStringDateInvariant());
        Assert.True(string.Empty == dateNull.ToDbStringDateInvariant());
        Assert.False(null == dateNull.ToDbStringDateInvariant());
    }


    [Fact]
    public void TestToDbStringDateTimeShortInvariant()
    {
        Assert.True("202012010000" == date1.ToDbStringDateTimeShortInvariant());
        Assert.True("202301311320" == dateTime1.ToDbStringDateTimeShortInvariant());
        Assert.True(string.Empty == dateNull.ToDbStringDateTimeShortInvariant());
        Assert.False(null == dateNull.ToDbStringDateTimeShortInvariant());
    }




    public static readonly object[][] CorrectData =
    {
        new object[] { null, ""},

        new object[] { DateTime.MinValue, "000000"},
        new object[] { (DateTime?)new DateTime(0001, 01, 01, 00, 00, 00), "000000"},

        new object[] { DateTime.MaxValue, "235959"},
        new object[] { (DateTime?)new DateTime(9999, 12, 31, 23, 59, 59), "235959"},

        new object[] { (DateTime?)new DateTime(2020, 01, 01, 23, 50, 31), "235031"},
        new object[] { (DateTime?)new DateTime(0001, 01, 01), "000000"},
    };

    [Theory, MemberData(nameof(CorrectData))]
    public void ToDbStringDateTimeShortInvariantTest(DateTime? providedDate, string expected)
    {
        string converted = providedDate.ToDbStringTimeInvariant();
        Assert.Equal(expected, converted);
    }
}