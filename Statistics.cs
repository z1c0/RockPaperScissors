internal class Statistics
{
	private readonly Dictionary<IPlayer, int> _ranking = new();

	private int _gamesPlayed;
	private int _winsTotal;
	private int _lossesTotal;
	private int _tiesTotal;
	private IPlayer? _player1;
	private IPlayer? _player2;

	internal void BeginGame(IPlayer player1, IPlayer player2)
	{
		_player1 = player1;
		_player2 = player2;
		_gamesPlayed = 0;
		_winsTotal = 0;
		_lossesTotal = 0;
		_tiesTotal = 0;
	}

	internal void EndGame()
	{
		if (Wins > 50)
		{
			_ranking.TryAdd(_player1!, 0);
			_ranking[_player1!]++;
		}
	}

	internal void Update(Result result)
	{
		_gamesPlayed++;
		switch (result)
		{
			case Result.Win:
				_winsTotal++;
				break;

			case Result.Lose:
				_lossesTotal++;
				break;

			case Result.Tie:
				_tiesTotal++;
				break;

			default:
				throw new InvalidOperationException();
		}
	}

	internal void PrintGame()
	{
		Console.WriteLine($"--- [{_player1}] vs [{_player2}]");
		Console.WriteLine($"Number of games played: {_gamesPlayed}");
		Console.WriteLine($"  Wins: {Wins}% ({_winsTotal})");
		Console.WriteLine($"  Losses: {Losses}% ({_lossesTotal})");
		Console.WriteLine($"  Ties: {Ties}% ({_tiesTotal})");
		Console.WriteLine();
	}

	internal void PrintRanking()
	{
		Console.WriteLine($"Most successful strategies:");
		foreach (var s in _ranking.OrderByDescending(e => e.Value).Take(5))
		{
			Console.WriteLine($"  {s.Key} - score: {s.Value}");
		}
	}

	internal int Wins => (int)((double)_winsTotal / _gamesPlayed * 100);

	internal int Losses => (int)((double)_lossesTotal / _gamesPlayed * 100);
	internal int Ties => (int)((double)_tiesTotal / _gamesPlayed * 100);
}

