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
    /// 
    ///     GET /getAdvertisement
    ///     {
    ///     "uuid": "01e70f56-8889-4335-8a3c-54bbb75d8062",
    ///     "expires": "2023-07-22T19:59:59.978",
    ///     "municipal": "OSLO",
    ///     "title": "Title of advertisement",
    ///     "description": "Sometimes this is in HTML",
    ///     "jobTitle": JobTitle,
    ///     "employer": "Name of the employer",
    ///     "engagementType": "Fast"
    ///     }
    /// 
    /// </remarks>
    /// <response code="200">Returns list of advertisements saved in Database</response>
    [HttpGet("getAdvertisements")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public virtual async Task<List<V1Advertisement>> Get()
    {
        _logger.LogInformation("Getting advertisements from Database, {time}", DateTimeOffset.Now);
        var ResponseAdvertisements = await _lettsokDbContext.advertisements
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
        
        foreach (var Advertisement in ResponseAdvertisements)
        {
            await CheckExpiration(Advertisement);
        }

        ResponseAdvertisements = await _lettsokDbContext.advertisements
        .Select(Advertisement => new V1Advertisement
        {
            Uuid = Advertisement.Uuid,
            Expires = Advertisement.Expires,
            Municipal = Advertisement.Municipal,
            Title = Advertisement.Title,
            Description = Advertisement.Description,
            JobTitle = Advertisement.JobTitle,
            Employer = Advertisement.Employer,
            EngagementType = Advertisement.EngagementType

        })
        .ToListAsync();

        return ResponseAdvertisements;



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
    /// </remarks>
    /// <response code="201">Returns empty error list and newly created object</response>
    /// <response code="400">Returns a bad request, something wrong with JSON in POST</response>
    /// <response code="500">Returns internal server error, found to be case if "uuid" is missing in POST</response>
    [HttpPost("saveAdvertisement")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<V1Result<V2Advertisement>> SaveAdvertisements(V2Advertisement advertisementPost)
    {
        _logger.LogInformation("Saving Advertisement {advertisement}", advertisementPost + " time: " + DateTimeOffset.Now);
        var AdvertisementsFromDb = await Get();

        if (AdvertisementsFromDb.Count > 0)
        {
            Boolean ItemInDatabase = false;
            _logger.LogDebug("The database is not empty. Making a check to see if the advertisement is in the database, time: {time}", DateTimeOffset.Now);
            foreach (var Item in AdvertisementsFromDb)
            {
                if (Item.Uuid == advertisementPost.Uuid)
                {
                    ItemInDatabase = true;
                    _logger.LogDebug(Item.Uuid + " Is already in database, time: {time}", DateTimeOffset.Now);
                }
            }
            if (!ItemInDatabase)
            {
                _logger.LogDebug(advertisementPost + " Is not in database. Adding advertisement to database, time: {time}", DateTimeOffset.Now);
                var Advertisement = new Advertisement()
                {
                    Uuid = advertisementPost.Uuid,
                    Expires = advertisementPost.Expires,
                    Municipal = advertisementPost.WorkLocations[0].Municipal,
                    Title = advertisementPost.Title,
                    Description = advertisementPost.Description,
                    JobTitle = advertisementPost.JobTitle,
                    Employer = advertisementPost.Employer.Name,
                    EngagementType = advertisementPost.EngagementType
                };
                _lettsokDbContext.Add(Advertisement);
            } 
        }
        else
        {
            _logger.LogDebug("The database is empty. Adding the first advertisement, time: {time}", DateTimeOffset.Now);
            var Advertisement = new Advertisement()
            {
                Uuid = advertisementPost.Uuid,
                Expires = advertisementPost.Expires,
                Municipal = advertisementPost.WorkLocations[0].Municipal,
                Title = advertisementPost.Title,
                Description = advertisementPost.Description,
                JobTitle = advertisementPost.JobTitle,
                Employer = advertisementPost.Employer.Name,
                EngagementType = advertisementPost.EngagementType
            };
            _lettsokDbContext.Add(Advertisement);
        }
        _logger.LogDebug("Saving changes to Database, time: {time}", DateTimeOffset.Now);
        await _lettsokDbContext.SaveChangesAsync();

        var Result = new V1Result<V2Advertisement>();
        _logger.LogDebug("Error codes from saving: " + Result.Errors + " time: {time}", DateTimeOffset.Now);
        Result.Value = new V2Advertisement
        {
            Uuid = advertisementPost.Uuid,
            Expires = advertisementPost.Expires,
            WorkLocations = advertisementPost.WorkLocations,
            Title = advertisementPost.Title,
            Description = advertisementPost.Description,
            JobTitle = advertisementPost.JobTitle,
            Employer = advertisementPost.Employer,
            EngagementType = advertisementPost.EngagementType
        };

        return Result;
    }


    private async Task<V1Advertisement> CheckExpiration(V1Advertisement deleteAdvertisement)
    {
        _logger.LogDebug("Checking when job advertisement expires of advertisement: " + deleteAdvertisement.Uuid + " time: {time}", DateTimeOffset.Now);
        DateTime Date = DateTime.Today;
        if (DateTime.Compare(Date, (DateTime)deleteAdvertisement.Expires) > 0)
        {
            var Advertisement = new Advertisement
            {
                Uuid = deleteAdvertisement.Uuid
            };
            _lettsokDbContext.ChangeTracker.Clear();
            _lettsokDbContext.Remove(Advertisement);
            await _lettsokDbContext.SaveChangesAsync();
        }
        return deleteAdvertisement;
    }
}

