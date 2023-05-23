using System;
namespace JobListingsDatabaseService.gRPC.Model;

	public class Employer
	{
        public string? Name { get; set; }
        public int? Orgnr { get; set; }
        public string? Description { get; set; }
        public string? Homepage { get; set; }
    }


