using System;
using System.Xml.Linq;

namespace ChallongeApiClient
{
	public class ChallongeMatch
	{
		public ChallongeMatch(string id, string player1Id, string player2Id, MatchState state, int round, string tournamentId, string winningPlayerId, string losingPlayerId)
		{
			Id = id;
			Player1Id = player1Id;
			Player2Id = player2Id;
			State = state;
			Round = round;
			TournamentId = tournamentId;
			WinningPlayerId = winningPlayerId;
			LosingPlayerId = losingPlayerId;
		}

		public string Id { get; }
		public string Player1Id { get; }
		public string Player2Id { get; }
		public MatchState State { get; }
		public int Round { get; }
		public string TournamentId { get; }

		// Winning and losing player Ids are -1 by default
		public string WinningPlayerId { get; private set; }
		public string LosingPlayerId { get; private set; }

		public void ReportWinner(string winningPlayerId, string losingPlayerId)
		{
			WinningPlayerId = winningPlayerId;
			LosingPlayerId = losingPlayerId;
		}
	}
}
