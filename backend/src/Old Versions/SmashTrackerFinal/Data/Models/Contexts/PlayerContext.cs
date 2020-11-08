using SmashTrackerFinal.Enums;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmashTrackerFinal.Data.Models.Contexts
{
	public class PlayerContext : DbContext
	{
		public DbSet<Player> Players { get; set; }
	}
}
