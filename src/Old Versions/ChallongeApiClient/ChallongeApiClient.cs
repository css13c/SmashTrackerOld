using ChallongeApiClient.Properties;
using Newtonsoft.Json;
using PlayerData;
using RestSharp;
using RestSharp.Authenticators;
using SmashTracker.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;

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
			var request = new RestRequest($"tournaments/{tournamentId}.json?include_participants=1&include_matches=1", Method.GET);
			request.AddHeader("Accept", "application/json");
			var bracketJson = m_client.Execute(request);

			if (bracketJson.ResponseStatus != ResponseStatus.Completed)
				throw new Exception("Rest request failed. Try again.");

			ChallongeBracket bracket = JsonConvert.DeserializeObject<BracketResponse>(bracketJson.Content).tournament;
			//bracket.Initialize();

			if (!bracketJson.Content.Contains("pending"))
				bracket.Started = true;

			return bracket;
		}

		public ChallongeBracket CreateBracket(string name = null, BracketType bracketType = BracketType.DoubleElimination)
		{
			// Create url using Guid
			Regex rgx = new Regex("[^a-zA-Z0-9 -]");
			Guid g = Guid.NewGuid();
			string url = Convert.ToBase64String(g.ToByteArray());
			url = rgx.Replace(url, "");

			// if name is null, name it with the guid
			if (name == null)
				name = url;

			RestRequest request = new RestRequest($"tournaments.json?tournament[name]={name}&tournament[tournament_type]={bracketType.ToRequestString()}&tournament[url]={url}", Method.POST);

			var bracketJson = m_client.Execute(request);

			if (bracketJson.ResponseStatus != ResponseStatus.Completed)
				throw new Exception("Rest request failed. Try again.");

			ChallongeBracket bracket = JsonConvert.DeserializeObject<BracketResponse>(bracketJson.Content).tournament;
			bracket.Started = false;
			return bracket;
		}

		public ChallongeBracket StartTournament(string tournamentId)
		{
			var request = new RestRequest($"tournaments/{tournamentId}/start.json?include_participants=1&include_matches=1", Method.POST);
			request.AddHeader("Accept", "application/json");
			var bracketJson = m_client.Execute(request);

			if (bracketJson.ResponseStatus != ResponseStatus.Completed)
				throw new Exception("Rest request failed. Try again.");

			ChallongeBracket bracket = JsonConvert.DeserializeObject<BracketResponse>(bracketJson.Content).tournament;
			//bracket.Initialize();
			bracket.Started = true;
			return bracket;
		}

		public ChallongeBracket FinalizeTournament(string tournamentId)
		{
			var request = new RestRequest($"tournaments/{tournamentId}/finalize.json?include_participants=1&include_matches=1", Method.POST);
			request.AddHeader("Accept", "application/json");
			var bracketJson = m_client.Execute(request);

			if (bracketJson.ResponseStatus != ResponseStatus.Completed)
				throw new Exception("Rest request failed. Try again.");

			ChallongeBracket bracket = JsonConvert.DeserializeObject<BracketResponse>(bracketJson.Content).tournament;
			//bracket.Initialize();
			return bracket;
		}

		public ReadOnlyCollection<ChallongeMatch> GetMatches(string tournamentId, MatchState state = MatchState.All, string participantId = null)
		{
			RestRequest request;
			if (participantId == null)
				request = new RestRequest($"tournaments/{tournamentId}/matches.json?state={state.ToRequestString()}", Method.GET);
			else
				request = new RestRequest($"tournaments/{tournamentId}/matches.json?state={state.ToRequestString()}&participant_id={participantId}", Method.GET);

			var result = m_client.Execute(request);
			if (result.ResponseStatus != ResponseStatus.Completed)
				throw new Exception("Rest request failed. Try again.");

			return JsonConvert.DeserializeObject<List<MatchWrapper>>(result.Content).Select(m => m.Match).ToReadOnlyCollection();
		}

		public ChallongeMatch GetMatch(string tournamentId, string matchId)
		{
			var request = new RestRequest($"tournaments/{tournamentId}/matches/{matchId}.json", Method.GET);

			var result = m_client.Execute(request);
			if (result.ResponseStatus != ResponseStatus.Completed)
				throw new Exception("Rest request failed. Try again.");

			return JsonConvert.DeserializeObject<MatchWrapper>(result.Content).Match;
		}

		public ChallongeMatch UpdateMatch(string tournamentId, string matchId, string scores, string winnerId)
		{
			var request = new RestRequest($"tournaments/{tournamentId}/matches/{matchId}.json?match[scores_csv]={scores}&match[winner_id]={winnerId}", Method.PUT);

			var result = m_client.Execute(request);
			if (result.ResponseStatus != ResponseStatus.Completed)
				throw new Exception("Rest request failed. Try again.");

			return JsonConvert.DeserializeObject<MatchWrapper>(result.Content).Match;
		}

		public ReadOnlyCollection<ChallongePlayer> GetPlayers(string tournamentId)
		{
			var response = m_client.Execute(new RestRequest($"tournaments/{tournamentId}/participants.json"));
			if (response.ResponseStatus != ResponseStatus.Completed)
				throw new Exception("Rest request failed. Try again.");

			return JsonConvert.DeserializeObject<List<PlayerWrapper>>(response.Content).Select(p => p.Player).ToReadOnlyCollection();
		}

		// playersToAdd will be seeded in the order they exist, so they must be passed in with the highest seeded player being first
		public bool TryBulkAddPlayers(string tournamentId, IEnumerable<string> playersToAdd)
		{
			if (playersToAdd == null || playersToAdd.Count() == 0 || tournamentId == null)
				return false;

			try
			{
				BulkAddPlayers(tournamentId, playersToAdd);
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
			m_client.Execute(new RestRequest($"tournaments/{tournamentId}/participants/{playerId}.json"));
		}

		public ReadOnlyCollection<ChallongePlayer> RandomizeSeeds(string tournamentId)
		{
			var result = m_client.Execute(new RestRequest($"tournaments/{tournamentId}/participants/randomize.json"));

			return JsonConvert.DeserializeObject<List<PlayerWrapper>>(result.Content).Select(p => p.Player).ToReadOnlyCollection();
		}

		private ReadOnlyCollection<ChallongePlayer> BulkAddPlayers(string tournamentId, IEnumerable<string> playersToAdd)
		{
			var request = new StringBuilder($"tournaments/{tournamentId}/participants/bulk_add.json?");
			var nameAtt = "participants[][name]";
			var seedAtt = "participants[][seed]";
			int seedCounter = 1;

			foreach(var player in playersToAdd)
			{
				request.Append($"{nameAtt}={player}&{seedAtt}={seedCounter++}&");
			}

			var response = m_client.Execute(new RestRequest(request.ToString(), Method.POST));

			return JsonConvert.DeserializeObject<List<PlayerWrapper>>(response.Content).Select(p => p.Player).ToReadOnlyCollection();
		}

		private readonly RestClient m_client;
	}
}