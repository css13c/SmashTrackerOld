using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SmashTracker.Utility;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ChallongeApiClient
{
	public class ChallongeBracket
	{
		public ChallongeBracket(string id, BracketType bracketType, ReadOnlyCollection<MatchWrapper> matches, ReadOnlyCollection<PlayerWrapper> participants)
		{
			Id = id;
			BracketType = bracketType;
			Matches = matches;
			Participants = participants;

			Initialize();
		}

		[JsonProperty("id")]
		public string Id { get; set; }
		[JsonProperty("tournament_type")]
		[JsonConverter(typeof(BracketTypeConverter))]
		public BracketType BracketType { get; set; }
		[JsonProperty("matches")]
		public ReadOnlyCollection<MatchWrapper> Matches { get; set; }
		[JsonProperty("participants")]
		public ReadOnlyCollection<PlayerWrapper> Participants { get; set; }
		public bool Started { get; set; }
		public ReadOnlyCollection<ChallongeMatch> MatchList { get; set; }
		public ReadOnlyCollection<ChallongePlayer> PlayerList { get; set; }

		private void Initialize()
		{
			if (Matches != null)
				MatchList = Matches.Select(m => m.Match).ToReadOnlyCollection();
			if (Participants != null)
				PlayerList = Participants.Select(p => p.Player).ToReadOnlyCollection();
		}
	}
}
