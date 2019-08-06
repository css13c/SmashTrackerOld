using SmashTracker.Data.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;

namespace SmashTracker.Data.Entities
{
    // This table is currently manually curated.
    // TODO: Add SQL queries with initial data
    public class GameCharacter : IEntity
	{
		public Guid Id { get; set; }

		public Guid GameId { get; set; }

        [MaxLength(50)]
		public string Name { get; set; }
	}
}
