using ChallongeApiClient.Properties;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using SmashTracker.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace ChallongeApiClient
{
	public sealed class ChallongeClient
	{
		public ChallongeClient()
		{
			m_client = new RestClient
			{
				BaseUrl = new Uri(Settings.Default.BaseUri),
				Authenticator = new HttpBasicAuthenticator(Settings.Default.Username, Settings.Default.ApiKey),
			};
		}

		public ChallongeBracket GetBracket(string tournamentId)
		{
			var request = new RestRequest($"{tournamentId}.json?include_participants=1&include_matches=1", Method.GET);
			request.AddHeader("Accept", "application/json");
			var bracketJson = m_client.Execute(request);

			if (bracketJson.ResponseStatus != ResponseStatus.Completed)
				throw new Exception("Rest request failed. Try again.");

			ChallongeBracket bracket = JsonConvert.DeserializeObject<ChallongeBracket>(bracketJson.Content);
			bracket.Initialize();

			if (!bracketJson.Content.Contains("pending"))
				bracket.Started = true;

			return bracket;
		}

		public ChallongeBracket CreateBracket(string name = null, BracketType bracketType = BracketType.Double_Elimination)
		{
			RestRequest request;

			if (name == null)
				request = new RestRequest($".json?tournament[tournament_type]={bracketType.ToRequestString()}");
			else
				request = new RestRequest($".json?tournament[name]={name}&tournament[tournament_type]={bracketType.ToRequestString()}");

			var bracketJson = m_client.Execute(request);

			if (bracketJson.ResponseStatus != ResponseStatus.Completed)
				throw new Exception("Rest request failed. Try again.");

			ChallongeBracket bracket = JsonConvert.DeserializeObject<ChallongeBracket>(bracketJson.Content);
			bracket.Initialize();
			bracket.Started = false;
			return bracket;
		}

		public ChallongeBracket StartTournament(string tournamentId)
		{
			var request = new RestRequest($"{tournamentId}.json?include_participants=1&include_matches=1", Method.GET);
			request.AddHeader("Accept", "application/json");
			var bracketJson = m_client.Execute(request);

			if (bracketJson.ResponseStatus != ResponseStatus.Completed)
				throw new Exception("Rest request failed. Try again.");

			ChallongeBracket bracket = JsonConvert.DeserializeObject<ChallongeBracket>(bracketJson.Content);
			bracket.Initialize();
			bracket.Started = true;
			return bracket;
		}

		public ReadOnlyCollection<ChallongeMatch> GetMatches(string tournamentId, MatchState state = MatchState.All, string participantId = null)
		{
			RestRequest request;
			if (participantId == null)
				request = new RestRequest($"{tournamentId}/matches.json?state={state.ToRequestString()}", Method.GET);
			else
				request = new RestRequest($"{tournamentId}/matches.json?state={state.ToRequestString()}&participant_id={participantId}", Method.GET);

			var result = m_client.Execute(request);
			if (result.ResponseStatus != ResponseStatus.Completed)
				throw new Exception("Rest request failed. Try again.");

			List<ChallongeMatch> matchList = JsonConvert.DeserializeObject<List<ChallongeMatch>>(result.Content);
			return matchList.ToReadOnlyCollection();
		}

		public ChallongeMatch GetMatch(string tournamentId, string matchId)
		{
			var request = new RestRequest($"{tournamentId}/matches/{matchId}.json", Method.GET);

			var result = m_client.Execute(request);
			if (result.ResponseStatus != ResponseStatus.Completed)
				throw new Exception("Rest request failed. Try again.");

			return JsonConvert.DeserializeObject<ChallongeMatch>(result.Content);
		}

		public ChallongeMatch UpdateMatch(string tournamentId, string matchId, string scores, string winnerId)
		{
			var request = new RestRequest($"{tournamentId}/matches/{matchId}.json?match[scores_csv]={scores}&match[winner_id]={winnerId}", Method.PUT);

			var result = m_client.Execute(request);
			if (result.ResponseStatus != ResponseStatus.Completed)
				throw new Exception("Rest request failed. Try again.");

			return JsonConvert.DeserializeObject<ChallongeMatch>(result.Content);
		}

		public ReadOnlyCollection<ChallongePlayer> GetPlayers(string tournamentId)
		{
			var response = m_client.Execute(new RestRequest($"{tournamentId}/participants.json"));
			if (response.ResponseStatus != ResponseStatus.Completed)
				throw new Exception("Rest request failed. Try again.");

			return JsonConvert.DeserializeObject<List<ChallongePlayer>>(response.Content).ToReadOnlyCollection();
		}

		// playersToAdd will be seeded in the order they exist, so they must be passed in with the highest seeded player being first
		public bool TryBulkAddPlayers(string tournamentId, IEnumerable<string> playersToAdd, out ReadOnlyCollection<ChallongePlayer> addedPlayers)
		{
			addedPlayers = null;

			if (playersToAdd == null || playersToAdd.Count() == 0 || tournamentId == null)
				return false;

			try
			{
				addedPlayers = BulkAddPlayers(tournamentId, playersToAdd);
			}
			catch
			{
				return false;
			}

			return true;
		}

		// DQ's player from all future matches if tournament is in progress
		public void DeletePlayer(string tournamentId, string playerId)
		{
			m_client.Execute(new RestRequest($"{tournamentId}/participants/{playerId}.json"));
		}

		public ReadOnlyCollection<ChallongePlayer> RandomizeSeeds(string tournamentId)
		{
			var result = m_client.Execute(new RestRequest($"{tournamentId}/participants/randomize.json"));

			return JsonConvert.DeserializeObject<List<ChallongePlayer>>(result.Content).ToReadOnlyCollection();
		}

		private ReadOnlyCollection<ChallongePlayer> BulkAddPlayers(string tournamentId, IEnumerable<string> playersToAdd)
		{
			var request = new StringBuilder($"{tournamentId}/participants/bulk_add.json?");
			var nameAtt = "participants[][name]";
			var seedAtt = "participants[][seed]";
			int seedCounter = 1;

			foreach(var player in playersToAdd)
			{
				request.Append($"{nameAtt}={player}&{seedAtt}={seedCounter++}&");
			}

			var response = m_client.Execute(new RestRequest(request.ToString()));

			return JsonConvert.DeserializeObject<List<ChallongePlayer>>(response.Content).ToReadOnlyCollection();
		}

		private readonly RestClient m_client;
	}
}