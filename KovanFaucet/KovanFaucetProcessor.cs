using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KovanFaucet
{
    public sealed record KovanFaucetRequest
    {
        [JsonPropertyName("text")]
        public string Text { get; init; }
    }

    public static class KovanFaucetProcessor
    {
        private const string URL = "https://gitter.im/api/v1/rooms/58ba6dc5d73408ce4f4e4d68/chatMessages";
        private const string TOKEN = "x-access-token";

        public static async Task SendRequestAsync(KovanAddressCollection addressCollection, 
            Action<string> action = null,
            Action<string> errorAction = null)
        {
            if (addressCollection == null || !addressCollection.Any())
            {
                await Task.CompletedTask;
                errorAction?.Invoke("\nNo ETH address provided!");
                return;
            }

            foreach (var address in addressCollection)
            {
                await SendRequestAsync(address, action, errorAction);
            }
        }

        private static async Task SendRequestAsync(KovanAddress address, 
            Action<string> action = null,
            Action<string> errorAction = null)
        
        {
            try
            {
                var client = WebRequest.Create(URL);
                var authToken = KovanAuthProcessor.LoadAuthentication();

                var payload = new KovanFaucetRequest
                {
                    Text = address.EthereumAddress
                };
                var payloadToJson = JsonSerializer.Serialize(payload);
                var payloadToBytes = Encoding.ASCII.GetBytes(payloadToJson);

                client.Method = "POST";
                client.ContentType = "application/json";
                client.GetRequestStream()
                    .Write(payloadToBytes, 0, payloadToBytes.Length);
                client.Headers.Add(TOKEN, authToken.AccessToken);
                client.Headers.Add("Host", "gitter.im");

                action?.Invoke($"\nRequest: {address.EthereumAddress}");

                var respone = await client.GetResponseAsync();

                action?.Invoke($"Response: {((HttpWebResponse)respone).StatusCode}");
            }
            catch (Exception ex)
            {
                errorAction?.Invoke($"Response: {ex.Message}");
            }
        }
    }
}
