using JobListingsDatabaseService.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserPreferencesDatabaseService.Model.V1;

namespace UserPreferencesDatabaseService.Controllers.V1;

[ApiController]
[Route("[controller]")]
public class V1UserPreferencesDatabaseController : ControllerBase
{


    private readonly ILogger<V1UserPreferencesDatabaseController> _logger;

    public V1UserPreferencesDatabaseController(ILogger<V1UserPreferencesDatabaseController> logger)
    {
        _logger = logger;
    }
    
    [HttpGet("")]
    public async Task<List<V1User>> Get()
    {
        var dbContext = new AdvertisementDbContext();

        var responseUsers = await dbContext.users
            .Select(user => new V1User
            {
                Id = user.Id,
                Name = user.Name,
                Interested = user.Interested,
                Uninterested = user.Uninterested
            }).ToListAsync();

        return responseUsers;
    }
    
    [HttpPost("saveUser")]
    public async Task<V1Result<V1User>> saveUser(V1User userPost)
    {
        var dbContext = new AdvertisementDbContext();
        var User = new User()
        {
            Id = userPost.Id,
            Name = userPost.Name,
            Interested = userPost.Interested,
            Uninterested = userPost.Uninterested
        };
        dbContext.Add(User);

        await dbContext.SaveChangesAsync();

        var result = new V1Result<V1User>();
        result.Value = new V1User
        {
            Id = userPost.Id,
            Name = userPost.Name,
            Interested = userPost.Interested,
            Uninterested = userPost.Uninterested
        };
        return result;
    }
        
    
}

