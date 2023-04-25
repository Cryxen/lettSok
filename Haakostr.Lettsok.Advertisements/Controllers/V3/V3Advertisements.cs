using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Advertisements.Model.V1;
using Microsoft.AspNetCore.Mvc;

namespace Advertisements.Controllers.V3;

[ApiController]
[Route("api/v3/Advertisements")]
public class V3Advertisements : ControllerBase
{
    // For connection to JobListingsDatabaseService
    static HttpClient client = new HttpClient();


    public List<V1Advertisement> advertisements { get; private set; }

    private readonly ILogger<V3Advertisements> _logger;

    public V3Advertisements(ILogger<V3Advertisements> logger)
    {
        _logger = logger;
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpGet("GetJobs")]
    public async Task<ActionResult<List<V1Advertisement>>> GetJobs()
    {
        // Endpoint URL: https://arbeidsplassen.nav.no/public-feed/api/v1/ads
        // Documentation (Including public key): https://github.com/navikt/pam-public-feed
        using var client = new HttpClient();

        // Insert public key to header of request.
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer",
            "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJzdWIiOiJwdWJsaWMudG9rZW4udjFAbmF2Lm5vIiwiYXVkIjoiZmVlZC1hcGktdjEiLCJpc3MiOiJuYXYubm8iLCJpYXQiOjE1NTc0NzM0MjJ9.jNGlLUF9HxoHo5JrQNMkweLj_91bgk97ZebLdfx3_UQ");

        // URL of public API.
        var jsonUrl = "https://arbeidsplassen.nav.no/public-feed/api/v1/ads";

        // Retrieve JSON from API.
        string? json = await client.GetStringAsync(jsonUrl);

        var advertisements = ParseJson(json);

        // Make filtered result fit to post API in JobListingsDatabaeController

        await postAdvertisementsToDatabase(advertisements);

        return Ok(advertisements);
    }

    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("GetJob")]
    public async Task<ActionResult<V1Advertisement>> GetJobs(string uuid)
    {
        // Endpoint URL: https://arbeidsplassen.nav.no/public-feed/api/v1/ads
        // Documentation (Including public key): https://github.com/navikt/pam-public-feed
        using var client = new HttpClient();

        // Insert public key to header of request.
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer",
            "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJzdWIiOiJwdWJsaWMudG9rZW4udjFAbmF2Lm5vIiwiYXVkIjoiZmVlZC1hcGktdjEiLCJpc3MiOiJuYXYubm8iLCJpYXQiOjE1NTc0NzM0MjJ9.jNGlLUF9HxoHo5JrQNMkweLj_91bgk97ZebLdfx3_UQ");

        // URL of public API.
        var jsonUrl = "https://arbeidsplassen.nav.no/public-feed/api/v1/ads";

        // Retrieve JSON from API.
        string? json = await client.GetStringAsync(jsonUrl);
        ParseJson(json);

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
            if (uuid != "" && title != "" && description != "" )
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
    /*

    From: https://learn.microsoft.com/en-us/aspnet/web-api/overview/advanced/calling-a-web-api-from-a-net-client

    static async Task RunAsync()
    {
        // Update port # in the following line.
        client.BaseAddress = new Uri("http://localhost:64195/");
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
    }

      'http://localhost:5201/V1/Advertisements/saveAdvertisement' 

    */

    // TODO: FIND OUT WHY IT'S NOT WORKING -- Had to make JobListingsDatabaseService start as HTTP.
    // TODO: BUG: As the API pulls several of same entry down, the program stops instead of pulling the rest of the jobs.
    private async Task<Uri?> postAdvertisementsToDatabase(List<V1Advertisement> advertisements)
    {
        client.BaseAddress = new Uri("http://localhost:5201/");
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

        foreach (var advertisement in advertisements)
        {
            HttpResponseMessage  response = await client.PostAsJsonAsync("api/V2/Advertisements/saveAdvertisement", advertisement);
            //response.EnsureSuccessStatusCode();
            //return response.Headers.Location;
        }
        return null;

    }
}

