using PlayerData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace SmashTracker.Controls
{
	/// <summary>
	/// Interaction logic for PlayerBox.xaml
	/// </summary>
	public partial class PlayerBox : UserControl
	{
		public PlayerBox(string playerName, double playerRating, IEnumerable<string> playerTags, IEnumerable<Character> characters)
		{
			InitializeComponent();

			PlayerName.Content = playerName;
			PlayerRating.Content = $"{ratingText}{playerRating.ToString()}";

			foreach(string tag in playerTags)
			{
				PlayerTags.Items.Add(tag);
			}

			for(int columnIndex = 0; columnIndex < 5; columnIndex++)
			{
				var character = characters.ElementAt(columnIndex).ToString();
				var imageSource = new BitmapImage(new Uri($"pack://application:,,,/Images/{character}.png"));
				var image = new Image { Source = imageSource };
				Grid.SetColumn(image, columnIndex);
				CharacterGrid.Children.Add(image);
			}
		}

		private const string ratingText = "Rating: ";
	}
}
