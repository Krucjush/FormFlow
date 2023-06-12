using FormFlow.Models;
using Microsoft.EntityFrameworkCore;

namespace FormFlow.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<Form> Forms { get; set; }
        public DbSet<FormResponse> FormResponses { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<ResponseEntry> ResponseEntries { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
    }
}
