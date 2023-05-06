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
            var mockLogger = new Mock<ILogger<V5UserPreferencesDatabaseController>>();
            var _logger = mockLogger.Object;

            using var context = Fixture.CreateContext();
            var controller = new V5UserPreferencesDatabaseController(_logger, context);

            //Act


            V3Uninterested uninterest = new()
            {
                AdvertisementUuid = "02ceec90-06ab-4222-8f80-9664a58f2a22",
                UserGuid = Guid.NewGuid()
            };

            List<V3Uninterested> expected = new();
            expected.Add(uninterest);

            //Act
            var actual = await controller.getUninterest();

            //Assert
            Assert.Equal(expected.ElementAt(0).AdvertisementUuid, actual.ElementAt(0).AdvertisementUuid);
            //Assert
        }

        [Fact]
        public async void Post_New_Uninterested_Advertisement()
        {
            //Arrange
            var mockLogger = new Mock<ILogger<V5UserPreferencesDatabaseController>>();
            var _logger = mockLogger.Object;

            using var context = Fixture.CreateContext();
            var controller = new V5UserPreferencesDatabaseController(_logger, context);

            //Get a userID
            var users = await controller.Get();

            V3Uninterested expected = new()
            {
                AdvertisementUuid = "02ceec90-06ab-4222-8f80-9664a58f2a22",
                UserGuid = users.ElementAt(1).Id
            };

            int index = 0;

            //Act
            var result = await controller.saveUninterest(expected);

            var actual = await controller.getUninterest();

            foreach (var item in actual)
            {
                if (item.UserGuid == expected.UserGuid && item.AdvertisementUuid == expected.AdvertisementUuid)
                {
                    index = actual.IndexOf(item);
                }
            }

            //Assert
            Assert.True(result.Value.UserGuid.Equals(expected.UserGuid));
            Assert.True(result.Value.AdvertisementUuid.Equals(expected.AdvertisementUuid));
            Assert.Equal(expected.UserGuid, actual.ElementAt(index).UserGuid);
            Assert.Equal(expected.AdvertisementUuid, actual.ElementAt(index).AdvertisementUuid);
        }

        [Fact]
        public async void Delete_Uninterest_From_Database()
        {
            //Arrange
            var mockLogger = new Mock<ILogger<V5UserPreferencesDatabaseController>>();
            var _logger = mockLogger.Object;

            using var context = Fixture.CreateContext();
            var controller = new V5UserPreferencesDatabaseController(_logger, context);

            bool actual = false;

            //Get a userID and save interest to be deleted
            var users = await controller.Get();

            V3Uninterested UninterestToBeSaved = new()
            {
                AdvertisementUuid = "02ceec90-06ab-4222-8f80-9664a58f2a22",
                UserGuid = users.ElementAt(1).Id
            };

            var saveUninterest = await controller.saveUninterest(UninterestToBeSaved);

            //Act
            await controller.deleteUninterest(UninterestToBeSaved.UserGuid, UninterestToBeSaved.AdvertisementUuid);

            //Get Interested advertisements to see if the advertisement has been deleted
            var result = await controller.getUninterest();

            foreach (var item in result)
            {
                if (item.UserGuid == UninterestToBeSaved.UserGuid && item.AdvertisementUuid == UninterestToBeSaved.AdvertisementUuid)
                {
                    actual = true;
                }
            }

            //Assert
            Assert.False(actual);
        }

        public void Dispose()
        => Fixture.Cleanup();
    }
}

