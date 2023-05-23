using JobListingsDatabaseService.Data;
using JobListingsDatabaseService.gRPC.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682
builder.WebHost.ConfigureKestrel(options =>
{
    // Setup a HTTP/2 endpoint without TLS.
    options.ListenLocalhost(5080, o => o.Protocols =
        HttpProtocols.Http2);
});


// Add services to the container.
builder.Services.AddGrpc();

builder.Services.AddDbContext<JobListingsDbContext>(options =>
{
    //Using: https://dev.mysql.com/doc/connector-net/en/connector-net-entityframework-core.html
    options.UseMySQL(builder.Configuration.GetConnectionString("LettsokDb"));
});


var app = builder.Build();



// Configure the HTTP request pipeline.
app.MapGrpcService<AdvertisementService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();

