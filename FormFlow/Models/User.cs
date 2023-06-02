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
		public string Email { get; set; }
		[BsonElement("PasswordHash")] 
		public string PasswordHash { get; set; }
		[BsonElement("Roles")]
		public string[] Roles { get; set; }
		[BsonElement("Forms")] 
		public List<Form> Forms { get; set; }
	}
}
