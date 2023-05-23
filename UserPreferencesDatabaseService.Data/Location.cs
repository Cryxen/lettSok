using System;

namespace UserPreferencesDatabaseService.Data
{
	public class Location
	{
        public int Id { get; set; }

        public string Municipality { get; set; }

        public ICollection<SearchLocation> SearchLocations { get; set; }
    }
}

