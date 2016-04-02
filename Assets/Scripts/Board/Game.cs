using UnityEngine;
using System.Collections.Generic;

public enum Player
{
	Player,
	Opponent
}

public enum GameType
{
	BasicNim,
	RowNim,
	FibonacciNim
}

public class Game : MonoBehaviour
{
	// Types
	public enum State
	{
		PlayerTurn,
		OpponentTurn
	}

	// Static variables
	public static Game instance;

	// Public variables
	public float pileRadius;
	public Board board;
	public PlayerHand playerHand;
	public EnemyHand enemyHand;

	// Private variables
	private State state_;
	private GameType gameType_;
	private Pile chosenPile_;
	private int takenRocks_;
	private int maxAvailableRocks_;

	private int opponentRocksToTake_;
	private Pile opponentPileToTakeFrom_;

	private Pile YourPile
	{
		get
		{
			switch (state_)
			{
				case State.PlayerTurn:
					return board.playerPile;
				case State.OpponentTurn:
					return board.opponentPile;
				default:
					throw new System.InvalidOperationException();
			}
		}
	}
	private Pile TheirPile
	{
		get
		{
			switch (state_)
			{
				case State.PlayerTurn:
					return board.opponentPile;
				case State.OpponentTurn:
					return board.playerPile;
				default:
					throw new System.InvalidOperationException();
			}
		}
	}

	// Initialization
	public void Awake()
	{
		instance = this;
	}

	// State transitions
	private void TransitionState(State newState)
	{
		state_ = newState;
		switch (state_)
		{
			case State.PlayerTurn:
				ResetForNextTurn();
				enemyHand.Inactive();

				playerHand.Reach(PlayerOnGrabRock);
				break;
			case State.OpponentTurn:
				ResetForNextTurn();
				playerHand.Inactive();

				GetWorstMove(out opponentRocksToTake_, out opponentPileToTakeFrom_);
				OpponentGrabNextRock();
				break;
			default:
				break;
		}
	}

	// Public interface
	public bool CanAdvanceTurn()
	{
		return takenRocks_ > 0;
	}
	public void AdvanceTurn()
	{
		if (board.RocksLeft == 0)
		{
			switch (state_)
			{
				case State.PlayerTurn:
					Level.instance.OnGameFinished(Player.Player);
					break;
				case State.OpponentTurn:
					Level.instance.OnGameFinished(Player.Opponent);
					break;
				default:
					throw new System.InvalidOperationException();
			}
			return;
		}

		switch (state_)
		{
			case State.PlayerTurn:
				TransitionState(State.OpponentTurn);
				break;
			case State.OpponentTurn:
				TransitionState(State.PlayerTurn);
				break;
			default:
				throw new System.InvalidOperationException();
		}
	}
	public void BeginNim(GameType gameType, int[] rockCounts, Player startingPlayer, float mistakeChance)
	{
		board.Reset();
		gameType_ = gameType;

		if (rockCounts.Length == 1)
		{
			Pile pile = board.AddPile(Vector2.zero);
			for (int i = 0; i < rockCounts[0]; ++i)
			{
				Rock rock = board.AddRock();
				pile.AddRock(rock);
			}
		}
		else
		{
			for (int i = 0; i < rockCounts.Length; ++i)
			{
				float angle = Mathf.PI * 2.0f * i / rockCounts.Length;
				Pile pile = board.AddPile(new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * pileRadius);

				for (int j = 0; j < rockCounts[i]; ++j)
				{
					Rock rock = board.AddRock();
					pile.AddRock(rock);
				}
			}
		}

		switch (startingPlayer)
		{
			case Player.Player:
				TransitionState(State.PlayerTurn);
				break;
			case Player.Opponent:
				TransitionState(State.OpponentTurn);
				break;
			default:
				break;
		}
	}
	public void GetBestMove(out int rocksToTake, out Pile fromPile)
	{
		switch (gameType_)
		{
			case GameType.BasicNim:
				fromPile = board.Piles[0];

				// Less than three stones left, win
				if (board.RocksLeft <= 3)
					rocksToTake = board.RocksLeft;
				// On a bad number, take a random number of rocks
				else if (board.RocksLeft % 4 == 0)
					rocksToTake = Mathf.FloorToInt(Random.value * 3) + 1;
				// Go down to the nearest multiple of 4
				else
					rocksToTake = board.RocksLeft % 4;

				break;
			default:
				throw new System.InvalidOperationException();
		}
	}
	public void GetWorstMove(out int rocksToTake, out Pile fromPile)
	{
		switch (gameType_)
		{
			case GameType.BasicNim:
				fromPile = board.Piles[0];

				// Only one stone left, must take
				if (board.RocksLeft == 1)
					rocksToTake = 1;
				// On a bad number, take a random number of rocks
				else if (board.RocksLeft % 4 == 1)
					rocksToTake = Mathf.FloorToInt(Random.value * 3) + 1;
				// Go down to one more than the nearest multiple of 4
				else
					rocksToTake = (board.RocksLeft + 3) % 4;

				break;
			default:
				throw new System.InvalidOperationException();
		}
	}
	public bool CanGrabRock(Rock rock)
	{
		// If you have chosen a pile, the rock isn't from that pile, and you're not taking from your own pile
		if (chosenPile_ != null && rock.Pile != chosenPile_ && rock.Pile != YourPile)
			return false;

		// If you're taking from your opponent's pile
		if (rock.Pile == TheirPile)
			return false;

		// If you're taking from your own pile
		if (rock.Pile == YourPile)
			return takenRocks_ > 0;

		// If you're taking from another pile
		return takenRocks_ < maxAvailableRocks_;
	}
	public bool CanMoveRock(Rock rock, Pile pile)
	{
		// If you're taking from your own pile
		if (rock.Pile == YourPile)
			return pile == chosenPile_;

		// If you're taking from a board pile
		return pile == YourPile;
	}
	public void MoveRock(Rock rock, Pile pile)
	{
		// If you're taking from your own pile
		if (rock.Pile == YourPile)
		{
			--takenRocks_;
			if (takenRocks_ == 0)
				chosenPile_ = null;
		}
		else
		{
			++takenRocks_;
			chosenPile_ = rock.Pile;
		}

		// Remove from old pile
		rock.Pile.RemoveRock(rock);
		// Add to new pile
		pile.AddRock(rock);
	}

	// Private interface
	private void ResetForNextTurn()
	{
		switch (gameType_)
		{
			case GameType.BasicNim:
				maxAvailableRocks_ = 3;
				break;
			case GameType.RowNim:
				maxAvailableRocks_ = 9999;
				break;
			case GameType.FibonacciNim:
				maxAvailableRocks_ = takenRocks_ * 2;
				break;
			default:
				throw new System.InvalidOperationException();
		}

		chosenPile_ = null;
		takenRocks_ = 0;
	}
	private void OpponentGrabNextRock()
	{
		if (opponentRocksToTake_ > 0)
		{
			Rock rock = opponentPileToTakeFrom_.GetRandomRockInPile();
			enemyHand.Reach(rock.transform, OpponentGrabRockAndRetract);
		}
		else
			AdvanceTurn();
	}
	private void OpponentGrabRockAndRetract()
	{
		Level.instance.OnRockGrabbed();

		enemyHand.Grab();
		enemyHand.setPilePos(board.opponentPile.transform.position);
		enemyHand.Retract(OpponentReleaseRockAndContinue);
	}
	private void OpponentReleaseRockAndContinue()
	{
		Level.instance.OnRockReleased();

		enemyHand.Release();
		--opponentRocksToTake_;
		OpponentGrabNextRock();
	}
	private void PlayerOnGrabRock()
	{
		Level.instance.OnRockGrabbed();

		playerHand.Retract(PlayerOnReleaseRock);
	}
	private void PlayerOnReleaseRock()
	{
		Level.instance.OnRockReleased();

		if (board.RocksLeft == 0)
			AdvanceTurn();
		else
			playerHand.Reach(PlayerOnGrabRock);
	}
}