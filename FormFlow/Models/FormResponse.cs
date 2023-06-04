using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FormFlow.Models
{
	public class FormResponse
	{
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string Id { get; set; }
		[BsonElement("FormId")] 
		public string FormId { get; set; }
		[BsonElement("Email")] 
		public string Email { get; set; }
		[BsonElement("Responses")] 
		public List<ResponseEntry>? Responses { get; set; }
	}
}
