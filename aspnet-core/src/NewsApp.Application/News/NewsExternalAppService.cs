using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using NewsApp.News;
using Volo.Abp.Application.Services;
using Microsoft.Extensions.Logging;
using Google.Api;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;


namespace NewsApp.News
{
    public class NewsExternalAppService : ApplicationService, INewsExternalAppService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<NewsExternalAppService> _logger;

        public NewsExternalAppService(IHttpClientFactory httpClientFactory, ILogger<NewsExternalAppService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<List<NewsDto>> SearchExternalAsync(string query, int pageSize = 20)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return new List<NewsDto>();
            }

            var client = _httpClientFactory.CreateClient("NewsApi");
            // usamos el endpoint /everything de newsapi.org
            var url = $"everything?q={Uri.EscapeDataString(query)}&pageSize={pageSize}&language=en";

            HttpResponseMessage response;
            try
            {
                response = await client.GetAsync(url);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling external news API");
                throw;
            }

            if (!response.IsSuccessStatusCode)
            {
                var err = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("External news API returned {Status}: {Body}", response.StatusCode, err);
                throw new ApplicationException($"External news API error: {response.StatusCode}");
            }

            var content = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var result = JsonSerializer.Deserialize<NewsApiResponse>(content, options);
            if (result?.Articles == null)
            {
                return new List<NewsDto>();
            }

            return result.Articles.Select(a => new NewsDto
            {
                Id = 0,
                Title = a.Title ?? string.Empty,
                Content = a.Content ?? a.Description ?? string.Empty,
                Author = a.Author,
                PublishedAt = a.PublishedAtDateTime
            }).ToList();
        }

        private class NewsApiResponse
        {
            public string Status { get; set; }
            public int TotalResults { get; set; }
            public Article[] Articles { get; set; }
        }

        private class Article
        {
            public SourceInfo Source { get; set; }
            public string Author { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public string Url { get; set; }
            public string UrlToImage { get; set; }
            public string PublishedAt { get; set; }
            public string Content { get; set; }

            // Helper to parse publishedAt to DateTime?
            public DateTime? PublishedAtParsed
            {
                get
                {
                    if (DateTime.TryParse(PublishedAt, out var dt)) return dt;
                    return null;
                }
            }

            // Map PublishedAt as DateTime? for DTO
            public DateTime? PublishedAtDateTime => PublishedAtParsed;
        }
    }
}