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
        var ResponseUsers = await _UserPreferencesDbContext.users
            .Select(User => new V2User
            {
                Id = User.Id,
                Name = User.Name,
            }).ToListAsync();

        return ResponseUsers;
    }

    
    [HttpPost("saveUser")]
    public async Task<V2Result<V2User>> SaveUser(V2User userPost)
    {
        var User = new User()
        {
            Id = userPost.Id,
            Name = userPost.Name,
        };
        _UserPreferencesDbContext.Add(User);

        await _UserPreferencesDbContext.SaveChangesAsync();

        var Result = new V2Result<V2User>();
        Result.Value = new V2User
        {
            Id = userPost.Id,
            Name = userPost.Name,
        };
        return Result;
    }

   
        
    
}

