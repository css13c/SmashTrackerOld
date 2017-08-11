using NUnit.Framework;
using System;

namespace PlayerData.Tests
{
	[TestFixture]
	class PlayerDatabaseTestFixture
	{
		[Test]
		public static void CreateEmptyDatabase()
		{
			var database = new PlayerDatabase();
			Assert.IsTrue(database.Players.Count == 0);
		}

		[TestCase(null)]
		public static void AddingNullPlayerThrowsException(Player player)
		{
			var database = new PlayerDatabase();
			Assert.Throws<InvalidOperationException>(delegate { database.AddPlayer(player); });
		}
	}
}
