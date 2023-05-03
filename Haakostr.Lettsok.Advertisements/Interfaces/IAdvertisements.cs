﻿using System;
using Advertisements.Model.V1;
using Advertisements.Model.V2;
using Microsoft.AspNetCore.Mvc;

namespace Advertisements.Interfaces
{
	public interface IAdvertisements
	{
		static HttpClient client { get; set; }


        Task<List<V2Advertisement>> GetJobs();
        Task<List<V2Advertisement>> GetJobsByLocation(string location);
	}
}
