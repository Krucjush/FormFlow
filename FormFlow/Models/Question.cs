using System.ComponentModel.DataAnnotations;
using FormFlow.Models.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FormFlow.Models
{
	public class Question
	{
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string Id { get; set; }
		[BsonElement("FormId")]
		[Required(ErrorMessage = "FormId is required.")]
		public string FormId { get; set; }
		[BsonElement("Text")]
		[Required(ErrorMessage = "Text is required.")]
		public string Text { get; set; }
		[BsonElement("Type")]
		[Required(ErrorMessage = "Type is required.")]
		public QuestionType Type { get; set; }
		[BsonElement("Options")]
		public List<string>? Options { get; set; }
	}
}
