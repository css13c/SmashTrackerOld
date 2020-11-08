using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

using SmashTracker.Data.Interfaces;

namespace SmashTracker.Data.Entities
{
    public class GamePlayerAssociation : IEntity, IAuditable
    {
        public Guid Id { get; set; }

        public Guid PlayerId { get; set; }
        public Player Player { get; set; }

        public Guid GameId { get; set; }
        public Game Game { get; set; }

        public List<GamePlayerCharacter> Characters { get; set; }

        public double RatingMean { get; set; }

        public double RatingStandardDeviation { get; set; }

        public DateTimeOffset? CreatedOn { get; set; }

        public DateTimeOffset? ModifiedOn { get; set; }

        public string GetRatingString()
        {
            return $"μ={RatingMean}, σ={RatingStandardDeviation}";
        }

        public string GetPlayerInfoString()
        {
            return $"Name: {Player.FirstName} {Player.LastName}, Rating: {GetRatingString()}";
        }
    }
}
