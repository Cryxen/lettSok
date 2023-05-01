using JobListingsDatabaseService.Test;
using Microsoft.Extensions.Logging;
using UserPreferencesDatabaseService.Controllers.V5;
using UserPreferencesDatabaseService.Model.V3;


namespace UserPreferencesDatabaseService.Test;

[Collection("UserPreferenceCollection")]
public class UserTest : IDisposable
{
    private readonly ILogger<V5UserPreferencesDatabaseController>? _logger;

    public UserTest(DatabaseFixtureTest fixture)
    => Fixture = fixture;

    public DatabaseFixtureTest Fixture { get; }


    [Fact]
    public async void Get_Users_From_Database()
    {
        //Arrange
        using var context = Fixture.CreateContext();
        var controller = new V5UserPreferencesDatabaseController(_logger, context);

        V3User user1 = new()
        {
            Name = "Test bruker 1",
            Id = Guid.NewGuid()
        };
        V3User user2 = new()
        {
            Name = "Test bruker 1",
            Id = Guid.NewGuid()
        };

        List<V3User> expected = new();
        expected.Add(user1);
        expected.Add(user2);

        //Act
        var actual = await controller.Get();
        //Assert
        Assert.Equal(expected.ElementAt(1).Name, actual.ElementAt(1).Name);
        Assert.Equal(expected.ElementAt(0).Name, actual.ElementAt(0).Name);
        Assert.Equal(2, actual.Count);

    }

    [Fact]
    public async void Save_User_To_Database()
    {
        //Arrange
        using var context = Fixture.CreateContext();
        var controller = new V5UserPreferencesDatabaseController(_logger, context);
        bool userFound = false;
        V3User user = new()
        {
            Id = Guid.NewGuid(),
            Name = "Ola Nordmann"
        };

        //Act
        var saveUser = await controller.saveUser(user);
        var userList = await controller.Get();

        foreach (var item in userList)
        {
            if(item.Name == user.Name)
                userFound = true;
        }

        //Assert
        Assert.True(userFound);
        Assert.Equal(3, userList.Count);

    }

    public void Dispose()
        => Fixture.Cleanup();
    
}
