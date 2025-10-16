using GamiPort.Data;                       // ApplicationDbContext�]Identity�Ρ^
using GamiPort.Models;                     // GameSpacedatabaseContext�]�~�ȸ�ơ^
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
			// �s�u�r��]��� DbContext ���ΦP�@�աF�Y���Ӥ��w�A�i���}���^
			// �|�̧ǧ� "GameSpace" -> "GameSpacedatabase"�F���S���N���
			// ------------------------------------------------------------
			var gameSpaceConn =
				builder.Configuration.GetConnectionString("GameSpace")
				?? builder.Configuration.GetConnectionString("GameSpacedatabase")
				?? throw new InvalidOperationException("Connection string 'GameSpace' not found.");

			// ------------------------------------------------------------
			// (A) DbContext ���U
			// 1) ApplicationDbContext�GIdentity �x�s�]�n�J/�ϥΪ�/Claims�^
			// 2) GameSpacedatabaseContext�G�A���~�ȸ�ơ]�q���B�峹�B�ȪA�K�^
			//    �o��Ӧb DI ���O���P���O�A�����Ĭ�
			// ------------------------------------------------------------
			builder.Services.AddDbContext<ApplicationDbContext>(options =>
			{
				options.UseSqlServer(gameSpaceConn);
				// �i��]�}�o�������^�Goptions.EnableSensitiveDataLogging();
			});

			builder.Services.AddDbContext<GameSpacedatabaseContext>(options =>
			{
				options.UseSqlServer(gameSpaceConn);
				// �i��]Ū�h�g�֭����^�Goptions.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
			});

			// EF ���}�o�̨ҥ~���]/errors + /migrations endpoint�^
			builder.Services.AddDatabaseDeveloperPageExceptionFilter();

			// ------------------------------------------------------------
			// (B) Identity ���U
			// AddDefaultIdentity ���t�uRazor Pages UI�BCookies�BSecurityStamp ���ҡv���w�]
			// Store ���V ApplicationDbContext�]�W���w���U�^
			// ------------------------------------------------------------
			builder.Services
				.AddDefaultIdentity<IdentityUser>(options =>
				{
					// �Y�ȮɨS���H�H����A�}�o���ҫ�ĳ�����H�c���һݨD
					// options.SignIn.RequireConfirmedAccount = false;
					options.SignIn.RequireConfirmedAccount = true;
				})
				.AddEntityFrameworkStores<ApplicationDbContext>();

			// ------------------------------------------------------------
			// (C) �A���q���A�ȡ]��b social_hub �ϰ�R�W�Ŷ��]OK�^
			// ------------------------------------------------------------
			builder.Services.AddScoped<
				GamiPort.Areas.social_hub.Services.Notifications.INotificationService,
				GamiPort.Areas.social_hub.Services.Notifications.NotificationService
			>();

			// ------------------------------------------------------------
			// MVC & Razor Pages
			//   - Razor Pages �� Identity UI �ϥ�
			//   - MVC ���A Areas / Controllers / Views
			// ------------------------------------------------------------
			builder.Services.AddControllersWithViews()
				// �i��G�Τ@ JSON �j�p�g�]�קK�uAPI�۰ʤp�g�v�ôb�^
				.AddJsonOptions(opt => { opt.JsonSerializerOptions.PropertyNamingPolicy = null; });

			builder.Services.AddRazorPages();

			// �i��G�� AJAX �n�a���� Token�]�M�A�e�� fetch header 'RequestVerificationToken' �����^
			builder.Services.AddAntiforgery(o => o.HeaderName = "RequestVerificationToken");

			// �i��G���� using HttpContext �����X�]���ɦb Service �nŪ�� User/Claims�^
			builder.Services.AddHttpContextAccessor();

			var app = builder.Build();

			// ------------------------------------------------------------
			// HTTP Pipeline�]�����n�鶶�ǫܭ��n�^
			// ------------------------------------------------------------
			if (app.Environment.IsDevelopment())
			{
				// ��� EF ���~�Ա��� & migrations endpoint
				app.UseMigrationsEndPoint();

				// �i��G�Ұʮ��˴��s�u�]�ֳt�������ա^
				using (var scope = app.Services.CreateScope())
				{
					var db1 = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
					var db2 = scope.ServiceProvider.GetRequiredService<GameSpacedatabaseContext>();
					// ���ѷ|��ҥ~�A��K�A�Ĥ@�ɶ����D�s�u�r����v�����D
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

			// �� ���Ҥ@�w�n�b���v���e
			app.UseAuthentication();
			app.UseAuthorization();

			// ------------------------------------------------------------
			// ���ѡGAreas �n�����U�]�� default ���^
			// ------------------------------------------------------------
			app.MapControllerRoute(
				name: "areas",
				pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
			);

			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}"
			);

			// Identity UI�]/Identity/...�^
			app.MapRazorPages();

			app.Run();
		}
	}
}
