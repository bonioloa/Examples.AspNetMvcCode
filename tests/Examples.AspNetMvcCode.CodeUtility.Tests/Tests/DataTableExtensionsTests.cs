namespace Examples.AspNetMvcCode.CodeUtility.Tests;

public class DataTableExtensionsTests
{
    private static readonly DataTable emptyDt = new();
    private static readonly DataTable nullDt = null;
    private static readonly DataTable validDt = GetDtWithRows();

    private static DataTable GetDtWithRows()
    {
        using DataTable dt = new("testTb");
        dt.Columns.Add("testColumn", typeof(string));
        dt.Columns.Add("test2", typeof(bool));

        DataRow dataRow = dt.NewRow();
        dataRow["testColumn"] = "test";
        dataRow["test2"] = true;
        dt.Rows.Add(dataRow);

        dataRow = dt.NewRow();
        dataRow["testColumn"] = "test2";
        dataRow["test2"] = false;
        dt.Rows.Add(dataRow);

        return dt;
    }


    [Fact]
    public void IsNullOrEmptyTest()
    {
        Assert.True(emptyDt.IsNullOrEmpty());
        Assert.True(nullDt.IsNullOrEmpty());
        Assert.False(validDt.IsNullOrEmpty());
    }


    [Fact]
    public void HasColumnsTest()
    {
        Assert.False(emptyDt.HasColumns());
        Assert.False(nullDt.HasColumns());
        Assert.True(validDt.HasColumns());
    }


    [Fact]
    public void HasRowsTest()
    {
        Assert.False(emptyDt.HasRows());
        Assert.False(nullDt.HasRows());
        Assert.True(validDt.HasRows());
    }


    [Fact]
    public void HasSameBasicSchemaAsTest()
    {
        Assert.True(nullDt.HasSameBasicSchemaAs(nullDt));
        Assert.True(new DataTable().HasSameBasicSchemaAs(new DataTable()));
        Assert.True(new DataTable().HasSameBasicSchemaAs(new DataTable(), true));

        Assert.True(new DataTable("test ").HasSameBasicSchemaAs(new DataTable("test")));
        Assert.True(new DataTable("test ").HasSameBasicSchemaAs(new DataTable("test"), true));

        Assert.True(GetDtWithRows().HasSameBasicSchemaAs(GetDtWithRows()));
        Assert.True(GetDtWithRows().HasSameBasicSchemaAs(GetDtWithRows(), true));

        Assert.False(nullDt.HasSameBasicSchemaAs(new DataTable()));
        Assert.False(nullDt.HasSameBasicSchemaAs(GetDtWithRows()));
        Assert.False(new DataTable("testTb").HasSameBasicSchemaAs(new DataTable("testTbWrong"), true));

        DataTable temp = GetDtWithRows();
        temp.TableName = "testTbWrong";
        Assert.False(temp.HasSameBasicSchemaAs(GetDtWithRows(), true));

        Assert.False(new DataTable("testTb").HasSameBasicSchemaAs(GetDtWithRows()));

        //column number mismatch test
        using DataTable dt = new("testTbColNumMismatch");
        dt.Columns.Add("testColumn", typeof(string));
        dt.Columns.Add("test2", typeof(bool));
        dt.Columns.Add("test3", typeof(bool));

        DataRow dataRow = dt.NewRow();
        dataRow["testColumn"] = "test";
        dataRow["test2"] = true;
        dataRow["test3"] = true;
        dt.Rows.Add(dataRow);

        dataRow = dt.NewRow();
        dataRow["testColumn"] = "test2";
        dataRow["test2"] = false;
        dataRow["test2"] = false;
        dt.Rows.Add(dataRow);

        Assert.False(dt.HasSameBasicSchemaAs(GetDtWithRows()));

        //column type mismatch
        using DataTable dt2 = new("testTbColTypeMismatch");
        dt2.Columns.Add("testColumn", typeof(string));
        dt2.Columns.Add("test2", typeof(string));

        dataRow = dt2.NewRow();
        dataRow["testColumn"] = "test";
        dataRow["test2"] = "test";
        dt2.Rows.Add(dataRow);

        dataRow = dt2.NewRow();
        dataRow["testColumn"] = "test2";
        dataRow["test2"] = "test";
        dt2.Rows.Add(dataRow);

        Assert.False(dt2.HasSameBasicSchemaAs(GetDtWithRows()));
    }

    [Fact]
    public void HasSameDataTests()
    {
        DataTable dt;
        DataRow dataRow;

        Assert.True(nullDt.HasSameData(nullDt));
        Assert.True(new DataTable().HasSameData(new DataTable()));
        Assert.True(new DataTable().HasSameData(new DataTable(), true));

        Assert.True(new DataTable("test ").HasSameData(new DataTable("test")));
        Assert.True(new DataTable("test ").HasSameData(new DataTable("test"), true));

        Assert.True(GetDtWithRows().HasSameData(GetDtWithRows()));
        Assert.True(GetDtWithRows().HasSameData(GetDtWithRows(), true));

        //same values but row entry order is inverted
        dt = new("testTb");
        dt.Columns.Add("testColumn", typeof(string));
        dt.Columns.Add("test2", typeof(bool));

        dataRow = dt.NewRow();
        dataRow["testColumn"] = "test2";
        dataRow["test2"] = false;
        dt.Rows.Add(dataRow);

        dataRow = dt.NewRow();
        dataRow["testColumn"] = "test";
        dataRow["test2"] = true;
        dt.Rows.Add(dataRow);

        Assert.True(dt.HasSameData(GetDtWithRows(), true));


        Assert.False(nullDt.HasSameData(new DataTable()));
        Assert.False(nullDt.HasSameData(GetDtWithRows()));
        Assert.False(new DataTable("testTb").HasSameData(new DataTable("testTbWrong"), true));


        dt = new("testTb");
        dt.Columns.Add("testColumn", typeof(string));
        dt.Columns.Add("test2", typeof(bool));

        dataRow = dt.NewRow();
        dataRow["testColumn"] = "test";
        dataRow["test2"] = false;//different value
        dt.Rows.Add(dataRow);

        dataRow = dt.NewRow();
        dataRow["testColumn"] = "test2";
        dataRow["test2"] = false;
        dt.Rows.Add(dataRow);

        Assert.False(dt.HasSameData(GetDtWithRows()));


        dt = new("testTb");
        dt.Columns.Add("testColumn", typeof(string));
        dt.Columns.Add("test2", typeof(bool));

        dataRow = dt.NewRow();
        dataRow["testColumn"] = "test";
        dataRow["test2"] = true;
        dt.Rows.Add(dataRow);

        dataRow = dt.NewRow();
        dataRow["testColumn"] = "test2";
        dataRow["test2"] = false;
        dt.Rows.Add(dataRow);

        dataRow = dt.NewRow();//additional row
        dataRow["testColumn"] = "test3";
        dataRow["test2"] = true;
        dt.Rows.Add(dataRow);

        Assert.False(dt.HasSameData(GetDtWithRows()));


        dt = new("testTb");
        dt.Columns.Add("testColumn", typeof(string));
        dt.Columns.Add("test2", typeof(bool));

        dataRow = dt.NewRow();
        dataRow["testColumn"] = "test";
        dataRow["test2"] = true;
        dt.Rows.Add(dataRow);

        dataRow = dt.NewRow();
        dataRow["testColumn"] = "test2";
        dataRow["test2"] = false;
        dt.Rows.Add(dataRow);

        dataRow = dt.NewRow();//duplicated row
        dataRow["testColumn"] = "test2";
        dataRow["test2"] = false;
        dt.Rows.Add(dataRow);

        Assert.False(dt.HasSameData(GetDtWithRows()));
    }
}