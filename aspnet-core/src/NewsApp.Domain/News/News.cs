
using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace NewsApp.News
{
    public class News : AuditedAggregateRoot<int>
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string? Author { get; set; }
        public DateTime? PublishedAt { get; set; }

        public News()
        {
            Title = string.Empty;
            Content = string.Empty;
        }
    }
}