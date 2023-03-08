using Microsoft.AspNetCore.Mvc;

namespace JobListingsDatabaseService.Controllers;

[ApiController]
[Route("[controller]")]
public class V1JobListingsDatabaseController : ControllerBase
{
    private readonly ILogger<V1JobListingsDatabaseController> _logger;

    public V1JobListingsDatabaseController(ILogger<V1JobListingsDatabaseController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public void Get()
    {
        
    }
}

