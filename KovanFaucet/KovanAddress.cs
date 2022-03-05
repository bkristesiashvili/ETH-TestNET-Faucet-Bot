using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace KovanFaucet
{
    [Serializable]
    public sealed record KovanAddress(string EthereumAddress);

    [Serializable]
    public sealed class KovanAddressCollection : List<KovanAddress> { }

    public static class KovanFileProcessor
    {
        public static KovanAddressCollection LoadFromFile(string filePath)
        {
            if (!File.Exists(filePath)) return null;

            var readToEnd = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<KovanAddressCollection>(readToEnd);
        }

        public static async Task SaveToFile(KovanAddress address, string filePath)
        {
            if (address == null) throw new ArgumentNullException(nameof(address));

            var fromFile = LoadFromFile(filePath) ?? new KovanAddressCollection();

            fromFile.Add(address);

            using var fstream = File.OpenWrite(filePath);
            using var sWriter = new StreamWriter(fstream);
            var toJson = JsonSerializer.Serialize(fromFile);
            await sWriter.WriteAsync(toJson);
            await sWriter.FlushAsync();
        }
    }
}
