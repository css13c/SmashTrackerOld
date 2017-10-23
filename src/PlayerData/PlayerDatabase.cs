using System.Collections.Generic;
using System.Linq;
using System.Data.SQLite;
using PlayerData;
using System.Collections.ObjectModel;
using Dapper;
using SmashTracker.Utility;
using PlayerData.Properties;
using TrueSkill;
using System.IO;

namespace SmashTracker.Data
{
	public class PlayerDatabase
	{
		public PlayerDatabase()
		{
			if (!File.Exists(@"C:\Users\Public\Documents\SmashTracker\SmashTrackerData.sqlite"))
			{
				Directory.CreateDirectory(@"C:\Users\Public\Documents\SmashTracker\");
				SQLiteConnection.CreateFile(@"C:\Users\Public\Documents\SmashTracker\SmashTrackerData.sqlite");
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
	RatingMean REAL NOT NULL,
	RatingSD REAL NOT NULL
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
INNER JOIN player_rating pr ON p.Id = pr.PlayerId
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
							Rating = new Rating(p.RatingMean, p.RatingSD),
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
INNER JOIN player_tags pt ON p.Id = pt.PlayerId
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
							Rating = new Rating(p.RatingMean, p.RatingSD),
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
WHERE p.Name LIKE @name;
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
							Rating = new Rating(p.RatingMean, p.RatingSD),
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
		public void AddPlayer(string addName, Rating addRating, Character addChar, string addTag)
		{
			const string insertSql = @"
INSERT INTO players(Name, RatingMean, RatingSD) VALUES
(@name, @mean, @sd);
";
			const string selectSql = @"
SELECT seq FROM sqlite_sequence 
WHERE name='players';
";
			var param = new DynamicParameters();
			param.Add("@name", addName);
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
			AddTag(playerId, addTag);
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
		/// Adds a tag to a player's tags.
		/// </summary>
		/// <param name="player">The id of the player.</param>
		/// <param name="addTag">The tag to add.</param>
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
							Rating = new Rating(p.RatingMean, p.RatingSD),
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
		public double RatingMean { get; set; }
		public double RatingSD { get; set; }
	}
}
