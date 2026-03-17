using System;
using System.Collections.Generic;

namespace NewsApp.ReadingLists
{
    public class ReadingListDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid OwnerUserId { get; set; }
        public List<int> NewsIds { get; set; } = new();
    }
}