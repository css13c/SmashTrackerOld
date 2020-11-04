using SmashTracker.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SmashTracker.Engines.TrueSkill.Helpers
{
    // Class created using https://github.com/moserware/Skills/blob/master/Skills/Numerics/GaussianDistribution.cs
    // Slight changes made to match personal coding style
    public class GaussianDistribution
    {
        #region Properties

        #region Public Properties

        // Precision is used here to simplify multiplication and addition of Gaussian Distributions. See http://www.moserware.com/assets/computing-your-skill/The%20Math%20Behind%20TrueSkill.pdf for more info.
        public double Mean { get; private set; }
        public double StandardDeviation { get; private set; }
        public double Precision { get; private set; }
        public double PrecisionMean { get; private set; }
        public double Variance { get; private set; }

        #endregion

        #region Private Properties

        // This makes the area under the distribution equal to 1.
        private double NormalizationConstant => 1 / (Math.Sqrt(2 * Math.PI) * StandardDeviation);

        // Moserware has this as a defined double array in ErrorFunctionCumulativeTo(), but I decided to extract in case these can be used later.
        private static readonly ReadOnlyCollection<double> ErrorCumulativeToCoefficients = new List<double>
        {
            -1.3026537197817094,
            6.4196979235649026e-1,
            1.9476473204185836e-2,
            -9.561514786808631e-3,
            -9.46595344482036e-4,
            3.66839497852761e-4,
            4.2523324806907e-5,
            -2.0278578112534e-5,
            -1.624290004647e-6,
            1.303655835580e-6,
            1.5626441722e-8,
            -8.5238095915e-8,
            6.529054439e-9,
            5.059343495e-9,
            -9.91364156e-10,
            -2.27365122e-10,
            9.6467911e-11,
            2.394038e-12,
            -6.886027e-12,
            8.94487e-13,
            3.13092e-13,
            -1.12708e-13,
            3.81e-16,
            7.106e-15,
            -1.523e-15,
            -9.4e-17,
            1.21e-16,
            -2.8e-17
        }.ToReadOnlyCollection();

        #endregion

        #endregion

        #region Constructors

        public GaussianDistribution(double mean, double standardDeviation)
        {
            Mean = mean;
            StandardDeviation = standardDeviation;
            Variance = standardDeviation.Squared();
            Precision = 1 / Variance;
            PrecisionMean = Precision * Mean;
        }

        private GaussianDistribution()
        {
        }

        #endregion

        #region Functions

        #region Public Functions

        /// <summary>
        /// Creates GaussianDistribution whose values are derived from the given precision and precisionMean.
        /// </summary>
        /// <param name="precisionMean">Precision mean to derive Gaussian Distribution from.</param>
        /// <param name="precision">Precision to derive Gaussian Distribution from.</param>
        /// <returns></returns>
        public static GaussianDistribution FromPrecisionMean(double precisionMean, double precision)
        {
            var gaussianDistribution = new GaussianDistribution
            {
                Precision = precision,
                PrecisionMean = precisionMean,
                Variance = 1 / precision
            };

            gaussianDistribution.StandardDeviation = Math.Sqrt(gaussianDistribution.Variance);
            gaussianDistribution.Mean = gaussianDistribution.PrecisionMean / gaussianDistribution.Precision;
            return gaussianDistribution;
        }

        /// <summary>
        /// Computes the absolute difference between two Gaussian Distributions.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static double AbsoluteDifference(GaussianDistribution left, GaussianDistribution right)
        {
            var precisionMeanDifference = Math.Abs(left.PrecisionMean - right.PrecisionMean);
            var precisionDifference = Math.Sqrt(Math.Abs(left.Precision - right.Precision));
            return Math.Max(precisionMeanDifference, precisionDifference);
        }

        // TODO: Determine function usage
        /// <summary>
        /// Unsure of the usage of this function
        /// </summary>
        /// <param name="numerator"></param>
        /// <param name="denominator"></param>
        /// <returns></returns>
        public static double LogProductNormalization(GaussianDistribution numerator, GaussianDistribution denominator)
        {
            if (numerator.Precision == 0 || denominator.Precision == 0)
            {
                return 0;
            }

            var varianceDifference = denominator.Variance - numerator.Variance;
            var meanDifference = numerator.Mean - denominator.Mean;
            var piConstant = Math.Log(Math.Sqrt(2 * Math.PI));
            return Math.Log(denominator.Variance) + piConstant - (Math.Log(varianceDifference)/2) + meanDifference.Squared() / (2*varianceDifference);
        }

        /// <summary>
        /// Gets the y value of a normalized distribution at the given x position.
        /// </summary>
        /// <param name="x">The x position to get the value of.</param>
        /// <returns></returns>
        public static double At(double x)
        {
            return At(x, 0, 1);
        }

        /// <summary>
        /// Gets the y value of the given distribution at the given x position.
        /// </summary>
        /// <param name="x">The x position to get the value of.</param>
        /// <param name="mean">The mean of the distribution.</param>
        /// <param name="standardDeviation">The standard deviation of the distribution.</param>
        /// <returns></returns>
        public static double At(double x, double mean, double standardDeviation)
        {
            // See http://mathworld.wolfram.com/NormalDistribution.html
            //                1              -(x-mean)^2 / (2*stdDev^2)
            // P(x) = ------------------- * e
            //        stdDev * sqrt(2*pi

            var multiplier = 1 / (standardDeviation*Math.Sqrt(2*Math.PI));
            var exponent = Math.Exp((-1*Math.Pow(x - mean, 2)) / (2*standardDeviation.Squared()));
            var result = multiplier*exponent;
            return result;
        }

        /// <summary>
        /// Gets the cumulative of the area underneath a normalized distribution up to the given x position.
        /// </summary>
        /// <param name="x">The x position to get the value for.</param>
        /// <returns></returns>
        public static double CumulativeTo(double x)
        {
            // Constant value. Not sure why the Moserware implementation does it this way, will update if better solution is found.
            var inverseSqRtOf2 = -0.707106781186547524400844362104;
            var result = ErrorFunctionCumulativeTo(inverseSqRtOf2*x);
            return result/2;
        }

        /// <summary>
        /// Gets the inverse of the cumulative area underneath a normalized distribution up to the given x position.
        /// </summary>
        /// <param name="x">The x position to get the value for.</param>
        /// <returns></returns>
        public static double InverseCumulativeTo(double x)
        {
            return InverseCumulativeTo(x, 0, 1);
        }

        /// <summary>
        /// Gets the inverse of the cumulative area underneath the given distribution up to the given x position.
        /// </summary>
        /// <param name="x">The x position to get the value for.</param>
        /// <param name="mean">The mean of the distribution.</param>
        /// <param name="standardDeviation">The standard deviation of the distribution.</param>
        /// <returns></returns>
        public static double InverseCumulativeTo(double x, double mean, double standardDeviation)
        {
            return mean - Math.Sqrt(2) * standardDeviation * InverseErrorFunctionCumulativeTo(2*x);
        }

        #endregion

        #region Override Functions

        public override string ToString()
        {
            return $"μ={Mean:0.0000}, σ={StandardDeviation:0.0000}";
        }

        #endregion

        #region Private Functions

        private static double ErrorFunctionCumulativeTo(double x)
        {
            // TODO: Study the function this is derived from to give variables better names.
            var z = Math.Abs(x);
            var t = 2 / (2 + z);
            var ty = 4*t - 2;

            double d = 0;
            double dd = 0;
            foreach (var coefficient in ErrorCumulativeToCoefficients)
            {
                var temp = d;
                d = ty*d - dd + coefficient;
                dd = temp;
            }

            var answer = t*Math.Exp((-z * z) + (ErrorCumulativeToCoefficients[0] + ty*d)/2 - dd);
            return x >= 0 ? answer : (2 - answer);
        }

        private static double InverseErrorFunctionCumulativeTo(double p)
        {
            if (p >= 2)
            {
                return -100;
            }

            if (p <= 0)
            {
                return 100;
            }

            var pp = p < 1 ? p : 2-p;
            var t = Math.Sqrt(-2*Math.Log(pp/2));
            var x = -0.70711*((2.30753 + t*0.27061) / (1.0 + t*(0.99229 + t*0.04481)) - t);
            for (var i = 0; i < 2; i++)
            {
                double error = ErrorFunctionCumulativeTo(x) - pp;
                x += error / (1.12837916709551257*Math.Exp(-x.Squared()) - x*error);
            }

            return p < 1 ? x : -x;
        }

        #endregion

        #endregion
    }
}
