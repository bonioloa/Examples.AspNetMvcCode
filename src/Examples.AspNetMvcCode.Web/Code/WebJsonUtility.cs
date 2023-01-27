namespace Examples.AspNetMvcCode.Web.Code;

public static class WebJsonUtility
{
    //this option will serialize properties with first letter to lowercase
    //so we can define json object with normal C# convention 
    //and serialization will change the case accordingly
    public static readonly JsonSerializerOptions JsonObjectModelOptions =
        new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
}