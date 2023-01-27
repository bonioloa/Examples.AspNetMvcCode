namespace Comunica.ProcessManager.Web.Code;

//https://blog.bitscry.com/2017/08/31/complex-objects-in-session-and-tempdata-variables/
public static class TempDataExtensions
{
    public static void Put<T>(this ITempDataDictionary tempData, string key, T value) where T : class
    {
        tempData[key] = JsonSerializer.Serialize(value);
    }

    public static T Get<T>(this ITempDataDictionary tempData, string key) where T : class
    {
        tempData.TryGetValue(key, out object o);
        return o == null ? null : JsonSerializer.Deserialize<T>((string)o);
    }

    public static T Peek<T>(this ITempDataDictionary tempData, string key) where T : class
    {
        object o = tempData.Peek(key);
        return o == null ? null : JsonSerializer.Deserialize<T>((string)o);
    }

    public static void SetInt64(this ITempDataDictionary tempData, string key, long value)
    {
        tempData[key] = value.ToString();
    }
    public static long GetLongSafe(this ITempDataDictionary tempData, string key)
    {
        return
            tempData != null
                && tempData.TryGetValue(key, out object data)
                && data != null
                && data is string tmpValueAsString
                && long.TryParse(tmpValueAsString, out long value)
            ? value
            : long.MinValue;
    }


    public static bool GetBooleanSafe(this ITempDataDictionary tempData, string key)
    {
        return
            tempData != null
                && tempData.TryGetValue(key, out object data)
                && data != null
                && data is string tmpValueAsString
                && bool.TryParse(tmpValueAsString, out bool value)
                && value;
    }
    public static void SetBoolean(this ITempDataDictionary tempData, string key, bool value)
    {
        tempData[key] = value.ToString();
    }
}
