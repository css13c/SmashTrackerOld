using System;
using System.Xml.Linq;

namespace ChallongeApiClient
{
	public class ChallongePlayer
	{
		public ChallongePlayer(string id, string tag)
		{
			Id = id;
			Tag = tag;
		}

		public string Id { get; set; }
		public string Tag { get; set; }
	}
}
