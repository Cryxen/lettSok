using System;
using Microsoft.EntityFrameworkCore;

namespace UserPreferencesDatabaseService.Data
{
	public class UserDbContext : DbContext
	{
		public DbSet<User> users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL("server=localhost;Database=DotnetDatabase;user=dbuser;password=password");
            base.OnConfiguring(optionsBuilder);

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(mb =>
            {
                mb.ToTable("User");
                mb.Property(User => User.Id);
                mb.Property(User => User.Name);
                mb.Property(User => User.Interested);
                mb.Property(User => User.Uninterested);

                mb.HasKey(User => User.Id);
            });
        }
    }
}

