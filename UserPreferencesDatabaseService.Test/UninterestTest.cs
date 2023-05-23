using System;
using JobListingsDatabaseService.Test;
using Microsoft.Extensions.Logging;
using Moq;
using UserPreferencesDatabaseService.Controllers.V5;
using UserPreferencesDatabaseService.Model.V3;

namespace UserPreferencesDatabaseService.Test
{
    [Collection("UserPreferenceCollection")]
    public class UninterestTest : IDisposable
	{
        public UninterestTest(DatabaseFixtureTest fixture)
        => Fixture = fixture;

        public DatabaseFixtureTest Fixture { get; }

        [Fact]
        public async void Get_Uninterests_In_Database()
        {
            //Arrange
            var MockLogger = new Mock<ILogger<V5UserPreferencesDatabaseController>>();
            var Logger = MockLogger.Object;

            using var Context = Fixture.CreateContext();
            var Controller = new V5UserPreferencesDatabaseController(Logger, Context);

            //Act


            V3Uninterested Uninterest = new()
            {
                AdvertisementUuid = "02ceec90-06ab-4222-8f80-9664a58f2a22",
                UserGuid = Guid.NewGuid()
            };

            List<V3Uninterested> Expected = new();
            Expected.Add(Uninterest);

            //Act
            var Actual = await Controller.GetUninterest();

            //Assert
            Assert.Equal(Expected.ElementAt(0).AdvertisementUuid, Actual.ElementAt(0).AdvertisementUuid);
            //Assert
        }

        [Fact]
        public async void Post_New_Uninterested_Advertisement()
        {
            //Arrange
            var MockLogger = new Mock<ILogger<V5UserPreferencesDatabaseController>>();
            var Logger = MockLogger.Object;

            using var Context = Fixture.CreateContext();
            var Controller = new V5UserPreferencesDatabaseController(Logger, Context);

            //Get a userID
            var Users = await Controller.Get();

            V3Uninterested Expected = new()
            {
                AdvertisementUuid = "02ceec90-06ab-4222-8f80-9664a58f2a22",
                UserGuid = Users.ElementAt(1).Id
            };

            int Index = 0;

            //Act
            var Result = await Controller.SaveUninterest(Expected);

            var Actual = await Controller.GetUninterest();

            foreach (var Item in Actual)
            {
                if (Item.UserGuid == Expected.UserGuid && Item.AdvertisementUuid == Expected.AdvertisementUuid)
                {
                    Index = Actual.IndexOf(Item);
                }
            }

            //Assert
            Assert.True(Result.Value.UserGuid.Equals(Expected.UserGuid));
            Assert.True(Result.Value.AdvertisementUuid.Equals(Expected.AdvertisementUuid));
            Assert.Equal(Expected.UserGuid, Actual.ElementAt(Index).UserGuid);
            Assert.Equal(Expected.AdvertisementUuid, Actual.ElementAt(Index).AdvertisementUuid);
        }

        [Fact]
        public async void Delete_Uninterest_From_Database()
        {
            //Arrange
            var MockLogger = new Mock<ILogger<V5UserPreferencesDatabaseController>>();
            var Logger = MockLogger.Object;

            using var Context = Fixture.CreateContext();
            var Controller = new V5UserPreferencesDatabaseController(Logger, Context);

            bool Actual = false;

            //Get a userID and save interest to be deleted
            var Users = await Controller.Get();

            V3Uninterested UninterestToBeSaved = new()
            {
                AdvertisementUuid = "02ceec90-06ab-4222-8f80-9664a58f2a22",
                UserGuid = Users.ElementAt(1).Id
            };

            var SaveUninterest = await Controller.SaveUninterest(UninterestToBeSaved);

            //Act
            await Controller.DeleteUninterest(UninterestToBeSaved.UserGuid, UninterestToBeSaved.AdvertisementUuid);

            //Get Interested advertisements to see if the advertisement has been deleted
            var Result = await Controller.GetUninterest();

            foreach (var Item in Result)
            {
                if (Item.UserGuid == UninterestToBeSaved.UserGuid && Item.AdvertisementUuid == UninterestToBeSaved.AdvertisementUuid)
                {
                    Actual = true;
                }
            }

            //Assert
            Assert.False(Actual);
        }

        public void Dispose()
        => Fixture.Cleanup();
    }
}

