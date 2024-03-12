using System;
using System.Net.Http;
using System.Threading.Tasks;
using mie.era.mvc.Helpers;
using mie.era.mvc.Interfaces;
using mie.era.mvc.Models;

public class HomeService : IHomeService
{
    private readonly HttpClient _httpClient;
    private IConfiguration _config;

    public HomeService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _httpClient.BaseAddress = new Uri(config.GetConnectionString("ERABackendEndpoint"));
        _config = config;
    }

    public async Task<DashboardViewModel> GetDashboardData(HttpContext context)
    {
        _httpClient.DefaultRequestHeaders.Add("MIEAuthorization", "Bearer " + context.GetToken());


        try
        {
            var completedReferencesResponse = await _httpClient.GetAsync("Statistics/GetCompletedReferencesCount");
            var totalReferencesResponse = await _httpClient.GetAsync("Statistics/GetTotalReferencesCount");
            var averageCompletionTimeResponse = await _httpClient.GetAsync("Statistics/GetAverageCompletionTime");
            var averageCandidateScoreResponse = await _httpClient.GetAsync("Statistics/GetAverageCandidateScore");

            completedReferencesResponse.EnsureSuccessStatusCode();
            totalReferencesResponse.EnsureSuccessStatusCode();
            averageCompletionTimeResponse.EnsureSuccessStatusCode();
            averageCandidateScoreResponse.EnsureSuccessStatusCode();

            string completedReferencesCountStr = await completedReferencesResponse.Content.ReadAsStringAsync();
            string totalReferencesCountStr = await totalReferencesResponse.Content.ReadAsStringAsync();
            string averageCompletionTimeStr = await averageCompletionTimeResponse.Content.ReadAsStringAsync();
            string averageCandidateScoreStr = await averageCandidateScoreResponse.Content.ReadAsStringAsync();

            int completedReferencesCount = int.Parse(completedReferencesCountStr);
            int totalReferencesCount = int.Parse(totalReferencesCountStr);
           

            var viewModel = new DashboardViewModel
            {
                CompletedReferencesCount = completedReferencesCount,
                TotalReferencesCount = totalReferencesCount,
                AverageCompletionTime = averageCompletionTimeStr,
                AverageCandidateScore = averageCandidateScoreStr
            };

            return viewModel;
        }
        catch (HttpRequestException ex)
        {
            // Handle HTTP request errors
            throw new Exception($"Error: {ex.Message}");
        }
    }
}
