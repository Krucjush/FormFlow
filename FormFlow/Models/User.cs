using System.ComponentModel.DataAnnotations;
using FormFlow.Models.Enums;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FormFlow.Models
{
	public class User
	{
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string Id { get; set; }
		[BsonElement("Email")]
		[Required(ErrorMessage = "Email is required.")]
		[EmailAddress(ErrorMessage = "Invalid email format.")]
		public string Email { get; set; }
		[BsonElement("PasswordHash")]
		[Required(ErrorMessage = "Password is required.")]
		[StringLength(60, MinimumLength = 60, ErrorMessage = "Invalid password hash.")]
		public string PasswordHash { get; set; }
		[BsonElement("Roles")] 
		public Roles[] Roles { get; set; } = { Enums.Roles.User };
		[BsonElement("Forms")] 
		public List<Form>? Forms { get; set; }
	}
}
