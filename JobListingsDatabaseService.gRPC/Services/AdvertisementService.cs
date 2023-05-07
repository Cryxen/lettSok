using System;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using JobListingsDatabaseService.Data;
using JobListingsDatabaseService.Model.V2;

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
            AdvertisementModel advertisement = new AdvertisementModel()
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
            return Task.FromResult(advertisement);
        }

        public override async Task<getAdvertisementParams> postAdvertisements(AdvertisementModel request, ServerCallContext context)
        {
            getAdvertisementParams emptyParams = new();



            _logger.LogInformation("Advertisement " + request.Title + " uuid: " + request.Uuid + " expires: " + request.Expires);


            V2Employer v2Employer = new()
            {
                name = request.Employer
            };
            List<V2WorkLocation> workLocations = new();

            V2WorkLocation workLocation = new()
            {
                municipal = request.WorkLocation
            };
            workLocations.Add(workLocation);



            V2Advertisement v2Advertisement = new V2Advertisement()
            {
                Uuid = request.Title,
                Employer = v2Employer,
                workLocations = workLocations,
                Title = request.Title,
                Description = request.Description,
                JobTitle = request.JobTitle,
                EngagementType = request.EngagementType,
                Expires = request.Expires.ToDateTime()
            };

            var advertisement = new Advertisement()
            {
                Uuid = request.Title,
                Employer = request.Employer,
                Municipal = request.WorkLocation,
                Title = request.Title,
                Description = request.Description,
                JobTitle = request.JobTitle,
                EngagementType = request.EngagementType,
                Expires = request.Expires.ToDateTime()
            };


            _lettsokDbContext.Add(advertisement);
            await _lettsokDbContext.SaveChangesAsync();

            return emptyParams;
        }


    }
}

