using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace NewsApp.EntityFrameworkCore.Services
{
    public class NewsApiClient : INewsApiClient
    {
        private readonly HttpClient _http;
        private readonly string _apiKey;

        public NewsApiClient(HttpClient http, IConfiguration configuration)
        {
            _http = http;
            _apiKey = configuration["ExternalNewsApi:ApiKey"]
            ?? throw new ArgumentNullException("ExternalNewsApi:ApiKey");
        }

        public async Task<IList<NewsArticle>> GetArticlesAsync(string query, DateTime? from = null)
        {
            var url = $"https://newsapi.org/v2/everything?q={Uri.EscapeDataString(query)}&apiKey={_apiKey}&pageSize=50&sortBy=publishedAt";
            if (from.HasValue) url += $"&from={from.Value.ToString("o")}";
            var resp = await _http.GetFromJsonAsync<NewsApiResponse>(url);
            var list = new List<NewsArticle>();
            if (resp?.Articles != null)
            {
                foreach (var a in resp.Articles)
                {
                    list.Add(new NewsArticle(a.Title, a.Description, a.Url, a.PublishedAt));
                }
            }
            return list;
        }

        private class NewsApiResponse
        {
            public string Status { get; set; } = null!;
            public int TotalResults { get; set; }
            public Article[] Articles { get; set; } = Array.Empty<Article>();
        }

        private class Article
        {
            public string Title { get; set; } = null!;
            public string? Description { get; set; }
            public string Url { get; set; } = null!;
            public DateTime? PublishedAt { get; set; }
        }
    }
}