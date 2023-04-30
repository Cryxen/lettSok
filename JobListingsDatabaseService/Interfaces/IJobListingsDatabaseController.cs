using System;
using JobListingsDatabaseService.Model.V1;
using JobListingsDatabaseService.Model.V2;

namespace JobListingsDatabaseService.Interfaces
{
    // Generics: https://stackoverflow.com/a/1344724
    public interface IJobListingsDatabaseController
	{
		Task<List<V1Advertisement>> Get();

        Task<V1Restult<V2Advertisement>> saveAdvertisements(V2Advertisement advertisementPost);

	}
}

