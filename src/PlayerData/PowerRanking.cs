using System.Collections.ObjectModel;

namespace PlayerData
{
	public class PowerRanking
	{
		public PowerRanking(ReadOnlyDictionary<int, Player> rankings, int size)
		{
			Rankings = rankings;
			Size = size;
		}

		public ReadOnlyDictionary<int, Player> Rankings { get; }
		public int Size { get; }
	}
}
