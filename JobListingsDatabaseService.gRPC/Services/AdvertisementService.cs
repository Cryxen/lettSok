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

        public override Task<AdvertisementModel> GetAdvertisement(getAdvertisementParams request, ServerCallContext context)
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

        public override async Task<getAdvertisementParams> PostAdvertisements(AdvertisementModel request, ServerCallContext context)
        {
            getAdvertisementParams EmptyParams = new();



            _logger.LogInformation("Advertisement " + request.Title + " uuid: " + request.Uuid + " expires: " + request.Expires);


            V2Employer V2Employer = new()
            {
                Name = request.Employer
            };
            List<V2WorkLocation> WorkLocations = new();

            V2WorkLocation WorkLocation = new()
            {
                Municipal = request.WorkLocation
            };
            WorkLocations.Add(WorkLocation);


            //TODO: Find out what this is for.
            V2Advertisement V2Advertisement = new V2Advertisement()
            {
                Uuid = request.Title,
                Employer = V2Employer,
                WorkLocations = WorkLocations,
                Title = request.Title,
                Description = request.Description,
                JobTitle = request.JobTitle,
                EngagementType = request.EngagementType,
                Expires = request.Expires.ToDateTime()
            };

            var Advertisement = new Advertisement()
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

            
            _lettsokDbContext.Add(Advertisement);
            await _lettsokDbContext.SaveChangesAsync();

            return EmptyParams;
        }


    }
}

