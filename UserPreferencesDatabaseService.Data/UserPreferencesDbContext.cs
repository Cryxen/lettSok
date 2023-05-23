using Microsoft.EntityFrameworkCore;

namespace UserPreferencesDatabaseService.Data
{

    public class UserPreferencesDbContext : DbContext
    {
        public DbSet<User> users { get; set; }
        public DbSet<InterestedAdvertisement> InterestedAdvertisements { get; set; }
        public DbSet<UninterestedAdvertisement> UninterestedAdvertisements { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<SearchLocation> SearchLocations { get; set; }

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
                .WithMany(i => i.InterestedAdvertisements)
                .HasForeignKey(i => i.UserId);


            modelBuilder.Entity<UninterestedAdvertisement>()
                .HasOne<User>(u => u.User)
                .WithMany(i => i.UninterestedAdvertisements)
                .HasForeignKey(i => i.UserId);

            modelBuilder.Entity<SearchLocation>()
            .HasKey(bc => new { bc.Id });
            modelBuilder.Entity<SearchLocation>()
                .HasOne(bc => bc.Location)
                .WithMany(b => b.SearchLocations)
                .HasForeignKey(bc => bc.LocationId);
            modelBuilder.Entity<SearchLocation>()
                .HasOne(bc => bc.User)
                .WithMany(c => c.SearchLocations)
                .HasForeignKey(bc => bc.UserId);
        }

    }

}