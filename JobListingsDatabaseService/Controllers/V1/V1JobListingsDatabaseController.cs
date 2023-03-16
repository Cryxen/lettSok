using JobListingsDatabaseService.Data;
using JobListingsDatabaseService.Model.V1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
    public async Task<V1Restult<IEnumerable<V1Advertisement>>> Get()
    {
        var dbContext = new AdvertisementDbContext();

        var responseAdvertisements = await dbContext.advertisements
            .Select(advertisement => new V1Advertisement
            {
                Expires = advertisement.Expires,
                Municipal = advertisement.Municipal,
                Title = advertisement.Title,
                Description = advertisement.Description,
                JobTitle = advertisement.JobTitle,
                Employer = advertisement.Employer,
                EngagementType = advertisement.EngagementType

            })
            .ToListAsync();
       
        return new V1Restult<IEnumerable<V1Advertisement>>(responseAdvertisements);
           
     
      
    }
    [HttpPost("saveAdvertisement")]
    public async Task<V1Restult<V1Advertisement>> saveAdvertisements(V1Advertisement advertisementPost)
    {

        var dbContext = new AdvertisementDbContext();
        var advertisement = new Advertisement()
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

        dbContext.Add(advertisement);
        await dbContext.SaveChangesAsync();

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

