using System.Collections.ObjectModel;

namespace ChallongeApiClient
{
	class ChallongeBracket
	{
		int Id { get; set; }
		ReadOnlyDictionary<int, ChallongeMatch> Matches { get; set; }
		ReadOnlyDictionary<int, ChallongePlayer> Players { get; set; }
		BracketType BracketType { get; set; }
	}
}
