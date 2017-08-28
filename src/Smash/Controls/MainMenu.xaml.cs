using SmashTracker.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SmashTracker.Controls
{
	/// <summary>
	/// Interaction logic for MainMenu.xaml
	/// </summary>
	public partial class MainMenu : UserControl
	{
		public MainMenu()
		{
			InitializeComponent();
		}

		private void Players_Click(object sender, RoutedEventArgs e)
		{
			PageSwitcher.Navigate(new PlayersPage());
		}
	}
}
