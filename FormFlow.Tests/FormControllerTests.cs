using FormFlow.Controllers;
using FormFlow.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using FormFlow.Data;
using Moq;
using FormFlow.Models.ViewModels;
using Microsoft.Extensions.Logging.Abstractions;
using FormFlow.Models.Enums;
using Xunit.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.SqlServer.Server;
using System.Diagnostics;
using Microsoft.Extensions.Primitives;

namespace FormFlow.Tests
{
	public class FormControllerTests
	{
		private readonly ITestOutputHelper _testOutputHelper;

		public FormControllerTests(ITestOutputHelper testOutputHelper)
		{
			_testOutputHelper = testOutputHelper;
		}
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
			var controller = new FormController(dbContext, null, null)
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

			var controller = new FormController(dbContext, null, null)
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
			var controller = new FormController(dbContext, null, null)
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

			var controller = new FormController(dbContext, null, null)
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
			var controller = new FormController(dbContext, null, null)
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
			var controller = new FormController(dbContext, null, null)
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
			var controller = new FormController(dbContext, null, null)
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
			var controller = new FormController(dbContext, null, null)
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
			var controller = new FormController(dbContext, null, null)
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
			var controller = new FormController(dbContext, null, null)
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

		[Fact]
		public void Modify_UnauthorizedUser_ReturnsUnauthorizedResult()
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
			var controller = new FormController(dbContext, null, null)
			{
				ControllerContext = mockControllerContext
			};

			// Act
			var result = controller.Modify(new FormViewModel(), "status", "type");

			// Assert
			Assert.IsType<UnauthorizedResult>(result);
		}
		[Fact]
		public void Modify_FormNotFound_ReturnsNotFoundResult()
		{
			// Arrange
			var dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
				.UseInMemoryDatabase(databaseName: "TestDatabase")
				.Options;

			const int formId = 1; // ID of the non-existent form

			using var dbContext = new AppDbContext(dbContextOptions);
			var controller = new FormController(dbContext, null, null);

			// Act
			var result = controller.Modify(formId);

			// Assert
			Assert.IsType<NotFoundResult>(result);
		}
		[Fact]
		public async Task Modify_NoQuestions_ReturnsBadRequestResult()
		{
			// Arrange
			var dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
				.UseInMemoryDatabase(databaseName: "TestDatabase")
				.Options;

			var claims = new List<Claim>
			{
				new(ClaimTypes.NameIdentifier, "user009")
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
			var controller = new FormController(dbContext, null, null)
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
					OwnerId = "user009"
				},
				Status = FormStatus.Public,
				QuestionTypes = new List<QuestionType>()
			};

			// Act
			var result = controller.Modify(formViewModel, "Public", "Open,MultipleOptions");

			// Assert
			Assert.IsType<BadRequestObjectResult>(result);
			var createdForm = await dbContext.Forms.FirstOrDefaultAsync(f => f.OwnerId == "user009" && f.Title == "Test Form");
			Assert.Null(createdForm);
		}
		[Fact]
		public async Task Modify_DataWithMultipleOptionsQuestionWithoutOption_ReturnsBadRequest()
		{
			// Arrange
			var dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
				.UseInMemoryDatabase(databaseName: "TestDatabase")
				.Options;

			// Create a mock claims identity with a mock claim for the user ID
			var claims = new List<Claim>
			{
				new(ClaimTypes.NameIdentifier, "user010")
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
			var controller = new FormController(dbContext, null, null)
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
					OwnerId = "user010"
				},
				Status = FormStatus.Public,
				QuestionTypes = new List<QuestionType>()
			};

			// Act
			var result = controller.Modify(formViewModel, "Public", "Open,MultipleOptions");

			// Assert
			Assert.IsType<BadRequestObjectResult>(result);
			var createdForm = await dbContext.Forms.Where(f => f.OwnerId == "user010").FirstOrDefaultAsync(f => f.Title == "Test Form");
			Assert.Null(createdForm);
		}
		[Fact]
		public void Modify_UserLoggedInAndFormExists_ReturnsViewResult()
		{
			// Arrange
			var dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
				.UseInMemoryDatabase(databaseName: "TestDatabase")
				.Options;

			const string userId = "user011";
			const int formId = 1;

			var claims = new List<Claim>
			{
				new(ClaimTypes.NameIdentifier, userId)
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

			using var dbContext = new AppDbContext(dbContextOptions);
			var controller = new FormController(dbContext, null, null)
			{
				ControllerContext = mockControllerContext
			};

			var existingForm = new Form
			{
				Id = formId,
				Title = "Existing Form",
				Questions = new List<Question>(),
				Status = FormStatus.Public,
				OwnerId = userId
			};

			dbContext.Forms.Add(existingForm);
			dbContext.SaveChanges();

			// Act
			var result = controller.Modify(formId);

			// Assert
			Assert.IsType<ViewResult>(result);
			var viewResult = (ViewResult)result;
			var model = viewResult.Model as FormViewModel;
			Assert.NotNull(model);
			Assert.Equal(formId, model.Form!.Id);
			Assert.Equal("Existing Form", model.Form.Title);
		}
		[Fact]
		public void FormHasResponses_ReturnsTrue_WhenFormHasResponses()
		{
			// Arrange
			var dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
				.UseInMemoryDatabase(databaseName: "TestDatabase")
				.Options;

			// Create a new in-memory database context
			using var dbContext = new AppDbContext(dbContextOptions);

			// Create a sample form with responses
			const int formId = 1;
			var formResponses = new List<FormResponse>
			{
				new() { FormId = formId, Email = "test@email.1", Responses = new List<ResponseEntry> { new() { Answer = "a", QuestionId = 1} }},
				new() { FormId = formId, Email = "test@email.2", Responses = new List<ResponseEntry> { new() { Answer = "ab", QuestionId = 2} }}
			};
			dbContext.FormResponses.AddRange(formResponses);
			dbContext.SaveChanges();

			// Create an instance of the controller
			var controller = new FormController(dbContext, null, null);

			// Act
			var result = controller.FormHasResponses(formId);

			// Assert
			Assert.True(result);
		}
		[Fact]
		public void Remove_FormNotFound_ReturnsNotFound()
		{
			// Arrange
			var dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
				.UseInMemoryDatabase(databaseName: "TestDatabase")
				.Options;

			var dbContext = new AppDbContext(dbContextOptions);
			var controller = new FormController(dbContext, null, null);

			const int nonExistingFormId = 123; // Assuming this form ID does not exist

			// Act
			var result = controller.Remove(nonExistingFormId);

			// Assert
			Assert.IsType<NotFoundResult>(result);
		}
		[Fact]
		public void Remove_FormHasResponses_ReturnsErrorMessageAndRedirectToIndex()
		{
			// Arrange
			var dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
				.UseInMemoryDatabase(databaseName: "TestDatabase")
				.Options;

			var dbContext = new AppDbContext(dbContextOptions);
			var controller = new FormController(dbContext, null, null);

			const int formId = 1; // Assuming this form ID has associated responses

			// Add a dummy response to simulate associated responses
			dbContext.Forms.Add(new Form { Id = formId, Title = "Test"});
			dbContext.FormResponses.Add(new FormResponse { FormId = formId, Email = "test@mail.com"});
			dbContext.SaveChanges();

			// Act
			var result = controller.Remove(formId);

			// Assert
			var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("Index", redirectToActionResult.ActionName);
			Assert.Null(redirectToActionResult.ControllerName);

			var routeValues = redirectToActionResult.RouteValues;
			Assert.NotNull(routeValues);
			Assert.Contains("errorMessage", routeValues.Keys);
			Assert.Equal("Cannot remove the form as it has associated responses.", routeValues["errorMessage"]);
		}
		[Fact]
		public void Remove_FormExistsNoResponses_RemovesFormAndRedirectsToIndex()
		{
			// Arrange
			var dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
				.UseInMemoryDatabase(databaseName: "TestDatabase")
				.Options;

			var dbContext = new AppDbContext(dbContextOptions);
			var controller = new FormController(dbContext, null, null);

			const int formId = 1; // Assuming this form ID exists and has no associated responses

			// Add the form to the Forms table
			var form = new Form { Id = formId, Title = "test"};
			dbContext.Forms.Add(form);
			dbContext.SaveChanges();

			// Act
			var result = controller.Remove(formId);

			// Assert
			var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("Index", redirectToActionResult.ActionName);
			Assert.Null(redirectToActionResult.ControllerName);

			// Check that the form was removed from the Forms table
			var removedForm = dbContext.Forms.FirstOrDefault(f => f.Id == formId);
			Assert.Null(removedForm);
		}
		[Fact]
		public void CanSubmitResponse_PublicForm_ReturnsTrue()
		{
			// Arrange
			var dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
				.UseInMemoryDatabase(databaseName: "TestDatabase")
				.Options;

			var form = new Form { Title = "test", Status = FormStatus.Public };
			const string userEmail = "test@example.com";
			const bool isAuthenticated = false;

			var dbContext = new AppDbContext(dbContextOptions);
			var controller = new FormController(dbContext, null, null);

			// Act
			var result = controller.CanSubmitResponse(form, userEmail, isAuthenticated);

			// Assert
			Assert.True(result);
		}
		[Fact]
		public void CanSubmitResponse_PrivateFormWithAuthenticatedUser_ReturnsTrue()
		{
			// Arrange
			var dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
				.UseInMemoryDatabase(databaseName: "TestDatabase")
				.Options;

			var form = new Form { Title = "test", Status = FormStatus.Private };
			const string userEmail = "test@example.com";
			const bool isAuthenticated = true; // Set the user as authenticated

			var dbContext = new AppDbContext(dbContextOptions);
			var controller = new FormController(dbContext, null, null);

			// Act
			var result = controller.CanSubmitResponse(form, userEmail, isAuthenticated);

			// Assert
			Assert.True(result);
		}
		[Fact]
		public void CanSubmitResponse_PrivateFormWithUnauthenticatedUser_ReturnsFalse()
		{
			// Arrange
			var dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
				.UseInMemoryDatabase(databaseName: "TestDatabase")
				.Options;

			var form = new Form { Title = "test", Status = FormStatus.Private };
			const string userEmail = "test@example.com";
			const bool isAuthenticated = false; // Set the user as unauthenticated

			var dbContext = new AppDbContext(dbContextOptions);
			var controller = new FormController(dbContext, null, null);

			// Act
			var result = controller.CanSubmitResponse(form, userEmail, isAuthenticated);

			// Assert
			Assert.False(result);
		}
		[Fact]
		public void CanSubmitResponse_DomainFormWithEmptyUserEmail_ReturnsFalse()
		{
			// Arrange
			var dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
				.UseInMemoryDatabase(databaseName: "TestDatabase")
				.Options;

			var form = new Form { Title = "test", Status = FormStatus.Private };
			const string userEmail = "";
			const bool isAuthenticated = false;

			var dbContext = new AppDbContext(dbContextOptions);
			var controller = new FormController(dbContext, null, null);

			// Act
			var result = controller.CanSubmitResponse(form, userEmail, isAuthenticated);

			// Assert
			Assert.False(result);
		}
		[Fact]
		public void CanSubmitResponse_DomainFormWithDomainMismatch_ReturnsFalse()
		{
			// Arrange
			var form = new Form { Status = FormStatus.Domain, OwnerId = "user024" };
			const string userEmail = "test@example.com";
			const bool isAuthenticated = false;

			var mockController = new Mock<FormController>(null, null, null);
			var classUnderTest = mockController.Object;

			// Mock the method calls for GetFromDomain and GetUserEmailDomain
			mockController.Setup(c => c.GetFromDomain(It.IsAny<Form>())).Returns("example.com");
			mockController.Setup(c => c.GetUserEmailDomain(It.IsAny<string>())).Returns("different.com");

			// Act
			var result = classUnderTest.CanSubmitResponse(form, userEmail, isAuthenticated);

			// Assert
			Assert.False(result);
		}
		[Fact]
		public void CanSubmitResponse_DomainFormWithDomainMatch_ReturnsTrue()
		{
			// Arrange
			var form = new Form { Status = FormStatus.Domain, OwnerId = "user025" };
			const string ownerEmail = "owner@example.com";
			const string customerEmail = "test@example.com";
			const bool isAuthenticated = false;

			var dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
				.UseInMemoryDatabase(databaseName: "TestDatabase")
				.Options;
			var formFlowContextOptions = new DbContextOptionsBuilder<FormFlowContext>()
				.UseInMemoryDatabase(databaseName: "TestIdentity")
				.Options;

			using var dbContext = new AppDbContext(dbContextOptions);
			using var formFlowContext = new FormFlowContext(formFlowContextOptions);
			var userStore = new UserStore<User>(formFlowContext);
			var identityOptions = new IdentityOptions();
			var optionsAccessor = Options.Create(identityOptions);
			var userManager = new UserManager<User>(
				userStore, optionsAccessor, null, null, null, null, null, null, null);
			var controller = new FormController(dbContext, formFlowContext, userManager);

			// Add the owner to the user database
			var owner = new User
			{
				Id = form.OwnerId,
				UserName = ownerEmail,
				Email = ownerEmail
			};
			formFlowContext.Users.Add(owner);
			formFlowContext.SaveChanges();

			// Act
			var result = controller.CanSubmitResponse(form, customerEmail, isAuthenticated);

			// Assert
			Assert.True(result);
		}
		[Fact]
		public void GetFromDomain_OwnerIsNull_ReturnsEmptyString()
		{
			// Arrange
			var dbContextOptions = new DbContextOptionsBuilder<FormFlowContext>()
				.UseInMemoryDatabase(databaseName: "TestDatabase")
				.Options;

			var form = new Form { OwnerId = "nonexistentuser" };

			using var dbContext = new FormFlowContext(dbContextOptions);
			var controller = new FormController(null, dbContext, null);

			// Act
			var result = controller.GetFromDomain(form);

			// Assert
			Assert.Equal(string.Empty, result);
		}
		[Fact]
		public void GetFromDomain_UserExists_ReturnsCorrectDomain()
		{
			// Arrange
			var dbContextOptions = new DbContextOptionsBuilder<FormFlowContext>()
				.UseInMemoryDatabase(databaseName: "TestDatabase")
				.Options;

			const string ownerId = "user027";
			const string emailAddress = "test@example.com";
			const string expectedDomain = "example.com";

			var form = new Form { OwnerId = ownerId };
			var user = new User { Id = ownerId, Email = emailAddress };

			using var dbContext = new FormFlowContext(dbContextOptions);
			dbContext.Users.Add(user);
			dbContext.SaveChanges();

			var controller = new FormController(null, dbContext, null);

			// Act
			var result = controller.GetFromDomain(form);

			// Assert
			Assert.Equal(expectedDomain, result);
		}
		[Fact]
		public void GetUserEmailDomain_EmailShorterThanOne_ReturnsEmptyString()
		{
			// Arrange
			const string email = "example@";
			var expectedDomain = string.Empty;

			var controller = new FormController(null, null, null);

			// Act
			var result = controller.GetUserEmailDomain(email);

			// Assert
			Assert.Equal(expectedDomain, result);
		}
		[Fact]
		public void GetUserEmailDomain_CorrectEmail_ReturnsDomain()
		{
			// Arrange
			const string email = "test@example.com";
			const string expectedDomain = "example.com";

			var controller = new FormController(null, null, null);

			// Act
			var result = controller.GetUserEmailDomain(email);

			// Assert
			Assert.Equal(expectedDomain, result);
		}
		[Fact]
		public async Task Display_FormIsNull_ReturnsNotFoundResult()
		{
			// Arrange
			var dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
				.UseInMemoryDatabase(databaseName: "TestDatabase")
				.Options;

			const int formId = 123; // Provide an existing form ID
			var dbContext = new AppDbContext(dbContextOptions);
			var controller = new FormController(dbContext, null, null);

			// Act
			var result = await controller.Display(formId);

			// Assert
			Assert.IsType<NotFoundResult>(result);
		}
		[Fact]
		public async Task Display_CanSubmitResponse_ReturnsViewResult()
		{
			// Arrange
			var dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
				.UseInMemoryDatabase(databaseName: "TestDatabase")
				.Options;
			var formFlowContextOptions = new DbContextOptionsBuilder<FormFlowContext>()
				.UseInMemoryDatabase(databaseName: "TestIdentity")
				.Options;

			const int formId = 123; // Provide an existing form ID
			await using var dbContext = new AppDbContext(dbContextOptions);
			await using var formFlowContext = new FormFlowContext(formFlowContextOptions);
			var userStore = new UserStore<User>(formFlowContext);
			var identityOptions = new IdentityOptions();
			var optionsAccessor = Options.Create(identityOptions);
			var userManager = new UserManager<User>(
				userStore, optionsAccessor, null, null, null, null, null, null, null);
			var controller = new FormController(dbContext, formFlowContext, userManager);

			// Set the User property manually for testing purposes
			var claims = new[]
			{
				new Claim(ClaimTypes.NameIdentifier, "user031"),
				new Claim(ClaimTypes.Name, "user031.example.com"),
				new Claim(ClaimTypes.Email, "user031.example.com")
			};
			var identity = new ClaimsIdentity(claims, "TestAuth");
			var claimsPrincipal = new ClaimsPrincipal(identity);
			controller.ControllerContext = new ControllerContext
			{
				HttpContext = new DefaultHttpContext
				{
					User = claimsPrincipal
				}
			};

			var user = new User
			{
				Id = "user031",
				UserName = "user031.example.com",
				Email = "user031.example.com",
			};
			formFlowContext.Users.Add(user);
			await formFlowContext.SaveChangesAsync();
			var form = new Form
			{
				Id = formId,
				Title = "Test Form"
			};
			dbContext.Forms.Add(form);
			await dbContext.SaveChangesAsync();

			// Act
			var result = await controller.Display(formId);

			// Assert
			Assert.IsType<ViewResult>(result);
		}
		[Fact]
		public async Task Display_PrivateFormWithoutPermission_RedirectsToHomeIndexWithErrorMessage()
		{
			// Arrange
			var form = new Form { Title = "Test Form", Status = FormStatus.Private, OwnerId = "user032" };
			const string userEmail = "test@example.com";
			const string expectedErrorMessage = "You don't have permission to contribute to this form.\nForm Status is Private.\nPlease confirm your email to contribute to this form.";

			var dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
				.UseInMemoryDatabase(databaseName: "TestDatabase")
				.Options;
			var formFlowContextOptions = new DbContextOptionsBuilder<FormFlowContext>()
				.UseInMemoryDatabase(databaseName: "TestIdentity")
				.Options;

			await using var dbContext = new AppDbContext(dbContextOptions);
			await using var formFlowContext = new FormFlowContext(formFlowContextOptions);
			var userStore = new UserStore<User>(formFlowContext);
			var identityOptions = new IdentityOptions();
			var optionsAccessor = Options.Create(identityOptions);
			var userManager = new UserManager<User>(
				userStore, optionsAccessor, null, null, null, null, null, null, null);

			var claims = new List<Claim>
			{
				new(ClaimTypes.Name, userEmail)
			};
			var identity = new ClaimsIdentity(claims);
			var user = new ClaimsPrincipal(identity);

			var controller = new FormController(dbContext, formFlowContext, userManager)
			{
				ControllerContext = new ControllerContext
				{
					HttpContext = new DefaultHttpContext { User = user }
				}
			};

			// Add the owner to the user database
			var owner = new User
			{
				Id = form.OwnerId,
				UserName = "owner@example.com",
				Email = "owner@example.com"
			};
			formFlowContext.Users.Add(owner);
			await formFlowContext.SaveChangesAsync();

			var formUser = new User
			{
				Id = "user032a",
				UserName = userEmail,
				Email = userEmail
			};
			formFlowContext.Users.Add(formUser);
			await formFlowContext.SaveChangesAsync();

			dbContext.Forms.Add(form);
			await dbContext.SaveChangesAsync();

			// Act
			var result = await controller.Display(1);

			// Assert
			var redirectResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("Index", redirectResult.ActionName);
			Assert.Equal("Home", redirectResult.ControllerName);
			Assert.Equal(expectedErrorMessage, redirectResult.RouteValues["errorMessage"]);
		}
		[Fact]
		public async Task Display_DomainFormWithoutPermission_RedirectsToHomeIndexWithErrorMessage()
		{
			// Arrange
			var form = new Form { Title = "Test Form", Status = FormStatus.Domain, OwnerId = "user033" };
			const string userEmail = "test@different.com";
			const string expectedErrorMessage = "You don't have permission to contribute to this form.\nForm Status is Domain.\nForm domain is `example.com`.\nYour domain is `different.com`.";

			var dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
				.UseInMemoryDatabase(databaseName: "TestDatabase")
				.Options;
			var formFlowContextOptions = new DbContextOptionsBuilder<FormFlowContext>()
				.UseInMemoryDatabase(databaseName: "TestIdentity")
				.Options;

			await using var dbContext = new AppDbContext(dbContextOptions);
			await using var formFlowContext = new FormFlowContext(formFlowContextOptions);
			var userStore = new UserStore<User>(formFlowContext);
			var identityOptions = new IdentityOptions();
			var optionsAccessor = Options.Create(identityOptions);
			var userManager = new UserManager<User>(
				userStore, optionsAccessor, null, null, null, null, null, null, null);

			var claims = new List<Claim>
			{
				new(ClaimTypes.Name, userEmail)
			};
			var identity = new ClaimsIdentity(claims);
			var user = new ClaimsPrincipal(identity);

			var controller = new FormController(dbContext, formFlowContext, userManager)
			{
				ControllerContext = new ControllerContext
				{
					HttpContext = new DefaultHttpContext { User = user }
				}
			};

			// Add the owner to the user database
			var owner = new User
			{
				Id = form.OwnerId,
				UserName = "owner@example.com",
				Email = "owner@example.com"
			};
			formFlowContext.Users.Add(owner);
			await formFlowContext.SaveChangesAsync();

			var formUser = new User
			{
				Id = "user032a",
				UserName = userEmail,
				Email = userEmail
			};
			formFlowContext.Users.Add(formUser);
			await formFlowContext.SaveChangesAsync();

			dbContext.Forms.Add(form);
			await dbContext.SaveChangesAsync();

			// Act
			var result = await controller.Display(1);

			// Assert
			var redirectResult = Assert.IsType<RedirectToActionResult>(result);
			Assert.Equal("Index", redirectResult.ActionName);
			Assert.Equal("Home", redirectResult.ControllerName);
			Assert.Equal(expectedErrorMessage, redirectResult.RouteValues["errorMessage"]);
		}
		[Fact]
		public async Task SubmitResponse_FormNotFound_ReturnsNotFoundResult()
		{
			// Arrange
			const int formId = 1; // Specify the ID of the non-existing form

			// Create an in-memory database
			var dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
				.UseInMemoryDatabase(databaseName: "TestDatabase")
				.Options;

			await using var dbContext = new AppDbContext(dbContextOptions);

			// Create a mock user identity
			var mockUserIdentity = new Mock<ClaimsIdentity>();
			mockUserIdentity.Setup(u => u.Name).Returns("test@example.com");

			// Create a mock claims principal
			var mockUserPrincipal = new Mock<ClaimsPrincipal>();
			mockUserPrincipal.Setup(p => p.Identity).Returns(mockUserIdentity.Object);

			// Create a mock HTTP context with the mock user principal
			var mockHttpContext = new Mock<HttpContext>();
			mockHttpContext.Setup(c => c.User).Returns(mockUserPrincipal.Object);

			// Create a mock controller context with the mock HTTP context
			var mockControllerContext = new ControllerContext
			{
				HttpContext = mockHttpContext.Object
			};

			// Create the controller with the mock controller context and the DbContext
			var controller = new FormController(dbContext, null, null)
			{
				ControllerContext = mockControllerContext
			};

			// Act
			var result = await controller.SubmitResponse(formId, null);

			// Assert
			Assert.IsType<NotFoundResult>(result);
		}
		[Fact]
		public async Task SubmitResponse_ValidFormAndData_ReturnsResponseSubmittedView()
		{
			// Arrange
			const int formId = 1; // Specify the ID of the existing form

			// Create an in-memory database
			var dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
				.UseInMemoryDatabase(databaseName: "TestDatabase")
				.Options;

			await using var dbContext = new AppDbContext(dbContextOptions);

			// Create a sample form with questions and options for testing
			var form = new Form
			{
				Id = formId,
				Title = "Test",
				Questions = new List<Question>
				{
					new()
					{
						Id = 1,
						Text = "Question 1",
						Type = QuestionType.Mark
					},
					new()
					{
						Id = 2,
						Text = "Question 2",
						Type = QuestionType.MultipleOptions,
						Options = new List<Option>
						{
							new() { Id = 1, Text = "Option 1" },
							new() { Id = 2, Text = "Option 2" }
						}
					},
					new() { Id = 3, Text = "Question 3", Type = QuestionType.Open }
				}
			};

			dbContext.Forms.Add(form);
			await dbContext.SaveChangesAsync();

			// Create a mock user identity
			var mockUserIdentity = new Mock<ClaimsIdentity>();
			mockUserIdentity.Setup(u => u.Name).Returns("test@example.com");

			// Create a mock claims principal
			var mockUserPrincipal = new Mock<ClaimsPrincipal>();
			mockUserPrincipal.Setup(p => p.Identity).Returns(mockUserIdentity.Object);

			// Create a mock HTTP context with the mock user principal
			var mockHttpContext = new Mock<HttpContext>();
			mockHttpContext.Setup(c => c.User).Returns(mockUserPrincipal.Object);

			// Create a mock controller context with the mock HTTP context
			var mockControllerContext = new ControllerContext
			{
				HttpContext = mockHttpContext.Object
			};

			// Create the controller with the mock controller context and the DbContext
			var controller = new FormController(dbContext, null, null)
			{
				ControllerContext = mockControllerContext
			};

			// Create a mock form collection with user responses
			var formCollection = new FormCollection(new Dictionary<string, StringValues>
			{
				{ "question_1", "4" }, // User response for Question 1 (Mark question)
                { "question_2", "2" }, // User response for Question 2 (Multiple options question)
                { "question_3", "Open response" } // User response for Question 3 (Open question)
            });

			// Act
			var result = await controller.SubmitResponse(formId, formCollection);

			// Assert
			Assert.IsType<ViewResult>(result);
			var viewResult = result as ViewResult;
			Assert.Equal("ResponseSubmitted", viewResult.ViewName);
		}
	}
}
