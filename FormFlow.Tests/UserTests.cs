using System.Threading.Tasks;
using AspNetCore.Identity.Mongo;
using AspNetCore.Identity.Mongo.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using FormFlow.Controllers;
using FormFlow.Data;
using FormFlow.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FormFlow.Data.Repositories;
using FormFlow.JWT;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace FormFlow.Tests
{
	public class UserTests
	{
		private Mock<IOptions<MongoDBSettings>> _mockOptions;
		private Mock<IMongoDatabase> _mockDB;
		private Mock<IMongoClient> _mockClient;

		public UserTests()
		{
			_mockOptions = new Mock<IOptions<MongoDBSettings>>();
			_mockDB = new Mock<IMongoDatabase>();
			_mockClient = new Mock<IMongoClient>();
		}
		[Fact]
		public void ApplicationDBContext_Constructor_Success()
		{
			var settings = new MongoDBSettings
			{
				ConnectionURI = "mongodb://tes123 ",
				DatabaseName = "TestDB"
			};

			_mockOptions.Setup(s => s.Value).Returns(settings);
			_mockClient.Setup(c => c
					.GetDatabase(_mockOptions.Object.Value.DatabaseName, null))
				.Returns(_mockDB.Object);

			var context = new MongoDbContext(_mockOptions.Object);

			Assert.NotNull(context);
		}
	}
}