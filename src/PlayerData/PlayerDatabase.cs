using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace PlayerData
{
	public class PlayerDatabase
	{
		public PlayerDatabase()
		{
			Players = new Dictionary<int, Player>().ToReadOnlyDictionary();
		}

		public PlayerDatabase(ReadOnlyCollection<Player> players)
		{
			Players = new ReadOnlyDictionary<int, Player>(players.ToDictionary(p => p.Id));
		}

		public ReadOnlyDictionary<int, Player> Players { get; private set; }

		/// <summary>
		/// Gets the top rated players.
		/// </summary>
		/// <param name="size">The number of players to get.</param>
		/// <returns></returns>
		public PowerRanking GetPR(int size)
		{
			if (Players.Values.Count < size)
				throw new InvalidOperationException("Player count is less than requested PR size.");

			var playersOrderedByRating = Players.Values.OrderByDescending(p => p.Rating).Take(size).ToReadOnlyCollection();
			return new PowerRanking(
				playersOrderedByRating.ToDictionary(p => playersOrderedByRating.IndexOf(p)).ToReadOnlyDictionary(),
				size);
		}

		/// <summary>
		/// Gets all players who play the given character.
		/// </summary>
		/// <param name="character">Identifies the character.</param>
		/// <returns></returns>
		public ReadOnlyCollection<Player> GetPlayersOfCharacter(Character character)
		{
			return Players.Values.Where(p => p.Characters.Contains(character)).ToReadOnlyCollection();
		}

		/// <summary>
		/// Gets all players whose names match the query.
		/// </summary>
		/// <param name="query">The search query.</param>
		/// <returns></returns>
		public ReadOnlyCollection<Player> GetPlayersByName(string query)
		{
			return Players.Values.ToLookup(p => p.Name)[query].ToReadOnlyCollection();
		}

		/// <summary>
		/// Gets the player with the specified id.
		/// </summary>
		/// <param name="id">Identifies the player.</param>
		/// <param name="player">The player returned.</param>
		/// <returns></returns>
		public bool TryGetPlayerById(int id, out Player player)
		{
			return Players.TryGetValue(id, out player);
		}

		/// <summary>
		/// Adds a player to the database.
		/// </summary>
		/// <param name="player">The player to add.</param>
		public void AddPlayer(Player player)
		{ 
			if (player == null)
				throw new InvalidOperationException("Invalid Player added");

			IDictionary<int, Player> temp = Players;
			temp.Add(new KeyValuePair<int, Player>(player.Id, player));
			Players = temp.ToReadOnlyDictionary();
		}

		private Player GetPlayerById(int id)
		{
			return Players[id];
		}
	}
}
