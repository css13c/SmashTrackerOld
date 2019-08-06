using SmashTrackerFinal.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmashTrackerFinal.Data.Models
{
	public class Player
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Tag { get; set; }
		public double RatingMean { get; set; }
		public double RatingSD { get; set; }
		public ObservableCollection<Character> Characters { get; set; }
	}
}
