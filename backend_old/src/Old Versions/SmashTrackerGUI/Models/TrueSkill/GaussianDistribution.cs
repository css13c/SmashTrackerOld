using SmashTracker.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmashTrackerGUI.Models.TrueSkill
{
	// Class created using https://github.com/moserware/Skills/blob/master/Skills/Numerics/GaussianDistribution.cs as a guide.
	public class GaussianDistribution
	{
		public GaussianDistribution(double mean, double standardDeviation)
		{
			Mean = mean;
			StandardDeviation = standardDeviation;
			Variance = standardDeviation.Squared();
			Precision = 1 / Variance;
			PrecisionMean = Precision * Mean;
		}

		// Only used internally to create new Distributions
		private GaussianDistribution(double mean, double standardDeviation, double variance, double precision, double precisionMean)
		{
			Mean = mean;
			StandardDeviation = standardDeviation;
			Variance = variance;
			Precision = precision;
			PrecisionMean = precisionMean;
		}

		// Precision is used here to simplify multiplication and addition of Gaussian Distributions. See http://www.moserware.com/assets/computing-your-skill/The%20Math%20Behind%20TrueSkill.pdf for more info.
		public double Mean { get; private set; }
		public double StandardDeviation { get; private set; }
		public double Precision { get; private set; }
		public double PrecisionMean { get; private set; }
		public double Variance { get; private set; }

		// This makes the area under the graph equal to 1.
		public double NormalizationConstant => 1 / (Math.Sqrt(2 * Math.PI) * StandardDeviation);

		public static double CDF(double x, double mean, double sd)
		{
			return (1 + ERF((x-mean) / (sd * Math.Sqrt(2)))) / 2;
		}

		// This returns the CDF of a normal gaussian curve
		public static double NormCDF(double x)
		{
			return CDF(x, 0, 1);
		}

		public static double ICDF(double x, double mean, double sd)
		{
			return mean - (Math.Sqrt(2) * sd * IERF(2 * x));
		}

		// Derived from page 265 of Numerical Recipes 3rd Edition
		public static double ERF(double x)
		{
			var z = Math.Abs(x);
			var t = 2 / (2 + z);
			var ty = 4 * t - 2;

			double d = 0;
			double dd = 0;

			for(int j=ERFCoefficients.Length-1; j > 0; j--)
			{
				double temp = d;
				d = ty * d - dd + ERFCoefficients[j];
				dd = temp;
			}

			var result = t * Math.Exp(-z * z + 0.5 * (ERFCoefficients[0] + ty * d) - dd);

			return x >= 0 ? result : 2 - result;
		}

		// Derived from page 265 of Numerical Recipes 3rd Edition
		public static double IERF(double p)
		{
			if (p >= 2)
				return -100;

			if (p <= 0)
				return 100;

			var pp = p < 1 ? p : 2 - p;
			var t = Math.Sqrt(-2 * Math.Log(pp / 2));
			var x = -0.70711 * ((2.30753 + t * 0.27061) / (1 + t * (0.992299 + t * 0.04481)) - t);
			for (int j=0; j<2; j++)
			{
				var err = ERF(x) - pp;
				x += err / (1.12837916709551257 * Math.Exp(-(x * x)) - x * err);
			}

			return p < 1 ? x : -x;
		}

		// Assumes a standard normal distribution, where mean = 0, sd = 1.
		public static double NormValueAt(double x)
		{
			var fraction = 1 / Math.Sqrt(2 * Math.PI);
			var multFactor = Math.Exp(-0.5 * x.Squared());
			return fraction * multFactor;
		}

		public static double[] ERFCoefficients = { -1.3026537197817094,
			6.4196979235649026e-1, 1.9476473204185836e-2,-9.561514786808631e-3,
			-9.46595344482036e-4, 3.66839497852761e-4, 4.2523324806907e-5,
			-2.0278578112534e-5, -1.624290004647e-6, 1.303655835580e-6,
			1.5626441722e-8, -8.5238095915e-8, 6.529054439e-9, 5.059343495e-9,
			-9.91364156e-10, -2.27365122e-10, 9.6467911e-11, 2.394038e-12,
			-6.886027e-12, 8.94487e-13, 3.13092e-13, -1.12708e-13, 3.81e-16,
			7.106e-15, -1.523e-15, -9.4e-17, 1.21e-16, -2.8e-17
		};

		// Below functions are not needed for basic form of TrueSkill, but are being kept in case I decide to expand the implementation.
		public static GaussianDistribution GaussianFromPrecisionMean(double precisionMean, double precision)
		{
			double variance = 1 / precision;

			return new GaussianDistribution(precisionMean / precision, Math.Sqrt(variance), variance, precision, precisionMean);
		}

		public static GaussianDistribution operator *(GaussianDistribution x, GaussianDistribution y)
		{
			return GaussianFromPrecisionMean(x.PrecisionMean + y.PrecisionMean, x.Precision + y.Precision);
		}

		public static double operator -(GaussianDistribution x, GaussianDistribution y)
		{
			return Math.Max(Math.Abs(x.PrecisionMean - y.PrecisionMean), Math.Sqrt(Math.Abs(x.Precision - y.Precision)));
		}

		public static GaussianDistribution operator /(GaussianDistribution top, GaussianDistribution bottom)
		{
			return GaussianFromPrecisionMean(top.PrecisionMean - bottom.PrecisionMean, top.Precision - bottom.Precision);
		}
	}
}
