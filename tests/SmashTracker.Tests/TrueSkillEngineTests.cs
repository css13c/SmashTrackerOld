using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using SmashTracker.Engines.TrueSkill;
using SmashTracker.Engines.TrueSkill.Interfaces;
using System.Threading.Tasks;
using Xunit;

namespace SmashTracker.Tests
{
	public class TrueSkillEngineTests
	{
		[Theory, AutoData]
		public void Default_Rating_Should_Have_Expected_Default_Values(double initialMean, double initialSd, double conservativeMultiplier)
		{
			// Arrange
			var trueSkillEngine = new TrueSkillEngine(initialMean, initialSd, conservativeMultiplier);

			// Act
			var rating = trueSkillEngine.GetDefaultRating();

			// Assert
			rating.StandardDeviation.Should().Be(initialSd, "The default rating should have the given initial standard deviation");
			rating.Mean.Should().Be(initialMean, "The default rating should have the given initial mean");
		}

		[Theory, AutoData]
		public void Should_Update_Both_Ratings(Rating winner, Rating loser)
		{
			// Arrange
			var trueSkillEngine = new TrueSkillEngine();
			var winnerInitialValue = new Rating(winner.Mean, winner.StandardDeviation);
			var loserInitialValue = new Rating(loser.Mean, loser.StandardDeviation);

			// Act
			trueSkillEngine.UpdateRatings(winner, loser);

			// Assert
			winner.Should().NotBeEquivalentTo(winnerInitialValue, "Update ratings should update both the winner's mean and standard deviation");
			loser.Should().NotBeEquivalentTo(loserInitialValue, "Update ratings should update both the loser's mean and standard deviation");
		}

		[Theory, AutoData]
		public void Winners_Rating_Should_Increase(Rating winner, Rating loser)
		{
			// Arrange
			var trueSkillEngine = new TrueSkillEngine();
			var winnerInitialValue = new Rating(winner.Mean, winner.StandardDeviation);

			// Act
			trueSkillEngine.UpdateRatings(winner, loser);

			// Assert
			winner.Mean.Should().BeGreaterThan(winnerInitialValue.Mean, "The winner's mean should have increased due to winning a match");
			winner.StandardDeviation.Should().BeLessThan(winnerInitialValue.StandardDeviation, "The winner's standard deviation should have decreased due to having played more matches");
		}

		[Theory, AutoData]
		public void Losers_Rating_Should_Decrease(Rating winner, Rating loser)
		{
			// Arrange
			var trueSkillEngine = new TrueSkillEngine();
			var loserInitialValue = new Rating(loser.Mean, loser.StandardDeviation);

			// Act
			trueSkillEngine.UpdateRatings(winner, loser);

			// Assert
			loser.Mean.Should().BeLessThan(loserInitialValue.Mean, "The loser's mean should have decreased due to losing a match.");
			loser.StandardDeviation.Should().BeLessThan(loserInitialValue.StandardDeviation, "The loser's standard deviation should have decreased due to having played more matches.");
		}

		[Theory, AutoData]
		public void Conservative_Rating_Equation_Outputs_Expected(double mean, double sd, double conservativeMultiplier)
		{
			// Copied from TrueSkillEngine.GetConservativeRating()
			var expectedConservativeRating = mean - conservativeMultiplier * sd;
			var trueSkillEngine = new TrueSkillEngine(initialMean: mean, initialStandardDeviation: sd, conservativeRatingMultiplier: conservativeMultiplier);
			trueSkillEngine.GetConservativeRating(trueSkillEngine.GetDefaultRating()).Should().Be(expectedConservativeRating, "The conservative rating is calculated using the given formula, so it should match when using the given values.");
		}
	}
}
