using Newtonsoft.Json;
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

		[JsonProperty("id")]
		public string Id { get; }
		[JsonProperty("player1_id")]
		public string Player1Id { get; }
		[JsonProperty("player2_id")]
		public string Player2Id { get; }
		[JsonProperty("state")]
		[JsonConverter(typeof(MatchStateConverter))]
		public MatchState State { get; }
		[JsonProperty("round")]
		public int Round { get; }
		[JsonProperty("tournament_id")]
		public string TournamentId { get; }

		// Winning and losing player Ids are -1 by default
		[JsonProperty("winner_id")]
		public string WinningPlayerId { get; set; }
		public string LosingPlayerId { get; set; }

		public void ReportWinner(string winningPlayerId, string losingPlayerId)
		{
			WinningPlayerId = winningPlayerId;
			LosingPlayerId = losingPlayerId;
		}
	}
}
