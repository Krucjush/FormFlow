using FormFlow.Controllers;
using FormFlow.Interfaces;
using FormFlow.JWT;
using FormFlow.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormFlow.Tests
{
	public class UserControllerLoginTests
	{
		private UserController _controller;
		private Mock<IUserRepository> _userRepositoryMock;

		public UserControllerLoginTests()
		{
			_userRepositoryMock = new Mock<IUserRepository>();
			var formRepositoryMock = new Mock<IFormRepository>();
			var jwtSettings = new JwtSettings();
			_controller = new UserController(_userRepositoryMock.Object, formRepositoryMock.Object, jwtSettings);
		}
		[Fact]
		public async Task Login_ValidCredentials_ReturnsOkResult()
		{
			// Arrange
			var model = new UserLoginModel
			{
				Email = "test@example.com",
				Password = "ValidPassword123"
			};

			_userRepositoryMock.Setup(repo => repo.GetByEmailAsync(model.Email))
				.ReturnsAsync(new User
				{
					Email = model.Email,
					PasswordHash = _controller.HashPassword(model.Password)
				});

			// Act
			var result = await _controller.Login(model);

			// Assert
			// Assert
			Assert.IsType<RedirectToActionResult>(result);
			var redirectResult = (RedirectToActionResult)result;
			Assert.Equal("Index", redirectResult.ActionName);
			Assert.Equal("Home", redirectResult.ControllerName);
			_userRepositoryMock.Verify(repo => repo.GetByEmailAsync(model.Email), Times.Once);
		}

		[Fact]
		public async Task Login_InvalidCredentials_ReturnsUnauthorizedResult()
		{
			// Arrange
			var model = new UserLoginModel
			{
				Email = "test@example.com",
				Password = "InvalidPassword"
			};

			_userRepositoryMock.Setup(repo => repo.GetByEmailAsync(model.Email))
				.ReturnsAsync((User)null);

			// Act
			var result = await _controller.Login(model);

			// Assert
			Assert.IsType<BadRequestObjectResult>(result);
			var badRequestResult = (BadRequestObjectResult)result;
			Assert.IsType<SerializableError>(badRequestResult.Value);

			var errors = (SerializableError)badRequestResult.Value;
			Assert.True(errors.ContainsKey("Email"));
			var errorMessages = (string[])errors["Email"];
			Assert.Single(errorMessages);
			Assert.Equal("Invalid email or password.", errorMessages[0]);

			_userRepositoryMock.Verify(repo => repo.GetByEmailAsync(model.Email), Times.Once);
		}
	}
}
