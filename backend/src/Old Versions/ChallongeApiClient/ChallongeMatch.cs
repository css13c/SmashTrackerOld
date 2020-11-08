using Newtonsoft.Json;
using System;
using System.Xml.Linq;

namespace ChallongeApiClient
{
	public class ChallongeMatch
	{
		public ChallongeMatch(string id, string player1Id, string player2Id, MatchState state, int round, string tournamentId)
		{
			Id = id;
			Player1Id = player1Id;
			Player2Id = player2Id;
			State = state;
			Round = round;
			TournamentId = tournamentId;
		}

		[JsonProperty("id")]
		public string Id { get; }
		[JsonProperty("player1_id")]
		public string Player1Id { get; }
		[JsonProperty("player2_id")]
		public string Player2Id { get; }
		[JsonProperty("state")]
		[JsonConverter(typeof(MatchStateConverter))]
		public MatchState State { get; private set; }
		[JsonProperty("round")]
		public int Round { get; }
		[JsonProperty("tournament_id")]
		public string TournamentId { get; }

		// Winning and losing player Ids are -1 by default
		[JsonProperty("winner_id")]
		public string WinningPlayerId { get; set; }
		public string LosingPlayerId { get; set; }

		public void ReportWinner(string winningPlayerId)
		{
			WinningPlayerId = winningPlayerId;
			LosingPlayerId = winningPlayerId == Player1Id ? Player2Id : Player1Id;
			State = MatchState.Complete;
		}

		// Converts scores from Winner-Loser to P1-P2 for Challonge API
		public string ConvertScoreCSV(string scores, string winningPlayerId)
		{
			var splitScores = scores.Split('-');
			var winnerScore = splitScores[0];
			var loserScore = splitScores[1];
			return winningPlayerId == Player1Id ? winnerScore + "-" + loserScore : loserScore + "-" + winnerScore;
		}
	}
}
