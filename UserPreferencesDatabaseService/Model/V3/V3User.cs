﻿using System;
using JobListingsDatabaseService.Data;

namespace UserPreferencesDatabaseService.Model.V3
{
	public class V3User
	{
		public Guid Id { get; set; }

		public string Name { get; set; }

        //public ICollection<UninterestedAdvertisement>? uninterestedAdvertisements { get; set; }
        //public ICollection<InterestedAdvertisement>? interestedAdvertisements { get; set; }
    }
	
}

