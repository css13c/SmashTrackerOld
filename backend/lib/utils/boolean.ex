defmodule SmashTracker.Utils.Boolean do
  @moduledoc """
  Reusable boolean utilities
  """

  @spec to_int(boolean) :: integer
  def to_int(b) when is_nil(b), do: 0
  def to_int(true), do: 1
  def to_int(false), do: 0
end
