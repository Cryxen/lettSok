using System;
using JobListingsDatabaseService.Data;

namespace UserPreferencesDatabaseService.Data
{
	public class SearchLocation
	{
        public int Id { get; set; }

        public string Location { get; set; }

        public List<User> Users { get; } = new();

    }
}

