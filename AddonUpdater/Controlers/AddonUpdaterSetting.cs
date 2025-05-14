using AddonUpdater.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace AddonUpdater.Controlers
{
    class AddonUpdaterSetting
    {

        public static Setting Setting = new();

        public static Task GetSettingsTask()
        {
            return Task.Run(async() => await GetSettingsAsync());
        }

        private static async Task GetSettingsAsync()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            using HttpClient client = new();
            client.Timeout = TimeSpan.FromSeconds(10);
            
            try
            {
                var response = await client.GetAsync("https://gist.githubusercontent.com/accidev/7699f8279a4bce54f26c220c0be8be9c/raw/adb1647b01c5146e57fdf41d476e7afde8f276de/AddonUpdaterSettings");
                response.EnsureSuccessStatusCode();
                
                string result = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(result))
                {
                    Setting = JsonConvert.DeserializeObject<Setting>(result) ?? new Setting();
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Ошибка HTTP запроса: {ex.Message}");
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine("Запрос был отменен из-за истечения таймаута");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла ошибка: {ex.Message}");
            }
        }
    }
}
