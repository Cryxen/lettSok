using System;
namespace BlazorView.Data

{
	public static class LoggedInUserService
	{
        public static Guid Id { get; set; }

        public static string Name { get; set; }

        public static List<string> Interests { get; set; }
        public static List<string> Uninterests { get; set; }

        private static HttpClient s_client = new HttpClient();

    }
}

