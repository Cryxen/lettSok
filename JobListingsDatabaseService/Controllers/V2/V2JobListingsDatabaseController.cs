using JobListingsDatabaseService.Data;
using JobListingsDatabaseService.Model.V1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace JobListingsDatabaseService.Controllers.V2;

[ApiController]
[Route("api/V2/Advertisements")]
public class V2JobListingsDatabaseController : ControllerBase
{
    private readonly ILogger<V2JobListingsDatabaseController> _logger;
    private readonly JobListingsDbContext _lettsokDbContext;

    public V2JobListingsDatabaseController(ILogger<V2JobListingsDatabaseController> logger, JobListingsDbContext lettsokDbContext)
    {
        _logger = logger;
        _lettsokDbContext = lettsokDbContext;
    }

    /// <summary>
    /// Retrieves advertisements from database
    /// </summary>
    /// <returns>List of advertisements</returns>
    [HttpGet("getAdvertisements")]
    public async Task<List<V1Advertisement>> Get()
    {
        //var dbContext = new LettsokDbContext();

        var responseAdvertisements = await _lettsokDbContext.advertisements
            .Select(advertisement => new V1Advertisement
            {
                Uuid = advertisement.Uuid,
                Expires = advertisement.Expires,
                Municipal = advertisement.Municipal,
                Title = advertisement.Title,
                Description = advertisement.Description,
                JobTitle = advertisement.JobTitle,
                Employer = advertisement.Employer,
                EngagementType = advertisement.EngagementType

            })
            .ToListAsync();
        
        foreach (var advertisement in responseAdvertisements)
        {
            await checkExpiration(advertisement, _lettsokDbContext);
        }

        //return new V1Restult<IEnumerable<V1Advertisement>>(responseAdvertisements);
        return responseAdvertisements;
           
     
      
    }

    /// <summary>
    /// Save advertisement to database
    /// </summary>
    /// <param name="advertisementPost">Reflects advertisement model</param>
    /// <returns>Returns V1Result error codes</returns>
    [HttpPost("saveAdvertisement")]
    public async Task<V1Restult<V1Advertisement>> saveAdvertisements(V1Advertisement advertisementPost)
    {
        var advertisementsFromDb = await Get();
        //var dbContext = new LettsokDbContext();

        if (advertisementsFromDb.Count > 0)
        {
            foreach (var item in advertisementsFromDb)
            {
                if (item.Uuid == advertisementPost.Uuid)
                {
                    Console.WriteLine("Already in Database");
                }
                else
                {
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
                    break;
                }
            }
        }
        else
        {
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
        }
        
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


    private async Task<V1Advertisement> checkExpiration(V1Advertisement deleteAdvertisement, JobListingsDbContext dbContext)
    {
        DateTime date = DateTime.Today;
        if (DateTime.Compare(date, (DateTime)deleteAdvertisement.Expires) > 0)
        {
            var advertisement = new Advertisement
            {
                Uuid = deleteAdvertisement.Uuid
            };

            dbContext.Remove(advertisement);
            await dbContext.SaveChangesAsync();
        }
        return deleteAdvertisement;
    }
}

