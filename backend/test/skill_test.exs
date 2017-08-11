defmodule SkillTest do
  use ExUnit.Case, async: true
  doctest SmashTracker.Skill

  alias SmashTracker.Skill, as: Skill
  @sigma_delta 0.0001

  test "returns default rating" do
    {mu, sigma} = Skill.new_rating()
    assert mu == 25.0
    assert_in_delta sigma, 8.3333333, @sigma_delta
  end

  test "returns given rating" do
    {mu, sigma} = Skill.new_rating(1250, 35)
    assert mu == 1250
    assert sigma == 35
  end

  test "only give mu" do
    {mu, sigma} = Skill.new_rating(1250)
    assert mu == 1250
    assert_in_delta sigma, 8.3333333, @sigma_delta
  end

  test "0 is default rating estimate" do
    assert 0 == Skill.new_rating() |> Skill.rating_est()
  end

  test "normal update" do
    {p1_mu, p1_sig} = Skill.new_rating()
    {p2_mu, p2_sig} = Skill.new_rating()
    assert [
            {1, 27.63523138347365, 8.065506316323548},
            {2, 22.36476861652635, 8.065506316323548}
           ] == Skill.update_rating([{1, p1_mu, p1_sig}, {2, p2_mu, p2_sig}])
  end

  test "game with 1 player does not update" do
    {p1_mu, p1_sig} = Skill.new_rating()
    p1 = {1, p1_mu, p1_sig}
    assert [p1] == Skill.update_rating([p1])
  end

  test "no error thrown on empty game" do
    assert Skill.update_rating([])
  end
end
