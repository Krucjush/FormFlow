using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FormFlow.Models
{
	public class ResponseEntry
	{
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string Id { get; set; }
		[BsonElement("QuestionId")]
		[Required(ErrorMessage = "QuestionId is required.")]
		public string QuestionId { get; set; }
		[BsonElement("Answer")]
		[Required(ErrorMessage = "Answer is required.")]
		public string Answer { get; set; }
	}
}
