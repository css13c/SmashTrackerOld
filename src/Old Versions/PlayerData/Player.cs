using System;
using System.Collections.Generic;
using System.Text;
using TrueSkill;

namespace PlayerData
{
	public class Player
	{
		public Player(int id, string name, string tag, Rating rating, List<Character> characters)
		{
			Id = id;
			Name = name;
			Tag = tag;
			Rating = rating;
			Characters = characters;
		}

		public Player()
		{
			Tag = null;
			Characters = new List<Character>();
		}

		public int Id { get; set; }
		public string Name { get;  set; }
		public string Tag { get;  set; }
		public Rating Rating { get;  set; }
		public List<Character> Characters { get;  set; }
		public String CharString
		{
			get { return PrintChars(); }
		}

		public override string ToString()
		{
			return $"Name: {Name};  Tag: {Tag};  Characters: {PrintChars()};  Rating: {Rating}";
		}

		public string ToStringWithId()
		{
			return $"Id: {Id};  Name: {Name};  Tag: {Tag};  Characters: {PrintChars()};  Rating: {Rating}";
		}

		public string PrintChars()
		{
			StringBuilder str = new StringBuilder();
			foreach (var character in Characters)
				str.Append($"{character.ToOutput()}, ");
			str.Remove(str.Length - 2, 2);

			return str.ToString();
		}
	}
}
