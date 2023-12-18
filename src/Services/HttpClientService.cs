using Donace_BE_Project.Interfaces.Repositories;
using Donace_BE_Project.Models.Eto;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace Donace_BE_Project.Services
{
    public class HttpClientService : IHttpClientService
    {
        public async Task<T> CallApiPost<T>(string domain, string endpoint, T data) where T : BaseEto
        {
            using (var httpClient = new HttpClient())
            {
                // Cấu hình base address của API
                httpClient.BaseAddress = new Uri(domain);

                // Thêm thông tin xác thực vào header
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", data.Token);


                // Chuyển đổi dữ liệu thành JSON
                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Gửi POST request
                var response = await httpClient.PostAsync(endpoint, content);

                // Kiểm tra phản hồi từ API
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    return data;
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    return data;
                }
            }
        }
    }
}
