using ChallongeApiClient.Properties;
using RestSharp;
using RestSharp.Authenticators;
using SmashTracker.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Xml.Linq;

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
			var request = new RestRequest($"{tournamentId}.xml?include_participants=1&include_matches=1", Method.GET);
			request.AddHeader("Accept", "application/xml");
			var matchResult = m_client.Execute(request);

			if (matchResult.ResponseStatus != ResponseStatus.Completed)
				throw new Exception("Rest request failed. Try again.");

			return ParseBracketXml(matchResult.Content);
		}

		public ChallongeBracket CreateBracket(string name = null, BracketType bracketType = BracketType.Double_Elimination)
		{
			RestRequest request;

			if (name == null)
				request = new RestRequest($".xml?tournament[tournament_type]={bracketType.ToRequestString()}");
			else
				request = new RestRequest($".xml?tournament[name]={name}&tournament[tournament_type]={bracketType.ToRequestString()}");

			var matchResult = m_client.Execute(request);

			if (matchResult.ResponseStatus != ResponseStatus.Completed)
				throw new Exception("Rest request failed. Try again.");

			return ParseBracketXml(matchResult.Content);
		}

		public ChallongeBracket StartTournament(string tournamentId)
		{
			var request = new RestRequest($"{tournamentId}.xml?include_participants=1&include_matches=1", Method.GET);
			request.AddHeader("Accept", "application/xml");
			var matchResult = m_client.Execute(request);

			if (matchResult.ResponseStatus != ResponseStatus.Completed)
				throw new Exception("Rest request failed. Try again.");

			return ParseBracketXml(matchResult.Content);
		}

		public ReadOnlyCollection<ChallongeMatch> GetMatches(string tournamentId, MatchState state = MatchState.All, string participantId = null)
		{
			RestRequest request;
			if (participantId == null)
				request = new RestRequest($"{tournamentId}/matches.xml?state={state.ToRequestString()}", Method.GET);
			else
				request = new RestRequest($"{tournamentId}/matches.xml?state={state.ToRequestString()}&participant_id={participantId}", Method.GET);

			var result = m_client.Execute(request);
			if (result.ResponseStatus != ResponseStatus.Completed)
				throw new Exception("Rest request failed. Try again.");

			return ParseMatchListXml(result.Content);
		}

		public ChallongeMatch GetMatch(string tournamentId, string matchId)
		{
			var request = new RestRequest($"{tournamentId}/matches/{matchId}.xml", Method.GET);

			var result = m_client.Execute(request);
			if (result.ResponseStatus != ResponseStatus.Completed)
				throw new Exception("Rest request failed. Try again.");

			var match = XDocument.Parse(result.Content).Descendants("match").Single();

			return new ChallongeMatch(
					match.Element("id").Value,
					match.Element("player1-id").Value,
					match.Element("player2-id").Value,
					(MatchState)Enum.Parse(typeof(MatchState), match.Element("state").Value, ignoreCase: true),
					Int32.Parse(match.Element("round").Value),
					match.Element("tournament-id").Value,
					match.Element("winner-id").Value,
					match.Element("loser-id").Value);
		}

		public ChallongeMatch UpdateMatch(string tournamentId, string matchId, string scores, string winnerId)
		{
			var request = new RestRequest($"{tournamentId}/matches/{matchId}.xml?match[scores_csv]={scores}&match[winner_id]={winnerId}", Method.PUT);

			var result = m_client.Execute(request);
			if (result.ResponseStatus != ResponseStatus.Completed)
				throw new Exception("Rest request failed. Try again.");

			var match = XDocument.Parse(result.Content).Descendants("match").Single();

			return new ChallongeMatch(
					match.Element("id").Value,
					match.Element("player1-id").Value,
					match.Element("player2-id").Value,
					(MatchState)Enum.Parse(typeof(MatchState), match.Element("state").Value, ignoreCase: true),
					Int32.Parse(match.Element("round").Value),
					match.Element("tournament-id").Value,
					match.Element("winner-id").Value,
					match.Element("loser-id").Value);
		}

		public ReadOnlyCollection<ChallongePlayer> GetPlayers(string tournamentId)
		{
			var response = m_client.Execute(new RestRequest($"{tournamentId}/participants.xml"));
			if (response.ResponseStatus != ResponseStatus.Completed)
				throw new Exception("Rest request failed. Try again.");

			var xmlPlayers = XDocument.Parse(response.Content).Descendants("participant");
			List<ChallongePlayer> players = new List<ChallongePlayer>();
			foreach (var player in xmlPlayers)
			{
				players.Add(new ChallongePlayer(
					player.Element("id").Value,
					player.Element("display-name").Value));
			}

			return players.ToReadOnlyCollection();
		}

		// playersToAdd will be seeded in the order they exist, so they must be passed in with the highest seeded player being first
		public bool TryBulkAddPlayers(string tournamentId, List<string> playersToAdd, out ReadOnlyCollection<ChallongePlayer> addedPlayers)
		{
			addedPlayers = null;

			if (playersToAdd == null || playersToAdd.Count == 0 || tournamentId == null)
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
			m_client.Execute(new RestRequest($"{tournamentId}/participants/{playerId}.xml"));
		}

		public void RandomizeSeeds(string tournamentId)
		{
			m_client.Execute(new RestRequest($"{tournamentId}/participants/randomize.xml"));
		}

		internal ReadOnlyCollection<ChallongePlayer> BulkAddPlayers(string tournamentId, List<string> playersToAdd)
		{
			var request = new StringBuilder($"{tournamentId}/participants/bulk_add.xml?");
			var nameAtt = "participants[][name]";
			var seedAtt = "participants[][seed]";
			int seedCounter = 1;

			foreach(var player in playersToAdd)
			{
				request.Append($"{nameAtt}={player}&{seedAtt}={seedCounter++}");
			}

			var response = m_client.Execute(new RestRequest(request.ToString()));

			List<ChallongePlayer> playersAdded = new List<ChallongePlayer>();
			var xmlDoc = XDocument.Parse(response.Content);
			foreach (var xmlPlayer in xmlDoc.Descendants("participant"))
			{
				playersAdded.Add(new ChallongePlayer(
					xmlPlayer.Element("id").Value,
					xmlPlayer.Element("display-name").Value));
			}

			return playersAdded.ToReadOnlyCollection();
		}

		internal ChallongeBracket ParseBracketXml(string xml)
		{
			var xmlDoc = XDocument.Parse(xml);
			var tournament = xmlDoc.Element("tournament");

			List<ChallongeMatch> matches = new List<ChallongeMatch>();
			List<ChallongePlayer> players = new List<ChallongePlayer>();
			foreach (var player in xmlDoc.Descendants("participant"))
			{
				players.Add(new ChallongePlayer(
					player.Element("id").Value,
					player.Element("display-name").Value));
			}
			foreach (var match in xmlDoc.Descendants("match"))
			{
				matches.Add(new ChallongeMatch(
					match.Element("id").Value,
					match.Element("player1-id").Value,
					match.Element("player2-id").Value,
					(MatchState)Enum.Parse(typeof(MatchState), match.Element("state").Value, ignoreCase: true),
					Int32.Parse(match.Element("round").Value),
					match.Element("tournament-id").Value,
					match.Element("winner-id").Value,
					match.Element("loser-id").Value));
			}

			return new ChallongeBracket(
				tournament.Element("id").Value,
				matches.ToReadOnlyDictionary(m => m.Id),
				players.ToReadOnlyDictionary(p => p.Id),
				(BracketType)Enum.Parse(typeof(BracketType), tournament.Element("tournament-type").Value.Replace(' ', '_'), ignoreCase: true));
		}
		
		internal ReadOnlyCollection<ChallongeMatch> ParseMatchListXml(string xml)
		{
			var xmlDoc = XDocument.Parse(xml);
			List<ChallongeMatch> matches = new List<ChallongeMatch>();

			foreach (var match in xmlDoc.Descendants("match"))
			{
				matches.Add(new ChallongeMatch(
					match.Element("id").Value,
					match.Element("player1-id").Value,
					match.Element("player2-id").Value,
					(MatchState)Enum.Parse(typeof(MatchState), match.Element("state").Value, ignoreCase: true),
					Int32.Parse(match.Element("round").Value),
					match.Element("tournament-id").Value,
					match.Element("winner-id").Value,
					match.Element("loser-id").Value));
			}

			return matches.ToReadOnlyCollection();
		}

		private readonly RestClient m_client;
	}
}