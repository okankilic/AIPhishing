using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using AIPhishing.Business.Configurations;
using AIPhishing.Business.Integrations.Models;
using AIPhishing.Common.Exceptions;
using Microsoft.Extensions.Options;

namespace AIPhishing.Business.Integrations;

public class PhishingAiApiClient : IPhishingAiApiClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly PhishingAiConfiguration _configuration;

    public PhishingAiApiClient(
        IHttpClientFactory httpClientFactory, 
        IOptions<PhishingAiConfiguration> configurationOptions)
    {
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _configuration = configurationOptions?.Value ?? throw new ArgumentNullException(nameof(configurationOptions));
    }

    public Task<PhishingAiGetEmailContentResponse> CreateEmailContentAsync(string language, PhishingAiGetEmailContentRequest request)
    {
        return DoPostAsync<PhishingAiGetEmailContentRequest, PhishingAiGetEmailContentResponse>($"{_configuration.ApiBaseUrl}/{language}/api/create_email_content/", request);
    }

    public Task<string> GetReplyEmailContentAsync(string language, PhishingAiGetReplyEmailContentRequest request)
    {
        return DoPostAsync($"{_configuration.ApiBaseUrl}/{language}/api/create_reply_email_content/", request);
    }

    private async Task<TRes> DoPostAsync<TReq, TRes>(string url, TReq request)
        where TReq : class
        where TRes : class
    {
        using var httpClient = _httpClientFactory.CreateClient();

        AddApiKeyHeader(httpClient);
        
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

        var httpResponseMessage = await httpClient.PostAsync(url, content);

        // var httpResponseMessage = await httpClient.PostAsJsonAsync(url, request, new JsonSerializerOptions
        // {
        //     PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        // });

        return await ReturnApiResult<TRes>(httpResponseMessage);
    }
    
    private async Task<string> DoPostAsync<TReq>(string url, TReq request)
        where TReq : class
    {
        using var httpClient = _httpClientFactory.CreateClient();

        AddApiKeyHeader(httpClient);
        
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

        var httpResponseMessage = await httpClient.PostAsync(url, content);

        // var httpResponseMessage = await httpClient.PostAsJsonAsync(url, request, new JsonSerializerOptions
        // {
        //     PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        // });

        return await ReturnApiResult(httpResponseMessage);
    }
    
    private void AddApiKeyHeader(HttpClient httpClient)
    {
        //httpClient.DefaultRequestHeaders.Add("x-api-key", _configuration.ApiKey.ToString());
    }
    
    private static async Task<T> ReturnApiResult<T>(HttpResponseMessage httpResponseMessage) where T : class
    {
        var responseContent = await httpResponseMessage.Content.ReadAsStringAsync();
        
        if (!httpResponseMessage.IsSuccessStatusCode)
            throw new IntegrationException(nameof(PhishingAiApiClient), responseContent ?? httpResponseMessage.ReasonPhrase ?? $"An exception occured while integrating with Phising AI.");

        var apiResult = JsonSerializer.Deserialize<T>(responseContent, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        return apiResult!;
    }
    
    private static async Task<string> ReturnApiResult(HttpResponseMessage httpResponseMessage)
    {
        var responseContent = await httpResponseMessage.Content.ReadAsStringAsync();
        
        if (!httpResponseMessage.IsSuccessStatusCode)
            throw new IntegrationException(nameof(PhishingAiApiClient), responseContent ?? httpResponseMessage.ReasonPhrase ?? $"An exception occured while integrating with Phising AI.");

        return responseContent;
    }
}