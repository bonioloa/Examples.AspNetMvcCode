namespace Comunica.ProcessManager.Web.Code;

//https://blog.bitscry.com/2017/08/31/complex-objects-in-session-and-tempdata-variables/
//https://docs.microsoft.com/en-us/aspnet/core/fundamentals/app-state?view=aspnetcore-2.2
//https://www.talkingdotnet.com/store-complex-objects-in-asp-net-core-session/
public static class SessionCustomExtensions
{
    public static void Set<T>(this ISession session, string key, T value)
    {
        session.SetString(key, JsonSerializer.Serialize(value));
    }
    public static T Get<T>(this ISession session, string key)
    {
        string value = session.GetString(key);

        return value == null ? default :
            JsonSerializer.Deserialize<T>(value);
    }

    public static void SetBoolean(this ISession session, string key, bool value)
    {
        session.Set(key, BitConverter.GetBytes(value));
    }
    public static bool? GetBoolean(this ISession session, string key)
    {
        return
            session.TryGetValue(key, out byte[] data) && data != null
                ? BitConverter.ToBoolean(data, 0)
                : (bool?)null;
    }

    public static void SetDouble(this ISession session, string key, double value)
    {
        session.Set(key, BitConverter.GetBytes(value));
    }
    public static double? GetDouble(this ISession session, string key)
    {
        return
            session.TryGetValue(key, out byte[] data) && data != null
                ? BitConverter.ToDouble(data, 0)
                : (double?)null;
    }

    public static void SetInt64(this ISession session, string key, long value)
    {
        session.Set(key, BitConverter.GetBytes(value));
    }
    public static long? GetInt64(this ISession session, string key)
    {
        return
            session.TryGetValue(key, out byte[] data) && data != null
                ? BitConverter.ToInt64(data, 0)
                : (long?)null;
    }
    public static void SetGuid(this ISession session, string key, Guid value)
    {
        session.SetString(key, value.ToString());
    }
    public static Guid GetGuid(this ISession session, string key)
    {
        string guidStr = session.GetString(key);
        return
            Guid.TryParse(guidStr, out Guid guid)
            ? guid : Guid.Empty;
    }

    public static long GetLongSafe(this ISession session, string key)
    {
        if (session != null)
        {
            long? sessionValue = session.GetInt64(key);

            if (sessionValue.HasValue)
            {
                return (long)sessionValue;
            }
        }
        return long.MinValue;
    }

    public static bool GetBooleanSafe(this ISession session, string key)
    {
        if (session != null)
        {
            bool? sessionValue =
                session.GetBoolean(key);

            if (sessionValue.HasValue)
            {
                return (bool)sessionValue;
            }
        }
        return false;
    }
}
