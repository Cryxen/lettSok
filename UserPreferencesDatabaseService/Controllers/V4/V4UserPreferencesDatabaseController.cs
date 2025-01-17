﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserPreferencesDatabaseService.Data;
using UserPreferencesDatabaseService.Model.V3;

namespace UserPreferencesDatabaseService.Controllers.V4;

[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
[Route("[controller]")]
public class V4UserPreferencesDatabaseController : ControllerBase
{


    private readonly ILogger<V4UserPreferencesDatabaseController> _logger;
    private readonly UserPreferencesDbContext _UserPreferencesDbContext;

    public V4UserPreferencesDatabaseController(ILogger<V4UserPreferencesDatabaseController> logger, UserPreferencesDbContext UserPreferencesDbContext)
    {
        _logger = logger;
        _UserPreferencesDbContext = UserPreferencesDbContext;
    }

    /// <summary>
    /// Gets all users in Database
    /// </summary>
    /// <returns>JSON list of users</returns>
    [HttpGet("getUsers")]
    public async Task<List<V3User>> Get()
    {

        var ResponseUsers = await _UserPreferencesDbContext.users
            .Select(User => new V3User
            {
                Id = User.Id,
                Name = User.Name,
            }).ToListAsync();

        return ResponseUsers;
    }

    /// <summary>
    /// Saves user in Database
    /// </summary>
    /// <param name="userPost">name of user</param>
    /// <returns>Error codes</returns>
    [HttpPost("saveUser")]
    public async Task<V3Result<V3User>> SaveUser(V3User userPost)
    {
        var User = new User()
        {
            Id = Guid.NewGuid(),
            Name = userPost.Name,
        };
        _UserPreferencesDbContext.Add(User);

        await _UserPreferencesDbContext.SaveChangesAsync();

        var Result = new V3Result<V3User>();
        Result.Value = new V3User
        {
            Name = userPost.Name,
        };
        return Result;
    }

    /// <summary>
    /// Gets users interested advertisements
    /// </summary>
    /// <returns>Gets users interested advertisements</returns>
    [HttpGet("getInterest")]
    public async Task<List<V3Interested>> GetInterest()
    {

        var ResponseInterests = await _UserPreferencesDbContext.interestedAdvertisements
            .Select(Interest => new V3Interested
            {
                UserGuid = Interest.UserId,
                AdvertisementUuid = Interest.AdvertisementUuid
            }).ToListAsync();

        return ResponseInterests;
    }

    /// <summary>
    /// Saves interested advertisement in a one-to-many relationship in database
    /// </summary>
    /// <param name="interestPost">Uuid of advertisement, and guid of user</param>
    /// <returns>Error code</returns>
    [HttpPost("saveInterest")]
    public async Task<V3Result<V3Interested>> SaveInterest (V3Interested interestPost)
    {
        var Interest = new InterestedAdvertisement()
        {
            AdvertisementUuid = interestPost.AdvertisementUuid,
            UserId = interestPost.UserGuid,
        };
        _UserPreferencesDbContext.Add(Interest);

        await _UserPreferencesDbContext.SaveChangesAsync();

        var Result = new V3Result<V3Interested>();
        Result.Value = new V3Interested
        {
            UserGuid = interestPost.UserGuid,
            AdvertisementUuid = interestPost.AdvertisementUuid,
        };
        return Result;
    }

    /// <summary>
    /// Gets advertisements users did not find interesting
    /// </summary>
    /// <returns>Gets advertisements users did not find interesting</returns>
    [HttpGet("getUnInterest")]
    public async Task<List<V3Uninterested>> GetUninterest()
    {

        var ResponseUninterests = await _UserPreferencesDbContext.uninterestedAdvertisements
            .Select(Uninterest => new V3Uninterested
            {
                UserGuid = Uninterest.UserId,
                AdvertisementUuid = Uninterest.AdvertisementUuid
            }).ToListAsync();

        return ResponseUninterests;
    }


    /// <summary>
    /// Saves interested advertisement in a one-to-many relationship in database
    /// </summary>
    /// <param name="uninterestPost">Uuid of advertisement, and guid of user</param>
    /// <returns>Error code</returns>
    [HttpPost("saveUninterest")]
    public async Task<V3Result<V3Uninterested>> SaveUninterest(V3Uninterested uninterestPost)
    {
        var Interest = new UninterestedAdvertisement()
        {
            AdvertisementUuid = uninterestPost.AdvertisementUuid,
            UserId = uninterestPost.UserGuid,
        };
        _UserPreferencesDbContext.Add(Interest);

        await _UserPreferencesDbContext.SaveChangesAsync();

        var Result = new V3Result<V3Uninterested>();
        Result.Value = new V3Uninterested
        {
            UserGuid = uninterestPost.UserGuid,
            AdvertisementUuid = uninterestPost.AdvertisementUuid,
        };
        return Result;
    }
}

