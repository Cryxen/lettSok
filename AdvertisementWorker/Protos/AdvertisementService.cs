using System;
using Grpc.Core;

namespace JobListingsDatabaseService.gRPC.Services
{
	public class AdvertisementService : Advertisement.AdvertisementBase
	{
		private readonly ILogger<AdvertisementService> _logger;

        public AdvertisementService(ILogger<AdvertisementService> logger)
        {
            _logger = logger;
        }


        public override Task<AdvertisementModel> getAdvertisement(getAdvertisementParams request, ServerCallContext context)
        {
            AdvertisementModel advertisement = new AdvertisementModel()
            {
                Uuid = "Advertisement Uuid",
                Expires = "Date",
                WorkLocation = "Oslo",
                Title = "Title",
                Description = "Description",
                JobTitle = "Job Title",
                Employer = "Employer",
                EngagementType = "100%"
            };
            return Task.FromResult(advertisement);
        }
    }
}

