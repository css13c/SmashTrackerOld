using PlayerData;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;

namespace SmashTracker.Controls
{
	/// <summary>
	/// Interaction logic for PlayersPage.xaml
	/// </summary>
	public partial class PlayersPage : UserControl
	{
		public PlayersPage(IEnumerable<Player> players)
		{
			InitializeComponent();

			List<PlayerBox> playerInfo = new List<PlayerBox>();
			foreach(var player in players)
			{
				playerInfo.Add(new PlayerBox(player.Name, player.Rating, player.Tags, player.Characters));
			}

			for (int item = 0; item < playerInfo.Count; item += 2)
				playerInfo[item].Background = Brushes.Gray;

			PlayerInfoBox.Content = playerInfo;
		}
	}
}
