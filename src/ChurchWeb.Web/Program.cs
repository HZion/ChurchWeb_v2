using ChurchWeb.Application.Services;
using ChurchWeb.Core.Entities.Identity;
using ChurchWeb.Infrastructure.Data;
using ChurchWeb.Web.Models.Home;
using ChurchWeb.Web.Services;
using ChurchWeb.Web.MigrationScripts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);

// Configure ForwardedHeaders for Render proxy
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

// Check for migration commands
if (args.Length > 0 && args[0] == "inspect-gallery")
{
    var inspector = new GallerySchemaInspector(builder.Configuration);
    await inspector.InspectSchemaAsync();
    return;
}

if (args.Length > 0 && args[0] == "inspect-data")
{
    var inspector = new GalleryDataInspector(builder.Configuration);
    await inspector.InspectDataAsync();
    return;
}

if (args.Length > 0 && args[0] == "import-gallery")
{
    var importer = new SqlGalleryImporter(builder.Configuration);
    await importer.ImportAsync();
    return;
}

// DB 연결 문자열 (환경변수에서 읽기)
var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Connection string 'Default' not found.");

// PostgreSQL 연결 설정 (churchweb 스키마 사용, 마이그레이션 히스토리도 같은 스키마에)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString, npgsqlOptions =>
        npgsqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "churchweb")));

// ASP.NET Core Identity 설정
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 8;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;

        // 로그인 시도 제한
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.AllowedForNewUsers = true;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Cookie 설정
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromDays(30);
    options.SlidingExpiration = true;
});

// DataProtection 키 영속화 (Render Persistent Disk)
var keysPath = Environment.GetEnvironmentVariable("DP_KEYS_PATH") ?? Path.Combine(Directory.GetCurrentDirectory(), "keys");
var keysDir = new DirectoryInfo(keysPath);
if (!keysDir.Exists)
{
    keysDir.Create();
}
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(keysDir)
    .SetApplicationName("ChurchWeb");

// MVC + Razor Pages 지원
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Blazor Server 지원
builder.Services.AddServerSideBlazor();

// HttpClient 등록 (Blazor 컴포넌트에서 사용)
builder.Services.AddScoped<HttpClient>(sp =>
{
    var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
    var httpContext = httpContextAccessor.HttpContext;

    if (httpContext != null)
    {
        var request = httpContext.Request;
        var baseAddress = $"{request.Scheme}://{request.Host}";
        return new HttpClient { BaseAddress = new Uri(baseAddress) };
    }

    // Fallback for when HttpContext is not available
    return new HttpClient { BaseAddress = new Uri("https://localhost:5001") };
});

// HttpContextAccessor 등록
builder.Services.AddHttpContextAccessor();

// 서비스 등록
builder.Services.AddScoped<IHomeService, HomeService>();
builder.Services.AddScoped<IAboutService, AboutService>();
builder.Services.AddScoped<ISermonsService, SermonsService>();
builder.Services.AddScoped<INewsService, NewsService>();

// 업로드 경로 설정 (환경변수 또는 기본 경로)
var uploadsBasePath = Environment.GetEnvironmentVariable("UPLOADS_PATH");
string uploadsRootPath;
if (!string.IsNullOrEmpty(uploadsBasePath))
{
    // Render 퍼시스턴트 디스크 사용 (/var/data/uploads)
    uploadsRootPath = uploadsBasePath;
    if (!Directory.Exists(uploadsRootPath))
    {
        Directory.CreateDirectory(uploadsRootPath);
    }
}
else
{
    // 로컬 개발 환경 (wwwroot 사용)
    var env = builder.Environment;
    uploadsRootPath = env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
}

// 관리자 서비스
builder.Services.AddScoped<ChurchWeb.Infrastructure.Services.ISermonAdminService, ChurchWeb.Infrastructure.Services.SermonAdminService>();
builder.Services.AddSingleton<ChurchWeb.Infrastructure.Services.IChurchInfoCacheService, ChurchWeb.Infrastructure.Services.ChurchInfoCacheService>();
builder.Services.AddScoped<ChurchWeb.Infrastructure.Services.IChurchInfoService>(sp =>
    sp.GetRequiredService<ChurchWeb.Infrastructure.Services.IChurchInfoCacheService>());
builder.Services.AddScoped<ChurchWeb.Infrastructure.Services.IMenuAdminService, ChurchWeb.Infrastructure.Services.MenuAdminService>();
builder.Services.AddScoped<ChurchWeb.Infrastructure.Services.IYouTubeService, ChurchWeb.Infrastructure.Services.ChurchYouTubeService>();
builder.Services.AddScoped<ChurchWeb.Infrastructure.Services.IBulletinAdminService, ChurchWeb.Infrastructure.Services.BulletinAdminService>();
builder.Services.AddScoped<ChurchWeb.Infrastructure.Services.IAlbumAdminService>(sp =>
{
    var context = sp.GetRequiredService<AppDbContext>();
    var logger = sp.GetRequiredService<ILogger<ChurchWeb.Infrastructure.Services.AlbumAdminService>>();
    return new ChurchWeb.Infrastructure.Services.AlbumAdminService(context, logger, uploadsRootPath);
});
builder.Services.AddScoped<ChurchWeb.Infrastructure.Services.INoticeAdminService>(sp =>
{
    var context = sp.GetRequiredService<AppDbContext>();
    var logger = sp.GetRequiredService<ILogger<ChurchWeb.Infrastructure.Services.NoticeAdminService>>();
    return new ChurchWeb.Infrastructure.Services.NoticeAdminService(context, logger, uploadsRootPath);
});
builder.Services.AddScoped<ChurchWeb.Infrastructure.Services.ICalendarAdminService, ChurchWeb.Infrastructure.Services.CalendarAdminService>();
builder.Services.AddScoped<ChurchWeb.Infrastructure.Services.IEvangelistAdminService, ChurchWeb.Infrastructure.Services.EvangelistAdminService>();

// 네비게이션 메뉴 데이터 (싱글톤)
builder.Services.AddSingleton(new NavMenuVm
{
    MenuItems = new List<NavMenuItemVm>
    {
        new NavMenuItemVm
        {
            Title = "교회소개",
            Url = "/About/Vision",
            SubItems = new List<NavMenuItemVm>
            {
                new NavMenuItemVm { Title = "사명과 비전", Url = "/About/Vision" },
                new NavMenuItemVm { Title = "예배 안내", Url = "/About/Worship" },
                new NavMenuItemVm { Title = "섬기는 분들", Url = "/about/people" },
                new NavMenuItemVm { Title = "교회 역사", Url = "/About/History" },
                new NavMenuItemVm { Title = "오시는 길", Url = "/About/Location" }
            }
        },
        new NavMenuItemVm
        {
            Title = "말씀과 찬양",
            Url = "/sermons/sunday",
            SubItems = new List<NavMenuItemVm>
            {
                new NavMenuItemVm { Title = "주일 설교", Url = "/sermons/sunday" },
                new NavMenuItemVm { Title = "특별 설교", Url = "/sermons/special" }
            }
        },
        new NavMenuItemVm
        {
            Title = "소식",
            Url = "/news/bulletins",
            SubItems = new List<NavMenuItemVm>
            {
                new NavMenuItemVm { Title = "주보", Url = "/news/bulletins" },
                new NavMenuItemVm { Title = "갤러리", Url = "/news/gallery" },
                new NavMenuItemVm { Title = "교회소식", Url = "/news/notices" },
                new NavMenuItemVm { Title = "교회일정", Url = "/news/calendar" }
            }
        }
    }
});

var app = builder.Build();

// Configure the HTTP request pipeline.
// ForwardedHeaders를 가장 먼저 사용 (Render 프록시 대응)
app.UseForwardedHeaders();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// 기본 wwwroot 정적 파일 서빙
app.UseStaticFiles();

// 업로드 파일 정적 서빙 (Render 퍼시스턴트 디스크 또는 로컬 wwwroot)
if (!string.IsNullOrEmpty(uploadsBasePath))
{
    // Render 환경: /var/data/uploads를 /uploads URL로 매핑
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(uploadsBasePath),
        RequestPath = "/uploads"
    });
}

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

// MVC 라우팅
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Razor Pages 라우팅
app.MapRazorPages();

// Blazor Server 라우팅
app.MapBlazorHub();
app.MapFallbackToPage("/Admin/Blazor/{*path:nonfile}", "/_AdminHost");

// DB 초기화 (마이그레이션 + 관리자 계정 시드)
using (var scope = app.Services.CreateScope())
{
    await DbInitializer.InitializeAsync(scope.ServiceProvider);

    // ChurchInfo 캐시 초기화
    var churchInfoCache = scope.ServiceProvider.GetRequiredService<ChurchWeb.Infrastructure.Services.IChurchInfoCacheService>();
    await churchInfoCache.RefreshCacheAsync();
}

app.Run();
