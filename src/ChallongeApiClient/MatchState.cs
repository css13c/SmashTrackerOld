namespace ChallongeApiClient
{
	public enum MatchState
	{
		All,
		Open,
		Pending,
		Complete,
	}

	public static class EnumExtensions
	{
		public static string ToRequestString(this MatchState state)
		{
			switch (state)
			{
				case MatchState.All:
					return "all";
				case MatchState.Complete:
					return "complete";
				case MatchState.Open:
					return "open";
				case MatchState.Pending:
					return "pending";
				default:
					return "all";
			}
		}
	}
}
