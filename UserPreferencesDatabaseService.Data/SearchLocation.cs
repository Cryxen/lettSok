using System;

namespace UserPreferencesDatabaseService.Data
{
	public class SearchLocation
	{
		public int LocationId { get; set; }
		public Location Location { get; set; }
		public Guid UserId { get; set; }
		public User User { get; set; }
		public int Id { get; set; }

	}
}

