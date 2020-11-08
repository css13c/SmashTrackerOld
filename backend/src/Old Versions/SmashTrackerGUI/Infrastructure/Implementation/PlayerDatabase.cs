using System.Collections.Generic;
using System.Linq;
using System.Data.SQLite;
using System.Collections.ObjectModel;
using Dapper;
using SmashTracker.Utility;
using System.IO;
using System;
using SmashTrackerGUI.Properties;
using SmashTrackerGUI.Models;

namespace SmashTrackerGUI.Infrastructure
{
	public class PlayerDatabase
	{
		public PlayerDatabase()
		{
			if (!File.Exists($@"C:\SmashTracker\SmashTrackerData.sqlite"))
			{
				Directory.CreateDirectory($@"C:\SmashTracker\");
				SQLiteConnection.CreateFile($@"C:\SmashTracker\SmashTrackerData.sqlite");
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
		/// Gets all players in the database.
		/// </summary>
		/// <returns></returns>
		public ObservableCollection<Player> GetAllPlayers()
		{
			string sql = @"
SELECT * FROM players p
LEFT JOIN player_characters pc ON p.Id = pc.PlayerId;";

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
						player.Characters = new ObservableCollection<Character>();
					if (c != null)
						player.Characters.Add((Character)c.Character);

					return player;
				});
			}

			return new ObservableCollection<Player>(lookup.Values);
		}

		/// <summary>
		/// Removes the given player
		/// </summary>
		/// <param name="player"></param>
		public void RemovePlayer(Player player)
		{
			const string sql = @"
DELETE FROM players
WHERE Id = @playerId;

DELETE FROM player_characters
WHERE PlayerId = @playerId;
";

			var param = new DynamicParameters();
			param.Add("@playerId", player.Id);

			using (var dbConnection = new SQLiteConnection(Settings.Default.ConnectionString).OpenAndReturn())
				dbConnection.Execute(sql, param);
		}

		/// <summary>
		/// Adds a player to the database.
		/// </summary>
		public Player AddPlayer(string addName, Rating addRating, string addTag)
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

			return new Player(playerId, addName, addTag, addRating, new ObservableCollection<Character>());
		}

		internal void UpdatePlayer(Player editPlayer)
		{
			throw new NotImplementedException();
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

		// DB conversion classes
		internal class DbPlayerCharacter
		{
			public int Id { get; set; }
			public int Character { get; set; }
			public int PlayerId { get; set; }
		}

		internal class DbPlayer
		{
			public int Id { get; set; }
			public string Name { get; set; }
			public double RatingMean { get; set; }
			public double RatingSD { get; set; }
			public string Tag { get; set; }
		}
	}

}
