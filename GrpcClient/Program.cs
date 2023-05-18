// See https://aka.ms/new-console-template for more information
using Grpc.Net.Client;
using JobListingsDatabaseService.gRPC;

Console.WriteLine("Hello, World!");

var channel = GrpcChannel.ForAddress("https://localhost:5001");
var client = new Greeter.GreeterClient(channel);
var reply = await client.SayHelloAsync(input);

Console.ReadLine();