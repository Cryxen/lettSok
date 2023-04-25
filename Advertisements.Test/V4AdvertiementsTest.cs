using System.Collections.Generic;
using Advertisements.Controllers.V4;
using Advertisements.Interfaces;
using Advertisements.Model.V1;
using Advertisements.Model.V2;

using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;

namespace Advertisements.Test;

// https://learn.microsoft.com/en-us/aspnet/web-api/overview/testing-and-debugging/unit-testing-controllers-in-web-api

public class V4AdvertisementsTest 
{

    // Set up advertisements lists to be used for checking
    private V2Advertisement _expectedAdvertisement = new V2Advertisement
    {
        Uuid = "Uuid test",
        Expires = DateTime.Today,
        Title = "Test Advertisement",
        Description = "Description for a test advertisement",
        JobTitle = "Software tester",
        EngagementType = "Always"
    };

    List<V2Advertisement> expected = new();
    List<V2Advertisement> mockList = new();
    


    /// <summary>
    /// Tests that the method GetJobs() retrieves jobs from public API and parses the string to an object list containing the jobs
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task FetchJobsFromPublicAPI()
    {
        //Arrange
        expected.Add(_expectedAdvertisement);
        mockList.Add(_expectedAdvertisement);

        // Mockings
        var _logger = Mock.Of<ILogger<V4Advertisements>>();
        Mock<V4Advertisements> AdvertisementsMock = new Mock<V4Advertisements>(_logger);
        AdvertisementsMock.Setup(x => x.GetJobs()).ReturnsAsync(mockList);

        // The source that made it possible: https://stackoverflow.com/questions/18610920/alter-mockitype-object-after-object-property-has-been-called
        V4Advertisements v4AdvertisementObject = AdvertisementsMock.Object;
        List<V2Advertisement> actual = new();

        //Act
        actual = await v4AdvertisementObject.GetJobs();

        //Assert
        Assert.Equal(expected, actual);
    }


}

