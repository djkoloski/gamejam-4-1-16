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

	// Public variables
	public List<DialogueHook> dialogueHooks;
	public Board board;

	public bool Paused
	{
		get { return paused_; }
	}
	public int StoneCount
	{
		get { return 0; }
	}

	// Private variables
	private bool paused_;
	private int currentDialogueHookIndex_;
	private DialogueManager dialogueManager_;

	private DialogueHook CurrentDialogueHook
	{
		get { return dialogueHooks[currentDialogueHookIndex_]; }
	}

	// Initialization
	public void Awake()
	{
		paused_ = false;
		currentDialogueHookIndex_ = 0;
		dialogueManager_ = GetComponent<DialogueManager>();
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
	public void OnGrab()
	{
		RunHookedDialogue(StoneCount, TurnStage.Grab);
	}
	public void OnRelease()
	{
		RunHookedDialogue(StoneCount, TurnStage.Release);
	}
	public void OnDialogueHookFinished()
	{
		Unpause();
	}

	// Private interface
	private void RunHookedDialogue(int stoneCount, TurnStage turnStage)
	{
		DialogueHook dialogueHook = GetDialogueHook(stoneCount, turnStage);
		if (dialogueHook != null)
		{
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
		if (Input.GetKeyDown(KeyCode.A))
			OnGrab();
		if (Input.GetKeyDown(KeyCode.S))
			OnRelease();
	}
}