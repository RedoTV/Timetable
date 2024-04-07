using Microsoft.EntityFrameworkCore;
using TimetableServer.Models.DbModels;

namespace TimetableServer.Database
{
    public class TimetableDBContext : DbContext
    {
        public TimetableDBContext(DbContextOptions optionsBuilder) : base(optionsBuilder)
        {
            // Database.EnsureDeleted();
            Database.EnsureCreated();
        }
        
        public DbSet<Faculty> Faculties {get;set;}
        public DbSet<Semester> Semesters {get;set;}
        public DbSet<Group> Groups {get;set;}
        public DbSet<Week> Weeks {get;set;}
        public DbSet<Day> Days {get;set;}
        public DbSet<Lesson> Lessons {get;set;}
        public DbSet<Teacher> Teachers {get;set;}
        public DbSet<User> Users {get;set;}
    }
}