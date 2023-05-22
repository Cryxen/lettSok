using JobListingsDatabaseService.Data;
using JobListingsDatabaseService.Interfaces;
using JobListingsDatabaseService.Model.V1;
using JobListingsDatabaseService.Model.V2;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace JobListingsDatabaseService.Controllers.V2;

[ApiController]
[Route("api/V2/Advertisements")]
public class V2JobListingsDatabaseController : ControllerBase, IJobListingsDatabaseController
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
    /// <remarks>
    /// A sample return:
    /// <br />
    ///   { <br />
    ///   <blockquote>
    ///     "uuid": "01e70f56-8889-4335-8a3c-54bbb75d8062", <br />
    ///     "expires": "2023-07-22T19:59:59.978",<br />
    ///     "municipal": "OSLO",<br />
    ///     "title": "Title of advertisement",<br />
    ///     "description": "Sometimes this is in HTML", <br />
    ///     "jobTitle": JobTitle,<br />
    ///     "employer": "Name of the employer",<br />
    ///     "engagementType": "Fast"<br />
    ///   </blockquote>
    ///     }<br />
    /// 
    /// </remarks>
    /// <response code="200">Returns list of advertisements saved in Database</response>
    [HttpGet("getAdvertisements")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public virtual async Task<List<V1Advertisement>> Get()
    {
        _logger.LogInformation("Getting advertisements from Database, {time}", DateTimeOffset.Now);
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
            await checkExpiration(advertisement);
        }

        responseAdvertisements = await _lettsokDbContext.advertisements
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

        //return new V1Restult<IEnumerable<V1Advertisement>>(responseAdvertisements);
        return responseAdvertisements;



    }

    /// <summary>
    /// Save advertisement to database
    /// </summary>
    /// <param name="advertisementPost">Reflects advertisement model</param>
    /// <returns>Returns V1Result error codes</returns>
    /// <remarks>
    /// Sample value of message
    /// 
    ///     POST /saveAdvertisement
    ///     {
    ///         "uuid": "01e70f56-8889-4335-8a3c-54bbb75d8062",
    ///         "expires": "2023-05-22T21:04:21.919Z",
    ///         "workLocations": [
    ///         {
    ///             "municipal": "OSLO"
    ///         }],
    ///         "title": "Title of advertisement",
    ///         "description": "Description of advertisement, sometimes in HTML",
    ///         "jobTitle": "JobTitle",
    ///         "employer": {
    ///             "name": "Name of employer",
    ///         },
    ///         "engagementType": "Full time"
    ///     }
    ///     
    /// 
    /// </remarks>
    /// 
    [HttpPost("saveAdvertisement")]
    public async Task<V1Restult<V2Advertisement>> saveAdvertisements(V2Advertisement advertisementPost)
    {
        _logger.LogInformation("Saving Advertisement {advertisement}", advertisementPost + " time: " + DateTimeOffset.Now);
        var advertisementsFromDb = await Get();

        if (advertisementsFromDb.Count > 0)
        {
            Boolean ItemInDatabase = false;
            _logger.LogDebug("The database is not empty. Making a check to see if the advertisement is in the database, time: {time}", DateTimeOffset.Now);
            foreach (var item in advertisementsFromDb)
            {
                if (item.Uuid == advertisementPost.Uuid)
                {
                    ItemInDatabase = true;
                    _logger.LogDebug(item.Uuid + " Is already in database, time: {time}", DateTimeOffset.Now);
                }
            }
            if (!ItemInDatabase)
            {
                _logger.LogDebug(advertisementPost + " Is not in database. Adding advertisement to database, time: {time}", DateTimeOffset.Now);
                var advertisement = new Advertisement()
                {
                    Uuid = advertisementPost.Uuid,
                    Expires = advertisementPost.Expires,
                    Municipal = advertisementPost.workLocations[0].municipal,
                    Title = advertisementPost.Title,
                    Description = advertisementPost.Description,
                    JobTitle = advertisementPost.JobTitle,
                    Employer = advertisementPost.Employer.name,
                    EngagementType = advertisementPost.EngagementType
                };
                _lettsokDbContext.Add(advertisement);
            } 
        }
        else
        {
            _logger.LogDebug("The database is empty. Adding the first advertisement, time: {time}", DateTimeOffset.Now);
            var advertisement = new Advertisement()
            {
                Uuid = advertisementPost.Uuid,
                Expires = advertisementPost.Expires,
                Municipal = advertisementPost.workLocations[0].municipal,
                Title = advertisementPost.Title,
                Description = advertisementPost.Description,
                JobTitle = advertisementPost.JobTitle,
                Employer = advertisementPost.Employer.name,
                EngagementType = advertisementPost.EngagementType
            };
            _lettsokDbContext.Add(advertisement);
        }
        _logger.LogDebug("Saving changes to Database, time: {time}", DateTimeOffset.Now);
        await _lettsokDbContext.SaveChangesAsync();

        var result = new V1Restult<V2Advertisement>();
        _logger.LogDebug("Error codes from saving: " + result.Errors + " time: {time}", DateTimeOffset.Now);
        result.Value = new V2Advertisement
        {
            Uuid = advertisementPost.Uuid,
            Expires = advertisementPost.Expires,
            workLocations = advertisementPost.workLocations,
            Title = advertisementPost.Title,
            Description = advertisementPost.Description,
            JobTitle = advertisementPost.JobTitle,
            Employer = advertisementPost.Employer,
            EngagementType = advertisementPost.EngagementType
        };

        return result;
    }


    private async Task<V1Advertisement> checkExpiration(V1Advertisement deleteAdvertisement)
    {
        _logger.LogDebug("Checking when job advertisement expires of advertisement: " + deleteAdvertisement.Uuid + " time: {time}", DateTimeOffset.Now);
        DateTime date = DateTime.Today;
        if (DateTime.Compare(date, (DateTime)deleteAdvertisement.Expires) > 0)
        {
            var advertisement = new Advertisement
            {
                Uuid = deleteAdvertisement.Uuid
            };
            _lettsokDbContext.ChangeTracker.Clear();
            _lettsokDbContext.Remove(advertisement);
            await _lettsokDbContext.SaveChangesAsync();
        }
        return deleteAdvertisement;
    }
}

