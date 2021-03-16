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
    assert mu = 1250
    assert sigma == 35
  end

  test "0 is default rating estimate" do
    assert 0 == Skill.new_rating() |> Skill.rating_est()
  end

  test "update works" do
    {a_mu, a_sig} = Skill.new_rating(29.182, 4.782)
    {b_mu, b_sig} = Skill.new_rating(27.174)
    {c_mu, c_sig} = Skill.new_rating(16.672, 6.217)
    {d_mu, d_sig} = Skill.new_rating()

    a = {1, a_mu, a_sig}
    b = {2, b_mu, b_sig}
    c = {3, c_mu, c_sig}
    d = {4, d_mu, d_sig}
    assert [
            {1, 31.643721109067318, 4.5999011726035866},
            {2, 27.579203181313282, 4.711537319421646},
            {3, 16.96606210683349, 5.824625458553909},
            {4, 15.834345097607386, 7.1129977453618745}
          ] == Skill.update_rating([a, b, c, d])
  end
end
