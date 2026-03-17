using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;

namespace NewsApp.ReadingLists
{
    public class ReadingList : AuditedAggregateRoot<int>
    {
        public string Name { get; set; }
        public Guid OwnerUserId { get; set; }

        public ICollection<ReadingListItem> Items { get; set; }

        public ReadingList()
        {
            Name = string.Empty;
            Items = new List<ReadingListItem>();
        }
    }
}
