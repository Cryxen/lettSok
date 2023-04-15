using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserPreferencesDatabaseService.Data;
using UserPreferencesDatabaseService.Model.V1;

namespace UserPreferencesDatabaseService.Controllers.V1;

[ApiController]
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
        //var dbContext = new LettsokDbContext();

        var responseUsers = await _UserPreferencesDbContextlettsokDbContext.users
            .Select(user => new V1User
            {
                Id = user.Id,
                Name = user.Name,
            }).ToListAsync();

        return responseUsers;
    }
    
    [HttpPost("saveUser")]
    public async Task<V1Result<V1User>> saveUser(V1User userPost)
    {
        //var dbContext = new LettsokDbContext();
        var User = new User()
        {
            Id = userPost.Id,
            Name = userPost.Name,
        };
        _UserPreferencesDbContextlettsokDbContext.Add(User);

        await _UserPreferencesDbContextlettsokDbContext.SaveChangesAsync();

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

