using System;
using JobListingsDatabaseService.Data;

namespace UserPreferencesDatabaseService.Data
{
	public class SearchLocation
	{
        public int Id { get; set; }

        public Guid UserId { get; set; }

        public string Location { get; set; }

        public User User { get; set; }
    }
}

