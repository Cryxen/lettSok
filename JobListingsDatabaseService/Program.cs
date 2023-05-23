using System.Reflection;
using JobListingsDatabaseService.Data;
using JobListingsDatabaseService.gRPC.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

internal class Program
{
    private static async Task Main(string[] args)
    {

    var builder = WebApplication.CreateBuilder(args);
        // Add services to the container.

        builder.WebHost.ConfigureKestrel(options =>
        {
            // Setup a HTTP/2 endpoint without TLS for gRPC.
            options.ListenLocalhost(5080, o => o.Protocols =
                HttpProtocols.Http2);

            // Setup endpoint for Swagger
            options.ListenLocalhost(5081, o => o.Protocols =
            HttpProtocols.Http1AndHttp2);
        });



        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v2",
                Title = "Controller for Job Listings Database",
                Description = "A REST API controller for Lettsøks Job Listings Database service"
            });

            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });

        builder.Services.AddGrpc();

        builder.Services.AddDbContext<JobListingsDbContext>(options =>
        {
            //Using: https://dev.mysql.com/doc/connector-net/en/connector-net-entityframework-core.html
            options.UseMySQL(builder.Configuration.GetConnectionString("LettsokDb"));
        });

   
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
            options.RoutePrefix = string.Empty;
        });

        app.MapGrpcService<AdvertisementService>();
        app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");


        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        
        await using (var scope = app.Services.CreateAsyncScope())
        {
            var DbContext = scope.ServiceProvider.GetService<JobListingsDbContext>();
            await DbContext.Database.MigrateAsync();
        }
        

       

        app.Run();
    }
}