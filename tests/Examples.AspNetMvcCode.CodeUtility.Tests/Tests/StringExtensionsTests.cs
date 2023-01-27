namespace Examples.AspNetMvcCode.CodeUtility.Tests;

public class StringExtensionsTests
{
    private const string StrNull = null;
    private static readonly string StrEmpty = string.Empty;
    private const string StrWhitespaces = " \t     \r\n   ";
    private const string StrWhitespaces2 = "\t     \r\n   ";
    private const string StrWhitespaces3 = "\r\n     \r\n   ";
    private const string StringValid = "  cbshdcosnd   ";
    private const string StringUntrimmed = " \t cbshdcosnd    \r\n   ";
    private const string StringWithInternalNewLinesAndTabs = " \t cbshd\r\n\t \t\r\ncosnd    \r\n   ";
    private const string Capitalized = "REUIWBFEW";
    private const string LowerForEqual = "reuiwbfew";

    [Fact]
    public void StringHasValueTest()
    {
        Assert.True(StringValid.StringHasValue());
        Assert.True(StringUntrimmed.StringHasValue());

        Assert.False(StrNull.StringHasValue());
        Assert.False(StrEmpty.StringHasValue());
        Assert.False(StrWhitespaces.StringHasValue());
    }

    [Fact]
    public void EmptyTest()
    {
        Assert.False(StringValid.Empty());
        Assert.False(StringUntrimmed.Empty());

        Assert.True(StrNull.Empty());
        Assert.True(StrEmpty.Empty());
        Assert.True(StrWhitespaces.Empty());
    }


    [Fact]
    public void CleanTest()
    {
        Assert.True(string.Empty == StrNull.Clean());
        Assert.True(string.Empty == StrEmpty.Clean());
        Assert.True(string.Empty == StrWhitespaces.Clean());
        Assert.True("cbshdcosnd" == StringValid.Clean());
        Assert.True("cbshd\r\n\t \t\r\ncosnd" == StringWithInternalNewLinesAndTabs.Clean());
    }

    [Fact]
    public void CleanReplaceTextNewLinesTest()
    {
        Assert.True(string.Empty == StrNull.CleanReplaceTextNewLines("test"));
        Assert.True(string.Empty == StrEmpty.CleanReplaceTextNewLines("test"));
        Assert.True(string.Empty == StrWhitespaces.CleanReplaceTextNewLines("test"));
        Assert.True("teReplacedst" == @"te
st".CleanReplaceTextNewLines("Replaced"));

        Assert.True(string.Empty == "\r\n".CleanReplaceTextNewLines("test"));//in these cases, newlines got cleaned befor replacement
        Assert.True(string.Empty == "\n".CleanReplaceTextNewLines("test"));
        Assert.True(string.Empty == "\t".CleanReplaceTextNewLines("test"));

        Assert.True("pretestpost" == "pre\r\npost".CleanReplaceTextNewLines("test"));
        Assert.True("pretestpost" == "pre\npost".CleanReplaceTextNewLines("test"));
        Assert.True("pre\tpost" == "pre\tpost".CleanReplaceTextNewLines("test"));

        Assert.True("pretestposttestother" == "pre\r\npost\nother".CleanReplaceTextNewLines("test"));
    }

    [Fact]
    public void CleanReplaceTextTabs()
    {
        Assert.True(string.Empty == StrNull.CleanReplaceTextTabs("test"));
        Assert.True(string.Empty == StrEmpty.CleanReplaceTextTabs("test"));
        Assert.True(string.Empty == StrWhitespaces.CleanReplaceTextTabs("test"));

        Assert.True(string.Empty == "\t".CleanReplaceTextTabs("test"));

        Assert.True("pretestpost" == "pre\tpost".CleanReplaceTextTabs("test"));
        Assert.True("pretestpost" == "pre\tpost\t".CleanReplaceTextTabs("test"));
        Assert.True("pretestposttestother" == "pre\tpost\tother".CleanReplaceTextTabs("test"));
    }

    [Fact]
    public void CleanRemoveNewLinesAndTabsTest()
    {
        Assert.True(string.Empty == StrNull.CleanRemoveNewLinesAndTabs());
        Assert.True(string.Empty == StrEmpty.CleanRemoveNewLinesAndTabs());
        Assert.True(string.Empty == StrWhitespaces.CleanRemoveNewLinesAndTabs());
        Assert.True("cbshdcosnd" == StringUntrimmed.CleanRemoveNewLinesAndTabs());
        Assert.True("cbshd cosnd" == StringWithInternalNewLinesAndTabs.CleanRemoveNewLinesAndTabs());

    }

    [Fact]
    public void CleanReplaceHtmlNewLinesTest()
    {
        Assert.True(string.Empty == StrNull.CleanReplaceHtmlNewLines("REPLACE"));
        Assert.True(string.Empty == StrEmpty.CleanReplaceHtmlNewLines("REPLACE"));
        Assert.True(string.Empty == StrWhitespaces.CleanReplaceHtmlNewLines("REPLACE"));
        Assert.True("ddwedwdw" == "ddwedwdw".CleanReplaceHtmlNewLines("REPLACE"));
        Assert.True("dsREPLACE dwqdqw" == "\r\n  ds<br> dwqdqw  \t ".CleanReplaceHtmlNewLines("REPLACE"));
        Assert.True("ds REPLACEdwqdqw" == "  \r\n  ds <BR/>dwqdqw  \n".CleanReplaceHtmlNewLines("REPLACE"));
        Assert.True("dsREPLACE dwqdqw" == "\r\n  ds<  Br  > dwqdqw  \t ".CleanReplaceHtmlNewLines("REPLACE"));
        Assert.True("ds REPLACEdwqdqw" == "  \r\n  ds <bR  />dwqdqw  \n".CleanReplaceHtmlNewLines("REPLACE"));
    }

    [Fact]
    public void CleanReplaceHtmlNonBreakableSpacesTest()
    {
        Assert.True(string.Empty == StrNull.CleanReplaceHtmlNonBreakableSpaces("REPLACE"));
        Assert.True(string.Empty == StrEmpty.CleanReplaceHtmlNonBreakableSpaces("REPLACE"));
        Assert.True(string.Empty == StrWhitespaces.CleanReplaceHtmlNonBreakableSpaces("REPLACE"));
        Assert.True("ddwedwdw" == "ddwedwdw".CleanReplaceHtmlNonBreakableSpaces("REPLACE"));
        Assert.True("dsREPLACE dwqdqw" == "\r\n  ds&nbsp; dwqdqw  \t ".CleanReplaceHtmlNonBreakableSpaces("REPLACE"));
        Assert.True("ds REPLACE REPLACEdwqdqw" == "  \r\n  ds &nbsp; &nbsp;dwqdqw  \n".CleanReplaceHtmlNonBreakableSpaces("REPLACE"));
        Assert.True("dsREPLACE dwqdqw" == "\r\n  ds&nbsp; dwqdqw  \t ".CleanReplaceHtmlNonBreakableSpaces("REPLACE"));
        Assert.True("ds REPLACEdwqdqw" == "  \r\n  ds &nbsp;dwqdqw  \n".CleanReplaceHtmlNonBreakableSpaces("REPLACE"));
    }



    [Fact]
    public void ReplaceInvariantTest()
    {
        Assert.True(string.Empty == StrNull.ReplaceInvariant("test", "new"));
        Assert.True(string.Empty == StrEmpty.ReplaceInvariant("test", "new"));
        Assert.True(" \t     \t   " == StrWhitespaces.ReplaceInvariant("\r\n", "\t"));
        Assert.True(" \t newStringdcosnd    \r\n   " == StringUntrimmed.ReplaceInvariant("CbsH", "newString"));
    }


    [Fact]
    public void CleanAndFirstCharToUppercaseTest()
    {
        Assert.True(string.Empty == StrNull.CleanAndFirstCharToUppercase());
        Assert.True(string.Empty == StrEmpty.CleanAndFirstCharToUppercase());
        Assert.True(string.Empty == StrWhitespaces.CleanAndFirstCharToUppercase());
        Assert.True("Cbshdcosnd" == StringUntrimmed.CleanAndFirstCharToUppercase());
        Assert.True("Cbshd\r\n\t \t\r\ncosnd" == StringWithInternalNewLinesAndTabs.CleanAndFirstCharToUppercase());
        Assert.True("REUIWBFEW" == Capitalized.CleanAndFirstCharToUppercase());
    }

    [Fact]
    public void CleanAndFirstCharToLowercaseTest()
    {
        Assert.True(string.Empty == StrNull.CleanAndFirstCharToLowercase());
        Assert.True(string.Empty == StrEmpty.CleanAndFirstCharToLowercase());
        Assert.True(string.Empty == StrWhitespaces.CleanAndFirstCharToLowercase());
        Assert.True("cbshdcosnd" == StringUntrimmed.CleanAndFirstCharToLowercase());
        Assert.True("cbshd\r\n\t \t\r\ncosnd" == StringWithInternalNewLinesAndTabs.CleanAndFirstCharToLowercase());
        Assert.True("rEUIWBFEW" == Capitalized.CleanAndFirstCharToLowercase());
    }


    [Fact]
    public void EqualsInvariantTest()
    {
        Exception exception =
            Record.Exception(() =>
                StrNull.EqualsInvariant(null)
                );
        Assert.IsType<NullReferenceException>(exception);

        Assert.True(StrEmpty.EqualsInvariant(string.Empty));
        Assert.True(StrWhitespaces.EqualsInvariant(StrWhitespaces));
        Assert.True(StringValid.EqualsInvariant("  cbsHDcosND   "));
        Assert.True(Capitalized.EqualsInvariant(LowerForEqual));
    }


    [Fact]
    public void ContainsInvariantTest()
    {
        Exception exception;

        exception =
           Record.Exception(() =>
              StrNull.ContainsInvariant(null)
               );
        Assert.IsType<NullReferenceException>(exception);

        exception =
           Record.Exception(() =>
               StrNull.ContainsInvariant(string.Empty)
               );
        Assert.IsType<NullReferenceException>(exception);

        exception =
           Record.Exception(() =>
               string.Empty.ContainsInvariant(StrNull)
               );
        Assert.IsType<ArgumentNullException>(exception);

        Assert.True(StrEmpty.ContainsInvariant(string.Empty));
        Assert.True(StrWhitespaces.ContainsInvariant(" "));
        Assert.True(StringValid.ContainsInvariant("dco"));
        Assert.True(StringUntrimmed.ContainsInvariant(Environment.NewLine));
        Assert.True(StringWithInternalNewLinesAndTabs.ContainsInvariant("\t"));
        Assert.True(Capitalized.ContainsInvariant("uiw"));
        Assert.True(LowerForEqual.ContainsInvariant("REUI"));

        Assert.False(StringValid.ContainsInvariant(Environment.NewLine));
        Assert.False(Capitalized.ContainsInvariant("lll"));
        Assert.False(LowerForEqual.ContainsInvariant("BW"));
    }


    [Fact]
    public void StartsWithInvariantTest()
    {
        Exception exception;

        exception =
           Record.Exception(() =>
              StrNull.StartsWithInvariant(null)
               );
        Assert.IsType<NullReferenceException>(exception);

        exception =
           Record.Exception(() =>
               StrNull.StartsWithInvariant(string.Empty)
               );
        Assert.IsType<NullReferenceException>(exception);

        exception =
           Record.Exception(() =>
               string.Empty.StartsWithInvariant(StrNull)
               );
        Assert.IsType<ArgumentNullException>(exception);

        Assert.True(StrEmpty.StartsWithInvariant(string.Empty));
        Assert.True(StrWhitespaces.StartsWithInvariant(" "));
        Assert.True(StrWhitespaces2.StartsWithInvariant("\t"));
        Assert.True(StrWhitespaces3.StartsWithInvariant(Environment.NewLine));
        Assert.True(Capitalized.StartsWithInvariant("reui"));
        Assert.True(LowerForEqual.StartsWithInvariant("REUI"));

        Assert.False(StringValid.StartsWithInvariant(Environment.NewLine));
        Assert.False(Capitalized.StartsWithInvariant("lll"));
        Assert.False(LowerForEqual.StartsWithInvariant("BW"));
    }


    [Fact]
    public void EndsWithInvariantTest()
    {
        Exception exception;

        exception =
           Record.Exception(() =>
              StrNull.EndsWithInvariant(null)
               );
        Assert.IsType<NullReferenceException>(exception);

        exception =
           Record.Exception(() =>
               StrNull.EndsWithInvariant(string.Empty)
               );
        Assert.IsType<NullReferenceException>(exception);

        exception =
           Record.Exception(() =>
               string.Empty.EndsWithInvariant(StrNull)
               );
        Assert.IsType<ArgumentNullException>(exception);

        Assert.True(StrEmpty.EndsWithInvariant(string.Empty));
        Assert.True(StrWhitespaces.EndsWithInvariant(" "));
        Assert.True(Capitalized.EndsWithInvariant("ew"));
        Assert.True(LowerForEqual.EndsWithInvariant("EW"));

        Assert.False(StringValid.EndsWithInvariant(Environment.NewLine));
        Assert.False(Capitalized.EndsWithInvariant("lll"));
        Assert.False(LowerForEqual.EndsWithInvariant("BW"));
    }


    [Fact]
    public void EqualsAnyInvariantTest()
    {
        IList<string> tocheckEmpty = new List<string>();
        IList<string> tocheckEmpties = new List<string>() { string.Empty, string.Empty };
        IList<string> tocheckNulls = new List<string>() { null, null };
        IList<string> tocheckValues = new List<string>() { "cbshdcosnd", "  cbSHdcosnd   " };

        Exception exception;

        exception =
           Record.Exception(() =>
              StrNull.EqualsAnyInvariant(null)
               );
        Assert.IsType<ArgumentNullException>(exception);

        Assert.True(StrNull.EqualsAnyInvariant(tocheckNulls));
        Assert.True(StrEmpty.EqualsAnyInvariant(tocheckEmpties));

        Assert.True(StringValid.EqualsAnyInvariant(tocheckValues));

        Assert.False(StringValid.EqualsAnyInvariant(tocheckEmpties));
    }


    private enum Test1
    {
        None,
        OkBoh,
        Error,
    }
    [Fact]
    public void ToEnumTest()
    {
        Exception exception;

        exception =
           Record.Exception(() =>
              StrNull.ToEnum<Test1>()
               );
        Assert.IsType<ArgumentNullException>(exception);

        exception =
           Record.Exception(() =>
              StrEmpty.ToEnum<Test1>()
               );
        Assert.IsType<ArgumentException>(exception);

        exception =
           Record.Exception(() =>
              "InvalidEnum".ToEnum<Test1>()
               );
        Assert.IsType<ArgumentException>(exception);

        Assert.True(Test1.OkBoh == "OkBoh".ToEnum<Test1>());
        Assert.True(Test1.Error == "ERROR".ToEnum<Test1>());
    }


    [Fact]
    public void ToEnumOrDefaultTest()
    {
        Assert.True(Test1.None == StrNull.ToEnumOrDefault<Test1>());
        Assert.True(Test1.None == StrEmpty.ToEnumOrDefault<Test1>());
        Assert.True(Test1.None == "InvalidEnum".ToEnumOrDefault<Test1>());
        Assert.True(Test1.OkBoh == "OkBoh".ToEnumOrDefault<Test1>());
        Assert.True(Test1.Error == "ERROR".ToEnumOrDefault<Test1>());
    }

    //[Fact]
    //public void Test()
    //{
    //  Assert.True();
    //
    //  Assert.False();
    //}
}