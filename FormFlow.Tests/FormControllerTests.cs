﻿using FormFlow.Controllers;
using FormFlow.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using FormFlow.Data;
using Moq;
using FormFlow.Models.ViewModels;
using Microsoft.Extensions.Logging.Abstractions;
using FormFlow.Models.Enums;

namespace FormFlow.Tests
{
	public class FormControllerTests
	{
		[Fact]
		public void Index_UnauthorizedUser_ReturnsUnauthorizedResult()
		{
			// Arrange
			var dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
				.UseInMemoryDatabase(databaseName: "TestDatabase")
				.Options;

			// Create a mock claims identity with no claims
			var mockClaimsIdentity = new Mock<ClaimsIdentity>();

			// Create a mock user identity with the mock claims identity
			var mockUserIdentity = new Mock<ClaimsPrincipal>();
			mockUserIdentity.Setup(u => u.Identity).Returns(mockClaimsIdentity.Object);

			// Create a mock HTTP context with the mock user identity
			var mockHttpContext = new Mock<HttpContext>();
			mockHttpContext.Setup(c => c.User).Returns(mockUserIdentity.Object);

			// Create a mock controller context with the mock HTTP context
			var mockControllerContext = new ControllerContext
			{
				HttpContext = mockHttpContext.Object
			};

			using var dbContext = new AppDbContext(dbContextOptions);
			var controller = new FormController(dbContext)
			{
				ControllerContext = mockControllerContext
			};

			// Act
			var result = controller.Index();

			// Assert
			Assert.IsType<UnauthorizedResult>(result);
		}

		[Fact]
		public async Task Index_AuthenticatedUser_ReturnsViewWithForms()
		{
			// Arrange
			var dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
				.UseInMemoryDatabase(databaseName: "TestDatabase")
				.Options;

			// Create a mock claims identity with a mock claim for the user ID
			var claims = new List<Claim>
			{
				new(ClaimTypes.NameIdentifier, "user000")
			};
			var mockClaimsIdentity = new Mock<ClaimsIdentity>();
			mockClaimsIdentity.Setup(c => c.FindFirst(ClaimTypes.NameIdentifier)).Returns(claims[0]);

			// Create a mock user identity with the mock claims identity
			var mockUserIdentity = new Mock<ClaimsPrincipal>();
			mockUserIdentity.Setup(u => u.Identity).Returns(mockClaimsIdentity.Object);

			// Create a mock HTTP context with the mock user identity
			var mockHttpContext = new Mock<HttpContext>();
			mockHttpContext.Setup(c => c.User).Returns(mockUserIdentity.Object);

			// Create a mock controller context with the mock HTTP context
			var mockControllerContext = new ControllerContext
			{
				HttpContext = mockHttpContext.Object
			};

			await using var dbContext = new AppDbContext(dbContextOptions);
			// Seed the in-memory database with some forms for the user
			var forms = new List<Form>
			{
				new() { OwnerId = "user000", Title = "Form 1" },
				new() { OwnerId = "user000", Title = "Form 2" },
				new() { OwnerId = "user456", Title = "Form 3" } // Form not owned by the user
			};
			await dbContext.Forms.AddRangeAsync(forms);
			await dbContext.SaveChangesAsync();

			var controller = new FormController(dbContext)
			{
				ControllerContext = mockControllerContext
			};

			// Act
			var result = controller.Index();

			// Assert
			var viewResult = Assert.IsType<ViewResult>(result);
			var model = Assert.IsType<FormViewModel>(viewResult.Model);
			Assert.Equal(2, model.ListForms.Count()); // Only forms owned by the user should be displayed
		}

		[Fact]
		public async Task Index_AuthenticatedUserWithoutForms_ReturnsIndexViewWithEmptyFormList()
		{
			// Arrange
			var dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
				.UseInMemoryDatabase(databaseName: "TestDatabase")
				.Options;

			// Create a mock claims identity with a mock claim for the user ID
			var claims = new List<Claim>
			{
				new(ClaimTypes.NameIdentifier, "user001")
			};
			var mockClaimsIdentity = new Mock<ClaimsIdentity>();
			mockClaimsIdentity.Setup(c => c.FindFirst(ClaimTypes.NameIdentifier)).Returns(claims[0]);

			// Create a mock user identity with the mock claims identity
			var mockUserIdentity = new Mock<ClaimsPrincipal>();
			mockUserIdentity.Setup(u => u.Identity).Returns(mockClaimsIdentity.Object);

			// Create a mock HTTP context with the mock user identity
			var mockHttpContext = new Mock<HttpContext>();
			mockHttpContext.Setup(c => c.User).Returns(mockUserIdentity.Object);

			// Create a mock controller context with the mock HTTP context
			var mockControllerContext = new ControllerContext
			{
				HttpContext = mockHttpContext.Object
			};

			await using var dbContext = new AppDbContext(dbContextOptions);
			var controller = new FormController(dbContext)
			{
				ControllerContext = mockControllerContext
			};

			// Act
			var result = controller.Index();

			// Assert
			var viewResult = Assert.IsType<ViewResult>(result);
			var model = Assert.IsType<FormViewModel>(viewResult.Model);
			Assert.Empty(model.ListForms); // No forms are created, so the ListForms should be empty
		}

		[Fact]
		public async Task Index_AuthenticatedUserWithoutFormsButFormsArePresent_ReturnsIndexViewWithEmptyFormList()
		{
			// Arrange
			var dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
				.UseInMemoryDatabase(databaseName: "TestDatabase")
				.Options;

			// Create a mock claims identity with a mock claim for the user ID
			var claims = new List<Claim>
			{
				new(ClaimTypes.NameIdentifier, "user002")
			};
			var mockClaimsIdentity = new Mock<ClaimsIdentity>();
			mockClaimsIdentity.Setup(c => c.FindFirst(ClaimTypes.NameIdentifier)).Returns(claims[0]);

			// Create a mock user identity with the mock claims identity
			var mockUserIdentity = new Mock<ClaimsPrincipal>();
			mockUserIdentity.Setup(u => u.Identity).Returns(mockClaimsIdentity.Object);

			// Create a mock HTTP context with the mock user identity
			var mockHttpContext = new Mock<HttpContext>();
			mockHttpContext.Setup(c => c.User).Returns(mockUserIdentity.Object);

			// Create a mock controller context with the mock HTTP context
			var mockControllerContext = new ControllerContext
			{
				HttpContext = mockHttpContext.Object
			};

			await using var dbContext = new AppDbContext(dbContextOptions);
			// Create a user ID that is different from the logged-in user ID
			const string otherUserId = "otheruser456";

			// Create some forms that are not created by the logged-in user
			var forms = new List<Form>
			{
				new() { Id = 1, OwnerId = otherUserId, Title = "test" },
				new() { Id = 2, OwnerId = otherUserId, Title = "test" }
			};

			dbContext.Forms.AddRange(forms);
			await dbContext.SaveChangesAsync();

			var controller = new FormController(dbContext)
			{
				ControllerContext = mockControllerContext
			};

			// Act
			var result = controller.Index();

			// Assert
			var viewResult = Assert.IsType<ViewResult>(result);
			var model = Assert.IsType<FormViewModel>(viewResult.Model);
			Assert.Empty(model
				.ListForms); // No forms are created by the logged-in user, so the ListForms should be empty
		}

		[Fact]
		public void Create_UnauthorizedUser_ReturnsUnauthorizedResult()
		{
			// Arrange
			var dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
				.UseInMemoryDatabase(databaseName: "TestDatabase")
				.Options;

			// Create a mock claims identity with no claims
			var mockClaimsIdentity = new Mock<ClaimsIdentity>();

			// Create a mock user identity with the mock claims identity
			var mockUserIdentity = new Mock<ClaimsPrincipal>();
			mockUserIdentity.Setup(u => u.Identity).Returns(mockClaimsIdentity.Object);

			// Create a mock HTTP context with the mock user identity
			var mockHttpContext = new Mock<HttpContext>();
			mockHttpContext.Setup(c => c.User).Returns(mockUserIdentity.Object);

			// Create a mock controller context with the mock HTTP context
			var mockControllerContext = new ControllerContext
			{
				HttpContext = mockHttpContext.Object
			};

			using var dbContext = new AppDbContext(dbContextOptions);
			var controller = new FormController(dbContext)
			{
				ControllerContext = mockControllerContext
			};

			// Act
			var result = controller.Create();

			// Assert
			Assert.IsType<UnauthorizedResult>(result);
		}

		[Fact]
		public async Task Create_AuthenticatedUser_ReturnsView()
		{
			// Arrange
			var dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
				.UseInMemoryDatabase(databaseName: "TestDatabase")
				.Options;

			// Create a mock claims identity with a mock claim for the user ID
			var claims = new List<Claim>
			{
				new(ClaimTypes.NameIdentifier, "user003")
			};
			var mockClaimsIdentity = new Mock<ClaimsIdentity>();
			mockClaimsIdentity.Setup(c => c.FindFirst(ClaimTypes.NameIdentifier)).Returns(claims[0]);

			// Create a mock user identity with the mock claims identity
			var mockUserIdentity = new Mock<ClaimsPrincipal>();
			mockUserIdentity.Setup(u => u.Identity).Returns(mockClaimsIdentity.Object);

			// Create a mock HTTP context with the mock user identity
			var mockHttpContext = new Mock<HttpContext>();
			mockHttpContext.Setup(c => c.User).Returns(mockUserIdentity.Object);

			// Create a mock controller context with the mock HTTP context
			var mockControllerContext = new ControllerContext
			{
				HttpContext = mockHttpContext.Object
			};

			await using var dbContext = new AppDbContext(dbContextOptions);
			var controller = new FormController(dbContext)
			{
				ControllerContext = mockControllerContext
			};

			// Act
			var result = controller.Create();

			// Assert
			var viewResult = Assert.IsType<ViewResult>(result);
			Assert.Null(viewResult.ViewName); // Ensure the default view is returned
		}

		[Fact]
		public async Task Create_ValidData_ReturnsRedirection()
		{
			// Arrange
			var dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
				.UseInMemoryDatabase(databaseName: "TestDatabase")
				.Options;

			// Create a mock claims identity with a mock claim for the user ID
			var claims = new List<Claim>
			{
				new(ClaimTypes.NameIdentifier, "user004")
			};
			var mockClaimsIdentity = new Mock<ClaimsIdentity>();
			mockClaimsIdentity.Setup(c => c.FindFirst(ClaimTypes.NameIdentifier)).Returns(claims[0]);

			// Create a mock user identity with the mock claims identity
			var mockUserIdentity = new Mock<ClaimsPrincipal>();
			mockUserIdentity.Setup(u => u.Identity).Returns(mockClaimsIdentity.Object);

			// Create a mock HTTP context with the mock user identity
			var mockHttpContext = new Mock<HttpContext>();
			mockHttpContext.Setup(c => c.User).Returns(mockUserIdentity.Object);

			// Create a mock controller context with the mock HTTP context
			var mockControllerContext = new ControllerContext
			{
				HttpContext = mockHttpContext.Object
			};

			await using var dbContext = new AppDbContext(dbContextOptions);
			var controller = new FormController(dbContext)
			{
				ControllerContext = mockControllerContext
			};

			var formViewModel = new FormViewModel
			{
				Form = new Form
				{
					Title = "Test Form",
					Questions = new List<Question>
					{
						new()
						{
							Text = "Test Question 1",
							Options = new List<Option>(),
							FormId = 0
						},
						new()
						{
							Text = "Test Question 2",
							Options = new List<Option>
							{
								new()
								{
									Text = "Test Option 1"
								},
								new()
								{
									Text = "Test Option 2"
								}
							},
							FormId = 0
						}
					},
					Status = FormStatus.Public,
					OwnerId = "user004"
				},
				Status = FormStatus.Public,
				QuestionTypes = new List<QuestionType>()
			};

			// Act
			var result = controller.Create(formViewModel, "Public", "Open,MultipleOptions");

			// Assert
			var redirectResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("Index", redirectResult.ActionName);
			Assert.Null(redirectResult.ControllerName);

			// Additional assertions to verify model binding
			await using var dbContextVerification = new AppDbContext(dbContextOptions);
			var savedForm = dbContext.Forms.FirstOrDefault(f => f.OwnerId == "user004");
			Assert.NotNull(savedForm);
			Assert.Equal("Test Form", savedForm.Title);
			Assert.Equal(2, savedForm.Questions!.Count);
			Assert.Equal("Public", savedForm.Status.ToString());
			Assert.Equal("user004", savedForm.OwnerId);
		}

		[Fact]
		public async Task Create_DataWithoutQuestions_ReturnsBadRequest()
		{
			// Arrange
			var dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
				.UseInMemoryDatabase(databaseName: "TestDatabase")
				.Options;

			// Create a mock claims identity with a mock claim for the user ID
			var claims = new List<Claim>
			{
				new(ClaimTypes.NameIdentifier, "user005")
			};
			var mockClaimsIdentity = new Mock<ClaimsIdentity>();
			mockClaimsIdentity.Setup(c => c.FindFirst(ClaimTypes.NameIdentifier)).Returns(claims[0]);

			// Create a mock user identity with the mock claims identity
			var mockUserIdentity = new Mock<ClaimsPrincipal>();
			mockUserIdentity.Setup(u => u.Identity).Returns(mockClaimsIdentity.Object);

			// Create a mock HTTP context with the mock user identity
			var mockHttpContext = new Mock<HttpContext>();
			mockHttpContext.Setup(c => c.User).Returns(mockUserIdentity.Object);

			// Create a mock controller context with the mock HTTP context
			var mockControllerContext = new ControllerContext
			{
				HttpContext = mockHttpContext.Object
			};

			await using var dbContext = new AppDbContext(dbContextOptions);
			var controller = new FormController(dbContext)
			{
				ControllerContext = mockControllerContext
			};

			var formViewModel = new FormViewModel
			{
				Form = new Form
				{
					Title = "Test Form",
					Questions = new List<Question>(),
					Status = FormStatus.Public,
					OwnerId = "user005"
				},
				Status = FormStatus.Public,
				QuestionTypes = new List<QuestionType>()
			};

			// Act
			var result = controller.Create(formViewModel, "Public", "Open,MultipleOptions");

			// Assert
			Assert.IsType<BadRequestObjectResult>(result);
			var createdForm = await dbContext.Forms.Where(f => f.OwnerId == "user005").FirstOrDefaultAsync(f => f.Title == "Test Form");
			Assert.Null(createdForm);
		}
		[Fact]
		public async Task Create_DataWithMultipleOptionsQuestionWithoutOption_ReturnsBadRequest()
		{
			// Arrange
			var dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
				.UseInMemoryDatabase(databaseName: "TestDatabase")
				.Options;

			// Create a mock claims identity with a mock claim for the user ID
			var claims = new List<Claim>
			{
				new(ClaimTypes.NameIdentifier, "user006")
			};
			var mockClaimsIdentity = new Mock<ClaimsIdentity>();
			mockClaimsIdentity.Setup(c => c.FindFirst(ClaimTypes.NameIdentifier)).Returns(claims[0]);

			// Create a mock user identity with the mock claims identity
			var mockUserIdentity = new Mock<ClaimsPrincipal>();
			mockUserIdentity.Setup(u => u.Identity).Returns(mockClaimsIdentity.Object);

			// Create a mock HTTP context with the mock user identity
			var mockHttpContext = new Mock<HttpContext>();
			mockHttpContext.Setup(c => c.User).Returns(mockUserIdentity.Object);

			// Create a mock controller context with the mock HTTP context
			var mockControllerContext = new ControllerContext
			{
				HttpContext = mockHttpContext.Object
			};

			await using var dbContext = new AppDbContext(dbContextOptions);
			var controller = new FormController(dbContext)
			{
				ControllerContext = mockControllerContext
			};

			var formViewModel = new FormViewModel
			{
				Form = new Form
				{
					Title = "Test Form",
					Questions = new List<Question>
					{
						new()
						{
							Text = "Test Question 1",
							Options = new List<Option>(),
							FormId = 0
						},
						new()
						{
							Text = "Test Question 2",
							Options = new List<Option>(),
							FormId = 0
						}
					},
					Status = FormStatus.Public,
					OwnerId = "user006"
				},
				Status = FormStatus.Public,
				QuestionTypes = new List<QuestionType>()
			};

			// Act
			var result = controller.Create(formViewModel, "Public", "Open,MultipleOptions");

			// Assert
			Assert.IsType<BadRequestObjectResult>(result);
			var createdForm = await dbContext.Forms.Where(f => f.OwnerId == "user006").FirstOrDefaultAsync(f => f.Title == "Test Form");
			Assert.Null(createdForm);
		}
		[Fact]
		public async Task Create_RedirectsToIndex()
		{
			// Arrange
			var dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
				.UseInMemoryDatabase(databaseName: "TestDatabase")
				.Options;

			// Create a mock claims identity with a mock claim for the user ID
			var claims = new List<Claim>
	{
		new(ClaimTypes.NameIdentifier, "user008")
	};
			var mockClaimsIdentity = new Mock<ClaimsIdentity>();
			mockClaimsIdentity.Setup(c => c.FindFirst(ClaimTypes.NameIdentifier)).Returns(claims[0]);

			// Create a mock user identity with the mock claims identity
			var mockUserIdentity = new Mock<ClaimsPrincipal>();
			mockUserIdentity.Setup(u => u.Identity).Returns(mockClaimsIdentity.Object);

			// Create a mock HTTP context with the mock user identity
			var mockHttpContext = new Mock<HttpContext>();
			mockHttpContext.Setup(c => c.User).Returns(mockUserIdentity.Object);

			// Create a mock controller context with the mock HTTP context
			var mockControllerContext = new ControllerContext
			{
				HttpContext = mockHttpContext.Object
			};

			await using var dbContext = new AppDbContext(dbContextOptions);
			var controller = new FormController(dbContext)
			{
				ControllerContext = mockControllerContext
			};

			var formViewModel = new FormViewModel
			{
				Form = new Form
				{
					Title = "Test Form",
					Questions = new List<Question>
				{
				new()
				{
					Text = "Test Question 1",
					Options = new List<Option>
					{
						new() { Text = "Option 1" },
						new() { Text = "Option 2" }
					},
					FormId = 0
				}
			},
			Status = FormStatus.Public,
			OwnerId = "user008"
			},
				Status = FormStatus.Public,
				QuestionTypes = new List<QuestionType>()
			};

			// Act
			var result = controller.Create(formViewModel, "Public", "MultipleOptions");

			// Assert
			var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("Index", redirectToActionResult.ActionName);
		}
	}
}