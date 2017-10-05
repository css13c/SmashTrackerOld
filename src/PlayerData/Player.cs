using System.Collections.Generic;
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
	}
}
