using System;
namespace JobListingsDatabaseService.Data
{
    public class UninterestedAdvertisement
    {
        public Advertisement Advertisement { get; set; }
        public User User { get; set; }
        public Guid UserGuid { get; set; }
        public string AdvertisementUuid { get; set; }
    }
}

