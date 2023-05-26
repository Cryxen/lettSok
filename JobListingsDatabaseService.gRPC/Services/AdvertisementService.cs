using System;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using JobListingsDatabaseService.Data;
using JobListingsDatabaseService.gRPC.Model;
using Microsoft.EntityFrameworkCore;
using Advertisement = JobListingsDatabaseService.Data.Advertisement;

namespace JobListingsDatabaseService.gRPC.Services
{
	public class AdvertisementService : AdvertisementgRPC.AdvertisementgRPCBase
    {
		private readonly ILogger<AdvertisementService> _logger;
        private readonly JobListingsDbContext _lettsokDbContext;

        public AdvertisementService(ILogger<AdvertisementService> logger, JobListingsDbContext lettsokDbContext)
        {
            _logger = logger;
            _lettsokDbContext = lettsokDbContext;
        }

        public override Task<AdvertisementModel> getAdvertisement(getAdvertisementParams request, ServerCallContext context)
        {
            var Time = Timestamp.FromDateTimeOffset(DateTime.Now);
            AdvertisementModel Advertisement = new AdvertisementModel()
            {
                Uuid = "Advertisement Uuid",
                Expires = Time,
                WorkLocation = "Oslo",
                Title = "Title",
                Description = "Description",
                JobTitle = "Job Title",
                Employer = "Employer",
                EngagementType = "100%"
            };
            return Task.FromResult(Advertisement);
        }

        public override async Task<getAdvertisementParams> postAdvertisements(AdvertisementModel request, ServerCallContext context)
        {

            var AdvertisementsInDatabase = await _lettsokDbContext.Advertisements
            .Select(advertisement => new JobListingsDatabaseService.gRPC.Model.Advertisement
            {
                Uuid = advertisement.Uuid
            })
            .ToListAsync();

            getAdvertisementParams EmptyParams = new();

            _logger.LogInformation("Advertisement " + request.Title + " uuid: " + request.Uuid + " expires: " + request.Expires);

            var Advertisement = new Advertisement()
            {
                Uuid = request.Uuid,
                Employer = request.Employer,
                Municipal = request.WorkLocation,
                Title = request.Title,
                Description = request.Description,
                JobTitle = request.JobTitle,
                EngagementType = request.EngagementType,
                Expires = request.Expires.ToDateTime()
            };

            if(AdvertisementsInDatabase.Any(item => item.Uuid == Advertisement.Uuid))
                {
                _logger.LogDebug("Advertisement {0} already exists in Database, time: {time} ", Advertisement.Uuid, DateTimeOffset.Now);
            }
            else
            {

                _lettsokDbContext.Add(Advertisement);
                await _lettsokDbContext.SaveChangesAsync();

            }

            
            return EmptyParams;
        }


    }
}

