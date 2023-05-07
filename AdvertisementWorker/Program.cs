using AdvertisementWorker;
using Grpc.Net.Client;
using JobListingsDatabaseService.gRPC;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
    })
    .Build();

var input = new HelloRequest { Name = "Tim" };



host.Run();
