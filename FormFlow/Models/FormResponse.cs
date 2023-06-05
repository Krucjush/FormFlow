using System.ComponentModel.DataAnnotations;
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
		[Required(ErrorMessage = "FormId is required.")]
		public string FormId { get; set; }
		[BsonElement("Email")]
		[Required(ErrorMessage = "Email is required.")]
		[EmailAddress(ErrorMessage = "Invalid email format.")]
		public string Email { get; set; }
		[BsonElement("Responses")] 
		public List<ResponseEntry>? Responses { get; set; }
	}
}
