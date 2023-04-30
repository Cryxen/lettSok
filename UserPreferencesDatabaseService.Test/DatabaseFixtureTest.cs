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
            Id = Guid.NewGuid()
        };
        User user2 = new()
        {
            Name = "Test bruker 1",
            Id = Guid.NewGuid()
        };

        public UserPreferencesDbContext CreateContext()
            => new UserPreferencesDbContext(
                new DbContextOptionsBuilder<UserPreferencesDbContext>()
                    .UseMySQL("server=localhost;Database=UserPreferencesDatabaseFixtureTest;user=dbuser;password=password") // TODO: Finn ut hvordan denne kan peke til .json
                    .Options);

        public DatabaseFixtureTest()
        {
            using var context = CreateContext();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            Cleanup();
        }
        
        public void Cleanup()
        {
            using var context = CreateContext();

            context.users.RemoveRange(context.users);
            context.AddRange(
                    user1,
                    user2
                );
            context.SaveChanges();
        }


    }
}

