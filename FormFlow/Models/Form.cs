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
		public string Title { get; set; }
		[BsonElement("Questions")]
		public List<Question> Questions { get; set; }
		[BsonElement("Status")]
		public FormStatus Status { get; set; }
		[BsonElement("OwnerId")] 
		public string? OwnerId { get; set; }
	}
}
