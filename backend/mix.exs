defmodule Backend.MixProject do
  use Mix.Project

  def project do
    [
      app: :backend,
      version: "0.1.0",
      elixir: "~> 1.10",
      description: description(),
      start_permanent: Mix.env() == :prod,
      deps: deps()
    ]
  end

  def description do
    """
    Backend for my SmashTracker application.
    Skill is tracked using the Weng-Lin rules using the Bradley-Terry model with full-pair, and implemented by myself.
    Additionally, the code from https://www.csie.ntu.edu.tw/~cjlin/papers/online_ranking/ was used for reference material.
    """
  end

  # Run "mix help compile.app" to learn about applications.
  def application do
    [
      extra_applications: [:logger]
    ]
  end

  # Run "mix help deps" to learn about dependencies.
  defp deps do
    [
      {:ex_doc, ">= 0.0.0", only: :dev, runtime: false},
      {:math, "~> 0.6.0"},
      #{:statistics, "~> 0.6.2"}
    ]
  end
end
