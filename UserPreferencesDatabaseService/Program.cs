using Microsoft.EntityFrameworkCore;
using UserPreferencesDatabaseService.Data;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();


        builder.Services.AddDbContext<UserPreferencesDbContext>(options =>
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
            var dbContext = scope.ServiceProvider.GetService<UserPreferencesDbContext>();
            await dbContext.Database.MigrateAsync(); //Migrations doesn't work for some reason TODO: Find out why.
        }

        app.Run();
    }
}