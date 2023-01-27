namespace Examples.AspNetMvcCode.CodeUtility.Tests;

public class DataRowExtensionsTests
{
    private static readonly DataRow TestRow = GetTestRow();
    private static readonly DataRow NullRow = null;

    private static readonly DateTime dateDb = new(2020, 09, 20);
    private static readonly DateTime dateAndTime = new(2020, 09, 20, 12, 45, 23);
    private static readonly DateTime dateMin = DateTime.MinValue.Date;
    private static readonly DateTime? dateNull = null;

    private static DataRow GetTestRow()
    {
        using DataTable dt = new();

        dt.Columns.Add("strDbNull", typeof(string));
        dt.Columns.Add("strNull", typeof(string));
        dt.Columns.Add("strEmpty", typeof(string));

        dt.Columns.Add("strUntrimmed", typeof(string));
        dt.Columns.Add("strWithNewLinesWithin", typeof(string));

        dt.Columns.Add("strValid", typeof(string));

        dt.Columns.Add("strFlagTrue1", typeof(string));
        dt.Columns.Add("strFlagTrue2", typeof(string));
        dt.Columns.Add("strFlagTrue3", typeof(string));
        dt.Columns.Add("strFlagFalse1", typeof(string));
        dt.Columns.Add("strFlagFalse2", typeof(string));
        dt.Columns.Add("strFlagFalse3", typeof(string));

        dt.Columns.Add("strDateDb", typeof(string));
        dt.Columns.Add("strTimeDb", typeof(string));
        dt.Columns.Add("strDateTimeDb", typeof(string));

        dt.Columns.Add("decimalPositive", typeof(decimal));
        dt.Columns.Add("decimalDbNull", typeof(decimal));

        dt.Columns.Add("longBoxedInDecimalNegative", typeof(decimal));
        dt.Columns.Add("intBoxedInDecimalNegative", typeof(decimal));

        dt.Columns.Add("intNegative", typeof(int));
        dt.Columns.Add("intDbNull", typeof(int));

        dt.Columns.Add("longNegative", typeof(long));
        dt.Columns.Add("longDbNull", typeof(long));

        dt.Columns.Add("boolTrue", typeof(bool));
        dt.Columns.Add("boolDbNull", typeof(bool));

        dt.Columns.Add("longZero", typeof(long));
        dt.Columns.Add("intZero", typeof(int));
        dt.Columns.Add("decimalZero", typeof(decimal));



        DataRow dataRow = dt.NewRow();
        dataRow["strDbNull"] = DBNull.Value;
        dataRow["strNull"] = null;
        dataRow["strEmpty"] = string.Empty;

        dataRow["strValid"] = "  cbshdcosnd   ";

        dataRow["strUntrimmed"] = " \t cbshdcosnd    \r\n   ";
        dataRow["strWithNewLinesWithin"] = " \t cbshd\r\n\t \t\r\ncosnd    \r\n   ";


        dataRow["strFlagTrue1"] = " VERO ";
        dataRow["strFlagTrue2"] = "TRUE";
        dataRow["strFlagTrue3"] = "S";
        dataRow["strFlagFalse1"] = "FALSE";
        dataRow["strFlagFalse2"] = "FALSO";
        dataRow["strFlagFalse3"] = "N";

        dataRow["strDateDb"] = "20200920";
        dataRow["strTimeDb"] = "124523";
        dataRow["strDateTimeDb"] = "20200920124523";

        dataRow["decimalPositive"] = 12345678901234567890123456789M;
        dataRow["decimalDbNull"] = DBNull.Value;

        dataRow["longBoxedInDecimalNegative"] = -987654321098765432L;
        dataRow["intBoxedInDecimalNegative"] = -1235678012;

        dataRow["intNegative"] = -1234567890;
        dataRow["intDbNull"] = DBNull.Value;

        dataRow["longNegative"] = -987654321098765432L;
        dataRow["longDbNull"] = DBNull.Value;

        dataRow["boolTrue"] = true;
        dataRow["boolDbNull"] = DBNull.Value;

        dataRow["longZero"] = 0;
        dataRow["intZero"] = 0;
        dataRow["decimalZero"] = 0;

        //dataRow[""] = ;
        //dataRow[""] = ;
        //dataRow[""] = ;
        //dataRow[""] = ;
        //dataRow[""] = ;
        //dataRow[""] = ;
        dt.Rows.Add(dataRow);

        return dt.Rows[0];
    }


    [Fact]
    public void CoalesceAndCleanTest()
    {
        Assert.True(string.Empty == TestRow.CoalesceAndClean("strDbNull"));
        Assert.True(string.Empty == TestRow.CoalesceAndClean("strNull"));
        Assert.True(string.Empty == TestRow.CoalesceAndClean("strEmpty"));
        Assert.True("cbshdcosnd" == TestRow.CoalesceAndClean("strUntrimmed"));
        Assert.True("cbshdcosnd" == TestRow.CoalesceAndClean("strValid"));
        Assert.True("cbshd\r\n\t \t\r\ncosnd" == TestRow.CoalesceAndClean("strWithNewLinesWithin"));

        Exception exception =
            Record.Exception(() =>
                TestRow.CoalesceAndClean("intNegative")
                );
        Assert.IsType<InvalidCastException>(exception);

        exception =
            Record.Exception(() =>
                TestRow.CoalesceAndClean("longNegative")
                );
        Assert.IsType<InvalidCastException>(exception);
    }


    [Fact]
    public void CoalesceCleanInAllString()
    {
        //rest of cases already handled by CoalesceAndCleanTest
        Assert.True("cbshd cosnd" == TestRow.CoalesceCleanInAllString("strWithNewLinesWithin"));
    }


    [Fact]
    public void GetLongFromFakeDecimalTest()
    {
        Assert.True(-987654321098765432L == TestRow.GetLongFromFakeDecimal("longBoxedInDecimalNegative"));
        Assert.True(-1235678012 == TestRow.GetLongFromFakeDecimal("intBoxedInDecimalNegative"));//int can be boxed in long
        Assert.True(0 == TestRow.GetLongFromFakeDecimal("decimalZero"));


        Exception exception;

        exception =
           Record.Exception(() =>
               TestRow.GetLongFromFakeDecimal("decimalPositive")
               );
        Assert.IsType<OverflowException>(exception);//a real decimal overflows

        exception =
           Record.Exception(() =>
               TestRow.GetLongFromFakeDecimal("decimalDbNull")
               );
        Assert.IsType<InvalidCastException>(exception);

        exception =
            Record.Exception(() =>
                TestRow.GetLongFromFakeDecimal("intNegative")
                );
        Assert.IsType<InvalidCastException>(exception);

        exception =
            Record.Exception(() =>
                TestRow.GetLongFromFakeDecimal("longNegative")
                );
        Assert.IsType<InvalidCastException>(exception);


        exception =
            Record.Exception(() =>
                TestRow.GetLongFromFakeDecimal("strValid")
                );
        Assert.IsType<InvalidCastException>(exception);
    }


    [Fact]
    public void GetIntFromFakeDecimalTest()
    {
        Assert.True(-1235678012 == TestRow.GetIntFromFakeDecimal("intBoxedInDecimalNegative"));
        Assert.True(0 == TestRow.GetIntFromFakeDecimal("decimalZero"));


        Exception exception;

        exception =
          Record.Exception(() =>
              TestRow.GetIntFromFakeDecimal("longBoxedInDecimalNegative")
              );
        Assert.IsType<OverflowException>(exception);//a long overflows

        exception =
           Record.Exception(() =>
               TestRow.GetIntFromFakeDecimal("decimalPositive")
               );
        Assert.IsType<OverflowException>(exception);//a decimal overflows

        exception =
           Record.Exception(() =>
               TestRow.GetIntFromFakeDecimal("decimalDbNull")
               );
        Assert.IsType<InvalidCastException>(exception);

        exception =
            Record.Exception(() =>
                TestRow.GetIntFromFakeDecimal("intNegative")
                );
        Assert.IsType<InvalidCastException>(exception);

        exception =
            Record.Exception(() =>
                TestRow.GetIntFromFakeDecimal("longNegative")
                );
        Assert.IsType<InvalidCastException>(exception);


        exception =
            Record.Exception(() =>
                TestRow.GetIntFromFakeDecimal("strValid")
                );
        Assert.IsType<InvalidCastException>(exception);
    }


    [Fact]
    public void GetBoolFromNumOrBitTest()
    {
        Exception exception;

        exception =
          Record.Exception(() =>
              NullRow.GetBoolFromNumOrBit("noCol")
              );
        Assert.IsType<ArgumentNullException>(exception);

        exception =
          Record.Exception(() =>
              TestRow.GetBoolFromNumOrBit("boolDbNull")
              );
        Assert.IsType<InvalidCastException>(exception);


        Assert.True(TestRow.GetBoolFromNumOrBit("boolTrue"));
        Assert.True(TestRow.GetBoolFromNumOrBit("decimalPositive"));

        Assert.False(TestRow.GetBoolFromNumOrBit("intNegative"));
        Assert.False(TestRow.GetBoolFromNumOrBit("longNegative"));
        Assert.False(TestRow.GetBoolFromNumOrBit("longZero"));
        Assert.False(TestRow.GetBoolFromNumOrBit("intZero"));
        Assert.False(TestRow.GetBoolFromNumOrBit("decimalZero"));
    }


    [Fact]
    public void GetBoolFromFlagStringTest()
    {
        Exception exception;

        exception =
          Record.Exception(() =>
              NullRow.GetBoolFromFlagString("noCol")
              );
        Assert.IsType<ArgumentNullException>(exception);

        exception =
          Record.Exception(() =>
              TestRow.GetBoolFromFlagString("boolDbNull")
              );
        Assert.IsType<InvalidCastException>(exception);

        exception =
          Record.Exception(() =>
              TestRow.GetBoolFromFlagString("intNegative")
              );
        Assert.IsType<InvalidCastException>(exception);

        exception =
          Record.Exception(() =>
              TestRow.GetBoolFromFlagString("longZero")
              );
        Assert.IsType<InvalidCastException>(exception);

        exception =
          Record.Exception(() =>
              TestRow.GetBoolFromFlagString("boolTrue")
              );
        Assert.IsType<InvalidCastException>(exception);

        exception =
         Record.Exception(() =>
             TestRow.GetBoolFromFlagString("strNull")
             );
        Assert.IsType<InvalidCastException>(exception);

        Assert.True(TestRow.GetBoolFromFlagString("strFlagTrue1"));
        Assert.True(TestRow.GetBoolFromFlagString("strFlagTrue2"));
        Assert.True(TestRow.GetBoolFromFlagString("strFlagTrue3"));

        Assert.False(TestRow.GetBoolFromFlagString("strFlagFalse1"));
        Assert.False(TestRow.GetBoolFromFlagString("strFlagFalse2"));
        Assert.False(TestRow.GetBoolFromFlagString("strFlagFalse3"));

        Assert.False(TestRow.GetBoolFromFlagString("strValid"));
        Assert.False(TestRow.GetBoolFromFlagString("strEmpty"));
    }


    [Fact]
    public void GetNullableBoolFromFlagStringTest()
    {
        Exception exception;

        exception =
          Record.Exception(() =>
              NullRow.GetNullableBoolFromFlagString("noCol")
              );
        Assert.IsType<ArgumentNullException>(exception);

        exception =
          Record.Exception(() =>
              TestRow.GetNullableBoolFromFlagString("intNegative")
              );
        Assert.IsType<InvalidCastException>(exception);

        exception =
          Record.Exception(() =>
              TestRow.GetNullableBoolFromFlagString("longZero")
              );
        Assert.IsType<InvalidCastException>(exception);

        exception =
          Record.Exception(() =>
              TestRow.GetNullableBoolFromFlagString("boolTrue")
              );
        Assert.IsType<InvalidCastException>(exception);


        Assert.True(TestRow.GetNullableBoolFromFlagString("strFlagTrue1"));
        Assert.True(TestRow.GetNullableBoolFromFlagString("strFlagTrue2"));
        Assert.True(TestRow.GetNullableBoolFromFlagString("strFlagTrue3"));

        Assert.False(TestRow.GetNullableBoolFromFlagString("strFlagFalse1"));
        Assert.False(TestRow.GetNullableBoolFromFlagString("strFlagFalse2"));
        Assert.False(TestRow.GetNullableBoolFromFlagString("strFlagFalse3"));


        Assert.True(null == TestRow.GetNullableBoolFromFlagString("boolDbNull"));//any DBNull will do
        Assert.True(null == TestRow.GetNullableBoolFromFlagString("strValid"));
        Assert.True(null == TestRow.GetNullableBoolFromFlagString("strEmpty"));
        Assert.True(null == TestRow.GetNullableBoolFromFlagString("strNull"));
    }


    [Fact]
    public void GetDateFromStringTest()
    {
        Exception exception;

        exception =
          Record.Exception(() =>
              NullRow.GetDateFromString("noCol")
              );
        Assert.IsType<ArgumentNullException>(exception);

        exception =
          Record.Exception(() =>
              TestRow.GetDateFromString(null)
              );
        Assert.IsType<ArgumentNullException>(exception);

        exception =
          Record.Exception(() =>
              TestRow.GetDateFromString(string.Empty)
              );
        Assert.IsType<ArgumentNullException>(exception);

        exception =
          Record.Exception(() =>
              TestRow.GetDateFromString("strValid")
              );
        Assert.IsType<FormatException>(exception);

        exception =
          Record.Exception(() =>
              TestRow.GetDateFromString("boolTrue")
              );
        Assert.IsType<InvalidCastException>(exception);

        Assert.True(dateMin == TestRow.GetDateFromString("strEmpty"));
        Assert.True(dateMin == TestRow.GetDateFromString("strNull"));
        Assert.True(dateMin == TestRow.GetDateFromString("longDbNull"));//any DBNull will do

        Assert.True(dateDb == TestRow.GetDateFromString("strDateDb"));
    }


    [Fact]
    public void GetNullableDateFromStringTest()
    {
        Exception exception;

        exception =
          Record.Exception(() =>
              NullRow.GetNullableDateFromString("noCol")
              );
        Assert.IsType<ArgumentNullException>(exception);

        exception =
          Record.Exception(() =>
              TestRow.GetNullableDateFromString(null)
              );
        Assert.IsType<ArgumentNullException>(exception);

        exception =
          Record.Exception(() =>
              TestRow.GetNullableDateFromString(string.Empty)
              );
        Assert.IsType<ArgumentNullException>(exception);

        exception =
          Record.Exception(() =>
              TestRow.GetNullableDateFromString("strValid")
              );
        Assert.IsType<FormatException>(exception);

        exception =
          Record.Exception(() =>
              TestRow.GetNullableDateFromString("boolTrue")
              );
        Assert.IsType<InvalidCastException>(exception);

        Assert.True(dateNull == TestRow.GetNullableDateFromString("strEmpty"));
        Assert.True(dateNull == TestRow.GetNullableDateFromString("strNull"));
        Assert.True(dateNull == TestRow.GetNullableDateFromString("longDbNull"));//any DBNull will do

        Assert.True(dateDb == TestRow.GetNullableDateFromString("strDateDb"));
    }


    [Fact]
    public void GetDateTimeFromStringsTest()
    {
        Exception exception;

        exception =
          Record.Exception(() =>
              NullRow.GetDateTimeFromStrings("noCol", "nocol")
              );
        Assert.IsType<ArgumentNullException>(exception);

        exception =
          Record.Exception(() =>
              TestRow.GetDateTimeFromStrings(null, "strNull")
              );
        Assert.IsType<ArgumentNullException>(exception);

        exception =
          Record.Exception(() =>
              TestRow.GetDateTimeFromStrings("longDbNull", string.Empty)
              );
        Assert.IsType<ArgumentNullException>(exception);

        exception =
          Record.Exception(() =>
              TestRow.GetDateTimeFromStrings("strDateDb", "strValid")
              );
        Assert.IsType<FormatException>(exception);

        exception =
          Record.Exception(() =>
              TestRow.GetDateTimeFromStrings("boolTrue", "strTimeDb")
              );
        Assert.IsType<InvalidCastException>(exception);

        Assert.True(dateMin == TestRow.GetDateTimeFromStrings("strEmpty", "strEmpty"));
        Assert.True(dateMin == TestRow.GetDateTimeFromStrings("strNull", "strNull"));
        Assert.True(dateMin == TestRow.GetDateTimeFromStrings("longDbNull", "strTimeDb"));

        Assert.True(dateAndTime == TestRow.GetDateTimeFromStrings("strDateDb", "strTimeDb"));
    }


    [Fact]
    public void GetNullableDateTimeFromStringsTest()
    {
        Exception exception;

        exception =
          Record.Exception(() =>
              NullRow.GetNullableDateTimeFromStrings("noCol", "nocol")
              );
        Assert.IsType<ArgumentNullException>(exception);

        exception =
          Record.Exception(() =>
              TestRow.GetNullableDateTimeFromStrings(null, "strNull")
              );
        Assert.IsType<ArgumentNullException>(exception);

        exception =
          Record.Exception(() =>
              TestRow.GetNullableDateTimeFromStrings("longDbNull", string.Empty)
              );
        Assert.IsType<ArgumentNullException>(exception);

        exception =
          Record.Exception(() =>
              TestRow.GetNullableDateTimeFromStrings("strDateDb", "strValid")
              );
        Assert.IsType<FormatException>(exception);

        exception =
          Record.Exception(() =>
              TestRow.GetNullableDateTimeFromStrings("boolTrue", "strTimeDb")
              );
        Assert.IsType<InvalidCastException>(exception);

        Assert.True(null == TestRow.GetNullableDateTimeFromStrings("strEmpty", "strEmpty"));
        Assert.True(null == TestRow.GetNullableDateTimeFromStrings("strNull", "strNull"));
        Assert.True(null == TestRow.GetNullableDateTimeFromStrings("longDbNull", "strTimeDb"));

        Assert.True(dateAndTime == TestRow.GetNullableDateTimeFromStrings("strDateDb", "strTimeDb"));
    }


    [Fact]
    public void GetDateTimeFromStringTest()
    {
        Exception exception;

        exception =
          Record.Exception(() =>
              NullRow.GetDateTimeFromString("noCol")
              );
        Assert.IsType<ArgumentNullException>(exception);

        exception =
          Record.Exception(() =>
              TestRow.GetDateTimeFromString(null)
              );
        Assert.IsType<ArgumentNullException>(exception);

        exception =
          Record.Exception(() =>
              TestRow.GetDateTimeFromString(string.Empty)
              );
        Assert.IsType<ArgumentNullException>(exception);

        exception =
          Record.Exception(() =>
              TestRow.GetDateTimeFromString("strValid")
              );
        Assert.IsType<FormatException>(exception);

        exception =
          Record.Exception(() =>
              TestRow.GetDateTimeFromString("boolTrue")
              );
        Assert.IsType<InvalidCastException>(exception);

        Assert.True(dateMin == TestRow.GetDateTimeFromString("strEmpty"));
        Assert.True(dateMin == TestRow.GetDateTimeFromString("strNull"));
        Assert.True(dateMin == TestRow.GetDateTimeFromString("longDbNull"));//any DBNull will do

        Assert.True(dateAndTime == TestRow.GetDateTimeFromString("strDateTimeDb"));
    }


    [Fact]
    public void GetNullableDateTimeFromStringTest()
    {
        Exception exception;

        exception =
          Record.Exception(() =>
              NullRow.GetNullableDateTimeFromString("noCol")
              );
        Assert.IsType<ArgumentNullException>(exception);

        exception =
          Record.Exception(() =>
              TestRow.GetNullableDateTimeFromString(null)
              );
        Assert.IsType<ArgumentNullException>(exception);

        exception =
          Record.Exception(() =>
              TestRow.GetNullableDateTimeFromString(string.Empty)
              );
        Assert.IsType<ArgumentNullException>(exception);

        exception =
          Record.Exception(() =>
              TestRow.GetNullableDateTimeFromString("strValid")
              );
        Assert.IsType<FormatException>(exception);

        exception =
          Record.Exception(() =>
              TestRow.GetNullableDateTimeFromString("boolTrue")
              );
        Assert.IsType<InvalidCastException>(exception);

        Assert.True(dateNull == TestRow.GetNullableDateTimeFromString("strEmpty"));
        Assert.True(dateNull == TestRow.GetNullableDateTimeFromString("strNull"));
        Assert.True(dateNull == TestRow.GetNullableDateTimeFromString("longDbNull"));//any DBNull will do

        Assert.True(dateAndTime == TestRow.GetNullableDateTimeFromString("strDateTimeDb"));
    }


    //[Fact]
    //public void Test()
    //{
    //}
}