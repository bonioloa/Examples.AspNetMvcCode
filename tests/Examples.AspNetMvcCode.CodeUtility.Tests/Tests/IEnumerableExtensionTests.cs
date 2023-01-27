namespace Examples.AspNetMvcCode.CodeUtility.Tests;

/// <summary>
/// tests also <see cref="IEnumerableStringExtensions"/>
/// </summary>
public class IEnumerableExtensionTests
{
    private static readonly IList<string> listStringElements = new List<string> { "one", "two", "three" };
    private static readonly IList<string> listStringElements2 = new List<string> { "ONE", "TWO" };
    private static readonly IList<string> listStringElements3 = new List<string> { "four", null };
    private static readonly IList<string> listStringElement = new List<string> { "five" };
    private static readonly IList<string> listStringElementsSame = new List<string> { "one", "one" };
    private static readonly IList<string> listStringElementsNull = new List<string> { null, null, null, null };
    private static readonly IList<string> listStringEmpty = new List<string>();
    private static readonly IList<string> listStringNull = null;

    private static readonly IList<long> listLongElements = new List<long> { 1, 2, 3 };
    private static readonly IList<long> listLongElements2 = new List<long> { 2, 3 };
    private static readonly IList<long> listLongElements3 = new List<long> { 2, 5 };
    private static readonly IList<long> listLongElement = new List<long> { 6 };
    private static readonly IList<long> listLongElementSame = new List<long> { 1, 1 };
    private static readonly IList<long> listLongEmpty = new List<long>();
    private static readonly IList<long> listLongNull = null;


    [Fact]
    public void IsNullOrEmptyTest()
    {
        Assert.True(listStringEmpty.IsNullOrEmpty());
        Assert.True(listStringNull.IsNullOrEmpty());

        Assert.False(listStringElements.IsNullOrEmpty());
        Assert.False(listStringElementsNull.IsNullOrEmpty());
    }


    [Fact]
    public void HasValuesTest()
    {
        Assert.False(listStringEmpty.HasValues());
        Assert.False(listStringNull.HasValues());

        Assert.True(listStringElements.HasValues());
        Assert.True(listStringElementsNull.HasValues());
    }


    [Fact]
    public void ContainsAllTest()
    {
        Assert.True(listLongElements.ContainsAll(listLongElements2));
        Assert.True(listLongElements.ContainsAll(listLongEmpty));
        Assert.True(listLongElements.ContainsAll(listLongNull));

        Assert.True(listLongNull.ContainsAll(listLongEmpty));

        Assert.False(listLongNull.ContainsAll(listLongElements3));
        Assert.False(listLongEmpty.ContainsAll(listLongElements3));
        Assert.False(listLongElements.ContainsAll(listLongElements3));
    }


    [Fact]
    public void ContainsAllInvariantTest()
    {
        Assert.True(listStringElements.ContainsAllInvariant(listStringElements2));
        Assert.True(listStringElements.ContainsAllInvariant(listStringEmpty));
        Assert.True(listStringElements.ContainsAllInvariant(listStringNull));

        Assert.True(listStringNull.ContainsAllInvariant(listStringEmpty));

        Assert.False(listStringNull.ContainsAllInvariant(listStringElements3));
        Assert.False(listStringEmpty.ContainsAllInvariant(listStringElements3));
        Assert.False(listStringElements.ContainsAllInvariant(listStringElements3));
    }


    [Fact]
    public void AreAllSameTest()
    {
        Assert.True(listStringEmpty.AreAllSame());
        Assert.True(listStringNull.AreAllSame());
        Assert.True(listLongEmpty.AreAllSame());
        Assert.True(listLongNull.AreAllSame());
        Assert.True(listStringElement.AreAllSame());
        Assert.True(listLongElement.AreAllSame());
        Assert.True(listStringElementsSame.AreAllSame());
        Assert.True(listLongElementSame.AreAllSame());
        Assert.True(listStringElementsNull.AreAllSame());

        Assert.False(listStringElements.AreAllSame());
        Assert.False(listLongElements.AreAllSame());
    }


    [Fact]
    public void HasDuplicatesTest()
    {
        Assert.False(listStringEmpty.HasDuplicates());
        Assert.False(listStringNull.HasDuplicates());

        Assert.False(listLongEmpty.HasDuplicates());
        Assert.False(listLongNull.HasDuplicates());

        Assert.False(listStringElement.HasDuplicates());
        Assert.False(listLongElement.HasDuplicates());

        Assert.True(listStringElementsSame.HasDuplicates());
        Assert.True(listLongElementSame.HasDuplicates());

        Assert.True(listStringElementsNull.HasDuplicates());

        Assert.False(listStringElements.AreAllSame());
        Assert.False(listLongElements.AreAllSame());
    }

    [Fact]
    public void GetDuplicatesTest()
    {
        Assert.True(listStringEmpty.GetDuplicates().Count == 0);
        Assert.True(listStringNull.GetDuplicates().Count == 0);
        Assert.True(listLongEmpty.GetDuplicates().Count == 0);
        Assert.True(listLongNull.GetDuplicates().Count == 0);
        Assert.True(listStringElement.GetDuplicates().Count == 0);
        Assert.True(listLongElement.GetDuplicates().Count == 0);
        Assert.True(listStringElementsSame.GetDuplicates().ContainsAll(new List<string> { "one" }));
        Assert.True(listLongElementSame.GetDuplicates().ContainsAll(new List<long> { 1 }));
        Assert.True(listStringElementsNull.GetDuplicates().ContainsAll(new List<string> { null }));

        Assert.True(listStringElements.GetDuplicates().Count == 0);
        Assert.True(listLongElements.GetDuplicates().Count == 0);
    }
}