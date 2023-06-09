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
using FormFlow.Interfaces;
using FormFlow.JWT;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Xunit.Abstractions;

namespace FormFlow.Tests
{
	public class UserControllerRegisterTests
	{
		private readonly Mock<IUserRepository> _userRepositoryMock;
		private readonly UserController _controller;

		public UserControllerRegisterTests(ITestOutputHelper testOutputHelper)
		{
			_userRepositoryMock = new Mock<IUserRepository>();
			var formRepositoryMock = new Mock<IFormRepository>();
			var jwtSettings = new JwtSettings();
			_controller = new UserController(_userRepositoryMock.Object, formRepositoryMock.Object, jwtSettings);
		}
		[Fact]
		public async Task Register_ValidUser_Success()
		{
			// Arrange
			var model = new UserRegistrationModel
			{
				Email = "test@example.com",
				Password = "ValidPassword123",
				ConfirmPassword = "ValidPassword123"
			};

			_userRepositoryMock.Setup(repo => repo.ExistsByEmailAsync(model.Email))
				.ReturnsAsync(false); // Simulate that the email does not exist in the database

			User createdUser = null;
			
			_userRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<User>()))
				.Callback<User>(u => createdUser = u)
				.Returns(Task.CompletedTask);
			
			// Act
			var result = await _controller.Register(model);

			// Assert
			Assert.IsType<RedirectToActionResult>(result);
			var redirectResult = (RedirectToActionResult)result;
			Assert.Equal("Index", redirectResult.ActionName);
			Assert.Equal("Home", redirectResult.ControllerName);

			_userRepositoryMock.Verify(repo => repo.ExistsByEmailAsync(model.Email), Times.Once);
			_userRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<User>()), Times.Once);

			Assert.NotNull(createdUser);
			Assert.Equal(model.Email, createdUser.Email);
		}

		[Fact]
		public async Task Register_ExistingEmail_BadRequest()
		{
			// Arrange
			var model = new UserRegistrationModel
			{
				Email = "test@example.com",
				Password = "ValidPassword123",
				ConfirmPassword = "ValidPassword123"
			};

			_userRepositoryMock.Setup(repo => repo.ExistsByEmailAsync(model.Email))
				.ReturnsAsync(true); // Simulate that the email already exists in the database

			// Act
			var result = await _controller.Register(model);

			// Assert
			Assert.IsType<BadRequestObjectResult>(result);
			var badRequestResult = (BadRequestObjectResult)result;
			Assert.IsType<SerializableError>(badRequestResult.Value);
			var errors = (SerializableError)badRequestResult.Value;

			// Check if the "Email" field has an error message
			Assert.True(errors.ContainsKey("Email"));
			var errorMessages = (string[])errors["Email"];
			Assert.Single(errorMessages); // Ensure only one error message is returned
			Assert.Equal("Email address already used.", errorMessages[0]);

			_userRepositoryMock.Verify(repo => repo.ExistsByEmailAsync(model.Email), Times.Once);
			_userRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<User>()), Times.Never);
		}
		[Fact]
		public async Task Register_InvalidEmail_ReturnsBadRequest()
		{
			// Arrange
			var model = new UserRegistrationModel
			{
				Email = "invalidemail",
				Password = "ValidPassword123",
				ConfirmPassword = "ValidPassword123"
			};

			// Act
			var result = await _controller.Register(model);

			// Assert
			Assert.IsType<BadRequestObjectResult>(result);
			var badRequestResult = (BadRequestObjectResult)result;
			Assert.IsType<SerializableError>(badRequestResult.Value);
			var errors = (SerializableError)badRequestResult.Value;

			// Check if the "Email" field has an error message
			Assert.True(errors.ContainsKey("Email"));
			var errorMessages = (string[])errors["Email"];
			Assert.Single(errorMessages); // Ensure only one error message is returned
			Assert.Equal("Invalid email format.", errorMessages[0]);
		}
		[Fact]
		public async Task Register_EmailWithoutDot_ReturnsBadRequest()
		{
			// Arrange
			var model = new UserRegistrationModel
			{
				Email = "invalidemail@invalid",
				Password = "ValidPassword123",
				ConfirmPassword = "ValidPassword123"
			};

			// Act
			var result = await _controller.Register(model);

			// Assert
			Assert.IsType<BadRequestObjectResult>(result);
			var badRequestResult = (BadRequestObjectResult)result;
			Assert.IsType<SerializableError>(badRequestResult.Value);
			var errors = (SerializableError)badRequestResult.Value;

			// Check if the "Email" field has an error message
			Assert.True(errors.ContainsKey("Email"));
			var errorMessages = (string[])errors["Email"];
			Assert.Single(errorMessages); // Ensure only one error message is returned
			Assert.Equal("Invalid email format.", errorMessages[0]);
		}
		[Fact]
		public async Task Register_EmailWithoutAtSign_ReturnsBadRequest()
		{
			// Arrange
			var model = new UserRegistrationModel
			{
				Email = "invalidemail.invalid",
				Password = "ValidPassword123",
				ConfirmPassword = "ValidPassword123"
			};

			// Act
			var result = await _controller.Register(model);

			// Assert
			Assert.IsType<BadRequestObjectResult>(result);
			var badRequestResult = (BadRequestObjectResult)result;
			Assert.IsType<SerializableError>(badRequestResult.Value);
			var errors = (SerializableError)badRequestResult.Value;

			// Check if the "Email" field has an error message
			Assert.True(errors.ContainsKey("Email"));
			var errorMessages = (string[])errors["Email"];
			Assert.Single(errorMessages); // Ensure only one error message is returned
			Assert.Equal("Invalid email format.", errorMessages[0]);
		}
		[Fact]
		public async Task Register_AtSignAndDotEmail_ReturnsBadRequest()
		{
			// Arrange
			var model = new UserRegistrationModel
			{
				Email = "@.",
				Password = "ValidPassword123",
				ConfirmPassword = "ValidPassword123"
			};

			// Act
			var result = await _controller.Register(model);

			// Assert
			Assert.IsType<BadRequestObjectResult>(result);
			var badRequestResult = (BadRequestObjectResult)result;
			Assert.IsType<SerializableError>(badRequestResult.Value);
			var errors = (SerializableError)badRequestResult.Value;

			// Check if the "Email" field has an error message
			Assert.True(errors.ContainsKey("Email"));
			var errorMessages = (string[])errors["Email"];
			Assert.Single(errorMessages); // Ensure only one error message is returned
			Assert.Equal("Invalid email format.", errorMessages[0]);
		}
		[Fact]
		public async Task Register_EmailWithoutUsername_ReturnsBadRequest()
		{
			// Arrange
			var model = new UserRegistrationModel
			{
				Email = "@invalid.invalid",
				Password = "ValidPassword123",
				ConfirmPassword = "ValidPassword123"
			};

			// Act
			var result = await _controller.Register(model);

			// Assert
			Assert.IsType<BadRequestObjectResult>(result);
			var badRequestResult = (BadRequestObjectResult)result;
			Assert.IsType<SerializableError>(badRequestResult.Value);
			var errors = (SerializableError)badRequestResult.Value;

			// Check if the "Email" field has an error message
			Assert.True(errors.ContainsKey("Email"));
			var errorMessages = (string[])errors["Email"];
			Assert.Single(errorMessages); // Ensure only one error message is returned
			Assert.Equal("Invalid email format.", errorMessages[0]);
		}
		[Fact]
		public async Task Register_EmailWithoutServer_ReturnsBadRequest()
		{
			// Arrange
			var model = new UserRegistrationModel
			{
				Email = "invalid@.invalid",
				Password = "ValidPassword123",
				ConfirmPassword = "ValidPassword123"
			};

			// Act
			var result = await _controller.Register(model);

			// Assert
			Assert.IsType<BadRequestObjectResult>(result);
			var badRequestResult = (BadRequestObjectResult)result;
			Assert.IsType<SerializableError>(badRequestResult.Value);
			var errors = (SerializableError)badRequestResult.Value;

			// Check if the "Email" field has an error message
			Assert.True(errors.ContainsKey("Email"));
			var errorMessages = (string[])errors["Email"];
			Assert.Single(errorMessages); // Ensure only one error message is returned
			Assert.Equal("Invalid email format.", errorMessages[0]);
		}
		[Fact]
		public async Task Register_EmailWithoutDomain_ReturnsBadRequest()
		{
			// Arrange
			var model = new UserRegistrationModel
			{
				Email = "invalid@invalid.",
				Password = "ValidPassword123",
				ConfirmPassword = "ValidPassword123"
			};

			// Act
			var result = await _controller.Register(model);

			// Assert
			Assert.IsType<BadRequestObjectResult>(result);
			var badRequestResult = (BadRequestObjectResult)result;
			Assert.IsType<SerializableError>(badRequestResult.Value);
			var errors = (SerializableError)badRequestResult.Value;

			// Check if the "Email" field has an error message
			Assert.True(errors.ContainsKey("Email"));
			var errorMessages = (string[])errors["Email"];
			Assert.Single(errorMessages); // Ensure only one error message is returned
			Assert.Equal("Invalid email format.", errorMessages[0]);
		}
		[Fact]
		public async Task Register_PasswordsNotMatching_BadRequest()
		{
			// Arrange
			var model = new UserRegistrationModel
			{
				Email = "test@example.com",
				Password = "ValidPassword123",
				ConfirmPassword = "DifferentPassword456" // Set a different password for confirm password
			};

			// Act
			var result = await _controller.Register(model);

			// Assert
			Assert.IsType<BadRequestObjectResult>(result);
			var badRequestResult = (BadRequestObjectResult)result;
			Assert.IsType<SerializableError>(badRequestResult.Value);
			var errors = (SerializableError)badRequestResult.Value;

			// Check if the "ConfirmPassword" field has an error message
			Assert.True(errors.ContainsKey("ConfirmPassword"));
			var errorMessages = (string[])errors["ConfirmPassword"];
			Assert.Single(errorMessages); // Ensure only one error message is returned
			Assert.Equal("Password and Confirm Password do not match.", errorMessages[0]);

			_userRepositoryMock.Verify(repo => repo.ExistsByEmailAsync(model.Email), Times.Never);
			_userRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<User>()), Times.Never);
		}
		[Fact]
		public async Task Register_WeakPassword_BadRequest()
		{
			// Arrange
			var model = new UserRegistrationModel
			{
				Email = "test@example.com",
				Password = "weak", // Set a weak password that doesn't meet the requirements
				ConfirmPassword = "weak"
			};

			// Act
			var result = await _controller.Register(model);

			// Assert
			Assert.IsType<BadRequestObjectResult>(result);
			var badRequestResult = (BadRequestObjectResult)result;
			Assert.IsType<SerializableError>(badRequestResult.Value);
			var errors = (SerializableError)badRequestResult.Value;

			// Check if the "Password" field has an error message
			Assert.True(errors.ContainsKey("Password"));
			var errorMessages = (string[])errors["Password"];
			Assert.Single(errorMessages); // Ensure only one error message is returned
			Assert.Equal("Password does not meet the requirements", errorMessages[0]);

			_userRepositoryMock.Verify(repo => repo.ExistsByEmailAsync(model.Email), Times.Never);
			_userRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<User>()), Times.Never);
		}
		[Fact]
		public async Task Register_PasswordLongEnoughWithoutCapitalAndNumber_BadRequest()
		{
			// Arrange
			var model = new UserRegistrationModel
			{
				Email = "test@example.com",
				Password = "weakpassword", // Set a weak password that doesn't meet the requirements
				ConfirmPassword = "weakpassword"
			};

			// Act
			var result = await _controller.Register(model);

			// Assert
			Assert.IsType<BadRequestObjectResult>(result);
			var badRequestResult = (BadRequestObjectResult)result;
			Assert.IsType<SerializableError>(badRequestResult.Value);
			var errors = (SerializableError)badRequestResult.Value;

			// Check if the "Password" field has an error message
			Assert.True(errors.ContainsKey("Password"));
			var errorMessages = (string[])errors["Password"];
			Assert.Single(errorMessages); // Ensure only one error message is returned
			Assert.Equal("Password does not meet the requirements", errorMessages[0]);

			_userRepositoryMock.Verify(repo => repo.ExistsByEmailAsync(model.Email), Times.Never);
			_userRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<User>()), Times.Never);
		}
		[Fact]
		public async Task Register_PasswordNotLongEnough_BadRequest()
		{
			// Arrange
			var model = new UserRegistrationModel
			{
				Email = "test@example.com",
				Password = "1@3$a", // Set a weak password that doesn't meet the requirements
				ConfirmPassword = "1@3$a"
			};

			// Act
			var result = await _controller.Register(model);

			// Assert
			Assert.IsType<BadRequestObjectResult>(result);
			var badRequestResult = (BadRequestObjectResult)result;
			Assert.IsType<SerializableError>(badRequestResult.Value);
			var errors = (SerializableError)badRequestResult.Value;

			// Check if the "Password" field has an error message
			Assert.True(errors.ContainsKey("Password"));
			var errorMessages = (string[])errors["Password"];
			Assert.Single(errorMessages); // Ensure only one error message is returned
			Assert.Equal("Password does not meet the requirements", errorMessages[0]);

			_userRepositoryMock.Verify(repo => repo.ExistsByEmailAsync(model.Email), Times.Never);
			_userRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<User>()), Times.Never);
		}
		[Fact]
		public async Task Register_PasswordLongOnlyLetters_BadRequest()
		{
			// Arrange
			var model = new UserRegistrationModel
			{
				Email = "test@example.com",
				Password = "weakPassword", // Set a weak password that doesn't meet the requirements
				ConfirmPassword = "weakPassword"
			};

			// Act
			var result = await _controller.Register(model);

			// Assert
			Assert.IsType<BadRequestObjectResult>(result);
			var badRequestResult = (BadRequestObjectResult)result;
			Assert.IsType<SerializableError>(badRequestResult.Value);
			var errors = (SerializableError)badRequestResult.Value;

			// Check if the "Password" field has an error message
			Assert.True(errors.ContainsKey("Password"));
			var errorMessages = (string[])errors["Password"];
			Assert.Single(errorMessages); // Ensure only one error message is returned
			Assert.Equal("Password does not meet the requirements", errorMessages[0]);

			_userRepositoryMock.Verify(repo => repo.ExistsByEmailAsync(model.Email), Times.Never);
			_userRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<User>()), Times.Never);
		}
		[Fact]
		public async Task Register_PasswordLongWithoutSpecialSignsAndCapitalLetters_BadRequest()
		{
			// Arrange
			var model = new UserRegistrationModel
			{
				Email = "test@example.com",
				Password = "weakpassword1", // Set a weak password that doesn't meet the requirements
				ConfirmPassword = "weakpassword1"
			};

			// Act
			var result = await _controller.Register(model);

			// Assert
			Assert.IsType<BadRequestObjectResult>(result);
			var badRequestResult = (BadRequestObjectResult)result;
			Assert.IsType<SerializableError>(badRequestResult.Value);
			var errors = (SerializableError)badRequestResult.Value;

			// Check if the "Password" field has an error message
			Assert.True(errors.ContainsKey("Password"));
			var errorMessages = (string[])errors["Password"];
			Assert.Single(errorMessages); // Ensure only one error message is returned
			Assert.Equal("Password does not meet the requirements", errorMessages[0]);

			_userRepositoryMock.Verify(repo => repo.ExistsByEmailAsync(model.Email), Times.Never);
			_userRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<User>()), Times.Never);
		}
		[Fact]
		public async Task Register_PasswordLongWithoutCapitalLettersAndNumbers_BadRequest()
		{
			// Arrange
			var model = new UserRegistrationModel
			{
				Email = "test@example.com",
				Password = "weakpassword@", // Set a weak password that doesn't meet the requirements
				ConfirmPassword = "weakpassword@"
			};

			// Act
			var result = await _controller.Register(model);

			// Assert
			Assert.IsType<BadRequestObjectResult>(result);
			var badRequestResult = (BadRequestObjectResult)result;
			Assert.IsType<SerializableError>(badRequestResult.Value);
			var errors = (SerializableError)badRequestResult.Value;

			// Check if the "Password" field has an error message
			Assert.True(errors.ContainsKey("Password"));
			var errorMessages = (string[])errors["Password"];
			Assert.Single(errorMessages); // Ensure only one error message is returned
			Assert.Equal("Password does not meet the requirements", errorMessages[0]);

			_userRepositoryMock.Verify(repo => repo.ExistsByEmailAsync(model.Email), Times.Never);
			_userRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<User>()), Times.Never);
		}
		[Fact]
		public async Task Register_PasswordLongWithoutLowercaseLettersAndNumbers_BadRequest()
		{
			// Arrange
			var model = new UserRegistrationModel
			{
				Email = "test@example.com",
				Password = "WEAKPASSWORD@", // Set a weak password that doesn't meet the requirements
				ConfirmPassword = "WEAKPASSWORD@"
			};

			// Act
			var result = await _controller.Register(model);

			// Assert
			Assert.IsType<BadRequestObjectResult>(result);
			var badRequestResult = (BadRequestObjectResult)result;
			Assert.IsType<SerializableError>(badRequestResult.Value);
			var errors = (SerializableError)badRequestResult.Value;

			// Check if the "Password" field has an error message
			Assert.True(errors.ContainsKey("Password"));
			var errorMessages = (string[])errors["Password"];
			Assert.Single(errorMessages); // Ensure only one error message is returned
			Assert.Equal("Password does not meet the requirements", errorMessages[0]);

			_userRepositoryMock.Verify(repo => repo.ExistsByEmailAsync(model.Email), Times.Never);
			_userRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<User>()), Times.Never);
		}
		[Fact]
		public async Task Register_PasswordLongWithoutLowercaseLettersAndSpecialCharacters_BadRequest()
		{
			// Arrange
			var model = new UserRegistrationModel
			{
				Email = "test@example.com",
				Password = "WEAKPASSWORD1", // Set a weak password that doesn't meet the requirements
				ConfirmPassword = "WEAKPASSWORD1"
			};

			// Act
			var result = await _controller.Register(model);

			// Assert
			Assert.IsType<BadRequestObjectResult>(result);
			var badRequestResult = (BadRequestObjectResult)result;
			Assert.IsType<SerializableError>(badRequestResult.Value);
			var errors = (SerializableError)badRequestResult.Value;

			// Check if the "Password" field has an error message
			Assert.True(errors.ContainsKey("Password"));
			var errorMessages = (string[])errors["Password"];
			Assert.Single(errorMessages); // Ensure only one error message is returned
			Assert.Equal("Password does not meet the requirements", errorMessages[0]);

			_userRepositoryMock.Verify(repo => repo.ExistsByEmailAsync(model.Email), Times.Never);
			_userRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<User>()), Times.Never);
		}
	}
}