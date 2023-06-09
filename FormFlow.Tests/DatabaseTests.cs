using FormFlow.Data;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace FormFlow.Tests
{
	public class DatabaseTests
	{
		private readonly ITestOutputHelper _testOutputHelper;
		private Mock<IOptions<MongoDBSettings>> _mockOptions;
		private Mock<IMongoDatabase> _mockDB;
		private Mock<IMongoClient> _mockClient;

		public DatabaseTests(ITestOutputHelper testOutputHelper)
		{
			_testOutputHelper = testOutputHelper;
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
