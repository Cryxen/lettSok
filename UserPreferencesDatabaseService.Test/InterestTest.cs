using System;
using JobListingsDatabaseService.Test;
using Microsoft.Extensions.Logging;
using Moq;
using UserPreferencesDatabaseService.Controllers.V5;
using UserPreferencesDatabaseService.Model.V3;

namespace UserPreferencesDatabaseService.Test
{
	[Collection("UserPreferenceCollection")]
	public class InterestTest : IDisposable
    {
        public InterestTest(DatabaseFixtureTest fixture)
        => Fixture = fixture;

        public DatabaseFixtureTest Fixture { get; }


        [Fact]
        public async void Get_Interests_In_Database()
        {
            //Arrange
            var MockLogger = new Mock<ILogger<V5UserPreferencesDatabaseController>>();
            var Logger = MockLogger.Object;

            using var Context = Fixture.CreateContext();
            var Controller = new V5UserPreferencesDatabaseController(Logger, Context);



            V3Interested Interest = new()
            {
                AdvertisementUuid = "02ceec90-06ab-4222-8f80-9664a58f2a22",
                UserGuid = Guid.NewGuid()
            };

            List<V3Interested> Expected = new();
            Expected.Add(Interest);

            //Act
            var Actual = await Controller.GetInterest();

            //Assert
            Assert.Equal(Expected.ElementAt(0).AdvertisementUuid, Actual.ElementAt(0).AdvertisementUuid);
        }

        [Fact]
        public async void Post_New_Interested_Advertisement()
        {
            //Arrange
            var MockLogger = new Mock<ILogger<V5UserPreferencesDatabaseController>>();
            var Logger = MockLogger.Object;

            using var Context = Fixture.CreateContext();
            var Controller = new V5UserPreferencesDatabaseController(Logger, Context);

            //Get a userID
            var Users = await Controller.Get();

            V3Interested Expected = new()
            {
                AdvertisementUuid = "02ceec90-06ab-4222-8f80-9664a58f2a22",
                UserGuid = Users.ElementAt(1).Id
            };

            int Index = 0;

            //Act
            var Result = await Controller.SaveInterest(Expected);

            var Actual = await Controller.GetInterest();

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
        public async void Delete_Interest_From_Database()
        {
            //Arrange
            var MockLogger = new Mock<ILogger<V5UserPreferencesDatabaseController>>();
            var Logger = MockLogger.Object;

            using var Context = Fixture.CreateContext();
            var Controller = new V5UserPreferencesDatabaseController(Logger, Context);

            bool Actual = false;

                //Get a userID and save interest to be deleted
            var Users = await Controller.Get();

            V3Interested InterestToBeSaved = new()
            {
                AdvertisementUuid = "02ceec90-06ab-4222-8f80-9664a58f2a22",
                UserGuid = Users.ElementAt(1).Id
            };

            var SaveInterest = await Controller.SaveInterest(InterestToBeSaved);

            //Act
            await Controller.DeleteInterest(InterestToBeSaved.UserGuid, InterestToBeSaved.AdvertisementUuid);

            //Get Interested advertisements to see if the advertisement has been deleted
            var Result = await Controller.GetInterest();

            foreach (var Item in Result)
            {
                if (Item.UserGuid == InterestToBeSaved.UserGuid && Item.AdvertisementUuid == InterestToBeSaved.AdvertisementUuid)
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

