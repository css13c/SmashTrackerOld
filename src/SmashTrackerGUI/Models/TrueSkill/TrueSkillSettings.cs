using SmashTrackerGUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmashTrackerGUI.Models.TrueSkill
{
	public static class TrueSkillSettings
	{
		// Initial and Standard Values
		public const double Beta = InitialMean / 6;
		public const double DrawProbability = 0;
		public const double DynamicsFactor = InitialMean / 300;
		public const double InitialMean = 25;
		public const double InitialStandardDeviation = InitialMean / 3;

		public static Rating DefaultRating()
		{
			return new Rating(InitialMean,InitialStandardDeviation);
		}

		public static double GetDrawMargin()
		{
			// This gets the draw margin from the Draw Probability. Formula pulled from http://www.moserware.com/assets/computing-your-skill/The%20Math%20Behind%20TrueSkill.pdf, page 15
			return GaussianDistribution.ICDF(1 / 2, 0, 1) * Math.Sqrt(1 + 1) * Beta;
		}
	}
}
