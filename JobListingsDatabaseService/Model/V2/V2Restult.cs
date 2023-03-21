using System;
using System.Text.Json.Serialization;

namespace JobListingsDatabaseService.Model.V2
{
	public class V2Restult<T>
	{
        public V2Restult()
        {
        }

        public V2Restult(T value)
		{
			Value = value;
		}

		public List<string> Errors { get; set; } = new List<string>();

		[JsonIgnore]
		public bool HasErrors => Errors.Any();
		
		public T Value { get; set; }
	}
}

