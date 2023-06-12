using FormFlow.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FormFlow.Data
{
    public class AppStoreContext : IdentityDbContext<IdentityUser>
    {
        public AppStoreContext(DbContextOptions<AppStoreContext> options) : base(options)
        {
            
        }
    }
}
