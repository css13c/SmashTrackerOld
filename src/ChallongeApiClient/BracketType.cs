namespace ChallongeApiClient
{
	public enum BracketType
	{
		Double_Elimination,
		Single_Elimination,
	}

	public static class BracketTypeExtensions
	{
		public static string ToRequestString(this BracketType source)
		{
			switch(source)
			{
				case BracketType.Single_Elimination:
					return "single elimination";
				default:
					return "double elimination";
			}
		}
	}
}
