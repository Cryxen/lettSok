using System;
using Newtonsoft.Json;
using BlazorView.Data;
using System.Linq;

namespace BlazorView.Data
{
	public class BackgroundUpdateJobsDb : BackgroundService
	{
        private readonly ILogger<BackgroundUpdateJobsDb> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly TimeSpan _period = TimeSpan.FromMinutes(10);

        List<Location> PrefferedLocations = new List<Location>();
        FetchJobListingsFromInternet fetchJobListingsFromInternet = new();

        public BackgroundUpdateJobsDb(ILogger<BackgroundUpdateJobsDb> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using PeriodicTimer timer = new PeriodicTimer(_period);


            var cancel = new CancellationTokenSource();
            await Task.Delay(10000, cancel.Token);

            _logger.LogInformation("Initializing Background Task");
            _logger.LogInformation("Executing Background Task: FetchJobListingsAndSaveToDb " + DateTime.Now);

            PrefferedLocations = await fetchFavoredLocations();

            if (PrefferedLocations.Any())
            {
                fetchJobListingsFromFavoredLocation();
            }
            else
            {
                _logger.LogInformation("Executing Background Task: FetchJobListingsAndSaveToDb " + DateTime.Now);
                await fetchJobListingsFromInternet.FetchJobListingsAndSaveToDb();
            }

            // Source of code: https://www.milanjovanovic.tech/blog/running-background-tasks-in-asp-net-core
            while (!stoppingToken.IsCancellationRequested &&
                   await timer.WaitForNextTickAsync(stoppingToken))
            {
                _logger.LogInformation("Executing Background Task: FetchJobListingsAndSaveToDb " + DateTime.Now);
                await fetchJobListingsFromInternet.FetchJobListingsAndSaveToDb();
            }
        }


        // List of municipalities in Norway
        string? locationsFromDb;
        IEnumerable<Location> locations = new List<Location>();

        // List of municipalities marked as favorable
        string? preferredLocationsFromDb;
        List<PreferredLocation> preferredSearchLocations = new List<PreferredLocation>();



        FetchLocationsFromDb LocationService = new();

        private async Task<List<Location>> fetchFavoredLocations()
        {

            List<Location> PrefferedLocations = new List<Location>();
            List<PreferredLocation> loggedInUserPreferredLocation = new List<PreferredLocation>();

            // Fetch list of municipalities in Norway from db
            locationsFromDb = await LocationService.FetchLocations();
            locations = JsonConvert.DeserializeObject<IEnumerable<Location>>(locationsFromDb);

            // Fetch list of preferred municipalities in Norway from db
            preferredLocationsFromDb = await LocationService.FetchPreferredLocations();
            preferredSearchLocations = JsonConvert.DeserializeObject<List<PreferredLocation>>(preferredLocationsFromDb);

            foreach (var item in preferredSearchLocations)
            {
                if (item.UserId == LoggedInUserService.Id)
                {
                    loggedInUserPreferredLocation.Add(item);
                }
            }

            foreach (var item in locations)
            {
                if (loggedInUserPreferredLocation.Any(i => i.LocationId == item.Id))
                {
                    PrefferedLocations.Add(item);
                }
            }
            return PrefferedLocations;
        }

        private async void fetchJobListingsFromFavoredLocation()
        {
            using PeriodicTimer timer = new PeriodicTimer(_period);

            foreach (var item in PrefferedLocations)
            {
                _logger.LogInformation("Fetching job listings based on location: " + item.Municipality);
                await fetchJobListingsFromInternet.FetchJobListingsFromLocationAndSaveToDb(item.Municipality);
                await Task.Delay(120000);
            }
        }
    }
}

/*
 * 
 * public class PeriodicBackgroundTask : BackgroundService
{
    private readonly TimeSpan _period = TimeSpan.FromSeconds(5);
    private readonly ILogger<PeriodicBackgroundTask> _logger;

    public PeriodicBackgroundTask(ILogger<PeriodicBackgroundTask> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using PeriodicTimer timer = new PeriodicTimer(_period);

        while (!stoppingToken.IsCancellationRequested &&
               await timer.WaitForNextTickAsync(stoppingToken))
        {
            _logger.LogInformation("Executing PeriodicBackgroundTask");
        }
    }
}
 * 
 * 
 * 
 * public class MyBackgroundService : BackgroundService
{
    private readonly ILogger<CollectionService> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public MyBackgroundService(ILogger<CollectionService> logger, IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("My Background Service is starting.");

        //Do your work here...
 */