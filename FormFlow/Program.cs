using AspNetCore.Identity.Mongo;
using AspNetCore.Identity.Mongo.Model;
using FormFlow.Data;
using FormFlow.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace FormFlow
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
			builder.Services.AddSingleton<IMongoClient>(new MongoClient(connectionString));
			builder.Services.AddScoped<IMongoDatabase>(serviceProvider =>
			{
				var client = serviceProvider.GetRequiredService<IMongoClient>();
				// ReSharper disable once StringLiteralTypo
				return client.GetDatabase("formresultscluster");
			});

			builder.Services.AddIdentityMongoDbProvider<MongoUser>();
			builder.Services.Configure<IdentityOptions>(o =>
			{
				// Password requirements
				o.Password.RequireDigit = true;
				o.Password.RequireLowercase = true;
				o.Password.RequireLowercase = true;
				o.Password.RequireNonAlphanumeric = true;
				o.Password.RequiredLength = 8;

				// Lockout settings
				o.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
				o.Lockout.MaxFailedAccessAttempts = 5;

				// Sign-in settings
				o.SignIn.RequireConfirmedEmail = true;
				o.SignIn.RequireConfirmedPhoneNumber = false;

				// Email requirements
				o.User.RequireUniqueEmail = true;
			});

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