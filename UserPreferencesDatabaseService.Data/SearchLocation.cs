using System;

namespace UserPreferencesDatabaseService.Data
{
	public class SearchLocation
	{
		public int LocationId { get; set; }
		public Location location { get; set; }
		public Guid UserId { get; set; }
		public User user { get; set; }
		public int Id { get; set; }

	}
}

