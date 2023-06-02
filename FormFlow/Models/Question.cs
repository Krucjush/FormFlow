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
		public string FormId { get; set; }
		[BsonElement("Text")]
		public string Text { get; set; }
		[BsonElement("Type")]
		public QuestionType Type { get; set; }
		[BsonElement("Options")]
		public List<string> Options { get; set; }
	}
}
