using JobListingsDatabaseService.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserPreferencesDatabaseService.Data;
using UserPreferencesDatabaseService.Model.V3;

namespace UserPreferencesDatabaseService.Controllers.V5;

[ApiController]
[Route("[controller]")]
public class V5UserPreferencesDatabaseController : ControllerBase
{


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
    /// Deletes interest
    /// TODO: Add a return method to see that everything went ok.
    /// </summary>
    /// <param name="v3Interested"></param>
    /// <returns></returns>
    [HttpDelete("deleteInterest")]
    public async Task DeleteInterest(V3Interested v3Interested)
    {
        List<V3Interested>interests = await getInterest();
        foreach (var item in interests)
        {
            if (item.AdvertisementUuid == v3Interested.AdvertisementUuid && item.UserGuid == v3Interested.UserGuid)
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

    [HttpDelete("deleteUninterest")]
    public async Task deleteUninterest(V3Uninterested v3Uninterested)
    {
        List<V3Uninterested> uninterests = await getUninterest();
        foreach (var item in uninterests)
        {
            if (item.AdvertisementUuid == v3Uninterested.AdvertisementUuid && item.UserGuid == v3Uninterested.UserGuid)
            {
                _UserPreferencesDbContext.Remove(_UserPreferencesDbContext.uninterestedAdvertisements.Single(i => i.Id == item.Id));
            }
        }
        await _UserPreferencesDbContext.SaveChangesAsync();
    }
}

