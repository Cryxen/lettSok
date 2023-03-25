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

    public V3UserPreferencesDatabaseController(ILogger<V3UserPreferencesDatabaseController> logger)
    {
        _logger = logger;
    }
    
    [HttpGet("getUsers")]
    public async Task<List<V3User>> Get()
    {
        var dbContext = new LettsokDbContext();

        var responseUsers = await dbContext.users
            .Select(user => new V3User
            {
                Id = user.Id,
                Name = user.Name,
                //interestedAdvertisements = user.interestedAdvertisements,
                //uninterestedAdvertisements = user.uninterestedAdvertisements
            }).ToListAsync();

        return responseUsers;
    }

    
    [HttpPost("saveUser")]
    public async Task<V3Result<V3User>> saveUser(V3User userPost)
    {
        var dbContext = new LettsokDbContext();
        var User = new User()
        {
            Id = Guid.NewGuid(),
            Name = userPost.Name,
        };
        dbContext.Add(User);

        await dbContext.SaveChangesAsync();

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
        var dbContext = new LettsokDbContext();
        var Interest = new InterestedAdvertisement()
        {
            AdvertisementUuid = interestPost.AdvertisementUuid,
            UserGuid = interestPost.UserGuid,
        };
        dbContext.Add(Interest);

        await dbContext.SaveChangesAsync();

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
        var dbContext = new LettsokDbContext();
        var Interest = new UninterestedAdvertisement()
        {
            AdvertisementUuid = uninterestPost.AdvertisementUuid,
            UserGuid = uninterestPost.UserGuid,
        };
        dbContext.Add(Interest);

        await dbContext.SaveChangesAsync();

        var result = new V3Result<V3Uninterested>();
        result.Value = new V3Uninterested
        {
            UserGuid = uninterestPost.UserGuid,
            AdvertisementUuid = uninterestPost.AdvertisementUuid,
        };
        return result;
    }
}

