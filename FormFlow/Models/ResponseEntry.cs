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
		public string QuestionId { get; set; }
		[BsonElement("Answer")] 
		public string Answer { get; set; }
	}
}
