using System.Reflection.Metadata;
using Castle.Core.Logging;
using JobListingsDatabaseService.Controllers.V2;
using JobListingsDatabaseService.Data;
using JobListingsDatabaseService.Model.V1;
using JobListingsDatabaseService.Model.V2;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.EntityFrameworkCore;

namespace JobListingsDatabaseService.Test;

public class JobListingsDatabaseServiceTest : IClassFixture<DatabaseFixtureTest>
{
    public JobListingsDatabaseServiceTest(DatabaseFixtureTest fixture)
    => Fixture = fixture;

    public DatabaseFixtureTest Fixture { get; }

    [Fact]
    public async void Get_Advertisements_From_Database()
    {
        // Arrange
        var MockLogger = new Mock<ILogger<V2JobListingsDatabaseController>>();
        var Logger = MockLogger.Object;

        using var Context = Fixture.CreateContext();
        var Controller = new V2JobListingsDatabaseController(Logger, Context);

        V1Advertisement Expected = new()
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
        var Actual = await Controller.Get();

        // Assert details of object
        Assert.Equal(Expected.Uuid, Actual.ElementAt(1).Uuid);
        Assert.Equal(Expected.Title, Actual.ElementAt(1).Title);
        Assert.Equal(Expected.Description, Actual.ElementAt(1).Description);
        Assert.Equal(Expected.Employer, Actual.ElementAt(1).Employer);
        Assert.Equal(Expected.JobTitle, Actual.ElementAt(1).JobTitle);
        Assert.Equal(Expected.Municipal, Actual.ElementAt(1).Municipal);
        Assert.Equal(Expected.Expires, Actual.ElementAt(1).Expires);

        Assert.Equal(2, Actual.Count);


        // Cleanup
        Fixture.Cleanup();
    }

    [Fact]
    public async void Post_Advertisement_To_database()
    {
        // Arrange
        var MockLogger = new Mock<ILogger<V2JobListingsDatabaseController>>();
        var Logger = MockLogger.Object;
        using var Context = Fixture.CreateContext();
        var Controller = new V2JobListingsDatabaseController(Logger, Context);

        // Object to be added
        V2Employer Employer = new()
        {
            Name = "Deg selv"
        };

        V2WorkLocation WorkLocation = new()
        {
            Municipal = "Oslo"
        };

        List<V2WorkLocation> WorkLocations = new();
        WorkLocations.Add(WorkLocation);

        V2Advertisement Expected = new()
        {
            Uuid = "02ceec90-06ab-4222-8f80-9664a58f2a29",
            Title = "Utvikler som hobby",
            Description = "Vil du jobbe med Utvikler ved siden av det du gjør til daglig? Søk deg inn på denne stillingen!",
            Employer = Employer,
            EngagementType = "Deltid",
            JobTitle = "Ingen spesiell tittel",
            WorkLocations = WorkLocations,
            Expires = DateTime.Today
        };

        //Act
        var SaveToDatabse = await Controller.SaveAdvertisements(Expected);

        // Find the object in database
        var Actual = await Controller.Get();
        int Index = 0;
        foreach (var Item in Actual)
        {
            if(Item.Uuid == Expected.Uuid)
            {
                Index = Actual.IndexOf(Item);
            }
        }

        //Assert
        Assert.Equal(Expected.Uuid, Actual.ElementAt(Index).Uuid);
        Assert.Equal(Expected.Title, Actual.ElementAt(Index).Title);
        Assert.Equal(Expected.Description, Actual.ElementAt(Index).Description);
        Assert.Equal(Expected.Employer.Name, Actual.ElementAt(Index).Employer);
        Assert.Equal(Expected.EngagementType, Actual.ElementAt(Index).EngagementType);
        Assert.Equal(Expected.JobTitle, Actual.ElementAt(Index).JobTitle);
        Assert.Equal(Expected.WorkLocations.First().Municipal, Actual.ElementAt(Index).Municipal);
        Assert.Equal(Expected.Expires, Actual.ElementAt(Index).Expires);

        // Cleanup
        Fixture.Cleanup();

    }

    

    [Fact]
        public async void Check_That_Expired_Listings_Are_Deleted()
        {
        //Arrange
        var mockLogger = new Mock<ILogger<V2JobListingsDatabaseController>>();
        var _logger = mockLogger.Object;

        using var context = Fixture.CreateContext();
        var controller = new V2JobListingsDatabaseController(_logger, context);

        V2Employer employer = new()
        {
            Name = "Deg selv"
        };

        V2WorkLocation workLocation = new()
        {
            Municipal = "Oslo"
        };

        List<V2WorkLocation> workLocations = new();
        workLocations.Add(workLocation);

        V2Advertisement advertisement = new()
        {
            Uuid = "02ceec90-06ab-4222-8f80-9664a58f2a10",
            Title = "Utvikler ved siden av jobb",
            Description = "Vil du jobbe med Utvikler ved siden av det du gjør til daglig? Da er denne stillingen for deg!",
            EngagementType = "Fulltid",
            Employer = employer,
            JobTitle = "Overarbeidet",
            WorkLocations = workLocations,
            Expires = DateTime.Today.AddDays(-1)
        };


        //Act
        var saveAdvertisement = controller.SaveAdvertisements(advertisement);
        var ListOfAdvertisements = await controller.Get();
        Boolean actual = false;
        
        foreach (var item in ListOfAdvertisements)
        {
            if (item.Uuid == advertisement.Uuid)
            {
                actual = true;
            }
        }
        
        //Assert
        Assert.False(actual);

        // Cleanup
        Fixture.Cleanup();

    }

}
