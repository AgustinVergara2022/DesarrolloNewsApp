using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;
using NewsApp.News;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;



namespace NewsApp.News
{
    public class NewsAppService : ApplicationService, NewsApp.News.INewsAppService
    {
        private readonly IRepository<News, int> _newsRepository;

        public NewsAppService(IRepository<News, int> newsRepository)
        {
            _newsRepository = newsRepository;
        }

        public async Task<List<NewsDto>> SearchAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return new List<NewsDto>();
            }

            // Busca por título o contenido que contengan la query
            var items = await _newsRepository.GetListAsync(n => n.Title.Contains(query) || n.Content.Contains(query));

            return items.Select(n => new NewsDto
            {
                Id = n.Id,
                Title = n.Title,
                Content = n.Content,
                Author = n.Author,
                PublishedAt = n.PublishedAt
            }).ToList();
        }
    }
}