using System.ComponentModel.DataAnnotations;
using FormFlow.Models.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FormFlow.Models
{
	public class Form
	{
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string Id { get; set; }
		[BsonElement("Title")]
		[Required(ErrorMessage = "Title is required.")]
		public string Title { get; set; }
		[BsonElement("Questions")]
		[Required(ErrorMessage = "Questions are required.")]
		[MinLength(1, ErrorMessage = "At least one question is required.")]
		public List<Question> Questions { get; set; }
		[BsonElement("Status")] 
		public FormStatus Status { get; set; } = FormStatus.Public;
		[BsonElement("OwnerId")] 
		public string? OwnerId { get; set; }
	}
}
