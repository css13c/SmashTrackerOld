using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrueSkill
{
	public class Rating
	{
		public Rating(double mean, double standardDeviation)
		{
			Mean = mean;
			StandardDeviation = standardDeviation;
		}

		public double Mean { get; private set; }
		public double StandardDeviation { get; private set; }

		// Multiply the standard deviation by this number to get a conservative estimate of the player's skill.
		public const int ConservativeMultiplier = 3;

		public double ConservativeRating
		{
			get { return Mean - (StandardDeviation * ConservativeMultiplier); }
		}

		public override string ToString()
		{
			return $"μ={Mean}, σ={StandardDeviation}";
		}
	} 
}
