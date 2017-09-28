using ChallongeApiClient;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SmashTracker.Data
{
	public class TournamentManager
	{
		public TournamentManager()
		{
			CurrentBracket = null;
			Client = new ChallongeClient();
		}

		public ChallongeBracket CurrentBracket { get; private set; }
		private ChallongeClient Client;

		public void CreateNewTournament()
		{
			CurrentBracket = Client.CreateBracket();
		}

		public void StartTournament()
		{

		}

		public bool CreateNewTournament(IEnumerable<string> players)
		{
			ReadOnlyCollection<ChallongePlayer> currentPlayers;
			CurrentBracket = Client.CreateBracket();

			return Client.TryBulkAddPlayers(CurrentBracket.Id, players, out currentPlayers);
		}
	}
}
