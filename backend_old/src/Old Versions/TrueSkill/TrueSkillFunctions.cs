using SmashTracker.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrueSkill
{
	public static class TrueSkillFunctions
	{
		// Both functions here assume non-draw, as draws are impossible in the game this is modeled for.
		// Update functions come from https://www.microsoft.com/en-us/research/project/trueskill-ranking-system/?from=http%3A%2F%2Fresearch.microsoft.com%2Fen-us%2Fprojects%2Ftrueskill%2Fdetails.aspx

		public static double VFunction(double drawMargin, double performanceDiff)
		{
			var denom = GaussianDistribution.NormCDF(performanceDiff - drawMargin);

			// Number taken from https://github.com/moserware/Skills/blob/master/Skills/TrueSkill/TruncatedGaussianCorrectionFunctions.cs#L28, seems to be a minimum value
			if (denom < 2.222758749e-162)
				return -performanceDiff + drawMargin;

			return GaussianDistribution.NormValueAt(performanceDiff - drawMargin) / denom;
		}

		public static double WFunction(double drawMargin, double performanceDiff)
		{
			var denom = GaussianDistribution.NormCDF(performanceDiff - drawMargin);

			// See above
			if(denom < 2.222758749e-162)
			{
				if (performanceDiff < 0)
					return 1;

				return 0;
			}

			var factor1 = VFunction(drawMargin, performanceDiff);
			var factor2 = factor1 + performanceDiff - drawMargin;

			return factor1 * factor2;
		}

		public static void UpdateRating(this Rating selfRating, bool won, Rating opponentRating)
		{
			var drawMargin = TrueSkillSettings.GetDrawMargin();
			var c = Math.Sqrt(2 * TrueSkillSettings.Beta.Squared() + selfRating.StandardDeviation + opponentRating.StandardDeviation);

			var skillDiff = selfRating.Mean - opponentRating.Mean;
			var resultMult = 1;

			if (!won)
			{
				skillDiff = opponentRating.Mean - selfRating.Mean;
				resultMult = -1;
			}

			// The dynamics is added to ensure the sd and mean never stagnate.
			var dynamicsVariance = selfRating.StandardDeviation.Squared() + TrueSkillSettings.DynamicsFactor.Squared();
			var meanMult = dynamicsVariance / c;
			var sdMult = dynamicsVariance / c.Squared();
			var v = VFunction(skillDiff / c, drawMargin / c);
			var w = WFunction(skillDiff / c, drawMargin / c);

			var newMean = selfRating.Mean + (resultMult * meanMult * v);
			var newSd = Math.Sqrt(dynamicsVariance * (1 - sdMult * w));

			selfRating = new Rating(newMean, newSd);
		}
	}
}
