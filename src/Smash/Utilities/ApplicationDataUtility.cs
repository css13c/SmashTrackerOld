using PlayerData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SmashTracker
{
	public static class ApplicationDataUtility
	{
		public static PlayerDatabase LoadPlayerData(string filepath = null)
		{
			if(filepath == null)
				return new PlayerDatabase();

			XDocument playerData = XDocument.Load(filepath);

			return new PlayerDatabase(playerData.Descendants("player").Select(PlayerFromXml).ToReadOnlyCollection());
		}

		public static void SavePlayerData(PlayerDatabase playerData)
		{
			XDocument playerDoc = new XDocument(new XElement("playerData", playerData.Players.Values.Select(PlayerToXml)));
			playerDoc.Save(savePath);
		}

		private static XElement PlayerToXml(Player player)
		{
			return new XElement("player",
				new XElement("id", player.Id.ToString()),
				new XElement("name", player.Name),
				new XElement("tags",
					player.Tags.Select(t => new XElement("tag", t))),
				new XElement("rating", player.Rating.ToString()),
				new XElement("characters",
					player.Characters.Select(c => new XElement("character", c.ToString()))));
		}

		private static Player PlayerFromXml(XElement player)
		{
			return new Player(
				Int32.Parse(player.Element("id").Value),
				player.Element("name").Value,
				player.Elements("tag").Select(t => t.Value).ToReadOnlyCollection(),
				Double.Parse(player.Element("rating").Value),
				player.Elements("character").Select(c => (Character) Enum.Parse(typeof(Character), c.Value)).ToReadOnlyCollection());
		}

		private static string savePath = @"E:\Code\SmashTracker\SmashTrackerPlayerData.xml";
	}
}
