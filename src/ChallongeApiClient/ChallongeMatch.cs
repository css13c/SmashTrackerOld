using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChallongeApiClient
{
	public class ChallongeMatch
	{
		public int Id { get; set; }
		public int Player1Id { get; set; }
		public int Player2Id { get; set; }
		public MatchState State { get; set; }
		public int Round { get; set; }
		public int GameCount { get; set; }
		public (int Player1GamesWon, int Player2GamesWon) Score { get; set; }
		public int TournamentId { get; set; }

		// Winning and losing player Ids are -1 by default
		public int WinningPlayerId { get; set; }
		public int LosingPlayerId { get; set; }
	}
}
