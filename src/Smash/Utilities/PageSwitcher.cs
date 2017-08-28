using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SmashTracker.Utilities
{
	public static class PageSwitcher
	{
		public static MainWindow Window;

		public static void Navigate(UserControl newPage)
		{
			Window.Navigate(newPage);
		}
	}
}
