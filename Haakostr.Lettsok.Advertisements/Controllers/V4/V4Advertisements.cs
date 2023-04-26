using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Advertisements.Interfaces;
using Advertisements.Model.V1;
using Advertisements.Model.V2;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace Advertisements.Controllers.V4;

[ApiController]
[Route("api/v4/Advertisements")]
public class V4Advertisements : ControllerBase, IAdvertisements
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

    /// <summary>
    /// Retrieves listings from public API
    /// </summary>
    /// <returns>Json String</returns>
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<string> RetrieveFromPublicAPI(string location="No Location")
    {
        var response = await client.GetAsync(setHttpClientToPublicAPISettings());

        if (location == "No Location")
        {
            response = await client.GetAsync(setHttpClientToPublicAPISettings());
        }
        else
        {
            response = await client.GetAsync(setHttpClientToPublicAPISettings() + "?municipal=" + location.ToUpper());
        }
        response.EnsureSuccessStatusCode();

        string json = await response.Content.ReadAsStringAsync();

        return json;
    }

    /// <summary>
    /// Parse json string to IList<JToken>
    /// </summary>
    /// <param name="json">Json string</param>
    /// <returns>Parsed list of objects</returns>
    [ApiExplorerSettings(IgnoreApi = true)]
    public IList<JToken> parseJson(string json)
    {
        JObject jsonObject = JObject.Parse(json);
        JsonArray jSONArray = new JsonArray();
        IList<JToken> results = jsonObject["content"].Children().ToList();

        return results;
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
    public virtual async Task<List<V2Advertisement>> GetJobs()
    {
        // Retrieve JSON from API.

        var json = await RetrieveFromPublicAPI();

        var results = parseJson(json);

        List<V2Advertisement> v2Advertisements = new List<V2Advertisement>();

        foreach (var item in results)
        {
            V2Advertisement v2Advertisement = item.ToObject<V2Advertisement>();
            v2Advertisements.Add(v2Advertisement);
        }


        // Save to database
        try
        {
            await postAdvertisementsToDatabase((List<V2Advertisement>)v2Advertisements);
        }
        catch (Exception e)
        {
            _logger.LogError("Saving to JobListings to db did not work " + e);
        }
        return v2Advertisements;
    }

    /// <summary>
    /// Get jobs by location from public API
    /// </summary>
    /// <param name="location">Municipality</param>
    /// <returns>Ok if advertisements are found</returns>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpGet("GetJobsByLocation")]
    public virtual async Task<List<V2Advertisement>> GetJobsByLocation (string location)
    {
        // Retrieve JSON from API.
        var json = await RetrieveFromPublicAPI(location);

        var results = parseJson(json);

        List<V2Advertisement> v2Advertisements = new List<V2Advertisement>();

        foreach (var item in results)
        {
            V2Advertisement v2Advertisement = item.ToObject<V2Advertisement>();
            v2Advertisements.Add(v2Advertisement);
        }


        try
        {
            await postAdvertisementsToDatabase((List<V2Advertisement>)v2Advertisements);
        }
        catch (Exception e)
        {
            _logger.LogError("Saving to JobListings to db did not work " + e);
        }
        return v2Advertisements;
    }
    

    /// <summary>
    /// Post method saving advertisements to database.
    /// Used https://learn.microsoft.com/en-us/aspnet/web-api/overview/advanced/calling-a-web-api-from-a-net-client as a source
    /// for how to make post.
    /// </summary>
    /// <param name="advertisements">List of Model advertisement</param>
    /// <returns></returns>
    private async Task<Uri?> postAdvertisementsToDatabase(List<V2Advertisement> advertisements)
    {
    //From: https://learn.microsoft.com/en-us/aspnet/web-api/overview/advanced/calling-a-web-api-from-a-net-client
        //client.BaseAddress = new Uri("http://localhost:5201/");
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

        foreach (var advertisement in advertisements)
        {
            HttpResponseMessage  response = await client.PostAsJsonAsync("https://localhost:7223/api/V2/Advertisements/saveAdvertisement", advertisement);
        }
        return null;

    }

    
    
    

}

