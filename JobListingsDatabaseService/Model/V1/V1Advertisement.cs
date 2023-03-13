using System;
namespace Haakostr.Lettsok.JobListingsDatabaseController.Model.V1
{
	public class V1Advertisement
	{
		public string? Uuid
		{
			get;
			set;
		}
		public string? Expires
		{
			get;
			set;
		}
		public string? Municipal
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
		public string? Employer
		{
			get;
			set;
		}
		public string? EngagementType
		{
			get;
			set;
		}

        /*
         * Virtual er funnet fra:
         *	https://learn.microsoft.com/en-us/ef/ef6/modeling/code-first/workflows/new-database  
         */
        public virtual List<V1Advertisement>? V1Advertisements { get; set; }

    }
}

