﻿using System;
namespace BlazorView.Data
{
	public class LoggedInUserService
	{
        public Guid Id { get; set; }

        public string Name { get; set; }

        public LoggedInUserService()
        {
            Name = "Standard Bruker";
        }
     
    }
}

