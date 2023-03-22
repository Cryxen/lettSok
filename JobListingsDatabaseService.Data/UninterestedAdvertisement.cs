using System;
namespace JobListingsDatabaseService.Data
{
	public class UninterestedAdvertisement
	{
		public Guid UserGuid { get; set; }
		public User User { get; set; }
		public string AdvertisementUuid { get; set; }
		public Advertisement advertisement { get; set; }
	}
}

