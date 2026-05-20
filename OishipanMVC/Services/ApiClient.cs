using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace OishipanMVC.Services
{
    public interface IApiClient
    {
        Task<T> GetAsync<T>(string endpoint);
        Task<T> PostAsync<T>(string endpoint, object data);
        Task<T> PutAsync<T>(string endpoint, object data);
        Task<bool> DeleteAsync(string endpoint);
        Task<T> PostFormAsync<T>(string endpoint, MultipartFormDataContent content);
        void SetAuthToken(string token);
        void ClearAuthToken();
    }

    public class ApiClient : IApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly List<Uri> _fallbackBaseAddresses = new();

        public ApiClient(HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;

            var baseUrl = configuration["ApiSettings:BaseUrl"];
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                throw new InvalidOperationException("ApiSettings:BaseUrl is not configured.");
            }

            if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out var baseAddress))
            {
                throw new InvalidOperationException("ApiSettings:BaseUrl must be a valid absolute URI.");
            }

            _httpClient.BaseAddress = baseAddress;
            _fallbackBaseAddresses.AddRange(GetFallbackBaseAddresses(baseAddress));

            var token = _httpContextAccessor.HttpContext?.Session.GetString("JwtToken");
            if (!string.IsNullOrWhiteSpace(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<T> GetAsync<T>(string endpoint)
        {
            var response = await SendAsync(() => _httpClient.GetAsync(endpoint), endpoint);
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        }

        public async Task<T> PostAsync<T>(string endpoint, object data)
        {
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var response = await SendAsync(() => _httpClient.PostAsync(endpoint, content), endpoint);

            var responseContent = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = !string.IsNullOrWhiteSpace(responseContent)
                    ? responseContent
                    : response.ReasonPhrase;

                throw new Exception($"API request failed: {response.StatusCode} - {errorMessage}");
            }

            return JsonSerializer.Deserialize<T>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        }

        public async Task<T> PutAsync<T>(string endpoint, object data)
        {
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var response = await SendAsync(() => _httpClient.PutAsync(endpoint, content), endpoint);

            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        }

        public async Task<bool> DeleteAsync(string endpoint)
        {
            var response = await SendAsync(() => _httpClient.DeleteAsync(endpoint), endpoint);
            return response.IsSuccessStatusCode;
        }

        public async Task<T> PostFormAsync<T>(string endpoint, MultipartFormDataContent content)
        {
            var response = await SendAsync(() => _httpClient.PostAsync(endpoint, content), endpoint);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        }

        public void SetAuthToken(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public void ClearAuthToken()
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }

        private async Task<HttpResponseMessage> SendAsync(Func<Task<HttpResponseMessage>> requestFunc, string endpoint)
        {
            try
            {
                return await requestFunc();
            }
            catch (HttpRequestException ex) when (IsConnectionRefused(ex) && _fallbackBaseAddresses.Any())
            {
                foreach (var fallback in _fallbackBaseAddresses)
                {
                    _httpClient.BaseAddress = fallback;
                    try
                    {
                        return await requestFunc();
                    }
                    catch (HttpRequestException retryEx) when (IsConnectionRefused(retryEx))
                    {
                        continue;
                    }
                }

                throw new Exception(GetConnectionErrorMessage(endpoint, ex), ex);
            }
            catch (HttpRequestException ex)
            {
                throw new Exception(GetConnectionErrorMessage(endpoint, ex), ex);
            }
        }

        private static bool IsConnectionRefused(HttpRequestException ex)
        {
            return ex.InnerException is SocketException socketEx && socketEx.SocketErrorCode == SocketError.ConnectionRefused;
        }

        private static string GetConnectionErrorMessage(string endpoint, HttpRequestException ex)
        {
            return $"API request failed for '{endpoint}': {ex.Message}. Please verify that the API is running at the configured BaseUrl.";
        }

        private static IEnumerable<Uri> GetFallbackBaseAddresses(Uri baseAddress)
        {
            if (!baseAddress.IsLoopback)
            {
                return Enumerable.Empty<Uri>();
            }

            var fallbackAddresses = new List<Uri>();
            if (baseAddress.Scheme == "http" && baseAddress.Port == 5000)
            {
                fallbackAddresses.Add(new Uri("https://localhost:5001"));
            }
            else if (baseAddress.Scheme == "https" && baseAddress.Port == 5001)
            {
                fallbackAddresses.Add(new Uri("http://localhost:5000"));
            }

            return fallbackAddresses;
        }
    }
}
