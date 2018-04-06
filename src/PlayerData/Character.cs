using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayerData
{
	public enum Character
	{
		error = -1,
		Fox,
		Wario,
		MetaKnight,
		CptFalcon,
		MrGameAndWatch,
		Marth,
		Ness,
		Ganondorf,
		Snake,
		Sheik,
		Ike,
		Luigi,
		Samus,
		ZeroSuitSamus,
		Falco,
		DonkeyKong,
		Charizard,
		Zelda,
		Peach,
		Mario,
		Wolf,
		Sonic,
		ToonLink,
		Ivysaur,
		Link,
		Kirby,
		Diddy,
		IceClimbers,
		Lucas,
		Roy,
		Rob,
		Mewtwo,
		Olimar,
		Lucario,
		Squirtle,
		Bowser,
		Jigglypuff,
		Pikachu,
		KingDedede,
		Yoshi,
		Pit,
		DarkPit,
	}

	public static class CharacterExtensions
	{
		public static string ToOutput(this Character character)
		{
			StringBuilder builder = new StringBuilder(character.ToString());
			for(int i=1; i<builder.Length; i++)
			{
				if (char.IsUpper(builder[i]))
					builder.Insert(i, ' ');
			}

			return builder.ToString();
		}
	}
}
