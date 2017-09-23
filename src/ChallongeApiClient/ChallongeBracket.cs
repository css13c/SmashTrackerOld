using Newtonsoft.Json;
using SmashTracker.Utility;
using System.Collections.ObjectModel;
using System.Xml.Linq;

namespace ChallongeApiClient
{
	public class ChallongeBracket
	{
		public ChallongeBracket(string id, ReadOnlyDictionary<string, ChallongeMatch> matches, ReadOnlyDictionary<string, ChallongePlayer> players, BracketType bracketType)
		{
			Id = id;
			Matches = matches;
			Players = players;
			BracketType = bracketType;
			Started = false;
		}

		[JsonProperty("id")]
		public string Id { get; set; }
		public ReadOnlyDictionary<string, ChallongeMatch> Matches { get; set; }
		public ReadOnlyDictionary<string, ChallongePlayer> Players { get; set; }
		[JsonProperty("tournament_type")]
		public BracketType BracketType { get; set; }
		[JsonProperty("matches")]
		public ReadOnlyCollection<ChallongeMatch> MatchList { get; set; }
		[JsonProperty("participants")]
		public ReadOnlyCollection<ChallongePlayer> PlayerList { get; set; }
		public bool Started { get; set; }

		public void Initialize()
		{
			Matches = MatchList.ToReadOnlyDictionary(m => m.Id);
			Players = PlayerList.ToReadOnlyDictionary(p => p.Id);
		}
	}
}
