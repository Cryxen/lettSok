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

    private static HttpClient s_client = new HttpClient();

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
    /// <remarks>
    /// Sample return:
    /// 
    ///     GET /getUsers
    ///     {
    ///         "id": "02603a46-83e4-4089-aeae-f2725c4e83c1",
    ///         "name": "test"
    ///     }
    ///     
    /// </remarks>
    /// <response code="200">OK, List of all users in database</response>
    [HttpGet("getUsers")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<List<V3User>> Get()
    {
        _logger.LogInformation("Getting all users from Database, time: {time}", DateTimeOffset.Now);
        var ResponseUsers = await _UserPreferencesDbContext.users
            .Select(User => new V3User
            {
                Id = User.Id,
                Name = User.Name,
            }).ToListAsync();

        return ResponseUsers;
    }

    /// <summary>
    /// Saves user in Database
    /// </summary>
    /// <param name="userPost">name of user</param>
    /// <returns>Error codes</returns>
    /// <remarks>
    /// Sample Post:
    ///
    ///     POST /saveUser
    ///     {
    ///         "name": "name of user"
    ///     }
    /// </remarks>
    /// <response code="201">Returns empty error list and newly created object</response>
    /// <response code="400">Returns a bad request, typically something wrong with JSON in POST request</response>
    [HttpPost("saveUser")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<V3Result<V3User>> SaveUser(V3User userPost)
    {
        _logger.LogInformation("Saving user with name: {0} to Database, time: {time}", userPost.Name, DateTimeOffset.Now);
        var User = new User()
        {
            Id = Guid.NewGuid(),
            Name = userPost.Name,
        };
        _UserPreferencesDbContext.Add(User);

        await _UserPreferencesDbContext.SaveChangesAsync();

        var Result = new V3Result<V3User>();
        Result.Value = new V3User
        {
            Name = userPost.Name,
        };
        return Result;
    }

    /// <summary>
    /// Gets users interested advertisements
    /// </summary>
    /// <returns>Gets users interested advertisements</returns>
    /// <remarks>
    /// sample return:
    ///
    ///     GET /getInterest
    ///     [{
    ///         "userGuid": "02603a46-83e4-4089-aeae-f2725c4e83c1",
    ///         "advertisementUuid": "0334eb60-4218-42e5-bf17-656ddd0abc3f",
    ///         "id": 1
    ///     }]
    /// </remarks>
    /// <response code="200">OK, list of all advertisements marked as interesting by all users</response>
    [HttpGet("getInterest")]
    [ProducesResponseType(StatusCodes.Status200OK)]

    public async Task<List<V3Interested>> GetInterest()
    {
        _logger.LogInformation("Getting all interests from Database, time: {time}", DateTimeOffset.Now);

        var ResponseInterests = await _UserPreferencesDbContext.InterestedAdvertisements
            .Select(interest => new V3Interested
            {
                UserGuid = interest.UserId,
                AdvertisementUuid = interest.AdvertisementUuid,
                Id = interest.Id
            }).ToListAsync();

        return ResponseInterests;
    }

    /// <summary>
    /// Saves a users interested advertisement 
    /// </summary>
    /// <param name="interestPost">Uuid of advertisement, and guid of user</param>
    /// <returns>Object that has been saved</returns>
    /// <remarks>
    /// A sample POST request:
    ///
    ///     POST /saveInterest
    ///     {
    ///         "userGuid": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///         "advertisementUuid": "string",
    ///     }
    /// </remarks>
    /// <response code="201">Returns empty error list, and newly created object</response>
    /// <response code="400">Returns a bad request, typically something wrong with JSON in POST request</response>
    /// <response code="500">Returns internal server error. This is found when userGuid and AdvertisementUuid can't be found in Database</response>
    [HttpPost("saveInterest")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<V3Result<V3Interested>> SaveInterest (V3Interested interestPost)
    {
        _logger.LogInformation("Saving interest post consisting of adverstisementUuid: {0}, and UserGuid: {1} to Database, time: {time}", interestPost.AdvertisementUuid, interestPost.UserGuid, DateTimeOffset.Now);

        var Interest = new InterestedAdvertisement()
        {
            AdvertisementUuid = interestPost.AdvertisementUuid,
            UserId = interestPost.UserGuid,
        };
        _UserPreferencesDbContext.Add(Interest);

        await _UserPreferencesDbContext.SaveChangesAsync();

        var Result = new V3Result<V3Interested>();
        Result.Value = new V3Interested
        {
            UserGuid = interestPost.UserGuid,
            AdvertisementUuid = interestPost.AdvertisementUuid,
        };
        return Result;
    }

    /// <summary>
    /// Deletes interest from database
    /// </summary>
    /// <param name="UserGuid"></param>
    /// <param name="AdvertisementUuid"></param>
    /// <returns>Status 200, success</returns>
    [HttpDelete("deleteInterest")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task DeleteInterest(Guid UserGuid, string AdvertisementUuid)
    {
        _logger.LogInformation("Deleting interest consiting of Advertisement Uuid: {0}, and UserGuid: {1} from Database, time: {time}", AdvertisementUuid, UserGuid, DateTimeOffset.Now);

        List<V3Interested>Interests = await GetInterest();
        foreach (var Item in Interests)
        {
            if (Item.AdvertisementUuid == AdvertisementUuid && Item.UserGuid == UserGuid)
            {
                _UserPreferencesDbContext.Remove(_UserPreferencesDbContext.InterestedAdvertisements.Single(i => i.Id == Item.Id ));
            }
        }
        await _UserPreferencesDbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Gets advertisements users did not find interesting
    /// </summary>
    /// <returns>Gets advertisements users did not find interesting</returns>
    /// 
    ///  <remarks>
    /// sample return:
    ///
    ///     GET /getUnInterest
    ///     [{
    ///         "userGuid": "02603a46-83e4-4089-aeae-f2725c4e83c1",
    ///         "advertisementUuid": "0334eb60-4218-42e5-bf17-656ddd0abc3f",
    ///         "id": 1
    ///     }]
    /// </remarks>
    /// <response code="200">OK, list of all advertisements marked as uninteresting by all users</response>
    [HttpGet("getUnInterest")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<List<V3Uninterested>> GetUninterest()
    {
        _logger.LogInformation("Getting all Uninterests from Database, time: {time}", DateTimeOffset.Now);

        var ResponseUninterests = await _UserPreferencesDbContext.UninterestedAdvertisements
            .Select(Uninterest => new V3Uninterested
            {
                UserGuid = Uninterest.UserId,
                AdvertisementUuid = Uninterest.AdvertisementUuid,
                Id = Uninterest.Id
            }).ToListAsync();

        return ResponseUninterests;
    }


    /// <summary>
    /// Saves Uninterested advertisement
    /// </summary>
    /// <param name="uninterestPost">Uuid of advertisement, and guid of user</param>
    /// <returns>Object that has been saved</returns>
    /// <remarks>
    /// A sample POST request:
    ///
    ///     POST /saveUninterest
    ///     {
    ///         "userGuid": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///         "advertisementUuid": "string",
    ///     }
    /// </remarks>
    /// <response code="201">Returns empty error list, and newly created object</response>
    /// <response code="400">Returns a bad request, typically something wrong with JSON in POST request</response>
    /// <response code="500">Returns internal server error. This is found when userGuid and AdvertisementUuid can't be found in Database</response>
    [HttpPost("saveUninterest")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<V3Result<V3Uninterested>> SaveUninterest(V3Uninterested uninterestPost)
    {
        _logger.LogInformation("Saving uninterest post consisting of adverstisementUuid: {0}, and UserGuid: {1} to Database, time: {time}", uninterestPost.AdvertisementUuid, uninterestPost.UserGuid, DateTimeOffset.Now);

        var Interest = new UninterestedAdvertisement()
        {
            AdvertisementUuid = uninterestPost.AdvertisementUuid,
            UserId = uninterestPost.UserGuid,
        };
        _UserPreferencesDbContext.Add(Interest);

        await _UserPreferencesDbContext.SaveChangesAsync();

        var Result = new V3Result<V3Uninterested>();
        Result.Value = new V3Uninterested
        {
            UserGuid = uninterestPost.UserGuid,
            AdvertisementUuid = uninterestPost.AdvertisementUuid,
        };
        return Result;
    }

    /// <summary>
    /// Deletes uninterest from database
    /// </summary>
    /// <param name="UserGuid"></param>
    /// <param name="AdvertisementUuid"></param>
    /// <returns>Status 200, success</returns>
    [HttpDelete("deleteUninterest")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task DeleteUninterest(Guid UserGuid, string AdvertisementUuid)
    {
        _logger.LogInformation("Deleting interest consiting of Advertisement Uuid: {0}, and UserGuid: {1} from Database, time: {time}", AdvertisementUuid, UserGuid, DateTimeOffset.Now);

        List<V3Uninterested> Uninterests = await GetUninterest();
        foreach (var Item in Uninterests)
        {
            if (Item.AdvertisementUuid == AdvertisementUuid && Item.UserGuid == UserGuid)
            {
                _UserPreferencesDbContext.Remove(_UserPreferencesDbContext.UninterestedAdvertisements.Single(i => i.Id == Item.Id));
            }
        }
        await _UserPreferencesDbContext.SaveChangesAsync();
    }


    /// <summary>
    /// Gets municipalities saved in database
    /// </summary>
    /// <returns>List of municipalities saved in database</returns>
    /// <remarks>
    /// sample return:
    ///
    ///     GET /getLocations
    ///     [{
    ///     "municipality": "Bodø",
    ///     "id": 1
    ///     }]
    /// </remarks>
    /// <response code="200">OK, list of all advertisements marked as uninteresting by all users</response>
    [HttpGet("getLocations")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<List<V3Location>> GetLocations()
    {
        _logger.LogInformation("Getting all locations from Database, time: {time}", DateTimeOffset.Now);

        var ResponseLocations = await _UserPreferencesDbContext.Locations
    .Select(Location => new V3Location
    {
        Municipality = Location.Municipality,
        Id = Location.Id

    }).ToListAsync();

        return ResponseLocations;
    }


    /// <summary>
    /// Gets list of users preferred municipalities saved in database
    /// </summary>
    /// <returns>List of users preferred municipalities saved in database</returns>
    /// <remarks>
    /// sample return:
    ///
    ///     GET /getLocations
    ///     [{
    ///         "userId": "02603a46-83e4-4089-aeae-f2725c4e83c1",
    ///         "locationId": 1,
    ///         "id": 1
    ///     }]
    /// </remarks>
    /// <response code="200">OK, list of all advertisements marked as uninteresting by all users</response>
    [HttpGet("getSearchLocations")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<List<V3SearchLocation>> GetSearchLocations()
    {
        _logger.LogInformation("Getting all SearchLocations from Database, time: {time}", DateTimeOffset.Now);

        var ResponseLocations = await _UserPreferencesDbContext.SearchLocations
        .Select(SearchLocation => new V3SearchLocation
        {
            UserId = SearchLocation.UserId,
            LocationId = SearchLocation.LocationId,
            Id = SearchLocation.Id
        }).ToListAsync();

        return ResponseLocations;
    }

    /// <summary>
    /// Saves preferred search location
    /// </summary>
    /// <param name="v3SearchLocation">LocationId and userId</param>
    /// <returns>Object that has been saved</returns>
    /// <remarks>
    /// A sample POST request:
    ///
    ///     POST /saveSearchLocation
    ///     {
    ///         "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///         "locationId": 2,
    ///     }
    /// </remarks>
    /// <response code="201">Returns empty error list, and newly created object</response>
    /// <response code="400">Returns a bad request, typically something wrong with JSON in POST request</response>
    /// <response code="500">Returns internal server error. This is found when userId and Locationid can't be found in Database</response>
    [HttpPost("saveSearchLocation")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<V3Result<V3SearchLocation>> SaveSearchLocation (V3SearchLocation v3SearchLocation)
    {
        _logger.LogInformation("Save Search Location with location ID {0}, and UserId: {1} to Database, at time: {time}", v3SearchLocation.LocationId, v3SearchLocation.UserId, DateTimeOffset.Now);
        var SearchLocation = new SearchLocation()
        {
            UserId = v3SearchLocation.UserId,
            LocationId = v3SearchLocation.LocationId
        };
        _UserPreferencesDbContext.Add(SearchLocation);

        await _UserPreferencesDbContext.SaveChangesAsync();

        var Result = new V3Result<V3SearchLocation>();
        Result.Value = new V3SearchLocation
        {
            UserId = v3SearchLocation.UserId,
            LocationId = v3SearchLocation.LocationId
        };
        return Result;
    }


    /// <summary>
    /// Deletes preferred search location from database
    /// </summary>
    /// <param name="UserGuid"></param>
    /// <param name="locationId"></param>
    /// <returns>Status 200, success</returns>
    [HttpDelete("deleteSearchLocation")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task DeleteSearchLocation(Guid UserId, int locationId)
    {
        _logger.LogInformation("Deleting Search location with Location id: {0}, and User Id: {1} from Database, time: {time}", locationId, UserId, DateTimeOffset.Now);
        List<V3SearchLocation> SearchLocations = await GetSearchLocations();
        foreach (var Item in SearchLocations)
        {
            if (Item.UserId == UserId && Item.LocationId == locationId)
            {
                _UserPreferencesDbContext.Remove(_UserPreferencesDbContext.SearchLocations.Single(i => i.Id == Item.Id));
            }
        }
        await _UserPreferencesDbContext.SaveChangesAsync();
    }



    /// <summary>
    /// Fetches municipalities from public API: https://ws.geonorge.no/kommuneinfo/v1/#/default/get_kommuner
    /// </summary>
    /// <returns>Status code 200: Success</returns>
    [HttpGet("updateLocationsFromGeoNorge")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task getMunipalitiesFromPublicAPI()
    {
        _logger.LogInformation("Fetch municipalities from geonorge, time: {time}", DateTimeOffset.Now);
        IEnumerable<V3Location> Locations = new List<V3Location>();

        var JsonUrl = "https://ws.geonorge.no/kommuneinfo/v1/kommuner";
        string? Json = await client.GetStringAsync("https://ws.geonorge.no/kommuneinfo/v1/kommuner");

        Locations = JsonConvert.DeserializeObject<IEnumerable<V3Location>>(Json.Replace("kommunenavn", "Municipality"));

        _UserPreferencesDbContext.RemoveRange(_UserPreferencesDbContext.Locations);

        foreach (var Item in Locations)
        {
            await SaveLocation(Item);
        }

    }


    private async Task<V3Result<V3Location>> SaveLocation(V3Location v3Location)
    {
        _logger.LogInformation("Saving Municipality {0} to Database, time: {time}", v3Location.Municipality, DateTimeOffset.Now);
        var Location = new Data.Location()
        {
            Municipality = v3Location.Municipality,
        };
        _UserPreferencesDbContext.Add(Location);

        await _UserPreferencesDbContext.SaveChangesAsync();

        var Result = new V3Result<V3Location>();
        Result.Value = new V3Location
        {
            Municipality = v3Location.Municipality,
        };
        return Result;
    }
}

