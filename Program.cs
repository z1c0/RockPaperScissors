using System.Diagnostics;

Randomness();
Simple();
Improved();
CompareImproved();

static List<IPlayer> CreatePlayerTypes(bool improved)
{
	var strategies = Enum.GetValues(typeof(Strategy)).Cast<Strategy>().ToList();
	strategies.Remove(Strategy.Random);
	strategies.Remove(Strategy.Rock);
	strategies.Remove(Strategy.Paper);
	strategies.Remove(Strategy.Scissors);

	if (improved)
	{
		return(
			from winStrategy in strategies
			from loseStrategy in strategies
			from tieStrategy in strategies
			select new ImprovedPlayer(winStrategy, loseStrategy, tieStrategy)).ToList<IPlayer>();
	}
	else
	{
		return strategies.Select(s => new Player(s)).ToList<IPlayer>();
	}
}

void Randomness()
{
	var player1 = new Player(Strategy.Random);
	var player2 = new Player(Strategy.Random);
	var statistics = new Statistics();
	Play(player1, player2, 1_000_000, statistics);
	statistics.PrintGame();
}

void Simple()
{
	Simulate(CreatePlayerTypes(false), 100_000);
}

void Improved()
{
	Simulate(CreatePlayerTypes(true), 1000);
}

void CompareImproved()
{
	var playerTypes = CreatePlayerTypes(true);

  var player1 = new ImprovedPlayer(Strategy.Forward, Strategy.Copy, Strategy.Forward);
  var player2 = new ImprovedPlayer(Strategy.Copy, Strategy.Forward, Strategy.Forward);

	var losing1 = new HashSet<IPlayer>();
	foreach (var p in playerTypes)
	{
		if (Play(player1, p, 1000, new()))
		{
			losing1.Add(p);
		}
	}
	var losing2 = new HashSet<IPlayer>();
	foreach (var p in playerTypes)
	{
		if (Play(player2, p, 1000, new()))
		{
			losing2.Add(p);
		}
	}

	var diff1 = losing1.Except(losing2);
	Console.WriteLine($"Only [{player1}] wins against:");
	foreach (var d in diff1)
	{
		Console.WriteLine($"- {d}");
	}
	var diff2 = losing2.Except(losing1);
	Console.WriteLine($"Only [{player2}] wins against:");
	foreach (var d in diff2)
	{
		Console.WriteLine($" - {d}");
	}
}

void Simulate(List<IPlayer> playerTypes, int rounds)
{
	var statistics = new Statistics();
	foreach (var player1 in playerTypes)
	{
		foreach (var player2 in playerTypes)
		{
			Play(player1, player2, rounds, statistics);
		}
	}
	statistics.PrintRanking();
}

bool Play(IPlayer player1, IPlayer player2, int rounds, Statistics statistics)
{
	void PlayWithStartMoves(Move movePlayer1, Move movePlayer2)
	{
		for (var i = 0; i < rounds; i++)
		{
			var (resultPlayer1, resultPlayer2) = Versus(movePlayer1, movePlayer2);
			statistics.Update(resultPlayer1);
			(movePlayer1, movePlayer2) = (player1.NextMove(resultPlayer1, movePlayer1, movePlayer2), player2.NextMove(resultPlayer2, movePlayer2, movePlayer1));
		}

		if (player1.IsRandom || player2.IsRandom)
		{
			Debug.Assert(Math.Abs(statistics.Wins - 33) <= 1);
			Debug.Assert(Math.Abs(statistics.Losses - 33) <= 1);
			Debug.Assert(Math.Abs(statistics.Ties - 33) <= 1);
		}
	}
	statistics.BeginGame(player1, player2);

	PlayWithStartMoves(Move.Rock, Move.Rock);
	PlayWithStartMoves(Move.Rock, Move.Paper);
	PlayWithStartMoves(Move.Rock, Move.Scissors);
	PlayWithStartMoves(Move.Paper, Move.Rock);
	PlayWithStartMoves(Move.Paper, Move.Paper);
	PlayWithStartMoves(Move.Paper, Move.Scissors);
	PlayWithStartMoves(Move.Scissors, Move.Rock);
	PlayWithStartMoves(Move.Scissors, Move.Paper);
	PlayWithStartMoves(Move.Scissors, Move.Scissors);
	
	var successful = statistics.Wins > 50;
	if (successful)
	{
		statistics.PrintGame();
	}
	statistics.EndGame();

	return successful;
}

static (Result, Result) Versus(Move movePlayer1, Move movePlayer2)
{
	if (movePlayer1 == movePlayer2)
	{
		return (Result.Tie, Result.Tie);
	}
	return movePlayer1 switch
	{
		Move.Rock => movePlayer2 == Move.Paper ? (Result.Lose, Result.Win) : (Result.Win, Result.Lose),
		Move.Paper => movePlayer2 == Move.Scissors ? (Result.Lose, Result.Win) : (Result.Win, Result.Lose),
		Move.Scissors => movePlayer2 == Move.Rock ? (Result.Lose, Result.Win) : (Result.Win, Result.Lose),
		_ => throw new InvalidOperationException(),
	};
}



