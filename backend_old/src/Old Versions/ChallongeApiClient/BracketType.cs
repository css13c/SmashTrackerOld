namespace ChallongeApiClient
{
	public enum BracketType
	{
		DoubleElimination,
		SingleElimination,
	}

	public static class BracketTypeExtensions
	{
		public static string ToRequestString(this BracketType source)
		{
			switch(source)
			{
				case BracketType.SingleElimination:
					return "single elimination";
				default:
					return "double elimination";
			}
		}
	}
}
