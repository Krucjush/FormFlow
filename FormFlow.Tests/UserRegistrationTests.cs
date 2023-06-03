using System.Threading.Tasks;
using AspNetCore.Identity.Mongo;
using AspNetCore.Identity.Mongo.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace FormFlow.Tests
{
	public class UserRegistrationTests
	{
		[Fact]
		public async Task RegisterUser_ValidCredentials_ReturnsSuccess()
		{
			//Arrange
			var serviceProvider = new ServiceCollection();
				serviceProvider.AddIdentityMongoDbProvider<MongoUser>();
				serviceProvider.Configure<IdentityOptions>(o =>
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
		}
	}
}