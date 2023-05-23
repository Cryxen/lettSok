using System;
using JobListingsDatabaseService.Test;
using Microsoft.Extensions.Logging;
using Moq;
using UserPreferencesDatabaseService.Controllers.V5;
using UserPreferencesDatabaseService.Data;
using UserPreferencesDatabaseService.Model.V3;

namespace UserPreferencesDatabaseService.Test
{
    [Collection("UserPreferenceCollection")]
    public class LocationTest : IDisposable
    {
        public LocationTest(DatabaseFixtureTest fixture)
        => Fixture = fixture;

        public DatabaseFixtureTest Fixture { get; }

        [Fact]
        public async void Get_Locations_From_Database()
        {
            //Arrange
            var MockLogger = new Mock<ILogger<V5UserPreferencesDatabaseController>>();
            var Logger = MockLogger.Object;

            using var Context = Fixture.CreateContext();
            var Controller = new V5UserPreferencesDatabaseController(Logger, Context);

            V3Location Expected = new()
            {
                Municipality = "Oslo"
            };

            //Act
            var Actual = await Controller.GetLocations();

            //Assert
            Assert.Equal(Expected.Municipality, Actual.ElementAt(0).Municipality);
        }

        [Fact]
        public async void Get_Search_Locations_From_Database()
        {
            //Arrange
            var MockLogger = new Mock<ILogger<V5UserPreferencesDatabaseController>>();
            var Logger = MockLogger.Object;

            using var Context = Fixture.CreateContext();
            var Controller = new V5UserPreferencesDatabaseController(Logger, Context);

            SearchLocation Expected = new()
            {
                LocationId = 1,
                UserId = Guid.Parse("0da1f25d-9b0b-4b06-8663-08db4a29b2a8")
            };

            //Act
            var Actual = await Controller.GetSearchLocations();

            //Assert
            Assert.Single(Actual);
            Assert.Equal(Expected.LocationId, Actual.ElementAt(0).LocationId);
            Assert.Equal(Expected.UserId, Actual.ElementAt(0).UserId);

        }

        [Fact]
        public async void Save_Search_Location_To_Database()
        {
            //Arrange
            var MockLogger = new Mock<ILogger<V5UserPreferencesDatabaseController>>();
            var Logger = MockLogger.Object;

            using var Context = Fixture.CreateContext();
            var Controller = new V5UserPreferencesDatabaseController(Logger, Context);

            bool SearchLocationInDatabase = false;

            var LocationList = await Controller.GetLocations();
            var UserList = await Controller.Get();
            V3SearchLocation SearchLocationToSave = new()
            {
                LocationId = LocationList.ElementAt(0).Id,
                UserId = UserList.ElementAt(0).Id
            };

            //Act
            var Result = await Controller.SaveSearchLocation(SearchLocationToSave);

            var SearchLocations = await Controller.GetSearchLocations();

            foreach (var Item in SearchLocations)
            {
                if (Item.LocationId == SearchLocationToSave.LocationId && Item.UserId == SearchLocationToSave.UserId)
                {
                    SearchLocationInDatabase = true;
                }
            }

            //Assert
            Assert.False(Result.HasErrors);
            Assert.True(SearchLocationInDatabase);
        }

        [Fact]
        public async void Delete_Search_Locations_From_Database()
        {
            //Arrange
            var MockLogger = new Mock<ILogger<V5UserPreferencesDatabaseController>>();
            var Logger = MockLogger.Object;

            using var Context = Fixture.CreateContext();
            var Controller = new V5UserPreferencesDatabaseController(Logger, Context);

            SearchLocation ToBeDeleted = new()
            {
                LocationId = 1,
                UserId = Guid.Parse("0da1f25d-9b0b-4b06-8663-08db4a29b2a8")
            };

            //Act
            await Controller.DeleteSearchLocation(ToBeDeleted.UserId, ToBeDeleted.LocationId);
            var Actual = await Controller.GetSearchLocations();

            //Assert
            Assert.Empty(Actual);
        }

        [Fact]
        public async void Fetch_Municipalities_From_Public_API()
        {
            //Arrange
            var MockLogger = new Mock<ILogger<V5UserPreferencesDatabaseController>>();
            var Logger = MockLogger.Object;

            using var Context = Fixture.CreateContext();
            var Controller = new V5UserPreferencesDatabaseController(Logger, Context);

            var DatabaseBeforeFetch = await Controller.GetLocations();
            bool MoreItemsIndatabase = false;

            //Act
            await Controller.getMunipalitiesFromPublicAPI();
            var DatabaseAfterFetch = await Controller.GetLocations();

            if (DatabaseBeforeFetch.Count < DatabaseAfterFetch.Count)
            {
                MoreItemsIndatabase = true;
            }

            //Assert
            Assert.True(MoreItemsIndatabase);
        }


        public void Dispose()
            => Fixture.Cleanup();
    }
}

