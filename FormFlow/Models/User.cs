using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using FormFlow.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace FormFlow.Models
{
    public class User : IdentityUser
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        [StringLength(60, MinimumLength = 60, ErrorMessage = "Invalid password hash.")]
        public string PasswordHash { get; set; }
        public ICollection<UserRole> UserRoles { get; set; }
        public List<Form>? Forms { get; set; }
    }
}
