using JobListingsDatabaseService.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserPreferencesDatabaseService.Model.V2;

namespace UserPreferencesDatabaseService.Controllers.V2;

[ApiController]
[Route("[controller]")]
public class V2UserPreferencesDatabaseController : ControllerBase
{


    private readonly ILogger<V2UserPreferencesDatabaseController> _logger;
    private readonly LettsokDbContext _lettsokDbContext;

    public V2UserPreferencesDatabaseController(ILogger<V2UserPreferencesDatabaseController> logger, LettsokDbContext lettsokDbContext)
    {
        _logger = logger;
        _lettsokDbContext = lettsokDbContext;
    }
    
    [HttpGet("")]
    public async Task<List<V2User>> Get()
    {
        //var dbContext = new LettsokDbContext();

        var responseUsers = await _lettsokDbContext.users
            .Select(user => new V2User
            {
                Id = user.Id,
                Name = user.Name,
                //interestedAdvertisements = user.interestedAdvertisements,
                //uninterestedAdvertisements = user.uninterestedAdvertisements
            }).ToListAsync();

        return responseUsers;
    }

    
    [HttpPost("saveUser")]
    public async Task<V2Result<V2User>> saveUser(V2User userPost)
    {
        //var dbContext = new LettsokDbContext();
        var User = new User()
        {
            Id = userPost.Id,
            Name = userPost.Name,
        };
        _lettsokDbContext.Add(User);

        await _lettsokDbContext.SaveChangesAsync();

        var result = new V2Result<V2User>();
        result.Value = new V2User
        {
            Id = userPost.Id,
            Name = userPost.Name,
        };
        return result;
    }

   
        
    
}

