using System.Reflection.Metadata;
using JobListingsDatabaseService.Controllers.V2;
using JobListingsDatabaseService.Data;
using JobListingsDatabaseService.Model.V1;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.EntityFrameworkCore;

namespace JobListingsDatabaseService.Test;

public class JobListingsDatabaseServiceTest : IClassFixture<DatabaseFixtureTest>
{
    private readonly ILogger<V2JobListingsDatabaseController>? _logger;

    public JobListingsDatabaseServiceTest(DatabaseFixtureTest fixture)
    => Fixture = fixture;

    public DatabaseFixtureTest Fixture { get; }

    [Fact]
    public async void Get_Advertisements_From_Database()
    {
        // Arrange
        using var context = Fixture.CreateContext();
        var controller = new V2JobListingsDatabaseController(_logger, context);

        V1Advertisement expected = new()
        {
            Uuid = "02ceec90-06ab-4222-8f80-9664a58f2a22",
            Title = "Utvikling i moderasjon",
            Description = "Vil du jobbe med utvikling i moderasjon? Søk denne jobben fordi beskrivelsen sier det er gøy!",
            Employer = "Samfunnet",
            EngagementType = "Livsstil",
            JobTitle = "Hobby-Utvikler",
            Municipal = "Oslo",
            Expires = DateTime.Today
        };

        // Act
        var advertisements = await controller.Get();

        // Assert details of object
        Assert.Equal(expected.Uuid, advertisements.ElementAt(1).Uuid);
        Assert.Equal(expected.Title, advertisements.ElementAt(1).Title);
        Assert.Equal(expected.Description, advertisements.ElementAt(1).Description);
        Assert.Equal(expected.Employer, advertisements.ElementAt(1).Employer);
        Assert.Equal(expected.JobTitle, advertisements.ElementAt(1).JobTitle);
        Assert.Equal(expected.Municipal, advertisements.ElementAt(1).Municipal);
        Assert.Equal(expected.Expires, advertisements.ElementAt(1).Expires);

        // Assert length of database
        Assert.Equal(2, advertisements.Count);
    }


}
