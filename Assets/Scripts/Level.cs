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
	public int stoneCount;
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
	public List<DialogueHook> dialogueHooks;

	public bool Paused
	{
		get { return paused_; }
	}

	// Private variables
	private bool paused_;
	private List<bool> dialogueHooksTriggered_;
	private DialogueManager dialogueManager_;
	private List<Rock> rocks_;

	// Initialization
	public void Awake()
	{
		instance = this;

		foreach (DialogueHook dialogueHook in dialogueHooks)
			dialogueHook.ResetTrigger();

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
	public void OnStoneGrabbed()
	{
		RunHookedDialogue(Game.instance.board.StonesLeft, TurnStage.Grab);
	}
	public void OnStoneReleased()
	{
		RunHookedDialogue(Game.instance.board.StonesLeft, TurnStage.Release);
	}
	public void OnDialogueHookFinished()
	{
		Unpause();
	}

	// Private interface
	private void RunHookedDialogue(int stoneCount, TurnStage turnStage)
	{
		DialogueHook dialogueHook = GetDialogueHook(stoneCount, turnStage);
		Debug.Log("Hook: " + dialogueHook);
		if (dialogueHook != null && !dialogueHook.Triggered)
		{
			Debug.Log("Triggering dialogue");
			dialogueHook.Trigger();
			Pause();
			dialogueManager_.BeginDialogue(dialogueHook.dialogue, OnDialogueHookFinished);
		}
	}
	private DialogueHook GetDialogueHook(int stoneCount, TurnStage turnStage)
	{
		foreach (DialogueHook dialogueHook in dialogueHooks)
		{
			if (dialogueHook.stoneCount == stoneCount && dialogueHook.turnStage == turnStage)
				return dialogueHook;
		}
		return null;
	}

	// Update
	public void Update()
	{
		if (Input.GetKeyDown(KeyCode.Q))
			Game.instance.BeginBasicNim(15, Player.Player);
		if (Input.GetKeyDown(KeyCode.Tab) && Game.instance.CanAdvanceTurn())
			Game.instance.AdvanceTurn();
	}
}