using GamiPort.Data;                       // ApplicationDbContext（Identity用）
using GamiPort.Models;                     // GameSpacedatabaseContext（業務資料）
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GamiPort
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// ------------------------------------------------------------
			// 連線字串（兩個 DbContext 都用同一組；若未來分庫，可分開取）
			// 會依序找 "GameSpace" -> "GameSpacedatabase"；都沒有就丟錯
			// ------------------------------------------------------------
			var gameSpaceConn =
				builder.Configuration.GetConnectionString("GameSpace")
				?? builder.Configuration.GetConnectionString("GameSpacedatabase")
				?? throw new InvalidOperationException("Connection string 'GameSpace' not found.");

			// ------------------------------------------------------------
			// (A) DbContext 註冊
			// 1) ApplicationDbContext：Identity 儲存（登入/使用者/Claims）
			// 2) GameSpacedatabaseContext：你的業務資料（通知、文章、客服…）
			//    這兩個在 DI 內是不同型別，互不衝突
			// ------------------------------------------------------------
			builder.Services.AddDbContext<ApplicationDbContext>(options =>
			{
				options.UseSqlServer(gameSpaceConn);
				// 可選（開發期除錯）：options.EnableSensitiveDataLogging();
			});

			builder.Services.AddDbContext<GameSpacedatabaseContext>(options =>
			{
				options.UseSqlServer(gameSpaceConn);
				// 可選（讀多寫少頁面）：options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
			});

			// EF 的開發者例外頁（/errors + /migrations endpoint）
			builder.Services.AddDatabaseDeveloperPageExceptionFilter();

			// ------------------------------------------------------------
			// (B) Identity 註冊
			// AddDefaultIdentity 內含「Razor Pages UI、Cookies、SecurityStamp 驗證」等預設
			// Store 指向 ApplicationDbContext（上面已註冊）
			// ------------------------------------------------------------
			builder.Services
				.AddDefaultIdentity<IdentityUser>(options =>
				{
					// 若暫時沒有寄信機制，開發環境建議關閉信箱驗證需求
					// options.SignIn.RequireConfirmedAccount = false;
					options.SignIn.RequireConfirmedAccount = true;
				})
				.AddEntityFrameworkStores<ApplicationDbContext>();

			// ------------------------------------------------------------
			// (C) 你的通知服務（放在 social_hub 區域命名空間也OK）
			// ------------------------------------------------------------
			builder.Services.AddScoped<
				GamiPort.Areas.social_hub.Services.Notifications.INotificationService,
				GamiPort.Areas.social_hub.Services.Notifications.NotificationService
			>();

			// ------------------------------------------------------------
			// MVC & Razor Pages
			//   - Razor Pages 供 Identity UI 使用
			//   - MVC 給你 Areas / Controllers / Views
			// ------------------------------------------------------------
			builder.Services.AddControllersWithViews()
				// 可選：統一 JSON 大小寫（避免「API自動小寫」疑惑）
				.AddJsonOptions(opt => { opt.JsonSerializerOptions.PropertyNamingPolicy = null; });

			builder.Services.AddRazorPages();

			// 可選：讓 AJAX 好帶防偽 Token（和你前面 fetch header 'RequestVerificationToken' 對應）
			builder.Services.AddAntiforgery(o => o.HeaderName = "RequestVerificationToken");

			// 可選：全域 using HttpContext 的場合（有時在 Service 要讀取 User/Claims）
			builder.Services.AddHttpContextAccessor();

			var app = builder.Build();

			// ------------------------------------------------------------
			// HTTP Pipeline（中介軟體順序很重要）
			// ------------------------------------------------------------
			if (app.Environment.IsDevelopment())
			{
				// 顯示 EF 錯誤詳情頁 & migrations endpoint
				app.UseMigrationsEndPoint();

				// 可選：啟動時檢測連線（快速煙霧測試）
				using (var scope = app.Services.CreateScope())
				{
					var db1 = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
					var db2 = scope.ServiceProvider.GetRequiredService<GameSpacedatabaseContext>();
					// 失敗會丟例外，方便你第一時間知道連線字串或權限問題
					db1.Database.CanConnect();
					db2.Database.CanConnect();
				}
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();
			app.UseRouting();

			// ★ 驗證一定要在授權之前
			app.UseAuthentication();
			app.UseAuthorization();

			// ------------------------------------------------------------
			// 路由：Areas 要先註冊（比 default 先）
			// ------------------------------------------------------------
			app.MapControllerRoute(
				name: "areas",
				pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
			);

			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}"
			);

			// Identity UI（/Identity/...）
			app.MapRazorPages();

			app.Run();
		}
	}
}
