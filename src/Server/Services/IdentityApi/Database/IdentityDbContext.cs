using IdentityApi.Models.DbModels;
using IdentityApi.Models.Helpers;
using Microsoft.EntityFrameworkCore;

namespace IdentityApi.Database
{
    public class IdentityDbContext : DbContext
    {
        public IdentityDbContext (DbContextOptions<IdentityDbContext> options) : base (options) 
        {
            Database.EnsureCreated();
        }
        public DbSet<User> Users { get; set; } = null!;
    }
}