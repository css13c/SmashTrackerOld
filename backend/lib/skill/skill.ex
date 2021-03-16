defmodule SmashTracker.Skill do
  @moduledoc """
  Utilities for the rating system. Some aspects of updating the rating are simplified
  since we are working under the assumption that a team consists of only 1 player.
  """

  @mu 25.0
  @sigma @mu / 3.0
  @beta @sigma / 2.0

  alias SmashTracker.Utils.Boolean, as: Bool

  @doc """
  Initializes a new rating with the given parameters. If no parameters given,
  defaults to environment settings.
  """
  @spec new_rating(float | nil, float | nil) :: {float, float}
  def new_rating(mu \\ nil, sigma \\ nil) do
    {mu || @mu, sigma || @sigma}
  end

  @doc """
  Gets the conservative ordinal value of the given rating.
  """
  @spec rating_est({float, float}) :: float
  def rating_est({mu, sigma}), do: max(mu - 3 * sigma, 0)

  @doc """
  Updates the rating for both players. Function assumes players are in ranked order,
  where the first player is first place, second is second place, and so on.
  """
  @spec update_rating(list({pos_integer, float, float})) :: list({pos_integer, float, float})
  def update_rating(players) do
    f_players = Enum.with_index(players, 1)
    |> Enum.map(fn {{id, mu, sig}, i} ->
      {id, mu, sig, i}
    end)

    # In the Weng-Lin implementation, we'd have to calculate each team players'
    # change. However, since each team is only 1 player, the calculation of
    # 'player_mu / team_mu' will always be 1, and likewise with sigma, so no
    # multiplier needs to be calculated for both omega and delta.
    Enum.map(f_players, fn {id, i_mu, i_sig, i} ->
      i_sigsq = i_sig * i_sig
      Enum.filter(f_players, fn {_, _, _, q} -> i != q end)
      |> Enum.map(fn {_, q_mu, q_sig, q} ->
        q_sigsq = q_sig * q_sig
        ciq = Math.sqrt(i_sigsq + q_sigsq + 2*@beta*@beta)
        piq = 1 / (1 + Math.exp((q_mu - i_mu) / ciq))
        sigsq_to_ciq = i_sigsq / ciq
        s = Bool.to_int(q > i)
        gamma = Math.sqrt(i_sigsq) / ciq
        omega = sigsq_to_ciq * (s - piq)
        delta = gamma * sigsq_to_ciq / ciq * piq * (1 - piq)
        {id, i_mu + omega, i_sig * max(1 - delta, 0.0001)}
      end)
    end)
  end
end
