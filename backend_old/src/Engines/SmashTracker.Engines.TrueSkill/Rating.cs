using SmashTracker.Engines.TrueSkill.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmashTracker.Engines.TrueSkill
{
	public class Rating : IRating
	{
		public double Mean { get; set; }

		public double StandardDeviation { get; set; }

		public Rating(double mean, double standardDeviation)
		{
			Mean = mean;
			StandardDeviation = standardDeviation;
		}

		public override string ToString() => $"μ={Mean:0.0000}, σ={StandardDeviation:0.0000}";
	}
}
