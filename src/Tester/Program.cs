using ChallongeApiClient;
using PlayerData;
using SmashTracker.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tester
{
	class Program
	{
		static void Main(string[] args)
		{
			PlayerDatabase pd = new PlayerDatabase();
			List<Player> playerList = new List<Player>();

			bool isInserted = false;
			bool addedChar = false;
			if (!isInserted)
			{
				pd.AddPlayer("Connor", 4.45, Character.Fox, "Tipzz");
				pd.AddPlayer("Swee", 4.45, Character.Fox, "ff");
				pd.AddPlayer("Crit", 2.12, Character.Charizard, "aa");
			}

			Console.WriteLine("Charizard Players: ");
			foreach (var p in pd.GetPlayersOfCharacter(Character.Charizard))
			{
				Console.WriteLine($"Name: {p.Name}, Tag: {p.Tags.First()}");
				if (!playerList.Any(s => p.Id == s.Id))
					playerList.Add(p);
			}

			Console.WriteLine("Fox Players: ");
			foreach (var p in pd.GetPlayersOfCharacter(Character.Fox))
			{
				Console.WriteLine($"Name: {p.Name}, Tag: {p.Tags.First()}");
				if (!playerList.Any(s => p.Id == s.Id))
					playerList.Add(p);
			}

			if (!addedChar)
			{
				var id = playerList.Where(p => p.Name == "Connor").Single().Id;
				pd.AddCharacter(id, Character.Charizard);
			}

			Console.WriteLine("Updated Charizard Players: ");
			foreach (var p in pd.GetPlayersOfCharacter(Character.Charizard))
			{
				Console.WriteLine($"Name: {p.Name}, Tag: {p.Tags.First()}");
				if (!playerList.Any(s => p.Id == s.Id))
					playerList.Add(p);
			}

			Console.Read();
		}
	}
}
