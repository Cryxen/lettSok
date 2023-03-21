using Microsoft.AspNetCore.Mvc;
using UserPreferences.Model.V1;

namespace UserPreferences.Controllers.V1;

[ApiController]
[Route("[controller]")]
public class V1UserPreferencesController : ControllerBase
{
    private readonly ILogger<V1UserPreferencesController> _logger;

    public V1UserPreferencesController(ILogger<V1UserPreferencesController> logger)
    {
        _logger = logger;
    }

    [HttpGet("GetUser")]
    public V1User Get()
    {
        V1User dummyData = new V1User
        {
            Id = Guid.NewGuid(),
            Name = "Gustav",
            Interested = { "Jobb1", "Jobb2", "Jobb3" },
            Uninterested = { "Jobb4", "Jobb5", "Jobb6" }
        };
        return dummyData;
    }

    [HttpPost("PostUser")]
    public V1User Post()
    {
        V1User dummyData = new V1User
        {
            Id = Guid.NewGuid(),
            Name = "Gustav",
            Interested = { "Jobb1", "Jobb2", "Jobb3" },
            Uninterested = { "Jobb4", "Jobb5", "Jobb6" }
        };
        return dummyData;
    }
}

