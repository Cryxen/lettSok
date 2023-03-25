using JobListingsDatabaseService.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserPreferencesDatabaseService.Model.V3;

namespace UserPreferencesDatabaseService.Controllers.V3;

[ApiController]
[Route("[controller]")]
public class V3UserPreferencesDatabaseController : ControllerBase
{


    private readonly ILogger<V3UserPreferencesDatabaseController> _logger;
    private readonly LettsokDbContext _lettsokDbContext;

    public V3UserPreferencesDatabaseController(ILogger<V3UserPreferencesDatabaseController> logger, LettsokDbContext lettsokDbContext)
    {
        _logger = logger;
        _lettsokDbContext = lettsokDbContext;
    }
    
    [HttpGet("getUsers")]
    public async Task<List<V3User>> Get()
    {

        var responseUsers = await _lettsokDbContext.users
            .Select(user => new V3User
            {
                Id = user.Id,
                Name = user.Name,
            }).ToListAsync();

        return responseUsers;
    }

    
    [HttpPost("saveUser")]
    public async Task<V3Result<V3User>> saveUser(V3User userPost)
    {
        var User = new User()
        {
            Id = Guid.NewGuid(),
            Name = userPost.Name,
        };
        _lettsokDbContext.Add(User);

        await _lettsokDbContext.SaveChangesAsync();

        var result = new V3Result<V3User>();
        result.Value = new V3User
        {
            Name = userPost.Name,
        };
        return result;
    }


    [HttpPost("saveInterest")]
    public async Task<V3Result<V3Interested>> saveInterest (V3Interested interestPost)
    {
        var Interest = new InterestedAdvertisement()
        {
            AdvertisementUuid = interestPost.AdvertisementUuid,
            UserGuid = interestPost.UserGuid,
        };
        _lettsokDbContext.Add(Interest);

        await _lettsokDbContext.SaveChangesAsync();

        var result = new V3Result<V3Interested>();
        result.Value = new V3Interested
        {
            UserGuid = interestPost.UserGuid,
            AdvertisementUuid = interestPost.AdvertisementUuid,
        };
        return result;
    }

    [HttpPost("saveUninterest")]
    public async Task<V3Result<V3Uninterested>> saveUninterest(V3Uninterested uninterestPost)
    {
        var Interest = new UninterestedAdvertisement()
        {
            AdvertisementUuid = uninterestPost.AdvertisementUuid,
            UserGuid = uninterestPost.UserGuid,
        };
        _lettsokDbContext.Add(Interest);

        await _lettsokDbContext.SaveChangesAsync();

        var result = new V3Result<V3Uninterested>();
        result.Value = new V3Uninterested
        {
            UserGuid = uninterestPost.UserGuid,
            AdvertisementUuid = uninterestPost.AdvertisementUuid,
        };
        return result;
    }
}

