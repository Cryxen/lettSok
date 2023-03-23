using System;
using System.Text.Json.Serialization;

namespace UserPreferencesDatabaseService.Model.V2
{
	/// <summary>
	/// TODO: Find out if we can use this class from a different project,
	/// as it is a copy paste from JobListingsDatabaseService
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class V2Result<T>
	{
        public V2Result()
        {
        }

        public V2Result(T value)
		{
			Value = value;
		}

		public List<string> Errors { get; set; } = new List<string>();

		[JsonIgnore]
		public bool HasErrors => Errors.Any();
		
		public T Value { get; set; }
	}
}

