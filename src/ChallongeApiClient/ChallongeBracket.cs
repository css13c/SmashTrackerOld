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
		}

		public string Id { get; set; }
		public ReadOnlyDictionary<string, ChallongeMatch> Matches { get; set; }
		public ReadOnlyDictionary<string, ChallongePlayer> Players { get; set; }
		public BracketType BracketType { get; set; }
	}
}
