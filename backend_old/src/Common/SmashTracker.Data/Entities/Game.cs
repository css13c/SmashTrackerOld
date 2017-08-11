using SmashTracker.Data.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;

namespace SmashTracker.Data.Entities
{
    // This table is currently manually curated.
    // TODO: Add SQL queries with initial data
    public class Game : IEntity
	{
		public Guid Id { get; set; }

        [MaxLength(100)]
		public string Name { get; set; }
	}
}
