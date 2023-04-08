using JobListingsDatabaseService.Data;
using Microsoft.EntityFrameworkCore;

namespace UserPreferencesDatabaseService.Data
{

    public class UserPreferencesDbContext : DbContext
    {
        public DbSet<User> users { get; set; }

        public UserPreferencesDbContext()
        {

        }

        public UserPreferencesDbContext(DbContextOptions options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(mb =>
            {
                mb.ToTable("User");
                mb.Property(User => User.Id);
                mb.Property(User => User.Name);

                mb.HasKey(User => User.Id);
            });

            // * Source: https://www.learnentityframeworkcore.com/configuration/many-to-many-relationship-configuration

            modelBuilder.Entity<UninterestedAdvertisement>()
            .HasKey(ia => new { ia.UserGuid, ia.AdvertisementUuid });
            modelBuilder.Entity<UninterestedAdvertisement>()
                .HasOne(ia => ia.User)
                .WithMany(u => u.uninterestedAdvertisements)
                .HasForeignKey(ia => ia.UserGuid);

            modelBuilder.Entity<InterestedAdvertisement>()
       .HasKey(ia => new { ia.UserGuid });
            modelBuilder.Entity<InterestedAdvertisement>()
                .HasOne(ia => ia.User)
                .WithMany(u => u.interestedAdvertisements)
                .HasForeignKey(ia => ia.UserGuid);
        }


    }

}