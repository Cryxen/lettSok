using System;
namespace BlazorView.Data

{
	public class LoggedInUserService
	{
        public Guid Id { get; set; }

        public string Name { get; set; }

        public List<string> interests { get; set; }
        public List<string> uninterests { get; set; }
    }
}

