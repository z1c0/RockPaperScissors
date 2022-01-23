enum Result
{
	Win,
	Lose,
	Tie,
}

enum Move
{
	Rock,
	Paper,
	Scissors,
}

enum Strategy
{
	Random,
	Keep,
	Forward,
	Backward,
	Copy,
	Rock,
	Paper,
	Scissors,
}

interface IPlayer
{
	bool IsRandom { get; }
	Move NextMove(Result lastResult, Move lastMove, Move lastMoveOtherPlayer);
}

internal class Player : IPlayer
{
	private static readonly Random _random = new();
	private readonly Strategy _strategy;

	public bool IsRandom => _strategy == Strategy.Random;

	internal Player(Strategy strategy)
	{
		_strategy = strategy;
	}

	public override string ToString() => _strategy.ToString();

	public Move NextMove(Result _, Move lastMove, Move lastMoveOtherPlayer) => GetNextMoveForStrategy(_strategy, lastMove, lastMoveOtherPlayer);

	internal static Move GetNextMoveForStrategy(Strategy strategy, Move lastMove, Move lastMoveOtherPlayer)
	{
		return strategy switch
		{
			Strategy.Random => (Move)_random.Next(3),
			Strategy.Keep => lastMove,
			Strategy.Forward => (Move)(((int)lastMove + 1) % 3),
			Strategy.Backward => (Move)(((int)lastMove + 3 - 1) % 3),
			Strategy.Copy => lastMoveOtherPlayer,
			Strategy.Rock => Move.Rock,
			Strategy.Paper => Move.Paper,
			Strategy.Scissors => Move.Scissors,
			_ => throw new NotImplementedException(strategy.ToString()),
		};
	}
}

internal class ImprovedPlayer : IPlayer
{
	private readonly Strategy _winStrategy;
	private readonly Strategy _loseStrategy;
	private readonly Strategy _tieStrategy;

	public bool IsRandom => _winStrategy == Strategy.Random || _loseStrategy == Strategy.Random || _tieStrategy == Strategy.Random;

	public ImprovedPlayer(Strategy winStrategy, Strategy loseStrategy, Strategy tieStrategy)
	{
		_winStrategy = winStrategy;
		_loseStrategy = loseStrategy;
		_tieStrategy = tieStrategy;
	}


	public override string ToString() => $"{_winStrategy}/{_loseStrategy}/{_tieStrategy}";

	public Move NextMove(Result lastResult, Move lastMove, Move lastMoveOtherPlayer)
	{
		var strategy = lastResult switch
		{
			Result.Win => _winStrategy,
			Result.Lose => _loseStrategy,
			Result.Tie => _tieStrategy,
			_ => throw new InvalidOperationException(),
		};
		return Player.GetNextMoveForStrategy(strategy, lastMove, lastMoveOtherPlayer);
	}
}