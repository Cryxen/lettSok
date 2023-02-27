using Microsoft.AspNetCore.Mvc;

namespace Haakostr.Lettsok.Advertisements.Controllers.V1;

[ApiController]
[Route("api/v1/Advertisements")]
public class V1Advertisements : ControllerBase
{
    private readonly ILogger<V1Advertisements> _logger;

    public V1Advertisements(ILogger<V1Advertisements> logger)
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

        return json;
    }


}

