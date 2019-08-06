using System;
using System.ComponentModel.DataAnnotations.Schema;

using SmashTracker.Data.Interfaces;

using SmashTracker.Engines.TrueSkill;

namespace SmashTracker.Data.Entities
{
	public class GamePlayerAssociation : IEntity, IAuditable
	{
		public Guid Id { get; set; }

		public Guid PlayerId { get; set; }

        [ForeignKey("Player")]
		public virtual Player Player { get; set; }

		public Guid GameId { get; set; }

		public double RatingMean { get; set; }

		public double RatingStandardDeviation { get; set; }

		public DateTimeOffset? CreatedOn { get; set; }

		public DateTimeOffset? ModifiedOn { get; set; }

		public Rating GetRating()
		{
			return new Rating(RatingMean, RatingStandardDeviation);
		}

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
