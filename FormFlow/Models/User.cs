using Microsoft.AspNetCore.Identity;

namespace FormFlow.Models
{
	public class User : IdentityUser
	{
		public ICollection<UserRole> UserRoles { get; set; }
		public List<Form>? Forms { get; set; }
	}
}
