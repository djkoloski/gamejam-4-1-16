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
	public Board board;

	// Private variables
	private State state_;
	private GameType gameType_;
	private Pile chosenPile_;
	private int takenRocks_;
	private int maxAvailableRocks_;

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

				// TODO: activate player and wait for them
				break;
			case State.OpponentTurn:
				ResetForNextTurn();

				// TODO: activate opponent, make decision for them, and do it
				break;
			default:
				break;
		}
	}

	// Public interface
	public void AdvanceTurn()
	{
		switch (state_)
		{
			case State.PlayerTurn:
				TransitionState(State.OpponentTurn);
				break;
			case State.OpponentTurn:
				TransitionState(State.PlayerTurn);
				break;
			default:
				break;
		}
	}
	public void BeginBasicNim(int numRocks, Player startingPlayer)
	{
		board.Reset();
		gameType_ = GameType.BasicNim;

		Pile pile = board.AddPile(Vector2.zero);
		for (int i = 0; i < numRocks; ++i)
		{
			Rock rock = board.AddRock();
			pile.AddRock(rock);
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
}