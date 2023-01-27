namespace Examples.AspNetMvcCode.Common;

//https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-converters-how-to?view=netcore-3.0
public class IHtmlContentConverter : JsonConverter<IHtmlContent>
{
    public override IHtmlContent Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options) =>
            new HtmlString(reader.GetString());

    public override void Write(
        Utf8JsonWriter writer
        , IHtmlContent value
        , JsonSerializerOptions options
        )
    {
        Guard.Against.Null(writer, nameof(IHtmlContentConverter));

        writer.WriteStringValue(
            value.GetStringContent()
            );
    }
}



public class IFormCollectionConverter : JsonConverter<IFormCollection>
{
    public override IFormCollection Read(
        ref Utf8JsonReader reader
        , Type typeToConvert
        , JsonSerializerOptions options
        )
    {
        throw new NotImplementedException($"{nameof(IFormCollectionConverter)} {nameof(JsonConverter)} {nameof(Read)} must be implemented");
    }

    public override void Write(
        Utf8JsonWriter writer
        , IFormCollection value
        , JsonSerializerOptions options
        )
    {
        Guard.Against.Null(writer, nameof(IFormCollectionConverter));

        writer.WriteStartObject();

        if (value.IsNullOrEmpty())
        {
            writer.WriteStringValue(string.Empty);
            writer.WriteEndObject();
            return;
        }
        foreach (string key in value.Keys)
        {
            writer.WritePropertyName(key);

            StringValues singleValue = value[key];
            writer.WriteStringValue(singleValue);
        }
        writer.WriteEndObject();
        return;
    }
}

//we need to create this serializer because CultureInfo has some serialization problem
public class CultureInfoConverter : JsonConverter<CultureInfo>
{
    public override CultureInfo Read(
        ref Utf8JsonReader reader
        , Type typeToConvert
        , JsonSerializerOptions options
        )
    {
        throw new NotImplementedException($"{nameof(CultureInfoConverter)} {nameof(JsonConverter)} {nameof(Read)} must be implemented");
    }

    public override void Write(
        Utf8JsonWriter writer
        , CultureInfo value
        , JsonSerializerOptions options
        )
    {
        Guard.Against.Null(writer, nameof(CultureInfoConverter));

        writer.WriteStartObject();

        if (value is null)
        {
            writer.WriteStringValue(string.Empty);
            writer.WriteEndObject();
            return;
        }

        writer.WritePropertyName(nameof(value.TwoLetterISOLanguageName));
        writer.WriteStringValue(value.TwoLetterISOLanguageName);

        writer.WritePropertyName(nameof(value.Name));
        writer.WriteStringValue(value.Name);

        writer.WritePropertyName(nameof(value.DisplayName));
        writer.WriteStringValue(value.DisplayName);

        writer.WriteEndObject();
        return;
    }
}