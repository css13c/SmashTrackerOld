using SmashTracker.Common;
using SmashTracker.Engines.TrueSkill.Helpers;
using SmashTracker.Engines.TrueSkill.Interfaces;
using System;

namespace SmashTracker.Engines.TrueSkill
{
	public class TrueSkillEngine : ITrueSkillEngine
	{
		// TODO: Determine defaults to be used within the application.
		#region DefaultProperties
		private const double DefaultInitialMean = 25.0;

		private const double DefaultInitialStandardDeviation = DefaultInitialMean / 3;

		private const double DefaultDrawProbability = 0.1;

		private const double DefaultConservativeRatingMulitplier = 3;

		private const double DefaultDynamicsFactor = DefaultInitialMean / 300;

		private const double DefaultBeta = DefaultInitialMean / 6;
		#endregion

		#region Properties
		private readonly double InitialMean;

		private readonly double InitialStandardDeviation;

		private readonly double DrawProbability;

		private readonly double ConservativeRatingMultiplier;

		private readonly double DynamicsFactor;

		private readonly double Beta;
		#endregion

		#region Constructor
		/// <summary>
		/// Creates TrueSkillEngine using default values.
		/// </summary>
		public TrueSkillEngine()
		{
			InitialMean = DefaultInitialMean;
			InitialStandardDeviation = DefaultInitialStandardDeviation;
			DrawProbability = DefaultDrawProbability;
			ConservativeRatingMultiplier = DefaultConservativeRatingMulitplier;
			DynamicsFactor = DefaultDynamicsFactor;
			Beta = DefaultBeta;
		}

		// TODO: Add descriptions for dynamics factor and beta
		/// <summary>
		/// Creates TrueSkillEngine using specified values, or defaults where no value is passed.
		/// </summary>
		/// <param name="initialMean">The initial mean of default ratings created.</param>
		/// <param name="initialStandardDeviation">The initial standard deviation of default ratings created.</param>
		/// <param name="drawProbability">The probability of a draw in the game given.</param>
		/// <param name="conservativeRatingMultiplier">The number of standard deviations from the mean to use for a conservative skill value.</param>
		/// <param name="dynamicsFactor"></param>
		/// <param name="beta"></param>
		public TrueSkillEngine(double initialMean = DefaultInitialMean,
							 double initialStandardDeviation = DefaultInitialStandardDeviation,
							 double conservativeRatingMultiplier = DefaultConservativeRatingMulitplier,
							 double drawProbability = DefaultDrawProbability,
							 double dynamicsFactor = DefaultDynamicsFactor,
							 double beta = DefaultBeta)
		{
			InitialMean = initialMean;
			InitialStandardDeviation = initialStandardDeviation;
			DrawProbability = drawProbability;
			ConservativeRatingMultiplier = conservativeRatingMultiplier;
			DynamicsFactor = dynamicsFactor;
			Beta = beta;
		}
		#endregion

		#region True Skill Helper Functions
		// Both functions here assume non-draw, as draws are impossible in the game this is modeled for.
		// Update functions come from https://www.microsoft.com/en-us/research/project/trueskill-ranking-system/?from=http%3A%2F%2Fresearch.microsoft.com%2Fen-us%2Fprojects%2Ftrueskill%2Fdetails.aspx

		private double VFunction(double drawMargin, double performanceDiff)
		{
			var denom = GaussianDistribution.NormCDF(performanceDiff - drawMargin);

			// Number taken from https://github.com/moserware/Skills/blob/master/Skills/TrueSkill/TruncatedGaussianCorrectionFunctions.cs#L28, seems to be a minimum value
			if (denom < 2.222758749e-162)
				return -performanceDiff + drawMargin;

			return GaussianDistribution.NormValueAt(performanceDiff - drawMargin) / denom;
		}

		private double WFunction(double drawMargin, double performanceDiff)
		{
			var denom = GaussianDistribution.NormCDF(performanceDiff - drawMargin);

			// See above
			if (denom < 2.222758749e-162)
			{
				if (performanceDiff < 0)
					return 1;

				return 0;
			}

			var factor1 = VFunction(drawMargin, performanceDiff);
			var factor2 = factor1 + performanceDiff - drawMargin;

			return factor1 * factor2;
		}
		#endregion

		#region Interface Functions
		public Rating GetDefaultRating()
		{
			return new Rating(InitialMean, InitialStandardDeviation);
		}

		// This gets the draw margin from the Draw Probability. Formula pulled from http://www.moserware.com/assets/computing-your-skill/The%20Math%20Behind%20TrueSkill.pdf, page 15
		public double GetDrawMargin() => GaussianDistribution.ICDF(1/2, 0, 1) * Math.Sqrt(2) * Beta;

		public void UpdateRatings(IRating winner, IRating loser)
		{
			var drawMargin = GetDrawMargin();
			var c = Math.Sqrt(2 * Beta.Squared() + winner.StandardDeviation + loser.StandardDeviation);
			var skillDiff = winner.Mean - loser.Mean;

			// The dynamics is added to ensure the sd and mean never stagnate.
			var dynamicsVariance = winner.StandardDeviation.Squared() + DynamicsFactor.Squared();
			var meanMult = dynamicsVariance / c;
			var sdMult = dynamicsVariance / c.Squared();
			var v = VFunction(skillDiff / c, drawMargin / c);
			var w = WFunction(skillDiff / c, drawMargin / c);

			winner.Mean += meanMult * v;
			winner.StandardDeviation = Math.Sqrt(dynamicsVariance * (1 - sdMult * w));
			loser.Mean -= meanMult * v;
			loser.StandardDeviation = Math.Sqrt(dynamicsVariance * (1 - sdMult * w));
		}

		public double GetConservativeRating(IRating rating)
		{
			return rating.Mean - ConservativeRatingMultiplier * rating.StandardDeviation;
		}

		#endregion
	}
}
