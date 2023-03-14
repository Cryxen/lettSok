using System;
namespace JobListingsDatabaseService.Model.V1
{
	public class V1Restult<T>
	{
		public List<string> Errors { get; set; } = new List<string>();

		public bool HasErrors => Errors.Any();
		
		public T? Value { get; set; }
	}
}

