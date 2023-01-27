namespace Examples.AspNetMvcCode.CodeUtility.Tests;

public class DataColumnExtensionsTests
{
    private readonly DataColumn NullCol = null;
    private readonly DataColumn EmptyCol = new();
    private readonly DataColumn ColProp = GetTestColumn();
    private readonly DataColumn ColNoProp = GetColumnNoProps();

    private static DataColumn GetTestColumn()
    {
        using DataTable dt = new();

        dt.Columns.Add("colProp", typeof(string));
        dt.Columns["colProp"].ExtendedProperties["testProperty"] = "prova";

        return dt.Columns["colProp"];
    }

    private static DataColumn GetColumnNoProps()
    {
        using DataTable dt = new();

        dt.Columns.Add("colNoProp", typeof(string));

        return dt.Columns["colNoProp"];
    }


    [Fact]
    public void HasExtendedPropertiesTest()
    {
        Assert.False(NullCol.HasExtendedProperties());
        Assert.False(EmptyCol.HasExtendedProperties());

        Assert.True(ColProp.HasExtendedProperties());

        Assert.False(ColNoProp.HasExtendedProperties());
    }
}