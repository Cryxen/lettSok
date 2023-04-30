﻿using System.Reflection.Metadata;
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
        var actual = await controller.Get();

        // Assert details of object
        Assert.Equal(expected.Uuid, actual.ElementAt(1).Uuid);
        Assert.Equal(expected.Title, actual.ElementAt(1).Title);
        Assert.Equal(expected.Description, actual.ElementAt(1).Description);
        Assert.Equal(expected.Employer, actual.ElementAt(1).Employer);
        Assert.Equal(expected.JobTitle, actual.ElementAt(1).JobTitle);
        Assert.Equal(expected.Municipal, actual.ElementAt(1).Municipal);
        Assert.Equal(expected.Expires, actual.ElementAt(1).Expires);

        Assert.Equal(2, actual.Count);


        // Cleanup
        Fixture.Cleanup();
    }

    [Fact]
    public async void Post_Advertisement_To_database()
    {
        // Arrange
        using var context = Fixture.CreateContext();
        var controller = new V2JobListingsDatabaseController(_logger, context);

        // Object to be added
        V2Employer employer = new()
        {
            name = "Deg selv"
        };

        V2WorkLocation workLocation = new()
        {
            municipal = "Oslo"
        };

        List<V2WorkLocation> workLocations = new();
        workLocations.Add(workLocation);

        V2Advertisement expected = new()
        {
            Uuid = "02ceec90-06ab-4222-8f80-9664a58f2a29",
            Title = "Utvikler som hobby",
            Description = "Vil du jobbe med Utvikler ved siden av det du gjør til daglig? Søk deg inn på denne stillingen!",
            Employer = employer,
            EngagementType = "Deltid",
            JobTitle = "Ingen spesiell tittel",
            workLocations = workLocations,
            Expires = DateTime.Today
        };

        //Act
        var saveToDatabse = await controller.saveAdvertisements(expected);

        // Find the object in database
        var actual = await controller.Get();
        int index = 0;
        foreach (var item in actual)
        {
            if(item.Uuid == expected.Uuid)
            {
                index = actual.IndexOf(item);
            }
        }

        //Assert
        Assert.Equal(expected.Uuid, actual.ElementAt(index).Uuid);
        Assert.Equal(expected.Title, actual.ElementAt(index).Title);
        Assert.Equal(expected.Description, actual.ElementAt(index).Description);
        Assert.Equal(expected.Employer.name, actual.ElementAt(index).Employer);
        Assert.Equal(expected.EngagementType, actual.ElementAt(index).EngagementType);
        Assert.Equal(expected.JobTitle, actual.ElementAt(index).JobTitle);
        Assert.Equal(expected.workLocations.First().municipal, actual.ElementAt(index).Municipal);
        Assert.Equal(expected.Expires, actual.ElementAt(index).Expires);

        // Cleanup
        Fixture.Cleanup();

    }

    [Fact]
    public async void Try_To_Add_Duplicate_Uuid()
    {
        //Arrange
        using var context = Fixture.CreateContext();
        var controller = new V2JobListingsDatabaseController(_logger, context);

        // Retrieve an already used Uuid
        var ListFromDatabase = await controller.Get();
        var UsedUuid = ListFromDatabase.First().Uuid;

        V2Employer employer = new()
        {
            name = "Deg selv"
        };

        V2WorkLocation workLocation = new()
        {
            municipal = "Oslo"
        };

        List<V2WorkLocation> workLocations = new();
        workLocations.Add(workLocation);

        V2Advertisement advertisement = new()
        {
            Uuid = UsedUuid,
            Title = "Utvikler ved siden av jobb",
            Description = "Vil du jobbe med Utvikler ved siden av det du gjør til daglig? Da er denne stillingen for deg!",
            EngagementType = "Fulltid",
            Employer = employer,
            JobTitle = "Overarbeidet",
            workLocations = workLocations,
            Expires = DateTime.Today
        };

        //Act and assert
        await Assert.ThrowsAsync<DbUpdateException>(() =>
        controller.saveAdvertisements(advertisement)
        );


        // Cleanup
        Fixture.Cleanup();

    }

    [Fact]
        public async void Check_That_Expired_Listings_Are_Deleted()
        {
        //Arrange
        using var context = Fixture.CreateContext();
        var controller = new V2JobListingsDatabaseController(_logger, context);

        V2Employer employer = new()
        {
            name = "Deg selv"
        };

        V2WorkLocation workLocation = new()
        {
            municipal = "Oslo"
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
            workLocations = workLocations,
            Expires = DateTime.Today.AddDays(-1)
        };


        //Act
        var saveAdvertisement = controller.saveAdvertisements(advertisement);
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
