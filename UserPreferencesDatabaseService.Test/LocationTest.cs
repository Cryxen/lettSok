using System;
using JobListingsDatabaseService.Test;
using Microsoft.Extensions.Logging;
using UserPreferencesDatabaseService.Controllers.V5;
using UserPreferencesDatabaseService.Data;
using UserPreferencesDatabaseService.Model.V3;

namespace UserPreferencesDatabaseService.Test
{
    [Collection("UserPreferenceCollection")]
    public class LocationTest : IDisposable
    {
        private readonly ILogger<V5UserPreferencesDatabaseController>? _logger;

        public LocationTest(DatabaseFixtureTest fixture)
        => Fixture = fixture;

        public DatabaseFixtureTest Fixture { get; }

        [Fact]
        public async void Get_Locations_From_Database()
        {
            //Arrange
            using var context = Fixture.CreateContext();
            var controller = new V5UserPreferencesDatabaseController(_logger, context);

            V3Location expected = new()
            {
                Municipality = "Oslo"
            };

            //Act
            var actual = await controller.getLocations();

            //Assert
            Assert.Equal(expected.Municipality, actual.ElementAt(0).Municipality);
        }

        [Fact]
        public async void Get_Search_Locations_From_Database()
        {
            //Arrange
            using var context = Fixture.CreateContext();
            var controller = new V5UserPreferencesDatabaseController(_logger, context);

            SearchLocation expected = new()
            {
                LocationId = 1,
                UserId = Guid.Parse("0da1f25d-9b0b-4b06-8663-08db4a29b2a8")
            };

            //Act
            var actual = await controller.getSearchLocations();

            //Assert
            Assert.Single(actual);
            Assert.Equal(expected.LocationId, actual.ElementAt(0).LocationId);
            Assert.Equal(expected.UserId, actual.ElementAt(0).UserId);

        }

        [Fact]
        public async void Save_Search_Location_To_Database()
        {
            //Arrange
            using var context = Fixture.CreateContext();
            var controller = new V5UserPreferencesDatabaseController(_logger, context);

            bool searchLocationInDatabase = false;

            var LocationList = await controller.getLocations();
            var UserList = await controller.Get();
            V3SearchLocation searchLocationToSave = new()
            {
                LocationId = LocationList.ElementAt(0).Id,
                UserId = UserList.ElementAt(0).Id
            };

            //Act
            var result = await controller.saveSearchLocation(searchLocationToSave);

            var searchLocations = await controller.getSearchLocations();

            foreach (var item in searchLocations)
            {
                if (item.LocationId == searchLocationToSave.LocationId && item.UserId == searchLocationToSave.UserId)
                {
                    searchLocationInDatabase = true;
                }
            }

            //Assert
            Assert.False(result.HasErrors);
            Assert.True(searchLocationInDatabase);
        }

        [Fact]
        public async void Delete_Search_Locations_From_Database()
        {
            //Arrange
            using var context = Fixture.CreateContext();
            var controller = new V5UserPreferencesDatabaseController(_logger, context);

            SearchLocation toBeDeleted = new()
            {
                LocationId = 1,
                UserId = Guid.Parse("0da1f25d-9b0b-4b06-8663-08db4a29b2a8")
            };

            //Act
            await controller.deleteSearchLocation(toBeDeleted.UserId, toBeDeleted.LocationId);
            var actual = await controller.getSearchLocations();

            //Assert
            Assert.Empty(actual);
        }

        [Fact]
        public async void Fetch_Municipalities_From_Public_API()
        {
            //Arrange
            using var context = Fixture.CreateContext();
            var controller = new V5UserPreferencesDatabaseController(_logger, context);

            var DatabaseBeforeFetch = await controller.getLocations();
            bool moreItemsIndatabase = false;

            //Act
            await controller.getMunipalitiesFromPublicAPI();
            var DatabaseAfterFetch = await controller.getLocations();

            if (DatabaseBeforeFetch.Count < DatabaseAfterFetch.Count)
            {
                moreItemsIndatabase = true;
            }

            //Assert
            Assert.True(moreItemsIndatabase);
        }


        public void Dispose()
            => Fixture.Cleanup();
    }
}

