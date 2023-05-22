using JobListingsDatabaseService.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

internal class Program
{
    private static async Task Main(string[] args)
    {

    var builder = WebApplication.CreateBuilder(args);
        // Add services to the container.

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
        });


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


        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        
        await using (var scope = app.Services.CreateAsyncScope())
        {
            var dbContext = scope.ServiceProvider.GetService<JobListingsDbContext>();
            await dbContext.Database.MigrateAsync();
        }
        

       

        app.Run();
    }
}