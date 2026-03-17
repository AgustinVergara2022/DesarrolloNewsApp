using System;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using NewsApp.Domain.Alerts;

namespace NewsApp.EntityFrameworkCore
{
    public partial class NewsAppDbContext : AbpDbContext<NewsAppDbContext>
    {
        
        public DbSet<Alert> Alerts { get; set; } = null!;

    }
}