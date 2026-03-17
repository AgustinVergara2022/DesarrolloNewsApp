using System.Collections.Generic;

namespace NewsApp.ReadingLists
{
    public class CreateUpdateReadingListDto
    {
        public string Name { get; set; } = string.Empty;
        public List<int> NewsIds { get; set; } = new();
    }
}