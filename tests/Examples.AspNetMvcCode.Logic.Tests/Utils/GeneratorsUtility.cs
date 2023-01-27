namespace Examples.AspNetMvcCode.Logic.Test.Utils;

internal static class GeneratorsUtility
{
    public static long GetStrictlyPositiveLong()
    {
        var fixture = new Fixture();
        fixture.Customizations.Add(new RandomNumericSequenceGenerator(1, long.MaxValue));
        return fixture.Create<long>();
    }

    public static string GetRandomStringOfLength(int length)
    {
        StringBuilder str_build = new();
        Random random = new();

        char letter;

        for (int i = 0; i < length; i++)
        {
            double flt = random.NextDouble();
            int shift = Convert.ToInt32(Math.Floor(25 * flt));
            letter = Convert.ToChar(shift + 65);
            str_build.Append(letter);
        }

        return str_build.ToString();
    }
}
