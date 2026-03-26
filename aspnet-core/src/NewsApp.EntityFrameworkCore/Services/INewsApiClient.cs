using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NewsApp.EntityFrameworkCore.Services
{
    public record NewsArticle(string Title, string Description, string Url, DateTime? PublishedAt);

    public interface INewsApiClient
    {
        Task<IList<NewsArticle>> GetArticlesAsync(string query, DateTime? from = null);
    }
}