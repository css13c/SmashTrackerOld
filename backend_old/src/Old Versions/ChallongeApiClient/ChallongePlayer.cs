using Newtonsoft.Json;
using System;
using System.Xml.Linq;

namespace ChallongeApiClient
{
	public class ChallongePlayer
	{
		public ChallongePlayer(string id, string tag, int seed)
		{
			Id = id;
			Tag = tag;
			Seed = seed;
		}

		[JsonProperty("id")]
		public string Id { get; set; }

		[JsonProperty("name")]
		public string Tag { get; set; }

		[JsonProperty("seed")]
		public int Seed { get; set; }
	}
}
