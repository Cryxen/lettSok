using JobListingsDatabaseService.Test;
using Microsoft.Extensions.Logging;
using Moq;
using UserPreferencesDatabaseService.Controllers.V5;
using UserPreferencesDatabaseService.Model.V3;


namespace UserPreferencesDatabaseService.Test;

[Collection("UserPreferenceCollection")]
public class UserTest : IDisposable
{

    public UserTest(DatabaseFixtureTest fixture)
    => Fixture = fixture;

    public DatabaseFixtureTest Fixture { get; }


    [Fact]
    public async void Get_Users_From_Database()
    {
        //Arrange
        var MockLogger = new Mock<ILogger<V5UserPreferencesDatabaseController>>();
        var Logger = MockLogger.Object;

        using var Context = Fixture.CreateContext();
        var Controller = new V5UserPreferencesDatabaseController(Logger, Context);

        V3User User1 = new()
        {
            Name = "Test bruker 1",
            Id = Guid.NewGuid()
        };
        V3User User2 = new()
        {
            Name = "Test bruker 1",
            Id = Guid.NewGuid()
        };

        List<V3User> Expected = new();
        Expected.Add(User1);
        Expected.Add(User2);

        //Act
        var Actual = await Controller.Get();
        //Assert
        Assert.Equal(Expected.ElementAt(1).Name, Actual.ElementAt(1).Name);
        Assert.Equal(Expected.ElementAt(0).Name, Actual.ElementAt(0).Name);
        Assert.Equal(2, Actual.Count);

    }

    [Fact]
    public async void Save_User_To_Database()
    {
        //Arrange
        var MockLogger = new Mock<ILogger<V5UserPreferencesDatabaseController>>();
        var Logger = MockLogger.Object;

        using var Context = Fixture.CreateContext();
        var Controller = new V5UserPreferencesDatabaseController(Logger, Context);
        bool UserFound = false;
        V3User User = new()
        {
            Id = Guid.NewGuid(),
            Name = "Ola Nordmann"
        };

        //Act
        var SaveUser = await Controller.SaveUser(User);
        var UserList = await Controller.Get();

        foreach (var Item in UserList)
        {
            if(Item.Name == User.Name)
                UserFound = true;
        }

        //Assert
        Assert.True(UserFound);
        Assert.Equal(3, UserList.Count);

    }

    public void Dispose()
        => Fixture.Cleanup();
    
}
