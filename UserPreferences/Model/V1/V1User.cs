using System;
namespace UserPreferences.Model.V1
{
	public class V1User
	{
		public Guid Id { get; set; }

		public string Name { get; set; }

		public Guid Interested { get; set; }

		public Guid Uninterested { get; set; }
	}
	
}

