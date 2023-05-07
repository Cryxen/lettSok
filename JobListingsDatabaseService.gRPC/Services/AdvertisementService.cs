using System;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace JobListingsDatabaseService.gRPC.Services
{
	public class AdvertisementService : AdvertisementgRPC.AdvertisementgRPCBase
    {
		private readonly ILogger<AdvertisementService> _logger;

        public AdvertisementService(ILogger<AdvertisementService> logger)
        {
            _logger = logger;
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

        public override Task<getAdvertisementParams> postAdvertisements(AdvertisementModel request, ServerCallContext context)
        {
            getAdvertisementParams emptyParams = new();

            _logger.LogInformation("Advertisement " + request.Title + " uuid: " + request.Uuid + " expires: " + request.Expires);

            return Task.FromResult(emptyParams);
        }


    }
}

