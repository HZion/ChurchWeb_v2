using ChurchWeb.Core.Entities.Common;
using ChurchWeb.Core.Entities.Home;
using ChurchWeb.Core.Entities.Identity;
using ChurchWeb.Core.Entities.News;
using ChurchWeb.Core.Entities.Outreach;
using ChurchWeb.Core.Entities.People;
using ChurchWeb.Core.Entities.Sermons;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ChurchWeb.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    // Common
    public DbSet<ChurchInfo> ChurchInfos { get; set; }
    public DbSet<MenuItem> MenuItems { get; set; }
    public DbSet<NavMenuItem> NavMenuItems { get; set; }
    public DbSet<HomeSection> HomeSections { get; set; }

    // Home
    public DbSet<HeroSlide> HeroSlides { get; set; }
    public DbSet<Vision> Visions { get; set; }
    public DbSet<VisionPractice> VisionPractices { get; set; }
    public DbSet<PastorGreeting> PastorGreetings { get; set; }
    public DbSet<QuickLink> QuickLinks { get; set; }

    // Sermons
    public DbSet<Sermon> Sermons { get; set; }

    // News
    public DbSet<Bulletin> Bulletins { get; set; }
    public DbSet<BulletinPage> BulletinPages { get; set; }
    public DbSet<Album> Albums { get; set; }
    public DbSet<AlbumPhoto> AlbumPhotos { get; set; }
    public DbSet<Notice> Notices { get; set; }
    public DbSet<NoticeAttachment> NoticeAttachments { get; set; }
    public DbSet<CalendarEvent> CalendarEvents { get; set; }

    // Outreach
    public DbSet<Evangelist> Evangelists { get; set; }

    // People
    public DbSet<Person> People { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 모든 테이블을 churchweb 스키마에 생성 (기존 public 스키마와 충돌 방지)
        modelBuilder.HasDefaultSchema("churchweb");

        // Vision - VisionPractice 관계
        modelBuilder.Entity<VisionPractice>()
            .HasOne(vp => vp.Vision)
            .WithMany(v => v.Practices)
            .HasForeignKey(vp => vp.VisionId)
            .OnDelete(DeleteBehavior.Cascade);

        // Bulletin - BulletinPage 관계
        modelBuilder.Entity<BulletinPage>()
            .HasOne(bp => bp.Bulletin)
            .WithMany(b => b.Pages)
            .HasForeignKey(bp => bp.BulletinId)
            .OnDelete(DeleteBehavior.Cascade);

        // Album - AlbumPhoto 관계
        modelBuilder.Entity<AlbumPhoto>()
            .HasOne(ap => ap.Album)
            .WithMany(a => a.Photos)
            .HasForeignKey(ap => ap.AlbumId)
            .OnDelete(DeleteBehavior.Cascade);

        // Notice - NoticeAttachment 관계
        modelBuilder.Entity<NoticeAttachment>()
            .HasOne(na => na.Notice)
            .WithMany(n => n.Attachments)
            .HasForeignKey(na => na.NoticeId)
            .OnDelete(DeleteBehavior.Cascade);

        // 인덱스
        modelBuilder.Entity<MenuItem>()
            .HasIndex(m => m.Key)
            .IsUnique();

        modelBuilder.Entity<HomeSection>()
            .HasIndex(hs => hs.Key)
            .IsUnique();

        modelBuilder.Entity<Sermon>()
            .HasIndex(s => new { s.IsVisible, s.PreachedOn });

        modelBuilder.Entity<Notice>()
            .HasIndex(n => new { n.IsVisible, n.PostedOn });

        // NavMenuItem 자기 참조 관계 (Parent-Children)
        modelBuilder.Entity<NavMenuItem>()
            .HasOne(n => n.Parent)
            .WithMany(n => n.Children)
            .HasForeignKey(n => n.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<NavMenuItem>()
            .HasIndex(n => new { n.ParentId, n.SortOrder });
    }
}
