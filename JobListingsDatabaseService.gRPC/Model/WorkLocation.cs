using System;
namespace JobListingsDatabaseService.gRPC.Model;


    public class WorkLocation
    {
        public string? Country { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? PostalCode { get; set; }
        public string? County { get; set; }
        public string? Municipal { get; set; }
    }


