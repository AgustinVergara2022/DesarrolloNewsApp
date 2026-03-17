using System;
namespace NewsApp.News
{
    public class NewsDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string? Author { get; set; }
        public DateTime? PublishedAt { get; set; }
    }
}