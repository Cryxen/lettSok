using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserPreferencesDatabaseService.Data;
using UserPreferencesDatabaseService.Model.V2;

namespace UserPreferencesDatabaseService.Controllers.V2;

[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
[Route("[controller]")]
public class V2UserPreferencesDatabaseController : ControllerBase
{


    private readonly ILogger<V2UserPreferencesDatabaseController> _logger;
    private readonly UserPreferencesDbContext _UserPreferencesDbContext;

    public V2UserPreferencesDatabaseController(ILogger<V2UserPreferencesDatabaseController> logger, UserPreferencesDbContext UserPreferencesDbContext)
    {
        _logger = logger;
        _UserPreferencesDbContext = UserPreferencesDbContext;
    }
    
    [HttpGet("")]
    public async Task<List<V2User>> Get()
    {
        //var dbContext = new LettsokDbContext();

        var responseUsers = await _UserPreferencesDbContext.users
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
        _UserPreferencesDbContext.Add(User);

        await _UserPreferencesDbContext.SaveChangesAsync();

        var result = new V2Result<V2User>();
        result.Value = new V2User
        {
            Id = userPost.Id,
            Name = userPost.Name,
        };
        return result;
    }

   
        
    
}

