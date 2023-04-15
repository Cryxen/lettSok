using System;

namespace UserPreferencesDatabaseService.Model.V2
{
	public class V2User
	{
		public Guid Id { get; set; }

		public string Name { get; set; }

        //public ICollection<UninterestedAdvertisement>? uninterestedAdvertisements { get; set; }
        //public ICollection<InterestedAdvertisement>? interestedAdvertisements { get; set; }
    }
	
}

