using ChallongeApiClient;
using PlayerData;
using SmashTracker.Data;
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

			PrintLine("This is SmashTracker. To get a list of commands and their functions, enter 'help'. Enter 'quit' to exit.");
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
					case "add player":
						AddPlayer(playerData);
						break;
					case "find players":
						var players = FindPlayers(playerData);
						foreach (var player in players)
							player.ToString();
						break;
				}

				PrintLine();
				Print("> ");
				input = Read();
			}
		}

		public static ReadOnlyCollection<Player> FindPlayers(PlayerDatabase playerData)
		{
			Print("Find players by name or character?");
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
				input = Read().ToLowerInvariant();
				Character search;
				while (!Enum.TryParse(input, out search))
				{
					Print("Enter a valid Character. See included 'characters.txt' for a list of valid characters: ");
					input = Read().ToLowerInvariant();
				}

				return playerData.GetPlayersOfCharacter(search);
			}
		}

		public static void AddPlayer(PlayerDatabase playerData)
		{
			Print("Name: ");
			string name = Read();
			Print("Initial Tag: ");
			string tag = Read();
			Print("Character: ");
			string characterString = Read().ToLowerInvariant();
			Character character;
			while (!Enum.TryParse(characterString, out character))
			{
				Print("Enter a valid Character. See included 'characters.txt' for a list of valid characters: ");
				characterString = Read();
			}

			playerData.AddPlayer(name, TrueSkillSettings.DefaultRating(), character, tag);
		}

		public static void PrintHelp()
		{
			PrintLine("");
		}

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
			return Console.ReadLine();
		}
	}
}
