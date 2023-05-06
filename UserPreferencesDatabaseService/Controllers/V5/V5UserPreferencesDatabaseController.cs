using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using UserPreferencesDatabaseService.Data;
using UserPreferencesDatabaseService.Model.V3;
using static Google.Protobuf.Reflection.SourceCodeInfo.Types;

namespace UserPreferencesDatabaseService.Controllers.V5;

[ApiController]
[Route("[controller]")]
public class V5UserPreferencesDatabaseController : ControllerBase
{

    private static HttpClient client = new HttpClient();

    private readonly ILogger<V5UserPreferencesDatabaseController> _logger;
    private readonly UserPreferencesDbContext _UserPreferencesDbContext;

    public V5UserPreferencesDatabaseController(ILogger<V5UserPreferencesDatabaseController> logger, UserPreferencesDbContext UserPreferencesDbContext)
    {
        _logger = logger;
        _UserPreferencesDbContext = UserPreferencesDbContext;
    }

    /// <summary>
    /// Gets all users in Database
    /// </summary>
    /// <returns>JSON list of users</returns>
    [HttpGet("getUsers")]
    public async Task<List<V3User>> Get()
    {
        _logger.LogInformation("Getting all users from Database, time: {time}", DateTimeOffset.Now);
        var responseUsers = await _UserPreferencesDbContext.users
            .Select(user => new V3User
            {
                Id = user.Id,
                Name = user.Name,
            }).ToListAsync();

        return responseUsers;
    }

    /// <summary>
    /// Saves user in Database
    /// </summary>
    /// <param name="userPost">name of user</param>
    /// <returns>Error codes</returns>
    [HttpPost("saveUser")]
    public async Task<V3Result<V3User>> saveUser(V3User userPost)
    {
        _logger.LogInformation("Saving user with name: {0} to Database, time: {time}", userPost.Name, DateTimeOffset.Now);
        var User = new User()
        {
            Id = Guid.NewGuid(),
            Name = userPost.Name,
        };
        _UserPreferencesDbContext.Add(User);

        await _UserPreferencesDbContext.SaveChangesAsync();

        var result = new V3Result<V3User>();
        result.Value = new V3User
        {
            Name = userPost.Name,
        };
        return result;
    }

    /// <summary>
    /// Gets users interested advertisements
    /// </summary>
    /// <returns>Gets users interested advertisements</returns>
    [HttpGet("getInterest")]
    public async Task<List<V3Interested>> getInterest()
    {
        _logger.LogInformation("Getting all interests from Database, time: {time}", DateTimeOffset.Now);

        var responseInterests = await _UserPreferencesDbContext.interestedAdvertisements
            .Select(interest => new V3Interested
            {
                UserGuid = interest.UserId,
                AdvertisementUuid = interest.AdvertisementUuid,
                Id = interest.Id
            }).ToListAsync();

        return responseInterests;
    }

    /// <summary>
    /// Saves interested advertisement in a one-to-many relationship in database
    /// </summary>
    /// <param name="interestPost">Uuid of advertisement, and guid of user</param>
    /// <returns>Error code</returns>
    [HttpPost("saveInterest")]
    public async Task<V3Result<V3Interested>> saveInterest (V3Interested interestPost)
    {
        _logger.LogInformation("Saving interest post consisting of adverstisementUuid: {0}, and UserGuid: {1} to Database, time: {time}", interestPost.AdvertisementUuid, interestPost.UserGuid, DateTimeOffset.Now);

        var Interest = new InterestedAdvertisement()
        {
            AdvertisementUuid = interestPost.AdvertisementUuid,
            UserId = interestPost.UserGuid,
        };
        _UserPreferencesDbContext.Add(Interest);

        await _UserPreferencesDbContext.SaveChangesAsync();

        var result = new V3Result<V3Interested>();
        result.Value = new V3Interested
        {
            UserGuid = interestPost.UserGuid,
            AdvertisementUuid = interestPost.AdvertisementUuid,
        };
        return result;
    }

    /// <summary>
    /// Deletes interest from database
    /// TODO: Add a return method to see that everything went ok.
    /// </summary>
    /// <param name="v3Interested"></param>
    /// <returns></returns>
    [HttpDelete("deleteInterest")]
    public async Task DeleteInterest(Guid UserGuid, string AdvertisementUuid)
    {
        _logger.LogInformation("Deleting interest consiting of Advertisement Uuid: {0}, and UserGuid: {1} from Database, time: {time}", AdvertisementUuid, UserGuid, DateTimeOffset.Now);

        List<V3Interested>interests = await getInterest();
        foreach (var item in interests)
        {
            if (item.AdvertisementUuid == AdvertisementUuid && item.UserGuid == UserGuid)
            {
                _UserPreferencesDbContext.Remove(_UserPreferencesDbContext.interestedAdvertisements.Single(i => i.Id == item.Id ));
            }
        }
        await _UserPreferencesDbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Gets advertisements users did not find interesting
    /// </summary>
    /// <returns>Gets advertisements users did not find interesting</returns>
    [HttpGet("getUnInterest")]
    public async Task<List<V3Uninterested>> getUninterest()
    {
        _logger.LogInformation("Getting all Uninterests from Database, time: {time}", DateTimeOffset.Now);

        var responseUninterests = await _UserPreferencesDbContext.uninterestedAdvertisements
            .Select(uninterest => new V3Uninterested
            {
                UserGuid = uninterest.UserId,
                AdvertisementUuid = uninterest.AdvertisementUuid,
                Id = uninterest.Id
            }).ToListAsync();

        return responseUninterests;
    }


    /// <summary>
    /// Saves interested advertisement in a one-to-many relationship in database
    /// </summary>
    /// <param name="uninterestPost">Uuid of advertisement, and guid of user</param>
    /// <returns>Error code</returns>
    [HttpPost("saveUninterest")]

    public async Task<V3Result<V3Uninterested>> saveUninterest(V3Uninterested uninterestPost)
    {
        _logger.LogInformation("Saving uninterest post consisting of adverstisementUuid: {0}, and UserGuid: {1} to Database, time: {time}", uninterestPost.AdvertisementUuid, uninterestPost.UserGuid, DateTimeOffset.Now);

        var Interest = new UninterestedAdvertisement()
        {
            AdvertisementUuid = uninterestPost.AdvertisementUuid,
            UserId = uninterestPost.UserGuid,
        };
        _UserPreferencesDbContext.Add(Interest);

        await _UserPreferencesDbContext.SaveChangesAsync();

        var result = new V3Result<V3Uninterested>();
        result.Value = new V3Uninterested
        {
            UserGuid = uninterestPost.UserGuid,
            AdvertisementUuid = uninterestPost.AdvertisementUuid,
        };
        return result;
    }

    /// <summary>
    /// Deletes uninterest from database
    /// TODO: Concider making return
    /// </summary>
    /// <param name="v3Uninterested"></param>
    /// <returns></returns>
    [HttpDelete("deleteUninterest")]
    public async Task deleteUninterest(Guid UserGuid, string AdvertisementUuid)
    {
        _logger.LogInformation("Deleting interest consiting of Advertisement Uuid: {0}, and UserGuid: {1} from Database, time: {time}", AdvertisementUuid, UserGuid, DateTimeOffset.Now);

        List<V3Uninterested> uninterests = await getUninterest();
        foreach (var item in uninterests)
        {
            if (item.AdvertisementUuid == AdvertisementUuid && item.UserGuid == UserGuid)
            {
                _UserPreferencesDbContext.Remove(_UserPreferencesDbContext.uninterestedAdvertisements.Single(i => i.Id == item.Id));
            }
        }
        await _UserPreferencesDbContext.SaveChangesAsync();
    }
    
    [HttpGet("getLocations")]
    public async Task<List<V3Location>> getLocations()
    {
        _logger.LogInformation("Getting all locations from Database, time: {time}", DateTimeOffset.Now);

        var responseLocations = await _UserPreferencesDbContext.locations
    .Select(location => new V3Location
    {
        Municipality = location.Municipality,
        Id = location.Id

    }).ToListAsync();

        return responseLocations;
    }



    [HttpGet("getSearchLocations")]
    public async Task<List<V3SearchLocation>> getSearchLocations()
    {
        _logger.LogInformation("Getting all SearchLocations from Database, time: {time}", DateTimeOffset.Now);

        var responseLocations = await _UserPreferencesDbContext.searchLocations
        .Select(searchLocation => new V3SearchLocation
        {
            UserId = searchLocation.UserId,
            LocationId = searchLocation.LocationId,
            Id = searchLocation.Id
        }).ToListAsync();

        return responseLocations;
    }

    [HttpPost("saveSearchLocation")]
    public async Task<V3Result<V3SearchLocation>> saveSearchLocation (V3SearchLocation v3SearchLocation)
    {
        _logger.LogInformation("Save Search Location with location ID {0}, and UserId: {1} to Database, at time: {time}", v3SearchLocation.LocationId, v3SearchLocation.UserId, DateTimeOffset.Now);
        var searchLocation = new SearchLocation()
        {
            UserId = v3SearchLocation.UserId,
            LocationId = v3SearchLocation.LocationId
        };
        _UserPreferencesDbContext.Add(searchLocation);

        await _UserPreferencesDbContext.SaveChangesAsync();

        var result = new V3Result<V3SearchLocation>();
        result.Value = new V3SearchLocation
        {
            UserId = v3SearchLocation.UserId,
            LocationId = v3SearchLocation.LocationId
        };
        return result;
    }

    /// <summary>
    /// Fetches municipalities from https://ws.geonorge.no/kommuneinfo/v1/#/default/get_kommuner
    /// </summary>
    /// <returns></returns>
    [HttpGet("updateLocationsFromGeoNorge")]
    public async Task getMunipalitiesFromPublicAPI()
    {
        _logger.LogInformation("Fetch municipalities from geonorge, time: {time}", DateTimeOffset.Now);
        IEnumerable<V3Location> locations = new List<V3Location>();

        var jsonUrl = "https://ws.geonorge.no/kommuneinfo/v1/kommuner";
        string? json = await client.GetStringAsync("https://ws.geonorge.no/kommuneinfo/v1/kommuner");

        locations = JsonConvert.DeserializeObject<IEnumerable<V3Location>>(json.Replace("kommunenavn", "Municipality"));

        _UserPreferencesDbContext.RemoveRange(_UserPreferencesDbContext.locations);

        foreach (var item in locations)
        {
            await saveLocation(item);
        }

    }


    private async Task<V3Result<V3Location>> saveLocation(V3Location v3Location)
    {
        _logger.LogInformation("Saving Municipality {0} to Database, time: {time}", v3Location.Municipality, DateTimeOffset.Now);
        var location = new Data.Location()
        {
            Municipality = v3Location.Municipality,
        };
        _UserPreferencesDbContext.Add(location);

        await _UserPreferencesDbContext.SaveChangesAsync();

        var result = new V3Result<V3Location>();
        result.Value = new V3Location
        {
            Municipality = v3Location.Municipality,
        };
        return result;
    }

    [HttpDelete("deleteSearchLocation")]
    public async Task deleteSearchLocation(Guid UserId, int locationId)
    {
        _logger.LogInformation("Deleting Search location with Location id: {0}, and User Id: {1} from Database, time: {time}", locationId, UserId, DateTimeOffset.Now);
        List<V3SearchLocation> searchLocations = await getSearchLocations();
        foreach (var item in searchLocations)
        {
            if (item.UserId == UserId && item.LocationId == locationId)
            {
                _UserPreferencesDbContext.Remove(_UserPreferencesDbContext.searchLocations.Single(i => i.Id == item.Id));
            }
        }
        await _UserPreferencesDbContext.SaveChangesAsync();
    }
}

