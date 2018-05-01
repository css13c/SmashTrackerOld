using SmashTrackerGUI.Infrastructure;

namespace SmashTrackerGUI.Models
{
	public class Rating : NotifyChange
	{
		public Rating(double mean, double standardDeviation)
		{
			m_Mean = mean;
			m_StandardDeviation = standardDeviation;
		}

		// Member Variables
		private double m_Mean { get; set; }
		public double Mean
		{
			get { return m_Mean; }
			set
			{
				m_Mean = value;
				RaisePropertyChanged();
			}
		}

		private double m_StandardDeviation { get; set; }
		public double StandardDeviation
		{
			get { return m_StandardDeviation; }
			set
			{
				m_StandardDeviation = value;
				RaisePropertyChanged();
			}
		}

		// Multiply the standard deviation by this number to get a conservative estimate of the player's skill.
		public const int ConservativeMultiplier = 3;

		public double ConservativeRating
		{
			get { return m_Mean - (m_StandardDeviation * ConservativeMultiplier); }
		}

		// Member Functions
		public override string ToString()
		{
			return $"μ={m_Mean}, σ={m_StandardDeviation}";
		}
	}
}
