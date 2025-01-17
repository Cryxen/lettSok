﻿using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using AdvertisementWorker.Model;
using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AdvertisementWorker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IConfiguration _configuration;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }

    private static HttpClient client = new HttpClient();
    private string _prefferedLocationsJson;
    private string _locationsJson;


    private List<Advertisement> _advertisements = new List<Advertisement>();
    private List<Location> _locationList = new();


    /// <summary>
    /// Set HttpClient to public API settings, making it available to be reused for other APIs.
    /// Values are set in appsettings.json.
    /// </summary>
    /// <returns>Url of the public API. Configured in appsettings.json.</returns>
    private static string SetHttpClientToPublicAPISettings()
    {
        // Making appsettings values available inside static method.
        var Configuration = new ConfigurationBuilder()
            .AddJsonFile("./appsettings.json")
            .Build();

        var PublicApiKey = Configuration.GetValue<string>("PublicAPI:AuthenticationKey");
        var PublicUrl = Configuration.GetValue<string>("PublicAPI:AuthenticationUrl");
        var PublicApiHeader = Configuration.GetValue<string>("PublicAPI:AuthenticationHeaderType");

        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(PublicApiHeader,
                PublicApiKey);

        // URL for public API.
        var JsonUrl = PublicUrl;

        return JsonUrl;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // Find out if any prefered saved locations have been saved:
            try
            {
                // Empty location list.
                _locationList.Clear();
                _logger.LogDebug("Clearing locationlist {time}", DateTimeOffset.Now);

                // Get all locations.
         
                    _locationsJson = await client.GetStringAsync("https://localhost:7293/V5UserPreferencesDatabase/getLocations", stoppingToken);
                    var Locations = JsonConvert.DeserializeObject<IEnumerable<Location>>(_locationsJson);
                    _logger.LogDebug("Fetching locations from database {time}", DateTimeOffset.Now);
           
                // Get preffered Locations.
                    _prefferedLocationsJson = await client.GetStringAsync("https://localhost:7293/V5UserPreferencesDatabase/getSearchLocations", stoppingToken);
                    var PrefferedLocations = JsonConvert.DeserializeObject<IEnumerable<PreferredLocation>>(_prefferedLocationsJson);
                    _logger.LogDebug("Fetching preffered locations from database, {time}", DateTimeOffset.Now);
                
                // Match locations with prefferedLocations. Make a list.
                _locationList = MatchFavoredLocations(Locations, PrefferedLocations);
            }
            catch (Exception e)
            {
                _logger.LogError("Fetching Preferred Locations from database did not work ");
                _logger.LogDebug("Error: {e}", e);
            }

            // If search locations have been saved.
            if (_locationList.Count > 0)
            {
                foreach (var Location in _locationList)
                {

                    _logger.LogInformation("Retrieving jobs for {municipality} at time {time}", Location.Municipality, DateTimeOffset.Now);


                    var AdvertisementsMunicipality = await FetchJobsAndParseFromPublicAPI(Location.Municipality);


                    try
                    {
                        await PostAdvertisementgRPC(_advertisements);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError("Saving to JobListings to db did not work " + e);
                    }
                    await Task.Delay(60000, stoppingToken);
                }
            }

            // Search locations -> Iterate through list and fetch jobs based on search location.

            // No search locations -> Pull from all jobs.
            else
            {
                _logger.LogInformation("Fetching jobs from public API without location at:{time}", DateTimeOffset.Now);

                var Advertisements = await FetchJobsAndParseFromPublicAPI();


                try
                {
                    await PostAdvertisementgRPC(Advertisements);

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
    /// Sends Advertisement to gRPC server.
    /// </summary>
    /// <param name="advertisements"></param>
    /// <returns></returns>
    private async Task PostAdvertisementgRPC(List<Advertisement> advertisements)
    {
        _logger.LogInformation("Trying to run gRPC, time: {time}", DateTimeOffset.Now);
        var Channel = GrpcChannel.ForAddress("http://localhost:5080");
        var GRPCclient = new AdvertisementgRPC.AdvertisementgRPCClient(Channel);
        {
            foreach (var Item in advertisements)
            {
                var TimestampExpires = Timestamp.FromDateTime((DateTime)Item.Expires);

                try
                {
                    await GRPCclient.postAdvertisementsAsync(new AdvertisementModel
                    {
                        Uuid = Item.Uuid,
                        Expires = TimestampExpires,
                        WorkLocation = Item.WorkLocations.ElementAt(0).Municipal,
                        Title = Item.Title,
                        Description = Item.Description,
                        JobTitle = Item.JobTitle,
                        Employer = Item.Employer.Name,
                        EngagementType = Item.EngagementType
                    });
                }
                catch (Exception e)
                {
                    _logger.LogDebug(e, "Something went wrong when trying to post Advertisement async, time: {time}", DateTimeOffset.Now);
                }




            }
        }
    }

    /// <summary>
    /// Match favored locations with preferred locations.
    /// </summary>
    /// <param name="locations"></param>
    /// <param name="preferredLocations"></param>
    /// <returns></returns>
    public List<Location> MatchFavoredLocations(IEnumerable<Location> locations, IEnumerable<PreferredLocation> preferredLocations)
    {
        List<Location> LocationList = new();

        foreach (var Location in locations)
        {
            foreach (var PreferredLocation in preferredLocations)
            {
                if (Location.Id == PreferredLocation.LocationId)
                {
                    LocationList.Add(Location);
                }
            }
        }
        return LocationList;
    }

    /// <summary>
    /// Fetch jobs from Public API and parse JSON.
    /// </summary>
    /// <param name="location">Municipalities to fetch jobs from.</param>
    /// <returns></returns>
    public async Task<List<Advertisement>> FetchJobsAndParseFromPublicAPI(string location = "No Location")
    {
        string Json;

        if (location == "No Location")
        {
            Json = await RetrieveFromPublicAPI();
        }
        else
        {
            Json = await RetrieveFromPublicAPI(location);
        }

        var Results = ParseJson(Json);
        List<Advertisement> Advertisements = new List<Advertisement>();
        foreach (var Item in Results)
        {
            Advertisement AdvertisementItem = Item.ToObject<Advertisement>();
            Advertisements.Add(AdvertisementItem);
        }

        return Advertisements;
    }


    /// <summary>
    /// Retrieves listings from public API.
    /// </summary>
    /// <returns>Json String.</returns>
    public async Task<string> RetrieveFromPublicAPI(string location = "No Location")
    {
        HttpResponseMessage? Response = new(); 
        if (location == "No Location")
        {
            Response = await client.GetAsync(SetHttpClientToPublicAPISettings());
        }
        else
        {
            Response = await client.GetAsync(SetHttpClientToPublicAPISettings() + "?municipal=" + location.ToUpper());
        }
        if (!Response.IsSuccessStatusCode)
        {
            _logger.LogError("Worker failed at: {time} to fetch listings from public API", DateTimeOffset.Now);
        }

        string Json = await Response.Content.ReadAsStringAsync();

        return Json;
    }

    /// <summary>
    /// Parse json string to object.
    /// </summary>
    /// <param name="json"></param>
    /// <returns>IList Json Object.</returns>
    public IList<JToken> ParseJson(string json)
    {
        JObject jsonObject = JObject.Parse(json);
        JsonArray jSONArray = new JsonArray();
        IList<JToken> results = jsonObject["content"].Children().ToList();

        return results;
    }

}

