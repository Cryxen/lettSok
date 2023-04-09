using System;
using System.Text.Json.Serialization;
using JobListingsDatabaseService.Data;

namespace UserPreferencesDatabaseService.Model.V3
{
	public class V3User
	{
        [JsonIgnore]
        public Guid Id { get; set; }

		public string Name { get; set; }
    }
	
}

