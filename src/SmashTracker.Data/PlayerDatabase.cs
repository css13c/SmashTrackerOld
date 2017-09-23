using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SQLite;
using SmashTracker.Data.Properties;
using PlayerData;
using System.Collections.ObjectModel;
using Dapper;
using SmashTracker.Utility;

namespace SmashTracker.Data
{
	public class PlayerDatabase
	{
		public PlayerDatabase()
		{
			// Creates the file if it does not exist, then connects to the database
			SQLiteConnection.CreateFile(@"E:/Code/SmashTracker/SmashTrackerData.sqlite");

			// Creates players table if it does not exist
			InitializeTables();
		}

		private void InitializeTables()
		{
			const string sql = @"
CREATE TABLE IF NOT EXISTS players (
	Id INTEGER PRIMARY KEY AUTOINCREMENT,
	Name TEXT NOT NULL,
	Rating REAL NOT NULL
);

CREATE TABLE IF NOT EXISTS player_characters (
	Id INTEGER PRIMARY KEY AUTOINCREMENT,
	Character VARCHAR(50),
    PlayerId INTEGER,
	FOREIGN KEY (PlayerId) REFERENCES players(Id)
);

CREATE TABLE IF NOT EXISTS player_tags (
	Id INTEGER PRIMARY KEY AUTOINCREMENT,
	Tag VARCHAR(20),
    PlayerId INTEGER,
	FOREIGN KEY (PlayerId) REFERENCES players(Id)
);
";
			using (var dbConnection = new SQLiteConnection(Settings.Default.ConnectionString).OpenAndReturn())
			{
				dbConnection.Execute(sql);
			}
		}

		/// <summary>
		/// Gets the top rated players.
		/// </summary>
		/// <param name="size">The number of players to get.</param>
		/// <returns></returns>
		public ReadOnlyCollection<Player> GetPR(int size)
		{
			const string sql = @"
SELECT * FROM players p
INNER JOIN player_characters pc ON p.Id = pc.PlayerId
INNER JOIN player_tags pt ON p.Id = pt.PlayerId
ORDER BY p.Rating
LIMIT @limit;
";

			var lookup = new Dictionary<int, Player>();
			using (var dbConnection = new SQLiteConnection(Settings.Default.ConnectionString).OpenAndReturn())
			{
				dbConnection.Query<Player, string, int, Player>(sql, (p, t, c) =>
				{
					Player player;
					if (!lookup.TryGetValue(p.Id, out player))
						lookup.Add(p.Id, player = p);

					Console.WriteLine($"Got Player {player.Name}");

					if (player.Tags == null)
						player.Tags = new List<string>();

					if (player.Characters == null)
						player.Characters = new List<Character>();

					player.Tags.Add(t);
					player.Characters.Add((Character) c);
					return player;
				}
				, new { limit = size });
			}

			return lookup.Values.ToReadOnlyCollection();
		}

		/// <summary>
		/// Gets all players who play the given character.
		/// </summary>
		/// <param name="character">Identifies the character.</param>
		/// <returns></returns>
		public ReadOnlyCollection<Player> GetPlayersOfCharacter(Character character)
		{
			const string sql = @"
SELECT * FROM player_characters pc
WHERE pc.Character = @character
INNER JOIN players p ON p.Id = pc.PlayerId
INNER JOIN player_tags pt on pc.PlayerId = pt.PlayerId;
";

			var lookup = new Dictionary<int, Player>();
			using (var dbConnection = new SQLiteConnection(Settings.Default.ConnectionString).OpenAndReturn())
			{
				dbConnection.Query<Player, string, string, Player>(sql, (p, t, c) =>
				{
					Player player;
					if (!lookup.TryGetValue(p.Id, out player))
						lookup.Add(p.Id, player = p);

					if (player.Tags == null)
						player.Tags = new List<string>();

					if (player.Characters == null)
						player.Characters = new List<Character>();

					player.Tags.Add(t);
					player.Characters.Add((Character)Enum.Parse(typeof(Character), c));
					return player;
				}
				, new { character = character.ToString() });
			}

			return lookup.Values.ToReadOnlyCollection();
		}

		/// <summary>
		/// Gets all players whose names match the query.
		/// </summary>
		/// <param name="query">The search query.</param>
		/// <returns></returns>
		public ReadOnlyCollection<Player> GetPlayersByName(string query)
		{
			const string sql = @"
SELECT * FROM players p
WHERE p.Name = @name
INNER JOIN player_characters pc ON p.Id = pc.PlayerId
INNER JOIN player_tags pt ON p.Id = pt.PlayerId;
";

			var lookup = new Dictionary<int, Player>();
			using (var dbConnection = new SQLiteConnection(Settings.Default.ConnectionString).OpenAndReturn())
			{
				dbConnection.Query<Player, string, string, Player>(sql, (p, t, c) =>
				{
					Player player;
					if (!lookup.TryGetValue(p.Id, out player))
						lookup.Add(p.Id, player = p);

					if (player.Tags == null)
						player.Tags = new List<string>();

					if (player.Characters == null)
						player.Characters = new List<Character>();

					player.Tags.Add(t);
					player.Characters.Add((Character)Enum.Parse(typeof(Character), c));
					return player;
				}
				, new { name = query });
			}

			return lookup.Values.ToReadOnlyCollection();
		}

		/// <summary>
		/// Gets the player with the specified id.
		/// </summary>
		/// <param name="id">Identifies the player.</param>
		/// <param name="player">The player returned.</param>
		/// <returns></returns>
		public bool TryGetPlayerById(int id, out Player player)
		{
			try
			{
				player = GetPlayerById(id);
				return true;
			}
			catch
			{
				player = null;
				return false;
			}
		}

		/// <summary>
		/// Adds a player to the database.
		/// </summary>
		public void AddPlayer(string addName, double addRating, Character addChar, string addTag)
		{
			const string insertSql = @"
INSERT INTO players(Name, Rating) VALUES
(@name, @rating);
";
			const string selectSql = @"
SELECT seq FROM sqlite_sequence 
WHERE name='players';
";

			using (var dbConnection = new SQLiteConnection(Settings.Default.ConnectionString).OpenAndReturn())
			{
				dbConnection.Execute(insertSql, new { name = addName, rating = addRating });
				int id = dbConnection.Query<int>(selectSql).Single();
				AddCharacter(id, addChar, dbConnection);
				AddTag(id, addTag, dbConnection);
			}
		}

		private void AddCharacter(int player, Character addChar, SQLiteConnection dbConnection)
		{
			const string sql = @"
INSERT INTO player_characters(Character, PlayerId) VALUES
(@character, @playerId);
";

			dbConnection.Execute(sql, new { character = addChar.ToString(), playerId = player });
		}

		private void AddTag(int player, string addTag, SQLiteConnection dbConnection)
		{
			const string sql = @"
INSERT INTO player_tags(Tag, PlayerId) VALUES
(@tag, @playerId);
";

			dbConnection.Execute(sql, new { tag = addTag, playerId = player });
		}

		private Player GetPlayerById(int id)
		{
			const string sql = @"
SELECT * FROM players p
WHERE p.Id = @query
INNER JOIN player_characters pc ON p.Id = pc.PlayerId
INNER JOIN player_tags pt ON p.Id = pt.PlayerId;
";

			var lookup = new Dictionary<int, Player>();
			using (var dbConnection = new SQLiteConnection(Settings.Default.ConnectionString).OpenAndReturn())
			{
				dbConnection.Query<Player, string, string, Player>(sql, (p, t, c) =>
				{
					Player player;
					if (!lookup.TryGetValue(p.Id, out player))
						lookup.Add(p.Id, player = p);

					if (player.Tags == null)
						player.Tags = new List<string>();

					if (player.Characters == null)
						player.Characters = new List<Character>();

					player.Tags.Add(t);
					player.Characters.Add((Character)Enum.Parse(typeof(Character), c));
					return player;
				}
				, new { query = id });
			}

			return lookup.Values.Single();
		}

		private Player Trial(Player p, string t, int c, ref Dictionary<int, Player> lookup)
		{
			Player player;
			if (!lookup.TryGetValue(p.Id, out player))
				lookup.Add(p.Id, player = p);

			if (player.Tags == null)
				player.Tags = new List<string>();

			if (player.Characters == null)
				player.Characters = new List<Character>();

			player.Tags.Add(t);
			player.Characters.Add((Character)c);
			return player;
		}
	}
}
