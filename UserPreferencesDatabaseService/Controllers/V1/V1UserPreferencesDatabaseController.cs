using Microsoft.AspNetCore.Mvc;
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
    /*
    [HttpGet("")]
    public Get()
    {
        throw NotImplementedException;
        return null;
    }

    [HttpPost("saveUser")]
    public async Task<V1Result<V1User>> saveUser(V1User userPost)
    {
        throw NotImplementedException;
        return null;
    }
    */
}

