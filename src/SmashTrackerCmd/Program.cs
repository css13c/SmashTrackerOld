using ChallongeApiClient;
using PlayerData;
using SmashTracker.Data;
using SmashTracker.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrueSkill;

namespace Tester
{
	class Program
	{
		static void Main(string[] args)
		{
			// Load in playerdata
			var playerData = new PlayerDatabase();
			var challonge = new ChallongeClient();

			PrintLine("Welcome to SmashTracker! To get a list of commands and their functions, enter 'help'. Enter 'quit' to exit.");
			Print("> ");

			string input = Read();
			bool quit = false;

			while(!quit)
			{
				switch (input.ToLowerInvariant())
				{
					case "quit":
						quit = true;
						break;
					case "help":
						PrintMainMenuHelp();
						break;
					case "add player":
						AddPlayer(playerData);
						break;
					case "find players":
						var players = FindPlayers(playerData);
						PrintLine("Found: ");
						foreach (var p in players)
							PrintLine($"- {p.ToString()}");
						break;
					case "find player":
						FindPlayer(playerData);
						break;
					case "get players":
					case "view players":
						players = playerData.GetAllPlayers();
						PrintLine("Found: ");
						foreach (var p in players)
							PrintLine($"- {p.ToString()}");
						break;
					case "get pr":
					case "get top players":
					case "view pr":
					case "view top players":
						var playerRanks = GetPR(playerData);
						foreach (var p in playerRanks)
						{
							PrintLine($"{p.Key}. {p.Value.Name}");
							PrintLine($"      - {p.Value.ToString()}");
						}
						break;
					case "start tournament":
						Print("Once you start a tournament, you cannot view or alter saved player information until it is complete. Would you like to continue? ");
						input = Read().ToLowerInvariant();
						if (input == "no" || input == "n")
							break;
						StartTournament(playerData, challonge);
						break;
				}
				
				Print("> ");
				input = Read();
			}
		}

		// Tournament Organizer functions. These are used to run a tournament bracket.
		public static void StartTournament(PlayerDatabase playerData, ChallongeClient challonge)
		{
			// Name bracket
			Print("Input bracket name: ");
			string name = Read();
			
			// Create bracket and list to hold players being added
			List<Player> participants = new List<Player>();

			// Add players to bracket/start bracket
			bool startedBracket = false;

			PrintLine();
			PrintLine("Type 'help' to see pre-tournament commands.");
			Print("> ");
			string input = Read().ToLowerInvariant();
			while (!startedBracket)
			{
				switch(input)
				{
					case "new":
						Player player = AddPlayer(playerData);
						participants.Add(player);
						PrintLine();
						break;
					case "add":
						player = AddExistingPlayerToCurrentBracket(playerData);
						if (player != null)
							participants.Add(player);
						PrintLine();
						break;
					case "cancel":
						return;
					case "start":
					case "start tournament":
						PrintLine("Once you start the bracket, you cannot add additional players. Begin bracket? ");
						input = Read().ToLowerInvariant();
						if (input == "y" || input == "yes")
							startedBracket = true;
						break;
					case "participants":
						PrintLine("Current Participants:");
						foreach (var p in participants)
							PrintLine($"* {p.Tag}");
						break;
					case "view database":
						PrintLine("Saved Players:");
						foreach (var p in playerData.GetAllPlayers())
							PrintLine($"* {p.Tag}");
						break;
					case "help":
						PrintTournamentCreationHelp();
						break;
				}

				if (!startedBracket)
				{
					Print("> ");
					input = Read().ToLowerInvariant();
				}
			}

			// Actually create the bracket now that we know the user wants to.
			ChallongeBracket newBracket = challonge.CreateBracket(name);

			while (!challonge.TryBulkAddPlayers(newBracket.Id.ToString(), participants.Select(p => p.Tag)))
			{
				Print("Error adding players to begin tournament. Try again? ");
				input = Read().ToLowerInvariant();
				if (input == "no" || input == "n")
				{
					PrintLine("Ok. Cancelling bracket.");
					Console.Clear();
					return;
				}
			}

			challonge.StartTournament(newBracket.Id.ToString());
			newBracket = challonge.GetBracket(newBracket.Id.ToString());
			newBracket.Started = true;

			// This is needed to be able to update player rankings during the tournament. 
			var playersByChallongeId = participants.ToReadOnlyDictionary(p => newBracket.PlayerList.Where(cp => p.Tag == cp.Tag).Select(cp => cp.Id).SingleOrDefault());
			RunTournament(playerData, challonge, newBracket, playersByChallongeId);
		}

		public static void RunTournament(PlayerDatabase playerData, ChallongeClient challonge, ChallongeBracket currentBracket, ReadOnlyDictionary<string, Player> playersByChallongeId)
		{
			// Initialize variables and start tournament
			int roundCount = 0;
			var playerIdRef = currentBracket.PlayerList.ToReadOnlyDictionary(p => p.Id);
			var playerTagRef = currentBracket.PlayerList.ToReadOnlyDictionary(p => p.Tag.ToLowerInvariant());
			PrintLine();
			PrintLine($"Beginning tournament {currentBracket.Id}");

			// Run each round until the bracket is done
			while (currentBracket.MatchList.Any(m => m.State != MatchState.Complete))
			{
				// Initialize current round information
				var currentRound = challonge.GetMatches(currentBracket.Id.ToString(), MatchState.Open);
				var matchesByPlayerId = new Dictionary<string, ChallongeMatch>();
				foreach (var match in currentRound)
				{
					matchesByPlayerId.Add(match.Player1Id, match);
					matchesByPlayerId.Add(match.Player2Id, match);
				}

				// Print out current round matches
				PrintLine($"{(TournamentRound)roundCount++} Round Matches: ");
				foreach (var match in currentRound)
				{
					PrintLine($"{playerIdRef[match.Player1Id].Tag} vs. {playerIdRef[match.Player2Id].Tag}");
				}

				// Read match results until current round is done
				PrintLine("To report match result, enter the tag of the winning player. To see matches not yet played, enter 'view matches'.");
				Print("> ");
				while (currentRound.Any(m => m.State != MatchState.Complete))
				{
					string input = Read();

					if (input.ToLowerInvariant() == "view matches")
					{
						foreach (var match in currentRound.Where(m => m.State == MatchState.Open))
							PrintLine($"{playerIdRef[match.Player1Id].Tag} vs. {playerIdRef[match.Player2Id].Tag}");
					}
					else if(input == "")
					{
						PrintLine("Enter a valid command.");
					}
					else
					{
						// Update challonge results
						string winnerTag = input.ToLowerInvariant();
						var challongeWinner = playerTagRef[winnerTag];
						var currentMatch = matchesByPlayerId[challongeWinner.Id];
						currentMatch.ReportWinner(challongeWinner.Id);
						PrintLine("Enter score in the format of Winner-Loser");
						Print("> ");
						string score = Read();
						score = currentMatch.ConvertScoreCSV(score, challongeWinner.Id);
						challonge.UpdateMatch(currentBracket.Id.ToString(), currentMatch.Id, score, challongeWinner.Id);

						// Update rankings and commit them to database.
						var winner = playersByChallongeId[challongeWinner.Id];
						var loser = playersByChallongeId[currentMatch.LosingPlayerId];
						loser.Rating.UpdateRating(won: false, opponentRating: winner.Rating);
						winner.Rating.UpdateRating(won: true, opponentRating: loser.Rating);
						playerData.UpdatePlayerRating(winner);
						playerData.UpdatePlayerRating(loser);

						PrintLine();
					}

					if(currentRound.Any(m => m.State != MatchState.Complete))
						Print("> ");
				}

				currentBracket = challonge.GetBracket(currentBracket.Id.ToString());
				PrintLine();
			}

			PrintLine("Bracket Complete!");
			challonge.FinalizeTournament(currentBracket.Id);
			PrintLine();
		}

		public static Player AddExistingPlayerToCurrentBracket(PlayerDatabase playerData)
		{
			PrintLine("Type in player to add from player database");
			Print("> ");
			string input = Read().ToLowerInvariant();

			// Get player to add
			ReadOnlyCollection<Player> results = playerData.GetPlayersByNameOrTag(input);
			Player player = null;
			while (results == null)
			{

				PrintLine("Found no players matching that name or tag. Try again.");
				input = Read();
				results = playerData.GetPlayersByNameOrTag(input.ToLowerInvariant());
			}

			// Handle mulitple results
			if (results.SingleOrDefault() == null && results.Count > 0)
			{
				foreach (var result in results)
					PrintLine($"* {result.ToStringWithId()}");
				Print($"Found more than one player, enter the id of the player you would like to add: ");
				input = Read().ToLowerInvariant();
				int id;
				while (!int.TryParse(input, out id) || results.Where(p => p.Id == id).Count() != 1)
				{
					Print("An invalid id was entered. Please enter a valid id: ");
					input = Read().ToLowerInvariant();
				}

				player = results.Where(p => p.Id == id).Single();
			}
			else if(results == null || results.Count == 0)
			{
				PrintLine("No players found. Try again.");
				return null;
			}
			else
				player = results.Single();

			Print($"Enter with prefered tag {player.Tag}? ");
			input = Read().ToLowerInvariant();
			if (input == "no" || input == "n")
			{
				PrintLine("Enter under which tag? This will change your prefered tag.");
				Print("New Tag: ");
				input = Read();
				Print($"Enter bracket with tag '{input}'?");
			}

			PrintLine($"Added {player.Tag} to bracket.");
			return player;
		}

		// TODO: Add commands for tournament sub-menu.
		// The help for running the tournament.
		public static void PrintTournamentHelp()
		{
			PrintLine("");
		}

		// The help for tournment creation.
		public static void PrintTournamentCreationHelp()
		{
			PrintLine(@"Commands:
* 'start' - Start the tournament.
* 'new' - Create a new player and add them to the tournament.
* 'add' - Add a player in the database to the tournament.
* 'cancel' - Cancel the creation of the tournament.
* 'participants' - See all players currently entered into the tournament.
* 'view database' - List all players in the player database.
* 'help' - Obviously you know what this does.");
		}

		// Main menu functions. Alter/view saved player data, see past tournaments
		public static ReadOnlyDictionary<int, Player> GetPR(PlayerDatabase playerData)
		{
			Print("Get how many players? ");
			string input = Read().ToLowerInvariant();
			int size;
			while (!int.TryParse(input, out size))
			{
				PrintLine("Please enter a valid number.");
				Print("Get how many players? ");
				input = Read();
			}

			var players = playerData.GetPR(size);
			return players.ToReadOnlyDictionary(p => players.IndexOf(p) + 1);
		}

		public static ReadOnlyCollection<Player> FindPlayers(PlayerDatabase playerData)
		{
			Print("Find players by name or character? ");
			string input = Read().ToLowerInvariant();
			if (input == "name" || input == "by name")
			{
				Print("Name to search for? ");
				input = Read().ToLowerInvariant();
				return playerData.GetPlayersByName(input);
			}
			else
			{
				Print("Get players of which character? ");
				Character search = GetCharacter();

				return playerData.GetPlayersOfCharacter(search);
			}
		}

		public static void FindPlayer(PlayerDatabase playerData)
		{
			// Get player
			Print("Player's name? ");
			string input = Read().ToLowerInvariant();
			Player player = playerData.GetPlayersByName(input).Single();
			PrintLine($"Found Player: {player.ToString()}.");

			// Check if user wants to alter player data
			Print("Alter player data? ");
			input = Read().ToLowerInvariant();
			if (input == "no" || input == "n")
				return;

			// Alter player data
			bool done = false;
			Print("What would you like to do? For help, enter 'help'. When done changing player info, enter 'done'.");
			input = Read();
			while (!done)
			{
				switch (input.ToLowerInvariant())
				{
					case "add character":
						Print($"Add which character to {player.Name}? ");
						string characterString = Read().ToLowerInvariant();
						Character character = GetCharacter();
						playerData.AddCharacter(player.Id, character);
						break;
					case "change tag":
						Print("Change tag to what? ");
						string tag = Read();
						playerData.ChangeTag(player.Id, tag);
						break;
					case "get info":
						PrintLine(player.ToString());
						break;
					// TODO: Add RemoveTag and RemoveCharacter functions to PlayerDatabase, and add that functionality here.
					/*case "remove tag":
						PrintLine($"Current tags are: {player.PrintTags()}");
						Print($"Remove which tag? ");
						input = Read();
						while(!player.Tags.Contains(input))
						{
							Print("Tag does not exist. Enter a valid tag to remove. Remove which tag? ");
							input = Read();
						}
						playerData.RemoveTag(player.Id, input);
						PrintLine("Tag Removed.");
						break;
					*/
					case "done":
						done = true;
						break;
					case "help":
						Print(@"The commands are: 
* 'Change Tag' - This changes the currently selected player's prefered tag.
* 'Add Character' - This adds a character to the currently selected player's information.
* 'Get Info' - This gets all of the currently selected players information, such as their rating, tags, and the characters they play.
* 'Done' - This exits the edit player sub-menu.");
						break;
				}

				Print("What would you like to do? For help, enter 'help'. When done changing player info, enter 'done'.");
				input = Read();
			}
		}

		public static Player AddPlayer(PlayerDatabase playerData)
		{
			Print("Name: ");
			string name = Read();
			Print("Initial Tag: ");
			string tag = Read();
			Print("Main Character: ");
			Character character = GetCharacter();

			return playerData.AddPlayer(name, TrueSkillSettings.DefaultRating(), character, tag);
		}

		public static Character GetCharacter()
		{
			string characterString = Read().ToLowerInvariant();
			Character character;
			while (!CharacterRef.TryGetValue(characterString, out character))
			{
				PrintLine("Enter a valid Character. See included 'characters.txt' for a list of valid characters: ");
				Print("Main Character: ");
				characterString = Read().ToLowerInvariant();
			}
			PrintLine($"Got character {character.ToOutput()}");

			return character;
		}

		// TODO: Add commands for main menu.
		public static void PrintMainMenuHelp()
		{
			PrintLine("");
		}

		// Generic helper functions to make reading and printing text to console easier
		public static void PrintLine(string text)
		{
			Console.WriteLine(text);
		}

		public static void PrintLine()
		{
			Console.WriteLine();
		}

		public static void Print(string text)
		{
			Console.Write(text);
		}

		public static string Read()
		{
			return Console.ReadLine().Trim();
		}

		public static ReadOnlyCollection<Character> CharacterList => Enum.GetValues(typeof(Character)).Cast<Character>().ToReadOnlyCollection();

		public static ReadOnlyDictionary<string, Character> CharacterRef => CharacterList.ToReadOnlyDictionary(c => c.ToString().ToLowerInvariant());
	}
}
