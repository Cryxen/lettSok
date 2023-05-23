using System;
namespace JobListingsDatabaseService.Model.V2;

    public class V2Advertisement
	{
		public string? Uuid
		{
			get;
			set;
		}
		public DateTime Expires
		{
			get;
			set;
		}
		public List<V2WorkLocation>? WorkLocations
		{
			get;
			set;
		}
		public string? Title
		{
			get;
			set;
		}
		public string? Description
		{
			get;
			set;
		}
		public string? JobTitle
		{
			get;
			set;
		}
		public V2Employer? Employer
		{
			get;
			set;
		}
		public string? EngagementType
		{
			get;
			set;
		}
	


    }
	


