using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PlayerData
{
	public class Player
	{
		public Player(int id, string name, List<string> tags, double rating, List<Character> characters)
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
		public double Rating { get;  set; }
		public List<Character> Characters { get;  set; }

		public void AddCharacter(Character character)
		{
			Characters = new List<Character>(Characters)
			{
				character
			};
		}

		public void AddTag(string tag)
		{
			Tags = new List<string>(Tags)
			{
				tag
			};
		}

		void ChangeName(string name)
		{
			Name = name;
		}

		void UpdateRating(double newRating)
		{
			Rating = newRating;
		}
	}
}
