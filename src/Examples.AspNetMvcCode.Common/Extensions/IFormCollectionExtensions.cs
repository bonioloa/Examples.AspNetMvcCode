namespace Examples.AspNetMvcCode.Common;

public static class IFormCollectionExtensions
{
    public static string Serialize(this IFormCollection form)
    {
        JsonSerializerOptions serializeOptions = new();
        serializeOptions.Converters.Add(new IFormCollectionConverter());
        serializeOptions.WriteIndented = true;
        return JsonSerializer.Serialize(form, serializeOptions);
    }

    //for deserialize implement read in IFormCollectionConverter
}