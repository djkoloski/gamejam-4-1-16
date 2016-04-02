using UnityEngine;
using System.Collections.Generic;

public enum TurnStage
{
	Grab,
	Release
}

[System.Serializable]
public class DialogueHook
{
	public TurnStage turnStage;
	public int rockCount;
	public Dialogue dialogue;

	public bool Triggered
	{
		get { return triggered_; }
	}

	private bool triggered_;

	public void ResetTrigger()
	{
		triggered_ = false;
	}
	public void Trigger()
	{
		triggered_ = true;
	}
}

public class Level : MonoBehaviour
{
	// Types
	public enum State
	{
		LevelIntro,
		PlayerTurn,
		OpponentTurn,
		LevelOutro
	}

	// Static variables
	public static Level instance;

	// Public variables
	public int currentGameDefinitionIndex;
	public List<GameDef> gameDefinitions;

	public bool Paused
	{
		get { return paused_; }
	}
	public GameDef CurrentGameDefinition
	{
		get { return gameDefinitions[currentGameDefinitionIndex]; }
	}

	// Private variables
	private bool paused_;
	private DialogueManager dialogueManager_;
	private List<Rock> rocks_;

	// Initialization
	public void Awake()
	{
		instance = this;

		paused_ = false;
		dialogueManager_ = GetComponent<DialogueManager>();
		rocks_ = new List<Rock>();
	}

	// Public interface
	public void Pause()
	{
		paused_ = true;
	}
	public void Unpause()
	{
		paused_ = false;
	}
	public void OnRockGrabbed()
	{
		RunHookedDialogue(Game.instance.board.RocksLeft, TurnStage.Grab);
	}
	public void OnRockReleased()
	{
		RunHookedDialogue(Game.instance.board.RocksLeft, TurnStage.Release);
	}
	public void OnDialogueHookFinished()
	{
		Unpause();
	}
	public void OnGameFinished(Player winner)
	{
		switch (winner)
		{
			case Player.Player:
				AdvanceToNextGame();
				break;
			case Player.Opponent:
				AdvanceToNextGame();
				break;
			default:
				throw new System.InvalidOperationException();
		}
	}
	public void OnAllGamesFinished()
	{
		Debug.Log("All games finished!");
	}

	// Private interface
	private void RunHookedDialogue(int rockCount, TurnStage turnStage)
	{
		DialogueHook dialogueHook = GetDialogueHook(rockCount, turnStage);
		if (dialogueHook != null && !dialogueHook.Triggered)
		{
			dialogueHook.Trigger();
			Pause();
			dialogueManager_.BeginDialogue(dialogueHook.dialogue, OnDialogueHookFinished);
		}
	}
	private DialogueHook GetDialogueHook(int rockCount, TurnStage turnStage)
	{

		foreach (DialogueHook dialogueHook in CurrentGameDefinition.dialogueHooks)
		{
			if (dialogueHook.rockCount == rockCount && dialogueHook.turnStage == turnStage)
				return dialogueHook;
		}
		return null;
	}
	private void AdvanceToNextGame()
	{
		++currentGameDefinitionIndex;
		if (currentGameDefinitionIndex >= gameDefinitions.Count)
			OnAllGamesFinished();
		else
			StartGame();
	}
	private void StartGame()
	{
		dialogueManager_.BeginDialogue(CurrentGameDefinition.introDialogue, OnIntroDialogueFinished);
	}
	private void OnIntroDialogueFinished()
	{
		Game.instance.BeginNim(CurrentGameDefinition.gameType, CurrentGameDefinition.rockCounts, CurrentGameDefinition.startingPlayer, CurrentGameDefinition.mistakeChance);
	}

	// Update
	public void Update()
	{
		if (Input.GetKeyDown(KeyCode.Q))
		{
			StartGame();
		}

		if (Input.GetKeyDown(KeyCode.Tab) && Game.instance.CanAdvanceTurn())
			Game.instance.AdvanceTurn();
	}
}