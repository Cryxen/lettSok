using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace JobListingsDatabaseService.Data
{
	public class AdvertisementDbContext : DbContext
	{
        /*
             * Source:
			 * https://learn.microsoft.com/en-us/ef/ef6/modeling/code-first/workflows/new-database 
			 */
		public DbSet<Advertisement> advertisements { get; set; }
        public DbSet<User> users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //Using: https://dev.mysql.com/doc/connector-net/en/connector-net-entityframework-core.html
            optionsBuilder.UseMySQL("server=localhost;Database=DotnetDatabase;user=dbuser;password=password");


            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
       

            modelBuilder.Entity<Advertisement>(mb =>
            {
                mb.ToTable("Advertisement");

                mb.Property(Advertisement => Advertisement.Uuid);
                mb.Property(Advertisement => Advertisement.Expires);
                mb.Property(Advertisement => Advertisement.Municipal);
                mb.Property(Advertisement => Advertisement.Title);
                mb.Property(Advertisement => Advertisement.Description);
                mb.Property(Advertisement => Advertisement.JobTitle);
                mb.Property(Advertisement => Advertisement.Employer);
                mb.Property(Advertisement => Advertisement.EngagementType);
                
                mb.HasKey(Advertisement => Advertisement.Uuid);



            });

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

