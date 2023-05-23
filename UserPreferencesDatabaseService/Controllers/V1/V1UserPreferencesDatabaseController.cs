using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserPreferencesDatabaseService.Data;
using UserPreferencesDatabaseService.Model.V1;

namespace UserPreferencesDatabaseService.Controllers.V1;

[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
[Route("[controller]")]
public class V1UserPreferencesDatabaseController : ControllerBase
{


    private readonly ILogger<V1UserPreferencesDatabaseController> _logger;
    private readonly UserPreferencesDbContext _UserPreferencesDbContextlettsokDbContext;

    public V1UserPreferencesDatabaseController(ILogger<V1UserPreferencesDatabaseController> logger, UserPreferencesDbContext UserPreferencesDbContext)
    {
        _logger = logger;
        _UserPreferencesDbContextlettsokDbContext = UserPreferencesDbContext;
    }
    
    [HttpGet("")]
    public async Task<List<V1User>> Get()
    {
        var ResponseUsers = await _UserPreferencesDbContextlettsokDbContext.users
            .Select(User => new V1User
            {
                Id = User.Id,
                Name = User.Name,
            }).ToListAsync();

        return ResponseUsers;
    }
    
    [HttpPost("saveUser")]
    public async Task<V1Result<V1User>> SaveUser(V1User userPost)
    {
        var User = new User()
        {
            Id = userPost.Id,
            Name = userPost.Name,
        };
        _UserPreferencesDbContextlettsokDbContext.Add(User);

        await _UserPreferencesDbContextlettsokDbContext.SaveChangesAsync();

        var Result = new V1Result<V1User>();
        Result.Value = new V1User
        {
            Id = userPost.Id,
            Name = userPost.Name,
            Interested = userPost.Interested,
            Uninterested = userPost.Uninterested
        };
        return Result;
    }
        
    
}

