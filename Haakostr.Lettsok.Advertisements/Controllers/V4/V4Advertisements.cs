using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Haakostr.Lettsok.Advertisements.Model;
using Haakostr.Lettsok.Advertisements.Model.V1;
using Microsoft.AspNetCore.Mvc;

namespace Haakostr.Lettsok.Advertisements.Controllers.V4;

[ApiController]
[Route("api/v4/Advertisements")]
public class V4Advertisements : ControllerBase
{
    // For connection to JobListingsDatabaseService
    private static HttpClient client = new HttpClient();
    

    private static string setHttpClientToPublicAPISettings()
    {
        // Client secret TODO: Make is a proper secret
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer",
                "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJzdWIiOiJwdWJsaWMudG9rZW4udjFAbmF2Lm5vIiwiYXVkIjoiZmVlZC1hcGktdjEiLCJpc3MiOiJuYXYubm8iLCJpYXQiOjE1NTc0NzM0MjJ9.jNGlLUF9HxoHo5JrQNMkweLj_91bgk97ZebLdfx3_UQ");
        // URL for public API
        var jsonUrl = "https://arbeidsplassen.nav.no/public-feed/api/v1/ads";

        return jsonUrl;
    }


    public List<V1Advertisement> advertisements { get; private set; }

    private readonly ILogger<V4Advertisements> _logger;

    public V4Advertisements(ILogger<V4Advertisements> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get all jobs from public API without Parameters
    /// Endpoint URL: https://arbeidsplassen.nav.no/public-feed/api/v1/ads
    /// Documentation (Including public key): https://github.com/navikt/pam-public-feed
    /// </summary>
    /// <returns>OK if successfull</returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpGet("GetJobs")]
    public async Task<ActionResult<List<V1Advertisement>>> GetJobs()
    {   
        // Retrieve JSON from API.
        string? json = await client.GetStringAsync(setHttpClientToPublicAPISettings());

        // parse string to json
        var advertisements = ParseJson(json);

        // Save to database
        await postAdvertisementsToDatabase(advertisements);

        return Ok(advertisements);
    }

    /// <summary>
    /// Get jobs by location from public API
    /// </summary>
    /// <param name="location">Municipality</param>
    /// <returns>Ok if advertisements are found</returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpGet("GetJobsByLocation")]
    public async Task<ActionResult<List<V1Advertisement>>> GetJobsByLocation (string location)
    {
        // Retrieve JSON from API.
        string? json = await client.GetStringAsync(setHttpClientToPublicAPISettings() + "?municipal=" + location.ToUpper());

        // parse string to json
        var advertisements = ParseJson(json);

        // Save to database
        await postAdvertisementsToDatabase(advertisements);

        return Ok(advertisements);
    }
    /*
     * This one is being moved to view. There is no need to have a method iterating through database in this microservice.
    /// <summary>
    /// Method to return a single job from public API.
    /// TODO: Turn it to search database?
    /// </summary>
    /// <param name="uuid">UUID of advertisement</param>
    /// <returns>Advertisement if found</returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("GetJob")]
    public async Task<ActionResult<V1Advertisement>> GetJobs(string uuid)
    {
        // Retrieve JSON from API.
        string? json = await client.GetStringAsync(setHttpClientToPublicAPISettings());


        var advertisements = ParseJson(json);

        foreach (var advertisement in advertisements)
        {
            if (advertisement.Uuid == uuid)
            {
                return Ok(advertisement);
            }
                
        }
        return NotFound();
    }

    */

    /// <summary>
    /// Post method saving advertisements to database.
    /// Used https://learn.microsoft.com/en-us/aspnet/web-api/overview/advanced/calling-a-web-api-from-a-net-client as a source
    /// for how to make post.
    /// </summary>
    /// <param name="advertisements">List of Model advertisement</param>
    /// <returns></returns>
    private async Task<Uri?> postAdvertisementsToDatabase(List<V1Advertisement> advertisements)
    {
    //From: https://learn.microsoft.com/en-us/aspnet/web-api/overview/advanced/calling-a-web-api-from-a-net-client
        //client.BaseAddress = new Uri("http://localhost:5201/");
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

        foreach (var advertisement in advertisements)
        {
            HttpResponseMessage  response = await client.PostAsJsonAsync("http://localhost:5201/api/V2/Advertisements/saveAdvertisement", advertisement);
        }
        return null;

    }

    
    // TODO: IS THIS NECESSARY?
    /// <summary>
    /// Parse string to make a json string. Used to make the string returned from public API to a usable JSON.
    /// </summary>
    /// <param name="json">String from public API</param>
    /// <returns></returns>
    private List<V1Advertisement> ParseJson(string json)
    {
        bool isNextUuid = false;
        bool isNextExpires = false;
        bool isNextMunicipal = false;
        bool isNextTitle = false;
        bool isNextDescription = false;
        bool isNextJobTitle = false;
        bool isNextEmployer = false;
        bool isNextEngagementType = false;

        string? uuid = "";
        string? expires = "";
        string? municipal = "";
        string? title = "";
        string? description = "";
        string? jobTitle = "";
        string? employer = "";
        string? engagementType = "";

        byte[] data = Encoding.UTF8.GetBytes(json);
        Utf8JsonReader reader = new(data);

        var advertisements = new List<V1Advertisement>();

        
        while (reader.Read())
        {
            // UUID
            if (reader.TokenType == JsonTokenType.PropertyName && reader.GetString() == "uuid")
            {
                isNextUuid = true;
            }
            if (reader.TokenType == JsonTokenType.String && isNextUuid)
            {
                uuid = reader.GetString();
                isNextUuid = false;
            }
            //EXPIRES
            if (reader.TokenType == JsonTokenType.PropertyName && reader.GetString() == "expires")
            {
                isNextExpires = true;
            }
            if (reader.TokenType == JsonTokenType.String && isNextExpires)
            {
                expires = reader.GetString();
                isNextExpires = false;
            }
            // MUNICIPAL
            if (reader.TokenType == JsonTokenType.PropertyName && reader.GetString() == "municipal")
            {
                isNextMunicipal = true;
            }
            if (reader.TokenType == JsonTokenType.String && isNextMunicipal)
            {
                municipal = reader.GetString();
                isNextMunicipal = false;
            }
            // TITLE
            if (reader.TokenType == JsonTokenType.PropertyName && reader.GetString() == "title")
            {
                isNextTitle = true;
            }
            if (reader.TokenType == JsonTokenType.String && isNextTitle)
            {
                title = reader.GetString();
                isNextTitle = false;
            }
            // DESCRIPTION
            if (reader.TokenType == JsonTokenType.PropertyName && reader.GetString() == "description")
            {
                isNextDescription = true;
            }
            if (reader.TokenType == JsonTokenType.String && isNextDescription)
            {
                description = reader.GetString();
                isNextDescription = false;
            }
            // JOBTITLE
            if (reader.TokenType == JsonTokenType.PropertyName && reader.GetString() == "jobtitle")
            {
                isNextJobTitle = true;
            }
            if (reader.TokenType == JsonTokenType.String && isNextJobTitle)
            {
                jobTitle = reader.GetString();
                isNextJobTitle = false;
            }
            //EMPLOYER
            if (reader.TokenType == JsonTokenType.PropertyName && reader.GetString() == "employer")
            {
                isNextEmployer = true;
            }
            if (reader.TokenType == JsonTokenType.String && isNextEmployer)
            {
                employer = reader.GetString();
                isNextEmployer = false;
            }
            // ENGAGEMENTTYPE
            if (reader.TokenType == JsonTokenType.PropertyName && reader.GetString() == "engagementtype")
            {
                isNextEngagementType = true;
            }
            if (reader.TokenType == JsonTokenType.String && isNextEngagementType)
            {
                engagementType = reader.GetString();
                isNextEngagementType = false;
            }
            if (uuid != "" && title != "" && description != "")
            {
                advertisements.Add(new V1Advertisement
                {
                    Uuid = uuid,
                    Expires = expires,
                    Municipal = municipal,
                    Title = title,
                    Description = description,
                    JobTitle = jobTitle,
                    Employer = employer,
                    EngagementType = engagementType
                });
            }
        }
        return advertisements;
    }

}

