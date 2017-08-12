using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChallongeApiClient
{
	class ChallongeBracket
	{
		int Id { get; set; }
		ReadOnlyDictionary<int, ChallongeMatch> Matches { get; set; }
		ReadOnlyDictionary<int, ChallongePlayer> Players { get; set; }
	}
}
