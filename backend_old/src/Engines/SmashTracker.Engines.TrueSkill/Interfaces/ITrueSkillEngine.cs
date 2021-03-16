using System;
using System.Collections.Generic;
using System.Text;

namespace SmashTracker.Engines.TrueSkill.Interfaces
{
	public interface ITrueSkillEngine
	{
		/// <summary>
		/// Gets the default rating for the given settings.
		/// </summary>
		/// <returns>New rating with default values</returns>
		Rating GetDefaultRating();

		/// <summary>
		/// Gets draw margin for the given settings.
		/// </summary>
		/// <returns>Draw margin derived from given draw probability</returns>
		double GetDrawMargin();

		/// <summary>
		/// Updates the ratings for each player based on the result of a match.
		/// </summary>
		/// <param name="winner">Rating representing the winning player.</param>
		/// <param name="loser">Rating representing the losing player.</param>
		void UpdateRatings(IRating winner, IRating loser);

		/// <summary>
		/// Gets a number representing the current conservative skill value of the given rating.
		/// </summary>
		/// <param name="rating">Rating to get the conservative skill value for.</param>
		/// <returns></returns>
		double GetConservativeRating(IRating rating);
	}
}
