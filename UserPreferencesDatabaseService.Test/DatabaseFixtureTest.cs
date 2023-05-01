using System;
using System.Reflection.Metadata;
using Microsoft.EntityFrameworkCore;
using UserPreferencesDatabaseService.Data;

namespace JobListingsDatabaseService.Test
{
    // Using fixture https://learn.microsoft.com/en-us/ef/core/testing/testing-with-the-database
    public class DatabaseFixtureTest 
	{
        private const string ConnectionString = "server=localhost;Database=DotnetDatabase;user=dbuser;password=password";

        private static bool _databaseInitialized;



        User user1 = new()
        {
            Name = "Test bruker 1",
            Id = Guid.Parse("0da1f25d-9b0b-4b06-8663-08db4a29b2a8") // Making a static GUID to be used as foreign key in db.
        };
        User user2 = new()
        {
            Name = "Test bruker 1",
            Id = Guid.NewGuid()
        };

        InterestedAdvertisement interest = new()
        {
            AdvertisementUuid = "02ceec90-06ab-4222-8f80-9664a58f2a22",
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

            using var context = CreateContext();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            Cleanup();
        }
        
        public void Cleanup()
        {
            using var context = CreateContext();

            context.users.RemoveRange(context.users);
            context.interestedAdvertisements.RemoveRange(context.interestedAdvertisements);
            context.AddRange(
                    user1,
                    user2,
                    interest
                );
            context.SaveChanges();
        }


    }

    [CollectionDefinition("UserPreferenceCollection")]
    public class TransactionalTestsCollection : ICollectionFixture<DatabaseFixtureTest>
    {
        // TO BE EMPTY. Used to apply Collection definition
        // See https://xunit.net/docs/shared-context for more info.
    }
}

