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
        public ICollection<UserRole> UserRoles { get; set; }
        public List<Form>? Forms { get; set; }
    }
}
