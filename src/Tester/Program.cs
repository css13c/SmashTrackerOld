using ChallongeApiClient;
using PlayerData;
using SmashTracker.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tester
{
	class Program
	{
		static void Main(string[] args)
		{
			PlayerDatabase pd = new PlayerDatabase();
			ChallongeClient client = new ChallongeClient();

			pd.AddPlayer("Connor", 4.45, Character.Fox, "Tipzz");
			var pr = pd.GetPR(1);

			Console.WriteLine($"{pr.Single().Name}");
			Console.Read();
		}
	}
}
