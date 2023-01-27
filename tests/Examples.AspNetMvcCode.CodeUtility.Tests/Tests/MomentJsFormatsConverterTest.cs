namespace Examples.AspNetMvcCode.CodeUtility.Tests;

public class MomentJsFormatsConverterTest
{
    [Fact]
    public void GetMomentJsFormatForStandardShortDateTest()
    {
        string format = MomentJsFormatsConverter.GetForStandardShortDate(new CultureInfo("it"));
        Assert.True("DD/MM/YYYY" == format || "DD[/]MM[/]YYYY" == format);

        format = MomentJsFormatsConverter.GetForStandardShortDate(new CultureInfo("en"));
        Assert.True("M/D/YYYY" == format || "M[/]D[/]YYYY" == format);
    }

    [Fact]
    public void GetMomentJsFormatForStandardDateTime()
    {
        string format = MomentJsFormatsConverter.GetForStandardDateTime(new CultureInfo("it"));
        Assert.True("DD/MM/YYYY HH:mm:ss" == format || "DD[/]MM[/]YYYY[ ]HH[:]mm[:]ss" == format);

        format = MomentJsFormatsConverter.GetForStandardDateTime(new CultureInfo("en"));
        Assert.True("M/D/YYYY h:mm:ss A" == format || "M[/]D[/]YYYY[ ]h[:]mm[:]ss[ ]A" == format);
    }

    [Fact]
    public void GetMomentJsFormatForInvariantForFileName()
    {
        string format = MomentJsFormatsConverter.GetInvariantForFileName(new CultureInfo("it"));
        Assert.True("YYYY-MM-DD_HH.mm.ss" == format || "YYYY[-]MM[-]DD[_]HH[.]mm[.]ss" == format);

        format = MomentJsFormatsConverter.GetInvariantForFileName(new CultureInfo("en"));
        Assert.True("YYYY-MM-DD_HH.mm.ss" == format || "YYYY[-]MM[-]DD[_]HH[.]mm[.]ss" == format);
    }
}