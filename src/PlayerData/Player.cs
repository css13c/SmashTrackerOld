using System.Collections.Generic;
using System.Text;
using TrueSkill;

namespace PlayerData
{
	public class Player
	{
		public Player(int id, string name, List<string> tags, Rating rating, List<Character> characters)
		{
			Id = id;
			Name = name;
			Tags = tags;
			Rating = rating;
			Characters = characters;
		}

		public Player()
		{
			Tags = new List<string>();
			Characters = new List<Character>();
		}

		public int Id { get; set; }
		public string Name { get;  set; }
		public List<string> Tags { get;  set; }
		public Rating Rating { get;  set; }
		public List<Character> Characters { get;  set; }

		public override string ToString()
		{
			return $"Name: {Name}\r\nTags: {PrintTags()}\r\nCharacters: {PrintChars()}\r\nRating: {Rating}";
		}

		private string PrintTags()
		{
			StringBuilder str = new StringBuilder();
			foreach (var tag in Tags)
				str.Append($"{tag}, ");
			str.Remove(str.Length - 3, 2);

			return str.ToString();
		}

		private string PrintChars()
		{
			StringBuilder str = new StringBuilder();
			foreach (var character in Characters)
				str.Append($"{character.ToString()}, ");
			str.Remove(str.Length - 3, 2);

			return str.ToString();
		}
	}
}
