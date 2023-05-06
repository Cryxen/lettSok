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
            var mockLogger = new Mock<ILogger<V5UserPreferencesDatabaseController>>();
            var _logger = mockLogger.Object;

            using var context = Fixture.CreateContext();
            var controller = new V5UserPreferencesDatabaseController(_logger, context);



            V3Interested interest = new()
            {
                AdvertisementUuid = "02ceec90-06ab-4222-8f80-9664a58f2a22",
                UserGuid = Guid.NewGuid()
            };

            List<V3Interested> expected = new();
            expected.Add(interest);

            //Act
            var actual = await controller.getInterest();

            //Assert
            Assert.Equal(expected.ElementAt(0).AdvertisementUuid, actual.ElementAt(0).AdvertisementUuid);
        }

        [Fact]
        public async void Post_New_Interested_Advertisement()
        {
            //Arrange
            var mockLogger = new Mock<ILogger<V5UserPreferencesDatabaseController>>();
            var _logger = mockLogger.Object;

            using var context = Fixture.CreateContext();
            var controller = new V5UserPreferencesDatabaseController(_logger, context);

            //Get a userID
            var users = await controller.Get();

            V3Interested expected = new()
            {
                AdvertisementUuid = "02ceec90-06ab-4222-8f80-9664a58f2a22",
                UserGuid = users.ElementAt(1).Id
            };

            int index = 0;

            //Act
            var result = await controller.saveInterest(expected);

            var actual = await controller.getInterest();

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
        public async void Delete_Interest_From_Database()
        {
            //Arrange
            var mockLogger = new Mock<ILogger<V5UserPreferencesDatabaseController>>();
            var _logger = mockLogger.Object;

            using var context = Fixture.CreateContext();
            var controller = new V5UserPreferencesDatabaseController(_logger, context);

            bool actual = false;

                //Get a userID and save interest to be deleted
            var users = await controller.Get();

            V3Interested InterestToBeSaved = new()
            {
                AdvertisementUuid = "02ceec90-06ab-4222-8f80-9664a58f2a22",
                UserGuid = users.ElementAt(1).Id
            };

            var saveInterest = await controller.saveInterest(InterestToBeSaved);

            //Act
            await controller.DeleteInterest(InterestToBeSaved.UserGuid, InterestToBeSaved.AdvertisementUuid);

            //Get Interested advertisements to see if the advertisement has been deleted
            var result = await controller.getInterest();

            foreach (var item in result)
            {
                if (item.UserGuid == InterestToBeSaved.UserGuid && item.AdvertisementUuid == InterestToBeSaved.AdvertisementUuid)
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

