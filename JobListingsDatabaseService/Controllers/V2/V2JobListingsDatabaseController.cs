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

    public V2JobListingsDatabaseController(ILogger<V2JobListingsDatabaseController> logger)
    {
        _logger = logger;
    }

    [HttpGet("")]
    public async Task<List<V1Advertisement>> Get()
    {
        var dbContext = new AdvertisementDbContext();

        var responseAdvertisements = await dbContext.advertisements
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
            await checkExpiration(advertisement, dbContext);
        }

        //return new V1Restult<IEnumerable<V1Advertisement>>(responseAdvertisements);
        return responseAdvertisements;
           
     
      
    }


    [HttpPost("saveAdvertisement")]
    public async Task<V1Restult<V1Advertisement>> saveAdvertisements(V1Advertisement advertisementPost)
    {
        var advertisementsFromDb = await Get();
        var dbContext = new AdvertisementDbContext();

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
                    dbContext.Add(advertisement);
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
            dbContext.Add(advertisement);
        }
        
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


    private async Task<V1Advertisement> checkExpiration(V1Advertisement deleteAdvertisement, AdvertisementDbContext dbContext)
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

