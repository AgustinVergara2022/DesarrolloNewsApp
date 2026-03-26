using Microsoft.EntityFrameworkCore;
using NewsApp.Themes;
using NewsApp.ReadingLists;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.EntityFrameworkCore;

namespace NewsApp.EntityFrameworkCore;

public partial class NewsAppDbContext :
    AbpDbContext<NewsAppDbContext>,
    IIdentityDbContext,
    ITenantManagementDbContext
{
    /* Add DbSet properties for your Aggregate Roots / Entities here. */

    #region Entities from the modules

    //Identity
    public DbSet<IdentityUser> Users { get; set; }
    public DbSet<IdentityRole> Roles { get; set; }
    public DbSet<IdentityClaimType> ClaimTypes { get; set; }
    public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }
    public DbSet<IdentityLinkUser> LinkUsers { get; set; }
    public DbSet<IdentityUserDelegation> UserDelegations { get; set; }
    public DbSet<NewsApp.Alerts.Alert> Alerts { get; set; }

    // Tenant Management
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<TenantConnectionString> TenantConnectionStrings { get; set; }

    #endregion

    #region Entidades de dominio

    public DbSet<Theme> Themes { get; set; }

    // Nueva entidad News
    public DbSet<NewsApp.News.News> News { get; set; }

    // Reading lists
    public DbSet<ReadingList> ReadingLists { get; set; }
    public DbSet<ReadingListItem> ReadingListItems { get; set; }

    #endregion

    public NewsAppDbContext(DbContextOptions<NewsAppDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        /* Include modules to your migration db context */

        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureBackgroundJobs();
        builder.ConfigureAuditLogging();
        builder.ConfigureIdentity();
        builder.ConfigureOpenIddict();
        builder.ConfigureFeatureManagement();
        builder.ConfigureTenantManagement();

        /* Configure your own tables/entities inside here */

        //Entidad Theme
        builder.Entity<Theme>(b =>
        {
           b.ToTable(NewsAppConsts.DbTablePrefix + "Themes", NewsAppConsts.DbSchema);
           b.ConfigureByConvention();
           b.Property(x => x.Name).IsRequired().HasMaxLength(128);
        });

        // Entidad News: tabla AppNews
        builder.Entity<NewsApp.News.News>(b =>
        {
            b.ToTable(NewsAppConsts.DbTablePrefix + "News", NewsAppConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.Title).IsRequired().HasMaxLength(256);
            b.Property(x => x.Content).IsRequired();
            b.Property(x => x.Author).HasMaxLength(128);
            b.Property(x => x.PublishedAt);
        });

        // ReadingList
        builder.Entity<ReadingList>(b =>
        {
            b.ToTable(NewsAppConsts.DbTablePrefix + "ReadingLists", NewsAppConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.Name).IsRequired().HasMaxLength(256);
            b.Property(x => x.OwnerUserId).IsRequired();
            b.HasMany(x => x.Items).WithOne().HasForeignKey(x => x.ReadingListId).IsRequired();
        });

        builder.Entity<ReadingListItem>(b =>
        {
            b.ToTable(NewsAppConsts.DbTablePrefix + "ReadingListItems", NewsAppConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.NewsId).IsRequired();
            b.Property(x => x.Order).IsRequired();
        });

        builder.Entity<NewsApp.Alerts.Alert>(b =>
        {
            b.ToTable("AppAlerts");
            b.ConfigureByConvention();
            b.Property(x => x.Keyword).IsRequired().HasMaxLength(256);
            b.Property(x => x.UserEmail).IsRequired().HasMaxLength(256);
            b.Property(x => x.LastSeenPublishedAt);
        });
    }
}