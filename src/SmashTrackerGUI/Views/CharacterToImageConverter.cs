using SmashTrackerGUI.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SmashTrackerGUI.Views
{
    [ValueConversion(typeof(Character), typeof(Uri))]
	public class CharacterToImageConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			Character character = (Character) Enum.Parse(typeof(Character), value.ToString());
			return new Uri($"pack://application:,,,/Images/{character.ToString("F")}.png");
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
