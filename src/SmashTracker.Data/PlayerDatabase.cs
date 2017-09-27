using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SQLite;
using SmashTracker.Data.Properties;
using PlayerData;
using System.Collections.ObjectModel;
using Dapper;
using SmashTracker.Utility;
using System.IO;

namespace SmashTracker.Data
{
	public class PlayerDatabase
	{
		public PlayerDatabase()
		{
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
			var param = new DynamicParameters();
			param.Add("@limit", size);

			var lookup = new Dictionary<int, Player>();
			using (var dbConnection = new SQLiteConnection(Settings.Default.ConnectionString).OpenAndReturn())
			{
				dbConnection.Query<DbPlayer, DbPlayerCharacter, DbPlayerTag, Player>(sql, (p, c, t) =>
				{
					Player player;
					if (!lookup.TryGetValue(p.Id, out player))
					{
						player = new Player
						{
							Id = p.Id,
							Name = p.Name,
							Rating = p.Rating
						};
						lookup.Add(p.Id, player);
					}

					if (player.Tags == null)
						player.Tags = new List<string>();

					if (player.Characters == null)
						player.Characters = new List<Character>();

					player.Tags.Add(t.Tag);
					player.Characters.Add((Character)c.Character);

					return player;
				}
				, param);
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
SELECT * FROM players p
INNER JOIN player_characters pc ON p.Id = pc.PlayerId
INNER JOIN player_tags pt on p.Id = pt.PlayerId
WHERE pc.Character = @character;
";
			var param = new DynamicParameters();
			param.Add("@character", (int) character);

			var lookup = new Dictionary<int, Player>();
			using (var dbConnection = new SQLiteConnection(Settings.Default.ConnectionString).OpenAndReturn())
			{
				dbConnection.Query<DbPlayer, DbPlayerCharacter, DbPlayerTag, Player>(sql, (p, c, t) =>
				{
					Player player;
					if (!lookup.TryGetValue(p.Id, out player))
					{
						player = new Player
						{
							Id = p.Id,
							Name = p.Name,
							Rating = p.Rating
						};
						lookup.Add(p.Id, player);
					}

					if (player.Tags == null)
						player.Tags = new List<string>();

					if (player.Characters == null)
						player.Characters = new List<Character>();

					player.Tags.Add(t.Tag);
					player.Characters.Add((Character)c.Character);

					return player;
				}
				, param);

				dbConnection.Close();
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
INNER JOIN player_characters pc ON p.Id = pc.PlayerId
INNER JOIN player_tags pt ON p.Id = pt.PlayerId
WHERE p.Name = @name;
";
			var param = new DynamicParameters();
			param.Add("@name", query);

			var lookup = new Dictionary<int, Player>();
			using (var dbConnection = new SQLiteConnection(Settings.Default.ConnectionString).OpenAndReturn())
			{
				dbConnection.Query<DbPlayer, DbPlayerCharacter, DbPlayerTag, Player>(sql, (p, c, t) =>
				{
					Player player;
					if (!lookup.TryGetValue(p.Id, out player))
					{
						player = new Player
						{
							Id = p.Id,
							Name = p.Name,
							Rating = p.Rating
						};
						lookup.Add(p.Id, player);
					}

					if (player.Tags == null)
						player.Tags = new List<string>();

					if (player.Characters == null)
						player.Characters = new List<Character>();

					player.Tags.Add(t.Tag);
					player.Characters.Add((Character)c.Character);

					return player;
				}
				, param);
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
			var param = new DynamicParameters();
			param.Add("@name", addName);
			param.Add("@rating", addRating);

			int playerId;
			using (var dbConnection = new SQLiteConnection(Settings.Default.ConnectionString).OpenAndReturn())
			{
				dbConnection.Execute(insertSql, param);
				playerId = dbConnection.Query<int>(selectSql).Single();
				dbConnection.Close();
			}

			AddCharacter(playerId, addChar);
			AddTag(playerId, addTag);
		}

		public void AddCharacter(int player, Character addChar)
		{
			const string sql = @"
INSERT INTO player_characters(Character, PlayerId) VALUES
(@character, @playerId);
";

			var param = new DynamicParameters();
			param.Add("@character", (int)addChar);
			param.Add("@playerId", player);

			using (var dbConnection = new SQLiteConnection(Settings.Default.ConnectionString).OpenAndReturn())
			{
				dbConnection.Execute(sql, param);
				dbConnection.Close();
			}
		}

		public void AddTag(int player, string addTag)
		{
			const string sql = @"
INSERT INTO player_tags(Tag, PlayerId) VALUES
(@tag, @playerId);
";
			var param = new DynamicParameters();
			param.Add("@tag", addTag);
			param.Add("@playerId", player);

			using (var dbConnection = new SQLiteConnection(Settings.Default.ConnectionString).OpenAndReturn())
			{
				dbConnection.Execute(sql, param);
			}
		}

		private Player GetPlayerById(int id)
		{
			const string sql = @"
SELECT * FROM players p
INNER JOIN player_characters pc ON p.Id = pc.PlayerId
INNER JOIN player_tags pt ON p.Id = pt.PlayerId
WHERE p.Id = @query;
";
			var param = new DynamicParameters();
			param.Add("@query", id);

			var lookup = new Dictionary<int, Player>();
			using (var dbConnection = new SQLiteConnection(Settings.Default.ConnectionString).OpenAndReturn())
			{
				dbConnection.Query<DbPlayer, DbPlayerCharacter, DbPlayerTag, Player>(sql, (p, c, t) =>
				{
					Player player;
					if (!lookup.TryGetValue(p.Id, out player))
					{
						player = new Player
						{
							Id = p.Id,
							Name = p.Name,
							Rating = p.Rating
						};
						lookup.Add(p.Id, player);
					}

					if (player.Tags == null)
						player.Tags = new List<string>();

					if (player.Characters == null)
						player.Characters = new List<Character>();

					player.Tags.Add(t.Tag);
					player.Characters.Add((Character)c.Character);

					return player;
				}
				, param);
			}

			return lookup.Values.Single();
		}
	}

	public class DbPlayerCharacter
	{
		public int Id { get; set; }
		public int Character { get; set; }
		public int PlayerId { get; set; }
	}

	public class DbPlayerTag
	{
		public int Id { get; set; }
		public string Tag { get; set; }
		public int PlayerId { get; set; }
	}

	public class DbPlayer
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public double Rating { get; set; }
	}
}
