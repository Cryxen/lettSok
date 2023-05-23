using System;
using System.Reflection.Metadata;
using Microsoft.EntityFrameworkCore;
using UserPreferencesDatabaseService.Data;

namespace JobListingsDatabaseService.Test
{
    // Using fixture https://learn.microsoft.com/en-us/ef/core/testing/testing-with-the-database
    public class DatabaseFixtureTest 
	{
        private const string _connectionString = "server=localhost;Database=DotnetDatabase;user=dbuser;password=password";

        private static bool s_databaseInitialized;



        User User1 = new()
        {
            Name = "Test bruker 1",
            Id = Guid.Parse("0da1f25d-9b0b-4b06-8663-08db4a29b2a8") // Making a static GUID to be used as foreign key in db.
        };
        User User2 = new()
        {
            Name = "Test bruker 1",
            Id = Guid.NewGuid()
        };

        InterestedAdvertisement Interest = new()
        {
            AdvertisementUuid = "02ceec90-06ab-4222-8f80-9664a58f2a22",
            UserId = Guid.Parse("0da1f25d-9b0b-4b06-8663-08db4a29b2a8")
        };

        UninterestedAdvertisement Uninterest = new()
        {
            AdvertisementUuid = "02ceec90-06ab-4222-8f80-9664a58f2a22",
            UserId = Guid.Parse("0da1f25d-9b0b-4b06-8663-08db4a29b2a8")
        };

        Location Location = new()
        {
            Id = 1,
            Municipality = "Oslo"
        };

        SearchLocation SearchLocation = new()
        {
            LocationId = 1,
            UserId = Guid.Parse("0da1f25d-9b0b-4b06-8663-08db4a29b2a8")
        };

        public UserPreferencesDbContext CreateContext()
            => new UserPreferencesDbContext(
                new DbContextOptionsBuilder<UserPreferencesDbContext>()
                    .UseMySQL("server=localhost;Database=UserPreferencesDatabaseFixtureTest;user=dbuser;password=password") // TODO: Finn ut hvordan denne kan peke til .json
                    .Options);

        public DatabaseFixtureTest()
        {
            Guid _id = Guid.NewGuid();

            using var Context = CreateContext();
            Context.Database.EnsureDeleted();
            Context.Database.EnsureCreated();

            Cleanup();
        }
        
        public void Cleanup()
        {
            using var Context = CreateContext();

            Context.users.RemoveRange(Context.users);
            Context.interestedAdvertisements.RemoveRange(Context.interestedAdvertisements);
            Context.uninterestedAdvertisements.RemoveRange(Context.uninterestedAdvertisements);
            Context.searchLocations.RemoveRange(Context.searchLocations);
            Context.locations.RemoveRange(Context.locations);
            Context.AddRange(
                    User1,
                    User2,
                    Interest,
                    Uninterest,
                    Location,
                    SearchLocation
                );
            Context.SaveChanges();
        }


    }

    [CollectionDefinition("UserPreferenceCollection")]
    public class TransactionalTestsCollection : ICollectionFixture<DatabaseFixtureTest>
    {
        // TO BE EMPTY. Used to apply Collection definition
        // See https://xunit.net/docs/shared-context for more info.
    }
}

