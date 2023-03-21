using System;
namespace JobListingsDatabaseService.Model.V2
{
	public class V2User
	{
		public Guid Id { get; set; }

		public string Name { get; set; }

		public List<V2Advertisement> Interested { get; set; }

		public List<V2Advertisement> Uninterested { get; set; }
	}
	
}

