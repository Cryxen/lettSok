using System.Text;
using System.Text.Json;
using Haakostr.Lettsok.Advertisements.Model;
using Microsoft.AspNetCore.Mvc;

namespace Haakostr.Lettsok.Advertisements.Controllers.V2;

[ApiController]
[Route("api/v2/Advertisements")]
public class V2Advertisements : ControllerBase
{
    private readonly ILogger<V2Advertisements> _logger;

    public V2Advertisements(ILogger<V2Advertisements> logger)
    {
        _logger = logger;
    }

    [HttpGet("GetJobs")]
    public async Task<string> GetJobs()
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

        return json;
    }

    private List<Advertisement> ParseJson(string json)
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

        var advertisements = new List<Advertisement>();


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

            advertisements.Add(new Advertisement
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
        return advertisements;
    }
}

