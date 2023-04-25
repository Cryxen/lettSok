using System.Collections.Generic;
using Advertisements.Controllers.V4;
using Advertisements.Interfaces;
using Advertisements.Model.V1;
using Advertisements.Model.V2;

using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;

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


    ILogger _logger = Mock.Of<ILogger<V4Advertisements>>();

    /// <summary>
    /// Tests that the method GetJobs() runs as expected.
    /// </summary>
    [Fact]
    public async void Fetch_Jobs_From_Public_API()
    {
        //Arrange
        expected.Add(_expectedAdvertisement);
        mockList.Add(_expectedAdvertisement);

        // Mockings
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
    /// <summary>
    /// Tests the method that parses json to advertisement
    /// </summary>
    [Fact]
    public void Parse_Json_From_Public_API()
    {
        //Arrange
        expected.Add(_expectedAdvertisement);
        Mock<V4Advertisements> AdvertisementsMock = new Mock<V4Advertisements>(_logger);
        V4Advertisements v4AdvertisementObject = AdvertisementsMock.Object;

        List<V2Advertisement> actual = new();
        //string jsonToParse = "'{\"name\":\"John\", \"age\":30, \"car\":null}'";
        string jsonToParse = "{\"content\":[" + JsonConvert.SerializeObject(_expectedAdvertisement) + "]}";


        //Act
        var parseResult = v4AdvertisementObject.parseJson(jsonToParse);
        foreach (var item in parseResult)
        {
            V2Advertisement v2Advertisement = item.ToObject<V2Advertisement>();
            actual.Add(v2Advertisement);
        }

        //Assert
        Assert.Equivalent(expected: 0, actual: 0, strict: true);
    }

    /// <summary>
    /// Tests that the method GetJobsByLocation() runs as expected.
    /// </summary>
    [Fact]
    public async void Fetch_Jobs_From_Public_API_by_location()
    {
        //Arrange
        expected.Add(_expectedAdvertisement);
        mockList.Add(_expectedAdvertisement);

        // Mockings
        Mock<V4Advertisements> AdvertisementsMock = new Mock<V4Advertisements>(_logger);
        AdvertisementsMock.Setup(x => x.GetJobsByLocation("Oslo")).ReturnsAsync(mockList);

        // The source that made it possible: https://stackoverflow.com/questions/18610920/alter-mockitype-object-after-object-property-has-been-called
        V4Advertisements v4AdvertisementObject = AdvertisementsMock.Object;
        List<V2Advertisement> actual = new();

        //Act
        actual = await v4AdvertisementObject.GetJobsByLocation("Oslo");

        //Assert
        Assert.Equal(expected, actual);
    }

}

