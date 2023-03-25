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
    private readonly LettsokDbContext _lettsokDbContext;

    public V1UserPreferencesDatabaseController(ILogger<V1UserPreferencesDatabaseController> logger, LettsokDbContext lettsokDbContext)
    {
        _logger = logger;
        _lettsokDbContext = lettsokDbContext;
    }
    
    [HttpGet("")]
    public async Task<List<V1User>> Get()
    {
        //var dbContext = new LettsokDbContext();

        var responseUsers = await _lettsokDbContext.users
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
        _lettsokDbContext.Add(User);

        await _lettsokDbContext.SaveChangesAsync();

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

