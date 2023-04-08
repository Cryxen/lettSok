using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace JobListingsDatabaseService.Data
{
	public class JobListingsDbContext : DbContext
	{
        /*
             * Source:
			 * https://learn.microsoft.com/en-us/ef/ef6/modeling/code-first/workflows/new-database 
			 */
		public DbSet<Advertisement> advertisements { get; set; }

        public JobListingsDbContext()
        {

        }

        public JobListingsDbContext(DbContextOptions options)
            : base(options)
        {

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

      
       

          

        }

        

    }
}

