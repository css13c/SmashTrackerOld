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
			Print("This is SmashTracker. To get a list of commands and their functions, enter 'help'.");

		}

		public static void PrintHelp()
		{
			Print($"This is" );
		}

		public static void Print(string text)
		{
			Console.WriteLine(text);
		}
	}
}
