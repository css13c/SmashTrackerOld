using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChallongeApiClient
{
	public class PlayerWrapper
	{
		[JsonProperty("participant")]
		public ChallongePlayer Player { get; set; }
	}
}
