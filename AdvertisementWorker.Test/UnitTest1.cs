using AdvertisementWorker.Model;
using Microsoft.Extensions.Logging;

namespace AdvertisementWorker.Test;

public class UnitTest1
{
    // Set up advertisement used for checking

    private readonly ILogger<Worker> _logger;

    private Advertisement advertisement = new()
    {
        Uuid = "Uuid test",
        Expires = DateTime.Today,
        Title = "Test Advertisement",
        Description = "Description for a test advertisement",
        JobTitle = "Software tester",
        EngagementType = "Always"
    };

    [Fact]
    public async void Public_API_Responds_With_JSON()
    {
        // Arrange
        Worker worker = new Worker(_logger);

        // Act
        var result = await worker.RetrieveFromPublicAPI();

        //Assert
        Assert.EndsWith("}", result);
    }

    [Fact]
    public async void Fetch_Jobs_From_Public_API_Based_On_Location()
    {
        // Arrange
        Worker worker = new Worker(_logger);

        // Act
        var JobListings = await worker.FetchJobsAndParseFromPublicAPI("Vestby");

        // Assert
        foreach (var listing in JobListings)
        {
            if (listing.WorkLocations.ElementAt(0).municipal.Equals("VESTBY"))
            {
                Assert.True(true);
            }
            else
            {
                Assert.True(false);
            }
        }
    }

    [Fact]
    public async void Fetch_Jobs_From_Public_API_Without_Location()
    {
        // Arrange
        Worker worker = new Worker(_logger);

        // Act
        var JobListings = await worker.FetchJobsAndParseFromPublicAPI();

        // Assert
        Assert.True(JobListings.Count > 0);
    }
}
