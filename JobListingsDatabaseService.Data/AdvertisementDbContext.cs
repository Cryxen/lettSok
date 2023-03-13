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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //root@127.0.0.1:3306
            //optionsBuilder.UseSqlServer("Server=127.0.0.1, 3306; Database=DotnetDatabase;Uid=root;Pwd=sejguf-ziBky4-xexfon;");
            //Using: https://dev.mysql.com/doc/connector-net/en/connector-net-entityframework-core.html
            optionsBuilder.UseMySQL("server=localhost;Database=DotnetDatabase;user=root;password=ejguf-ziBky4-xexfon");


            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        { 

            modelBuilder.Entity<Advertisement>(mb =>
            {
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
        }
    }
}

