using Volo.Abp.Domain.Entities;

namespace NewsApp.ReadingLists
{
    public class ReadingListItem : Entity<int>
    {
        public int ReadingListId { get; set; }
        public int NewsId { get; set; }
        public int Order { get; set; }
    }
}