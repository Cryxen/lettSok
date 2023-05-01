using System;
using JobListingsDatabaseService.Test;
using Microsoft.Extensions.Logging;
using UserPreferencesDatabaseService.Controllers.V5;
using UserPreferencesDatabaseService.Model.V3;

namespace UserPreferencesDatabaseService.Test
{
	[Collection("UserPreferenceCollection")]
	public class InterestTest : IDisposable
    {
        private readonly ILogger<V5UserPreferencesDatabaseController>? _logger;

        public InterestTest(DatabaseFixtureTest fixture)
        => Fixture = fixture;

        public DatabaseFixtureTest Fixture { get; }


        [Fact]
        public async void Get_Interests_In_Database()
        {
            //Arrange
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

            public void Dispose()
            => Fixture.Cleanup();
    }
}

