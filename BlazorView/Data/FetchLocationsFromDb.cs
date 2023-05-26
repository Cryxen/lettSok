using System;
using Newtonsoft.Json;
namespace BlazorView.Data

{
	public class FetchLocationsFromDb
	{
        private readonly ILogger _logger;

        public FetchLocationsFromDb(ILogger<FetchLocationsFromDb> logger)
        {
            this._logger = logger;
        }

        private static HttpClient s_client = new HttpClient();

        /// <summary>
        /// Fetch locations from Microservice UserPreferenceDatabaseService.
        /// </summary>
        /// <returns>Json list of locations.</returns>
        public async Task<string> FetchLocations()
        {
            string Json = "";
            try
            {
                _logger.LogDebug("Fetching locations from Database, time: {time}", DateTimeOffset.Now);

                Json = await s_client.GetStringAsync("https://localhost:7293/V5UserPreferencesDatabase/getLocations");
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Can't fetch locations from database, time: {time}", DateTimeOffset.Now);

            }
            return Json;
        }

        /// <summary>
        /// Fetch preferred locations from microservice UserPreferenceDatabaseService.
        /// </summary>
        /// <returns>Json list of preferred locations.</returns>
        public async Task<string> FetchPreferredLocations()
        {
            string Json = "";

            try
            {
                _logger.LogDebug("Fetching Preferred locations from Database, time: {time}", DateTimeOffset.Now);

                Json = await s_client.GetStringAsync("https://localhost:7293/V5UserPreferencesDatabase/getSearchLocations");
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Can't fetch preferred locations from database, time: {time}", DateTimeOffset.Now);

            }
            return Json;
        }

        /// <summary>
        /// Save preferred locations to UserPreferenceDatabaseService.
        /// </summary>
        /// <param name="preferredLocation">Preferred location to be saved to.</param>
        public async void PostPreferredLocation(PreferredLocation preferredLocation)
        {
            _logger.LogDebug("Saving preferred location with Location Id: {0} and User Id: {1}, time: {time}", preferredLocation.LocationId, preferredLocation.UserId, DateTimeOffset.Now);
            var Body = JsonConvert.SerializeObject(preferredLocation);
            StringContent Content = new StringContent(Body, encoding: System.Text.Encoding.UTF8, "application/json");
            using var Response = await s_client.PostAsync("https://localhost:7293/V5UserPreferencesDatabase/saveSearchLocation", Content);
        }

        /// <summary>
        /// Delete preferred locations from UserPreferenceDatabaseService.
        /// </summary>
        /// <param name="preferredLocation">Preferred Location to delete.</param>
        public async void DeletePreferredLocation(PreferredLocation preferredLocation)
        {
            _logger.LogDebug("Deleting preferred location with Location Id: {0} and User Id: {1}, time: {time}", preferredLocation.LocationId, preferredLocation.UserId, DateTimeOffset.Now);

            string Uri = $"https://localhost:7293/V5UserPreferencesDatabase/deleteSearchLocation?UserId={preferredLocation.UserId}&locationId={preferredLocation.LocationId}";
            using var Response = await s_client.DeleteAsync(Uri);
        }

        /// <summary>
        /// Fetch Municipalities from internet using Microservice UserPreferenceDatabaseService.
        /// </summary>
        /// <returns>Json of locations.</returns>
        public async Task<String> FetchLocationsFromInternet()
        {
            _logger.LogDebug("Fetching locations from internet, time: {time}", DateTimeOffset.Now);
            string Json = await s_client.GetStringAsync("https://localhost:7293/V5UserPreferencesDatabase/updateLocationsFromGeoNorge");
            return await FetchLocations();
        }
    }
}

