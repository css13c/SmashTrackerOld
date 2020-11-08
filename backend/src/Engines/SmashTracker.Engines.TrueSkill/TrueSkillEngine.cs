using SmashTracker.Common;
using SmashTracker.Engines.TrueSkill.Helpers;
using SmashTracker.Engines.TrueSkill.Interfaces;
using System;

namespace SmashTracker.Engines.TrueSkill
{
    /// <summary>
    /// Handles all TrueSkill calculations for a two-player or two-team game.
    /// </summary>
    /// <remarks>
    /// Source: https://github.com/moserware/Skills/blob/master/Skills/TrueSkill/TruncatedGaussianCorrectionFunctions.cs
    /// Slightly modified to fit project
    /// </remarks>
    public class TwoPlayerTrueSkillEngine : ITrueSkillEngine
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
        public TwoPlayerTrueSkillEngine()
        {
            InitialMean = DefaultInitialMean;
            InitialStandardDeviation = DefaultInitialStandardDeviation;
            DrawProbability = DefaultDrawProbability;
            ConservativeRatingMultiplier = DefaultConservativeRatingMulitplier;
            DynamicsFactor = DefaultDynamicsFactor;
            Beta = DefaultBeta;
        }

        /// <summary>
        /// Creates TrueSkillEngine using specified values, or defaults where no value is passed.
        /// </summary>
        /// <param name="initialMean">The initial mean of default ratings created.</param>
        /// <param name="initialStandardDeviation">The initial standard deviation of default ratings created.</param>
        /// <param name="drawProbability">The probability of a draw in the game given.</param>
        /// <param name="conservativeRatingMultiplier">The number of standard deviations from the mean to use for a conservative skill value.</param>
        /// <param name="dynamicsFactor"></param>
        /// <param name="beta"></param>
        public TwoPlayerTrueSkillEngine(double initialMean = DefaultInitialMean,
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
        private double VWithinMargin(double drawMargin, double performanceDiff, double c) => VWithinMargin(drawMargin / c, performanceDiff / c);
        private double VWithinMargin(double drawMargin, double performanceDiff)
        {
            var performanceDiffAbsoluteValue = Math.Abs(performanceDiff);
            var denominator = GaussianDistribution.CumulativeTo(drawMargin - performanceDiffAbsoluteValue) - GaussianDistribution.CumulativeTo(-drawMargin - performanceDiffAbsoluteValue);
            if (denominator < 2.222758749e-162)
            {
                return performanceDiff < 0 ? -performanceDiff - drawMargin : -performanceDiff + drawMargin;
            }

            var numerator = GaussianDistribution.At(-drawMargin - performanceDiffAbsoluteValue) - GaussianDistribution.At(drawMargin - performanceDiffAbsoluteValue);
            return performanceDiff < 0 ? -numerator / denominator : numerator / denominator;
        }

        private double VExceedsMargin(double drawMargin, double performanceDiff, double c) => VWithinMargin(drawMargin / c, performanceDiff / c);
        private double VExceedsMargin(double drawMargin, double performanceDiff)
        {
            var denominator = GaussianDistribution.CumulativeTo(performanceDiff - drawMargin);
            if (denominator < 2.222758749e-162)
            {
                return -performanceDiff + drawMargin;
            }

            return GaussianDistribution.At(performanceDiff - drawMargin) / denominator;
        }

        private double WWithinMargin(double drawMargin, double performanceDiff, double c) => WWithinMargin(drawMargin / c, performanceDiff / c);
        private double WWithinMargin(double drawMargin, double performanceDiff)
        {
            var performanceDiffAbsoluteValue = Math.Abs(performanceDiff);
            var denominator = GaussianDistribution.CumulativeTo(drawMargin - performanceDiffAbsoluteValue) - GaussianDistribution.CumulativeTo(-drawMargin - performanceDiffAbsoluteValue);
            if (denominator < 2.222758749e-162)
            {
                return 1;
            }

            var vt = VWithinMargin(drawMargin, performanceDiff);

            // Separating large return math shown in original work for readability
            var factor1 = drawMargin - performanceDiffAbsoluteValue;
            var factor2 = GaussianDistribution.At(drawMargin - performanceDiffAbsoluteValue);
            var factor3 = -drawMargin - performanceDiffAbsoluteValue;
            var factor4 = GaussianDistribution.At(-drawMargin - performanceDiffAbsoluteValue);
            var numerator = (factor1 * factor2) - (factor3 * factor4);

            return (vt * vt) + (numerator / denominator);
        }

        private double WExceedsMargin(double drawMargin, double performanceDiff, double c) => WExceedsMargin(drawMargin / c, performanceDiff / c);
        private double WExceedsMargin(double drawMargin, double performanceDiff)
        {
            var denominator = GaussianDistribution.CumulativeTo(performanceDiff - drawMargin);
            if (denominator < 2.222758749e-162)
            {
                return performanceDiff < 0 ? 1 : 0;
            }

            var vWin = VExceedsMargin(performanceDiff, drawMargin);
            return vWin*(vWin + performanceDiff - drawMargin);
        }
        #endregion

        #region Interface Functions
        public Rating GetDefaultRating()
        {
            return new Rating(InitialMean, InitialStandardDeviation);
        }

        // This gets the draw margin from the Draw Probability. Formula pulled from http://www.moserware.com/assets/computing-your-skill/The%20Math%20Behind%20TrueSkill.pdf, page 15
        public double GetDrawMargin() => GaussianDistribution.InverseCumulativeTo(1/2, 0, 1) * Math.Sqrt(2) * Beta;

        public void UpdateRatings(IRating winner, IRating loser)
        {
            var drawMargin = GetDrawMargin();
            var c = Math.Sqrt((2 * Beta.Squared()) + winner.StandardDeviation + loser.StandardDeviation);
            var skillDiff = winner.Mean - loser.Mean;

            // The dynamics is added to ensure the sd and mean never stagnate.
            var dynamicsVariance = winner.StandardDeviation.Squared() + DynamicsFactor.Squared();
            var meanMult = dynamicsVariance / c;
            var sdMult = dynamicsVariance / c.Squared();
            var v = VExceedsMargin(skillDiff, drawMargin, c);
            var w = WExceedsMargin(skillDiff, drawMargin, c);

            winner.Mean += meanMult * v;
            winner.StandardDeviation = Math.Sqrt(dynamicsVariance * (1 - (sdMult * w)));
            loser.Mean -= meanMult * v;
            loser.StandardDeviation = Math.Sqrt(dynamicsVariance * (1 - (sdMult * w)));
        }

        public double GetConservativeRating(IRating rating)
        {
            return rating.Mean - (ConservativeRatingMultiplier * rating.StandardDeviation);
        }

        #endregion
    }
}
