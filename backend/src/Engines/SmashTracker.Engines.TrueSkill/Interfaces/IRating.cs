using System;
using System.Collections.Generic;
using System.Text;

namespace SmashTracker.Engines.TrueSkill.Interfaces
{
	public interface IRating
	{
		double Mean { get; set; }

		double StandardDeviation { get; set; }
	}
}
