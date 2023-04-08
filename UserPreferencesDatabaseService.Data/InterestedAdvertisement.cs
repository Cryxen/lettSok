using System;
namespace JobListingsDatabaseService.Data
{
    public class InterestedAdvertisement
    {
        public User User { get; set; }
        public Guid UserGuid { get; set; }
        public string AdvertisementUuid { get; set; }
    }
}

