using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PlayerData
{
	public class Player
	{
		public Player(int id, string name, ReadOnlyCollection<string> tags, double rating, ReadOnlyCollection<Character> characters)
		{
			Id = id;
			Name = name;
			Tags = tags;
			Rating = rating;
			Characters = characters;
		}

		public int Id { get; }
		public string Name { get; private set; }
		public ReadOnlyCollection<string> Tags { get; private set; }
		public double Rating { get; private set; }
		public ReadOnlyCollection<Character> Characters { get; private set; }

		public void AddCharacter(Character character)
		{
			Characters = new List<Character>(Characters)
			{
				character
			}.AsReadOnly();
		}

		public void AddTag(string tag)
		{
			Tags = new List<string>(Tags)
			{
				tag
			}.AsReadOnly();
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
