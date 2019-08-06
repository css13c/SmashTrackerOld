using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChallongeApiClient
{
	public class MatchWrapper
	{
		[JsonProperty("match")]
		public ChallongeMatch Match { get; set; }
	}
}
