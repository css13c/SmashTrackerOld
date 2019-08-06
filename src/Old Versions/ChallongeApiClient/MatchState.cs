using System;
using Newtonsoft.Json;

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

	public class MatchStateConverter : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			string enumString = (string)reader.Value;
			return Enum.Parse(typeof(MatchState), enumString, true);
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(string);
		}
	}
}
