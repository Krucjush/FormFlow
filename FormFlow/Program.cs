using AspNetCore.Identity.Mongo;
using AspNetCore.Identity.Mongo.Model;
using FormFlow.Controllers;
using FormFlow.Data;
using FormFlow.Data.Repositories;
using FormFlow.JWT;
using FormFlow.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Microsoft.Extensions.Options;

namespace FormFlow
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			builder.Services.AddRazorPages();
			builder.Services.Configure<MongoDBSettings>(builder.Configuration.GetSection("MongoDB"));
			builder.Services.AddSingleton<UserRepository>();
			builder.Services.AddSingleton<FormRepository>();
			builder.Services.AddSingleton<FormResponseRepository>();
			builder.Services.AddSingleton<QuestionRepository>();
			builder.Services.AddSingleton<ResponseEntryRepository>();
			builder.Services.AddSingleton<UserController>();
			builder.Services.AddSingleton<FormController>();
			builder.Services.AddSingleton<QuestionController>();
			builder.Services.AddSingleton<ResponseEntryController>();
			builder.Services.AddSingleton<FormResponseController>();
			builder.Services.AddSingleton<JwtSettings>();

			var secretKey = JwtHelper.GenerateSecretKey(64);

			builder.Services.Configure<JwtSettings>(x =>
			{
				x.SecretKey = secretKey;
			});

			builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

			// Add services to the container.
			var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
			builder.Services.AddDbContext<ApplicationDbContext>(options =>
				options.UseSqlServer(connectionString));
			builder.Services.AddDatabaseDeveloperPageExceptionFilter();

			builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
				.AddEntityFrameworkStores<ApplicationDbContext>();
			builder.Services.AddControllersWithViews();

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseMigrationsEndPoint();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthorization();

			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}");

			app.MapRazorPages();

			app.Run();
		}
	}
}