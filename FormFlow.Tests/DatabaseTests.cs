using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using FormFlow.Data;
using FormFlow.Models;
using FormFlow.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Xunit.Abstractions;

namespace FormFlow.Tests
{
    public class DatabaseTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public DatabaseTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        private DbContextOptions<AppDbContext> GetInMemoryDbContextOptions()
        {
            return new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
        }
        [Fact]
        public void Can_Save_Form_To_Database()
        {
            // Arrange
            using (var context = new AppDbContext(GetInMemoryDbContextOptions()))
            {
                var question = new Question
                {
                    Text = "testQuestion",
                    Type = QuestionType.Mark,
                    FormId = 0
                };
                var form = new Form
                {
                    Title = "Test Form",
                    Questions = new List<Question> { question },
                    Status = FormStatus.Public
                };

                // Act
                context.Forms.Add(form);
                context.SaveChanges();
            }

            // Assert
            using (var context = new AppDbContext(GetInMemoryDbContextOptions()))
            {
                var savedForm = context.Forms.FirstOrDefault(f => f.Title == "Test Form");
                Assert.NotNull(savedForm);
            }
        }
        [Fact]
        public void Can_Save_FormResponse_To_Database()
        {
            // Arrange
            using (var context = new AppDbContext(GetInMemoryDbContextOptions()))
            {
                var formResponse = new FormResponse
                {
                    FormId = 0,
                    Email = "test@test.com"
                };

                // Act
                context.FormResponses.Add(formResponse);
                context.SaveChanges();
            }

            // Assert
            using (var context = new AppDbContext(GetInMemoryDbContextOptions()))
            {
                var savedFormResponse = context.FormResponses.FirstOrDefault(f => f.Email == "test@test.com");
                Assert.NotNull(savedFormResponse);
            }
        }
        [Fact]
        public void Can_Save_ResponseEntry_To_Database()
        {
            // Arrange
            using (var context = new AppDbContext(GetInMemoryDbContextOptions()))
            {
                var responseEntry = new ResponseEntry
                {
                    QuestionId = 0,
                    Answer = "test"
                };

                // Act
                context.ResponseEntries.Add(responseEntry);
                context.SaveChanges();
            }

            // Assert
            using (var context = new AppDbContext(GetInMemoryDbContextOptions()))
            {
                var savedResponseEntry = context.ResponseEntries.FirstOrDefault(r => r.Answer == "test");
                Assert.NotNull(savedResponseEntry);
            }
        }
        [Fact]
        public void Can_Save_User_To_Database()
        {
            // Arrange
            using (var context = new AppDbContext(GetInMemoryDbContextOptions()))
            {
                
                var user = new User
                {
                    Email = "test@test.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("test"),
                    UserRoles = new List<UserRole>(),
                    Forms = new List<Form>()
                };

                // Act
                context.Users.Add(user);
                context.SaveChanges();
            }

            // Assert
            using (var context = new AppDbContext(GetInMemoryDbContextOptions()))
            {
                var savedUser = context.Users.FirstOrDefault(u => u.Email == "test@test.com");
                Assert.NotNull(savedUser);
            }
        }
        [Fact]
        public void Form_Without_Title_Cannot_Be_Saved()
        {
            // Arrange
            using var context = new AppDbContext(GetInMemoryDbContextOptions());
            var form = new Form
            {
                // No title
                Questions = new List<Question>(),
                Status = FormStatus.Public
            };


            // Act and Assert
            Assert.ThrowsAny<Exception>(() =>
            {
                context.Forms.Add(form);
                context.SaveChanges();
            });
        }
        [Fact]
        public void Form_Without_Questions_Cannot_Be_Saved()
        {
            // Arrange
            using var context = new AppDbContext(GetInMemoryDbContextOptions());
            var form = new Form
            {
                Title = "Sample Form", // Set a title for the form
                Questions = new List<Question>() // Empty list of questions
            };

            // Act
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(form, null, null);
            var isValid = Validator.TryValidateObject(form, validationContext, validationResults, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, r => r.ErrorMessage == "At least one question is required.");
        }
        [Fact]
        public void FormResponse_Without_Email_Cannot_Be_Saved()
        {
            // Arrange
            using var context = new AppDbContext(GetInMemoryDbContextOptions());
            var formResponse = new FormResponse
            {
                FormId = 1,
                // Email is not set
                Responses = new List<ResponseEntry>()
            };

            // Act & Assert
            Assert.Throws<DbUpdateException>(() =>
            {
                context.FormResponses.Add(formResponse);
                context.SaveChanges(); // Trigger validation and save changes
            });
        }
        [Fact]
        public void ResponseEntry_Without_Answer_Cannot_Be_Saved()
        {
            // Arrange
            using var context = new AppDbContext(GetInMemoryDbContextOptions());
            var responseEntry = new ResponseEntry
            {
                QuestionId = 1, // Set a valid QuestionId
                Answer = string.Empty // Set an empty answer
            };

            // Act
            var validationContext = new ValidationContext(responseEntry, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(responseEntry, validationContext, validationResults, validateAllProperties: true);

            // Assert
            Assert.False(isValid);
            Assert.NotEmpty(validationResults);
            Assert.Contains(validationResults, result => result.ErrorMessage.Contains("Answer is required."));
        }
        [Fact]
        public void User_Without_Email_Cannot_Be_Saved()
        {
            // Arrange
            using var context = new AppDbContext(GetInMemoryDbContextOptions());
            var user = new User
            {
                Email = string.Empty, // Set an empty email
                PasswordHash = "passwordhash" // Set a valid password hash
            };

            // Act
            var validationContext = new ValidationContext(user, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(user, validationContext, validationResults, validateAllProperties: true);

            // Assert
            Assert.False(isValid);
            Assert.NotEmpty(validationResults);
            Assert.Contains(validationResults, result => result.ErrorMessage.Contains("Email is required."));
        }

        [Fact]
        public void User_Without_Password_Cannot_Be_Saved()
        {
            // Arrange
            using var context = new AppDbContext(GetInMemoryDbContextOptions());
            var user = new User
            {
                Email = "user@example.com", // Set a valid email
                PasswordHash = string.Empty // Set an empty password hash
            };

            // Act
            var validationContext = new ValidationContext(user, serviceProvider: null, items: null);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(user, validationContext, validationResults, validateAllProperties: true);

            // Assert
            Assert.False(isValid);
            Assert.NotEmpty(validationResults);
            Assert.Contains(validationResults, result => result.ErrorMessage.Contains("Password is required."));
        }
        public void Dispose()
        {
            using var context = new AppDbContext(GetInMemoryDbContextOptions());
            context.Database.EnsureDeleted();
        }
    }

}