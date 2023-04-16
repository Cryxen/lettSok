using System;
namespace BlazorView.Data

{
	public static class LoggedInUserService
	{
        public static Guid Id { get; set; }

        public static string Name { get; set; }

        public static List<string> interests { get; set; }
        public static List<string> uninterests { get; set; }
    }
}

