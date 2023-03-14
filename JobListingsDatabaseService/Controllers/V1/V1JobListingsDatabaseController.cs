using Haakostr.Lettsok.JobListingsDatabaseController.Model.V1;
using JobListingsDatabaseService.Model.V1;
using Microsoft.AspNetCore.Mvc;

namespace JobListingsDatabaseService.Controllers;

[ApiController]
[Route("V1/Advertisements")]
public class V1JobListingsDatabaseController : ControllerBase
{
    private readonly ILogger<V1JobListingsDatabaseController> _logger;

    public V1JobListingsDatabaseController(ILogger<V1JobListingsDatabaseController> logger)
    {
        _logger = logger;
    }

    [HttpGet("")]
    public IEnumerable<V1Advertisement> Get()
    {
        return new[]
        {
         new V1Advertisement
         {
             Uuid = "Random uuid",
             Expires = "When it expires",
             Municipal = "Ås",
             Title = "Strange job",
             Description = "Some weird HTML description",
             JobTitle = "The super job title",
             Employer = "Someone no one wants to work for",
             EngagementType = "Life"
         },
         new V1Advertisement
         {
             Uuid = "Random uuid 2",
             Expires = "When it expires 2",
             Municipal = "Ski",
             Title = "Strange job 2",
             Description = "Some weird HTML description 2",
             JobTitle = "The super job title 2",
             Employer = "Someone no one wants to work for 2",
             EngagementType = "Life 2"
         }
     };
    }
    [HttpPost]
    public V1Restult<V1Advertisement> saveAdvertisements(V1Advertisement advertisementPost)
    {

        var result = new V1Restult<V1Advertisement>();
        result.Value = new V1Advertisement
        {
            Uuid = advertisementPost.Uuid,
            Expires = advertisementPost.Expires,
            Municipal = advertisementPost.Municipal,
            Title = advertisementPost.Title,
            Description = advertisementPost.Description,
            JobTitle = advertisementPost.JobTitle,
            Employer = advertisementPost.Employer,
            EngagementType = advertisementPost.EngagementType
        };

        return result;
    }
}

