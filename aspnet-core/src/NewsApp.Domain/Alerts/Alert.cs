using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities.Auditing;

namespace NewsApp.Alerts
{
    public class Alert : AuditedAggregateRoot<int>
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        public string UserEmail { get; set; } = null!;

        [Required]
        public string Keyword { get; set; } = null!;

        public DateTime? LastSeenPublishedAt { get; set; }

        public Alert() { }
    }
}