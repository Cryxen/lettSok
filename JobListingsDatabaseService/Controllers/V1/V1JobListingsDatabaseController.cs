using JobListingsDatabaseService.Data;
using JobListingsDatabaseService.Model.V1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace JobListingsDatabaseService.Controllers.V1;

[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
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
    public async Task<V1Result<IEnumerable<V1Advertisement>>> Get()
    {
        //var dbContext = new LettsokDbContext();

        var ResponseAdvertisements = await _lettsokDbContext.Advertisements
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
       
        return new V1Result<IEnumerable<V1Advertisement>>(ResponseAdvertisements);
           
     
      
    }
    [HttpPost("saveAdvertisement")]
    public async Task<V1Result<V1Advertisement>> SaveAdvertisements(V1Advertisement advertisementPost)
    {

        var Advertisement = new Advertisement()
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

        _lettsokDbContext.Add(Advertisement);
        await _lettsokDbContext.SaveChangesAsync();

        var Result = new V1Result<V1Advertisement>();
        Result.Value = new V1Advertisement
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

        return Result;
    }



}

