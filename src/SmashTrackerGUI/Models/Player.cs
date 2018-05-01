using SmashTrackerGUI.Infrastructure;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace SmashTrackerGUI.Models
{
	public class Player : NotifyChange
	{
		public Player(int id, string name, string tag, Rating rating, ObservableCollection<Character> characters)
		{
			Id = id;
			m_Name = m_Name;
			m_Tag = m_Tag;
			m_Rating = m_Rating;
			Characters = characters;
		}

		public Player()
		{
			m_Tag = null;
			Characters = new ObservableCollection<Character>();
		}

		// Member Variables
		private int m_Id { get; set; }
		public int Id
		{
			get { return m_Id; }
			set
			{
				m_Id = value;
				RaisePropertyChanged();
			}
		}

		private string m_Name { get; set; }
		public string Name
		{
			get { return m_Name; }
			set
			{
				m_Name = value;
				RaisePropertyChanged();
			}
		}

		private string m_Tag { get; set; }
		public string Tag
		{
			get { return m_Tag; }
			set
			{
				m_Tag = value;
				RaisePropertyChanged();
			}
		}

		private Rating m_Rating { get; set; }
		public Rating Rating
		{
			get { return m_Rating; }
			set
			{
				m_Rating = value;
				RaisePropertyChanged();
			}
		}

		private ObservableCollection<Character> m_Characters { get; set; }
		public ObservableCollection<Character> Characters
		{
			get { return m_Characters; }
			set
			{
				m_Characters = value;
				RaisePropertyChanged();
			}
		}

		// Member functions
		public override string ToString()
		{
			return $"m_Name: {m_Name};  m_Tag: {m_Tag};  Characters: {PrintChars()};  m_Rating: {m_Rating}";
		}

		public string ToStringWithId()
		{
			return $"Id: {m_Id};  m_Name: {m_Name};  m_Tag: {m_Tag};  Characters: {PrintChars()};  m_Rating: {m_Rating}";
		}

		public string PrintChars()
		{
			StringBuilder str = new StringBuilder();
			foreach (var character in m_Characters)
				str.Append($"{character.ToOutput()}, ");
			str.Remove(str.Length - 2, 2);

			return str.ToString();
		}
	}
}
