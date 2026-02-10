namespace Cooklyn.FunctionalTests.TestUtilities;

public static class ApiRoutes
{
    public const string Base = "api/v1";
    public const string Health = "health";
    public const string Alive = "alive";

    public static class Users
    {
        public static string GetList => $"{Base}/users";
        public static string GetRecord(string id) => $"{Base}/users/{id}";
        public static string Create => $"{Base}/users";
        public static string Put(string id) => $"{Base}/users/{id}";
        public static string Delete(string id) => $"{Base}/users/{id}";
    }

    public static class Weather
    {
        public static string Get => $"{Base}/weather";
        public static string GetSecure => $"{Base}/weather/secure";
    }
}
