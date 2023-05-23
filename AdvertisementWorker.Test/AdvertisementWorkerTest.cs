using AdvertisementWorker.Model;
using Microsoft.Extensions.Logging;

namespace AdvertisementWorker.Test;

public class AdvertisementWorkerTest
{
    // Set up advertisement used for checking

    private readonly ILogger<Worker> _logger;

    private Advertisement _advertisement = new()
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
        Worker Worker = new Worker(_logger);

        // Act
        var Result = await Worker.RetrieveFromPublicAPI();

        //Assert
        Assert.EndsWith("}", Result);
    }

    [Fact]
    public async void Fetch_Jobs_From_Public_API_Based_On_Location()
    {
        // Arrange
        Worker Worker = new Worker(_logger);

        // Act
        var JobListings = await Worker.FetchJobsAndParseFromPublicAPI("Vestby");

        // Assert
        foreach (var Listing in JobListings)
        {
            bool WorkLocation = false;
            foreach (var Item in Listing.WorkLocations)
            {
                if (Item.Municipal is not null && Item.Municipal.Equals("VESTBY"))
                {
                    WorkLocation = true;
                }
            }
                Assert.True(WorkLocation);
            
           

        }
    }

    [Fact]
    public async void Fetch_Jobs_From_Public_API_Without_Location()
    {
        // Arrange
        Worker Worker = new Worker(_logger);

        // Act
        var JobListings = await Worker.FetchJobsAndParseFromPublicAPI();

        // Assert
        Assert.True(JobListings.Count > 0);
    }
}
