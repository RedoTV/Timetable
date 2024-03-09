using IdentityApi.Models.DbModels;
using IdentityApi.Models.Helpers;
using Microsoft.EntityFrameworkCore;

namespace IdentityApi.Data
{
    public class IdentityDbContext : DbContext
    {
        public IdentityDbContext (DbContextOptions<IdentityDbContext> options) : base (options) 
        {
            Database.EnsureCreated();
        }
        public DbSet<User> Users { get; set; } = null!;
        protected override void OnModelCreating (ModelBuilder model)
        {
            model.Entity<User>().HasData( 
                new User
                {
                    Id = 1,
                    Name = "RedoTV",
                    HashedPassword = "94s8XADhQ+8PtV3uzcUVxaN4bY/btU2WAM7rxaTNXZE=",
                    Role = RolesEnum.Admin
                } 
            );
        }

    }
}