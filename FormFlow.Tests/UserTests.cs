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

namespace FormFlow.Tests
{
	public class UserTests
	{
		[Fact]
		public async Task RegisterUser_ValidUser_ReturnsCreatedResult()
		{
			// Arrange
			var userRepositoryMock = new Mock<UserRepository>();
			var formRepositoryMock = new Mock<FormRepository>();
			var jwtSettingsMock = new Mock<JwtSettings>();
			var userController = new UserController(userRepositoryMock.Object, formRepositoryMock.Object, jwtSettingsMock.Object);

			var newUser = new UserRegistrationModel
			{
				Email = "test@example.com",
				Password = "examplePassword"
			};

			// Set up the mock behavior for the userRepositoryMock
			userRepositoryMock.Setup(u => u.CreateAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

			// Act
			var result = await userController.Register(newUser);

			// Assert
			Assert.IsType<CreatedAtActionResult>(result);
			userRepositoryMock.Verify(u => u.CreateAsync(newUser), Times.Once);
		}
	}
}