using System;
using System.Net;
using System.Net.Http;

namespace AddonUpdater.Utils
{
    /// <summary>
    /// Singleton HttpClient. HttpClient рассчитан на переиспользование —
    /// создание нового экземпляра на каждый запрос приводит к исчерпанию
    /// сокетов и медленным соединениям.
    /// </summary>
    public static class HttpClientProvider
    {
        private static readonly Lazy<HttpClient> _instance = new(CreateClient);

        public static HttpClient Instance => _instance.Value;

        private static HttpClient CreateClient()
        {
            ServicePointManager.SecurityProtocol =
                SecurityProtocolType.Tls |
                SecurityProtocolType.Tls11 |
                SecurityProtocolType.Tls12;

            var handler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };

            var client = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(30)
            };

            client.DefaultRequestHeaders.UserAgent.ParseAdd("AddonUpdater/1.0");
            return client;
        }
    }
}
