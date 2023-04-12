using System;
namespace BlazorView.Data
{
	public class Advertisement
	{
		public Advertisement()
		{
		}

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

        public override string ToString()
        {
            return base.ToString();
        }
    }
}

