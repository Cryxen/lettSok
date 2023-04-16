using System;
namespace BlazorView.Data
{
	public class BackgroundUpdateJobsDb : BackgroundService
	{
        private readonly ILogger<BackgroundUpdateJobsDb> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly TimeSpan _period = TimeSpan.FromMinutes(15);

        public BackgroundUpdateJobsDb(ILogger<BackgroundUpdateJobsDb> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using PeriodicTimer timer = new PeriodicTimer(_period);
            FetchJobListingsFromInternet fetchJobListingsFromInternet = new();

            while (!stoppingToken.IsCancellationRequested &&
                   await timer.WaitForNextTickAsync(stoppingToken))
            {
                _logger.LogInformation("Executing Background Task: FetchJobListingsAndSaveToDb " + DateTime.Now);
                await fetchJobListingsFromInternet.FetchJobListingsAndSaveToDb();
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