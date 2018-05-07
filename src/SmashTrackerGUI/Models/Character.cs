using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmashTrackerGUI.Models
{
	public enum Character
	{
		error = -1,
		Bayonetta,
		Bowser,
		BowserJr,
		Charizard,
		Cloud,
		Corrin,
		CptFalcon,
		DarkPit,
		DiddyKong,
		DK,
		Falco,
		Fox,
		Ganondorf,
		Greninja,
		IceClimbers,
		Ike,
		Ivysaur,
		Jigglypuff,
		KingDedede,
		Kirby,
		Link,
		LittleMac,
		Lucario,
		Lucas,
		Luigi,
		Mario,
		Marth,
		Megaman,
		MetaKnight,
		Mewtwo,
		MiiFighter,
		MiiGunner,
		MiiSwordsman,
		MrGameAndWatch,
		Ness,
		Olimar,
		Pacman,
		Palutena,
		Peach,
		Pikachu,
		Pit,
		Random,
		ROB,
		Robin,
		Rosalina,
		Roy,
		Ryu,
		Samus,
		Sheik,
		Shulk,
		Snake,
		Sonic,
		Squirtle,
		ToonLink,
		Villager,
		Wario,
		WiiFitTrainer,
		Wolf,
		Yoshi,
		YoungLink,
		Zelda,
		ZeroSuitSamus,
	}

	public static class CharacterExtensions
	{
		public static string ToOutput(this Character character)
		{
			StringBuilder builder = new StringBuilder(character.ToString());
			for (int i = 1; i < builder.Length; i++)
			{
				if (char.IsUpper(builder[i]))
					builder.Insert(i, ' ');
			}

			return builder.ToString();
		}
	}
}
