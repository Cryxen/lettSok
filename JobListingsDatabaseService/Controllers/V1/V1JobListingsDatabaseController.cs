using JobListingsDatabaseService.Data;
using JobListingsDatabaseService.Model.V1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace JobListingsDatabaseService.Controllers.V1;

[ApiController]
[Route("api/V1/Advertisements")]
public class V1JobListingsDatabaseController : ControllerBase
{
    private readonly ILogger<V1JobListingsDatabaseController> _logger;
    private readonly JobListingsDbContext _lettsokDbContext;

    public V1JobListingsDatabaseController(ILogger<V1JobListingsDatabaseController> logger, JobListingsDbContext lettsokDbContext)
    {
        _logger = logger;
        _lettsokDbContext = lettsokDbContext;
    }

    [HttpGet("")]
    public async Task<V1Restult<IEnumerable<V1Advertisement>>> Get()
    {
        //var dbContext = new LettsokDbContext();

        var responseAdvertisements = await _lettsokDbContext.advertisements
            .Select(advertisement => new V1Advertisement
            {
                Expires = (DateTime)advertisement.Expires,
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

        //var dbContext = new LettsokDbContext();
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

        _lettsokDbContext.Add(advertisement);
        await _lettsokDbContext.SaveChangesAsync();

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

