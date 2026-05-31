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

        public ApiClient(HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;

            var baseUrl = configuration["ApiSettings:BaseUrl"]
                ?? Environment.GetEnvironmentVariable("OISHIPAN_API_BASE_URL")
                ?? "http://localhost:8080";

            if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out var baseAddress))
            {
                throw new InvalidOperationException("ApiSettings:BaseUrl must be a valid absolute URI.");
            }

            _httpClient.BaseAddress = baseAddress;
        }

        public async Task<T> GetAsync<T>(string endpoint)
        {
            var response = await SendAsync(endpoint, () => _httpClient.GetAsync(endpoint));
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = !string.IsNullOrWhiteSpace(content)
                    ? content
                    : $"{response.StatusCode}: {response.ReasonPhrase}";

                var baseUrl = _httpClient.BaseAddress?.ToString() ?? "unknown";
                Console.WriteLine($"❌ API Error [{response.StatusCode}] {endpoint} @ {baseUrl}: {errorMessage}");
                throw new Exception($"API request failed: {response.StatusCode} - {errorMessage}");
            }

            if (string.IsNullOrWhiteSpace(content) || response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                if (typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(List<>))
                {
                    return (T)Activator.CreateInstance(typeof(T))!;
                }

                return default!;
            }

            try
            {
                return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
            }
            catch (JsonException ex)
            {
                var baseUrl = _httpClient.BaseAddress?.ToString() ?? "unknown";
                var message = $"Failed to deserialize JSON from '{endpoint}' (Status: {response.StatusCode}). Content: {content}. BaseUrl: {baseUrl}";
                Console.WriteLine($"❌ JSON PARSE: {message}");
                throw new Exception(message, ex);
            }
        }

        public async Task<T> PostAsync<T>(string endpoint, object data)
        {
            var json = JsonSerializer.Serialize(data);
            var response = await SendAsync(endpoint, async () => 
            {
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                return await _httpClient.PostAsync(endpoint, content);
            });

            var responseContent = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = !string.IsNullOrWhiteSpace(responseContent)
                    ? responseContent
                    : $"{response.StatusCode}: {response.ReasonPhrase}";

                var baseUrl = _httpClient.BaseAddress?.ToString() ?? "unknown";
                Console.WriteLine($"❌ API Error [{response.StatusCode}] {endpoint} @ {baseUrl}: {errorMessage}");
                throw new Exception($"API request failed: {response.StatusCode} - {errorMessage}");
            }

            return JsonSerializer.Deserialize<T>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        }

        public async Task<T> PutAsync<T>(string endpoint, object data)
        {
            var json = JsonSerializer.Serialize(data);
            var response = await SendAsync(endpoint, async () => 
            {
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                return await _httpClient.PutAsync(endpoint, content);
            });

            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        }

        public async Task<bool> DeleteAsync(string endpoint)
        {
            var response = await SendAsync(endpoint, () => _httpClient.DeleteAsync(endpoint));
            return response.IsSuccessStatusCode;
        }

        public async Task<T> PostFormAsync<T>(string endpoint, MultipartFormDataContent content)
        {
            var response = await SendAsync(endpoint, () => _httpClient.PostAsync(endpoint, content));

            var responseContent = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = !string.IsNullOrWhiteSpace(responseContent)
                    ? responseContent
                    : $"{response.StatusCode}: {response.ReasonPhrase}";

                var baseUrl = _httpClient.BaseAddress?.ToString() ?? "unknown";
                Console.WriteLine($"❌ API Error [{response.StatusCode}] {endpoint} @ {baseUrl}: {errorMessage}");
                throw new Exception($"API request failed: {response.StatusCode} - {errorMessage}");
            }

            if (string.IsNullOrWhiteSpace(responseContent) || response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                if (typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(List<>))
                {
                    return (T)Activator.CreateInstance(typeof(T))!;
                }

                return default!;
            }

            try
            {
                return JsonSerializer.Deserialize<T>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
            }
            catch (JsonException ex)
            {
                var baseUrl = _httpClient.BaseAddress?.ToString() ?? "unknown";
                var message = $"Failed to deserialize JSON from '{endpoint}' (Status: {response.StatusCode}). Content: {responseContent}. BaseUrl: {baseUrl}";
                Console.WriteLine($"❌ JSON PARSE: {message}");
                throw new Exception(message, ex);
            }
        }

        public void SetAuthToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return;

            _httpContextAccessor.HttpContext?.Session?.SetString("ApiToken", token);
        }

        public void ClearAuthToken()
        {
            _httpContextAccessor.HttpContext?.Session?.Remove("ApiToken");
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }

        private void AddAuthorizationHeader()
        {
            var token = _httpContextAccessor.HttpContext?.Session?.GetString("ApiToken");
            if (!string.IsNullOrWhiteSpace(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }

        private async Task<HttpResponseMessage> SendAsync(string endpoint, Func<Task<HttpResponseMessage>> requestFunc)
        {
            AddAuthorizationHeader();
            try
            {
                var response = await requestFunc();
                return response;
            }
            catch (OperationCanceledException ex)
            {
                var baseUrl = _httpClient.BaseAddress?.ToString() ?? "unknown";
                var message = $"API request timeout for '{endpoint}'. BaseUrl: {baseUrl}. The API may not be responding. Please ensure it's running.";
                Console.WriteLine($"❌ TIMEOUT: {message}");
                throw new Exception(message, ex);
            }
            catch (HttpRequestException ex) when (IsConnectionRefused(ex))
            {
                var baseUrl = _httpClient.BaseAddress?.ToString() ?? "unknown";
                var message = $"❌ Connection Refused for '{endpoint}' at {baseUrl}. " +
                    $"Ensure the API is running with: dotnet run --project OishipanAPI";
                Console.WriteLine(message);
                throw new Exception(message, ex);
            }
            catch (HttpRequestException ex)
            {
                var baseUrl = _httpClient.BaseAddress?.ToString() ?? "unknown";
                var message = $"API request failed for '{endpoint}': {ex.Message}. BaseUrl: {baseUrl}";
                Console.WriteLine($"❌ ERROR: {message}");
                throw new Exception(message, ex);
            }
            catch (Exception ex)
            {
                var message = $"Unexpected error calling '{endpoint}': {ex.Message}";
                Console.WriteLine($"❌ UNEXPECTED: {message}");
                throw new Exception(message, ex);
            }
        }

        private static bool IsConnectionRefused(HttpRequestException ex)
        {
            return ex.InnerException is SocketException socketEx && 
                (socketEx.SocketErrorCode == SocketError.ConnectionRefused || 
                 socketEx.Message.Contains("actively refused"));
        }
    }
}
