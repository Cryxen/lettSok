using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using AdvertisementWorker.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AdvertisementWorker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }

    private static HttpClient client = new HttpClient();
    string prefferedLocationsJson;
    string locationsJson;
    List<Location> locationList = new();
                    List<Advertisement> Advertisements = new List<Advertisement>();


    // Public API settings
    private static string setHttpClientToPublicAPISettings()
    {
        // Client secret TODO: Make is a proper secret
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer",
                "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJzdWIiOiJwdWJsaWMudG9rZW4udjFAbmF2Lm5vIiwiYXVkIjoiZmVlZC1hcGktdjEiLCJpc3MiOiJuYXYubm8iLCJpYXQiOjE1NTc0NzM0MjJ9.jNGlLUF9HxoHo5JrQNMkweLj_91bgk97ZebLdfx3_UQ");
        // URL for public API
        var jsonUrl = "https://arbeidsplassen.nav.no/public-feed/api/v1/ads";

        return jsonUrl;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // Find out if any prefered saved locations have been saved:
            try
            {
                // Empty location list
                locationList.Clear();


                // Get all locations
                locationsJson = await client.GetStringAsync("https://localhost:7293/V5UserPreferencesDatabase/getLocations", stoppingToken);
                var locations = JsonConvert.DeserializeObject<IEnumerable<Location>>(locationsJson);


                // Get preffered Locations
                prefferedLocationsJson = await client.GetStringAsync("https://localhost:7293/V5UserPreferencesDatabase/getSearchLocations", stoppingToken);
                var prefferedLocations = JsonConvert.DeserializeObject<IEnumerable<PreferredLocation>>(prefferedLocationsJson);

                // Match locations with prefferedLocations. Make a list
                foreach (var location in locations)
                {
                    foreach (var prefferedLocation in prefferedLocations)
                    {
                        if (location.Id == prefferedLocation.LocationId)
                        {
                            locationList.Add(location);
                        }
                    }
                }
            }
            catch 
            {
                _logger.LogError("Fetching Preferred Locations from database did not work ");
            }

            // If search locations have been saved
            if (locationList.Count > 0)
            {
                foreach (var location in locationList)
                {
                    var jsonMunicipality = await RetrieveFromPublicAPI(location.Municipality);
                    _logger.LogInformation("Retrieving jobs for {municipality}", location.Municipality);

                    var resultMunicipality = parseJson(jsonMunicipality);
                    List<Advertisement> AdvertisementsMunicipality = new List<Advertisement>();

                    foreach (var item in resultMunicipality)
                    {
                        Advertisement advertisementItem = item.ToObject<Advertisement>();
                        AdvertisementsMunicipality.Add(advertisementItem);
                    }

                    try
                    {
                        await postAdvertisementsToDatabase((List<Advertisement>)AdvertisementsMunicipality);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError("Saving to JobListings to db did not work " + e);
                    }
                    await Task.Delay(60000, stoppingToken);
                }
            }

            // Search locations -> Iterate through list and fetch jobs based on search location

            // No search locations -> Pull from all jobs
            else
            {
                var json = await RetrieveFromPublicAPI();
                var results = parseJson(json);
                List<Advertisement> Advertisements = new List<Advertisement>();
                foreach (var item in results)
                {
                    Advertisement advertisementItem = item.ToObject<Advertisement>();
                    Advertisements.Add(advertisementItem);
                }

                try
                {
                    await postAdvertisementsToDatabase((List<Advertisement>)Advertisements);
                }
                catch 
                {
                    _logger.LogError("Saving to JobListings to db did not work ");
                }
            }
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(120000, stoppingToken);
        }
    }


    /// <summary>
    /// Retrieves listings from public API
    /// </summary>
    /// <returns>Json String</returns>
    public async Task<string> RetrieveFromPublicAPI(string location = "No Location")
    {
        HttpResponseMessage? response = new(); 
        if (location == "No Location")
        {
            response = await client.GetAsync(setHttpClientToPublicAPISettings());
        }
        else
        {
            response = await client.GetAsync(setHttpClientToPublicAPISettings() + "?municipal=" + location.ToUpper());
        }
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Worker failed at: {time} to fetch listings from public API", DateTimeOffset.Now);
        }

        string json = await response.Content.ReadAsStringAsync();

        return json;
    }



    /// <summary>
    /// Post method saving advertisements to database.
    /// Used https://learn.microsoft.com/en-us/aspnet/web-api/overview/advanced/calling-a-web-api-from-a-net-client as a source
    /// for how to make post.
    /// </summary>
    /// <param name="advertisements">List of Model advertisement</param>
    /// <returns></returns>
    private async Task<Uri?> postAdvertisementsToDatabase(List<Advertisement> advertisements)
    {
        //From: https://learn.microsoft.com/en-us/aspnet/web-api/overview/advanced/calling-a-web-api-from-a-net-client
        //client.BaseAddress = new Uri("http://localhost:5201/");
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

        foreach (var advertisement in advertisements)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync("https://localhost:7223/api/V2/Advertisements/saveAdvertisement", advertisement);
        }
        return null;
    }


    /// <summary>
    /// Parse json string to object
    /// </summary>
    /// <param name="json"></param>
    /// <returns>IList Json Object</returns>
    public IList<JToken> parseJson(string json)
    {
        JObject jsonObject = JObject.Parse(json);
        JsonArray jSONArray = new JsonArray();
        IList<JToken> results = jsonObject["content"].Children().ToList();

        return results;
    }

}

