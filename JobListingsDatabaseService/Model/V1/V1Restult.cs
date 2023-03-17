﻿using System;
using System.Text.Json.Serialization;

namespace JobListingsDatabaseService.Model.V1
{
	public class V1Restult<T>
	{
        public V1Restult()
        {
        }

        public V1Restult(T value)
		{
			Value = value;
		}

		public List<string> Errors { get; set; } = new List<string>();

		[JsonIgnore]
		public bool HasErrors => Errors.Any();
		
		public T Value { get; set; }
	}
}
