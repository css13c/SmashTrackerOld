using ChallongeApiClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
