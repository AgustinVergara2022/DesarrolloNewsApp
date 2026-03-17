using System;
using System.Collections.Generic;
using NewsApp.Notifications;
using NewsApp.Users;
using Volo.Abp.Application.Dtos;

namespace NewsApp.Alerts
{
    public class AlertDto : EntityDto<int>
    {
        public bool active { get; set; }
        // relaciones
        public string topic { get; set; }
        public DateTime createdAt { get; set; }
        public UserDto User { get; set; }
        public ICollection<NotificationDto> Notifications { get; set; }
    }
}