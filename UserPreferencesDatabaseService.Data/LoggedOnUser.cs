using System;
namespace UserPreferencesDatabaseService.Data
{
	public class LoggedOnUser
	{
		public int Id { get; set; }

		public User user { get; set; }

        public Guid UserId { get; set; }

    }
}

