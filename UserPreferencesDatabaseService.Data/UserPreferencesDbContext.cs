using JobListingsDatabaseService.Data;
using Microsoft.EntityFrameworkCore;

namespace UserPreferencesDatabaseService.Data
{

    public class UserPreferencesDbContext : DbContext
    {
        public DbSet<User> users { get; set; }
        public DbSet<InterestedAdvertisement> interestedAdvertisements { get; set; }
        public DbSet<UninterestedAdvertisement> uninterestedAdvertisements { get; set; }

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

            modelBuilder.Entity<InterestedAdvertisement>()
                .HasOne<User>(u => u.User)
                .WithMany(i => i.interestedAdvertisements)
                .HasForeignKey(i => i.UserId);


            modelBuilder.Entity<UninterestedAdvertisement>()
                .HasOne<User>(u => u.User)
                .WithMany(i => i.UninterestedAdvertisements)
                .HasForeignKey(i => i.UserId);

            modelBuilder.Entity<SearchLocation>()
                .HasOne<User>(u => u.User)
                .WithMany(s => s.SearchLocations)
                .HasForeignKey(i => i.UserId);
        }

    }

}