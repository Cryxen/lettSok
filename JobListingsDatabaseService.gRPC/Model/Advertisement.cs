﻿using System;
namespace JobListingsDatabaseService.gRPC.Model;

    public class Advertisement
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
		public List<WorkLocation>? WorkLocations
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
		public Employer? Employer
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
	


