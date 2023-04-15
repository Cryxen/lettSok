using System;
namespace UserPreferencesDatabaseService.Data
{
    public class UninterestedAdvertisement
    {
        public int Id { get; set; }

        public Guid UserId { get; set; }

        public string AdvertisementUuid { get; set; }

        public User User { get; set; }
    }
}

