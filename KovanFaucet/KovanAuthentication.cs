using System.IO;
using System.Text.Json;

namespace KovanFaucet
{
    public sealed record KovanAuthToken(string AccessToken);

    public static class KovanAuthProcessor
    {
        private const string AUTH_FILE = "auth.dat";

        public static void ConfigAuthentication(KovanAuthToken token)
        {
            using var fstream = File.OpenWrite(AUTH_FILE);
            using var sWriter = new StreamWriter(fstream);
            var toJson = JsonSerializer.Serialize(token);
            sWriter.Write(toJson);
            sWriter.Flush();
        }

        public static KovanAuthToken LoadAuthentication()
        {
            if (!File.Exists(AUTH_FILE)) return null;

            var readToEnd = File.ReadAllText(AUTH_FILE);
            return JsonSerializer.Deserialize<KovanAuthToken>(readToEnd);
        }

        public static bool IsAuthenticated() => File.Exists(AUTH_FILE);
    }
}
