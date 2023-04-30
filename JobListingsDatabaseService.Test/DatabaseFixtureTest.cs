using System;
using JobListingsDatabaseService.Data;
using Microsoft.EntityFrameworkCore;

namespace JobListingsDatabaseService.Test
{
	public class DatabaseFixtureTest
	{
        private const string ConnectionString = "server=localhost;Database=DotnetDatabase;user=dbuser;password=password";

        private static readonly object _lock = new();
        private static bool _databaseInitialized;

        public DatabaseFixtureTest()
        {
            // https://learn.microsoft.com/en-us/ef/core/testing/testing-with-the-database Using fixture.


            Advertisement advertisement1 = new()
            {
                Uuid = "02ceec90-06ab-4222-8f80-9664a58f2a22",
                Title = "Utvikling i moderasjon",
                Description = "Vil du jobbe med utvikling i moderasjon? Søk denne jobben fordi beskrivelsen sier det er gøy!",
                Employer = "Samfunnet",
                EngagementType = "Livsstil",
                JobTitle = "Hobby-Utvikler",
                Municipal = "Oslo",
                Expires = DateTime.Today
            };

            Advertisement advertisement2 = new()
            {
                Uuid = "02ceec90-06ab-4222-8f80-9664a58f2a21",
                Title = "Alltid utvikling",
                Description = "Vil du jobbe med utvikling? Søk denne jobben fordi beskrivelsen sier det er gøy, og du vil aldri gjøre noe annet!",
                Employer = "Samfunnet",
                EngagementType = "Hele livet",
                JobTitle = "Utvikler",
                Municipal = "Oslo",
                Expires = DateTime.Today
            };

            lock (_lock)
            {
                if (!_databaseInitialized)
                {
                    using (var context = CreateContext())
                    {
                        context.Database.EnsureDeleted();
                        context.Database.EnsureCreated();
                        context.AddRange(
                            advertisement1,
                            advertisement2
                            );
                        context.SaveChanges();
                    }

                    _databaseInitialized = true;
                }
            }
        }

        public JobListingsDbContext CreateContext()
            => new JobListingsDbContext(
                new DbContextOptionsBuilder<JobListingsDbContext>()
                    .UseMySQL("server=localhost;Database=AdvertisementsFixtureTest;user=dbuser;password=password") // TODO: Finn ut hvordan denne kan peke til .json
                    .Options);
    }
}

