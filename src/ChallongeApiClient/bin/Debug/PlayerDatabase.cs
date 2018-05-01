using System.Collections.Generic;
using System.Linq;
using System.Data.SQLite;
using System.Collections.ObjectModel;
using Dapper;
using SmashTracker.Utility;
using PlayerData.Properties;
using TrueSkill;
using System.IO;
using System;

namespace PlayerData
{
	public class PlayerDatabase
	{
		public PlayerDatabase()
		{
			if (!File.Exists($@"{Environment.SpecialFolder.MyDocuments}\SmashTracker\SmashTrackerData.sqlite"))
			{
				Directory.CreateDirectory($@"{Environment.SpecialFolder.MyDocuments}\SmashTracker\");
				SQLiteConnection.CreateFile($@"{Environment.SpecialFolder.MyDocuments}\SmashTracker\SmashTrackerData.sqlite");
			}

			InitializeTables();
		}

		/// <summary>
		/// Initializes the tables in the database.
		/// </summary>
		private void InitializeTables()
		{
			const string sql = @"
CREATE TABLE IF NOT EXISTS players (
	Id INTEGER PRIMARY KEY AUTOINCREMENT,
	Name TEXT NOT NULL,
	Tag VARCHAR(30),
	RatingMean REAL NOT NULL,
	RatingSD REAL NOT NULL
);

CREATE TABLE IF NOT EXISTS player_characters (
	Id INTEGER PRIMARY KEY AUTOINCREMENT,
	Character VARCHAR(50),
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
LIMIT @limit;
";
			var param = new DynamicParameters();
			param.Add("@limit", size);

			var lookup = new Dictionary<int, Player>();
			using (var dbConnection = new SQLiteConnection(Settings.Default.ConnectionString).OpenAndReturn())
			{
				dbConnection.Query<DbPlayer, DbPlayerCharacter, Player>(sql, (p, c) =>
				{
					Player player;
					if (!lookup.TryGetValue(p.Id, out player))
					{
						player = new Player
						{
							Id = p.Id,
							Name = p.Name,
							Rating = new Rating(p.RatingMean, p.RatingSD),
							Tag = p.Tag
						};
						lookup.Add(p.Id, player);
					}

					if (player.Characters == null)
						player.Characters = new List<Character>();
					player.Characters.Add((Character)c.Character);

					return player;
				}
				, param);
			}

			return lookup.Values.OrderBy(p => p.Rating.ConservativeRating).ToReadOnlyCollection();
		}

		/// <summary>
		/// Gets all players in the database.
		/// </summary>
		/// <returns></returns>
		public ReadOnlyCollection<Player> GetAllPlayers()
		{
			string sql = @"
SELECT * FROM players p
INNER JOIN player_characters pc ON p.Id = pc.PlayerId;";

			var lookup = new Dictionary<int, Player>();
			using (var dbConnection = new SQLiteConnection(Settings.Default.ConnectionString).OpenAndReturn())
			{
				dbConnection.Query<DbPlayer, DbPlayerCharacter, Player>(sql, (p, c) =>
				{
					Player player;
					if (!lookup.TryGetValue(p.Id, out player))
					{
						player = new Player
						{
							Id = p.Id,
							Name = p.Name,
							Rating = new Rating(p.RatingMean, p.RatingSD),
							Tag = p.Tag
						};
						lookup.Add(p.Id, player);
					}

					if (player.Characters == null)
						player.Characters = new List<Character>();
					player.Characters.Add((Character)c.Character);

					return player;
				});
			}

			return lookup.Values.OrderBy(p => p.Rating.ConservativeRating).ToReadOnlyCollection();
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
WHERE pc.Character = @character;
";
			var param = new DynamicParameters();
			param.Add("@character", (int) character);

			var lookup = new Dictionary<int, Player>();
			using (var dbConnection = new SQLiteConnection(Settings.Default.ConnectionString).OpenAndReturn())
			{
				dbConnection.Query<DbPlayer, DbPlayerCharacter, Player>(sql, (p, c) =>
				{
					Player player;
					if (!lookup.TryGetValue(p.Id, out player))
					{
						player = new Player
						{
							Id = p.Id,
							Name = p.Name,
							Rating = new Rating(p.RatingMean, p.RatingSD),
							Tag = p.Tag
						};
						lookup.Add(p.Id, player);
					}

					if (player.Characters == null)
						player.Characters = new List<Character>();
					player.Characters.Add((Character)c.Character);

					return player;
				}
				, param);
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
WHERE p.Name LIKE @name;
";
			var param = new DynamicParameters();
			param.Add("@name", query);

			var lookup = new Dictionary<int, Player>();
			using (var dbConnection = new SQLiteConnection(Settings.Default.ConnectionString).OpenAndReturn())
			{
				dbConnection.Query<DbPlayer, DbPlayerCharacter, Player>(sql, (p, c) =>
				{
					Player player;
					if (!lookup.TryGetValue(p.Id, out player))
					{
						player = new Player
						{
							Id = p.Id,
							Name = p.Name,
							Rating = new Rating(p.RatingMean, p.RatingSD),
							Tag = p.Tag
						};
						lookup.Add(p.Id, player);
					}

					if (player.Characters == null)
						player.Characters = new List<Character>();
					player.Characters.Add((Character)c.Character);

					return player;
				}
				, param);
			}

			return lookup.Values.ToReadOnlyCollection();
		}

		/// <summary>
		/// Gets all players whose names or tags match the query.
		/// </summary>
		/// <param name="query">The search query</param>
		/// <returns></returns>
		public ReadOnlyCollection<Player> GetPlayersByNameOrTag(string query)
		{
			const string sql = @"
SELECT * FROM players p
INNER JOIN player_characters pc ON p.Id = pc.PlayerId
WHERE p.Name LIKE @name
OR p.Tag LIKE @name;
";
			var param = new DynamicParameters();
			param.Add("@name", query);

			var lookup = new Dictionary<int, Player>();
			using (var dbConnection = new SQLiteConnection(Settings.Default.ConnectionString).OpenAndReturn())
			{
				dbConnection.Query<DbPlayer, DbPlayerCharacter, Player>(sql, (p, c) =>
				{
					Player player;
					if (!lookup.TryGetValue(p.Id, out player))
					{
						player = new Player
						{
							Id = p.Id,
							Name = p.Name,
							Rating = new Rating(p.RatingMean, p.RatingSD),
							Tag = p.Tag
						};
						lookup.Add(p.Id, player);
					}

					if (player.Characters == null)
						player.Characters = new List<Character>();
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
		public Player AddPlayer(string addName, Rating addRating, Character addChar, string addTag)
		{
			const string insertSql = @"
INSERT INTO players(Name, Tag, RatingMean, RatingSD) VALUES
(@name, @tag, @mean, @sd);
";
			const string selectSql = @"
SELECT seq FROM sqlite_sequence 
WHERE name='players';
";
			var param = new DynamicParameters();
			param.Add("@name", addName);
			param.Add("@tag", addTag);
			param.Add("@mean", addRating.Mean);
			param.Add("@sd", addRating.StandardDeviation);

			int playerId;
			using (var dbConnection = new SQLiteConnection(Settings.Default.ConnectionString).OpenAndReturn())
			{
				dbConnection.Execute(insertSql, param);
				playerId = dbConnection.Query<int>(selectSql).Single();
				dbConnection.Close();
			}

			AddCharacter(playerId, addChar);

			return new Player(playerId, addName, addTag, addRating, new List<Character> { addChar });
		}

		/// <summary>
		/// Adds a character to the player's character list.
		/// </summary>
		/// <param name="player">The id of the player.</param>
		/// <param name="addChar">The character to add.</param>
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

		/// <summary>
		/// Changes the player's prefered tag.
		/// </summary>
		/// <param name="player">The id of the player.</param>
		/// <param name="addTag">The player's new tag.</param>
		public void ChangeTag(int player, string addTag)
		{
			const string sql = @"
UPDATE players SET Tag = @tag
WHERE Id = @playerId;
";
			var param = new DynamicParameters();
			param.Add("@tag", addTag);
			param.Add("@playerId", player);

			using (var dbConnection = new SQLiteConnection(Settings.Default.ConnectionString).OpenAndReturn())
			{
				dbConnection.Execute(sql, param);
			}
		}

		/// <summary>
		/// Updates the rating of the player.
		/// </summary>
		/// <param name="player">Identifies the player.</param>
		public void UpdatePlayerRating(Player player)
		{
			const string sql = @"
UPDATE players
SET RatingMean = @newMean, RatingSD = @newSD
WHERE Id = @id
";
			var param = new DynamicParameters();
			param.Add("@newMean", player.Rating.Mean);
			param.Add("@newSD", player.Rating.StandardDeviation);
			param.Add("@id", player.Id);

			using (var dbConnection = new SQLiteConnection(Settings.Default.ConnectionString).OpenAndReturn())
			{
				dbConnection.Execute(sql, param);
			}
		}

		/// <summary>
		/// Retrieves a player by his Id.
		/// </summary>
		/// <param name="id">The id of the player.</param>
		/// <returns></returns>
		private Player GetPlayerById(int id)
		{
			const string sql = @"
SELECT * FROM players p
INNER JOIN player_characters pc ON p.Id = pc.PlayerId
WHERE p.Id = @query;
";
			var param = new DynamicParameters();
			param.Add("@query", id);

			var lookup = new Dictionary<int, Player>();
			using (var dbConnection = new SQLiteConnection(Settings.Default.ConnectionString).OpenAndReturn())
			{
				dbConnection.Query<DbPlayer, DbPlayerCharacter, Player>(sql, (p, c) =>
				{
					Player player;
					if (!lookup.TryGetValue(p.Id, out player))
					{
						player = new Player
						{
							Id = p.Id,
							Name = p.Name,
							Rating = new Rating(p.RatingMean, p.RatingSD),
							Tag = p.Tag
						};
						lookup.Add(p.Id, player);
					}

					if (player.Characters == null)
						player.Characters = new List<Character>();
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

	public class DbPlayer
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public double RatingMean { get; set; }
		public double RatingSD { get; set; }
		public string Tag { get; set; }
	}
}
