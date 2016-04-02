using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[System.Serializable]
public class Dialogue
{
	public List<DialogueLine> lines;
}

[System.Serializable]
public class DialogueLine
{
	public DialogueSpeaker speaker;
	public string text;
	public float letterPause;
}

public enum DialogueSpeaker
{
	Grim,
	Murderer,
	Mother,
	Teenager
}

public static class DialogueSpeakerExtension
{
	public static string ID(this DialogueSpeaker speaker)
	{
		switch (speaker)
		{
			case DialogueSpeaker.Grim:
				return "grim";
			case DialogueSpeaker.Murderer:
				return "murderer";
			case DialogueSpeaker.Mother:
				return "mother";
			case DialogueSpeaker.Teenager:
				return "teenager";
			default:
				throw new System.NotImplementedException();
		}
	}
}

public class DialogueBox : MonoBehaviour
{
	// Types
	public enum State
	{
		Inactive,
		Speaking,
		Waiting
	}
	public delegate void Callback();

	// Public variables
	public Text text;

	// Private variables
	private State state_;
	private Animator animator_;
	private DialogueLine line_;
	private int dialogueLength_;
	private Callback callback_;
	private float timer_;

	// Initialization
	public void Awake()
	{
		animator_ = GetComponent<Animator>();
		line_ = null;
		dialogueLength_ = 0;
		callback_ = null;

		TransitionState(State.Inactive);
	}

	// State transitions
	private void TransitionState(State newState)
	{
		state_ = newState;
		switch (state_)
		{
			case State.Inactive:
				text.text = "";
				animator_.Play("inactive", 0);
				break;
			case State.Speaking:
				SetDialogueLength(0);
				animator_.Play("speaking_bottom", 0);
				animator_.Play(line_.speaker.ID() + "_speak", 1);
				break;
			case State.Waiting:
				SetDialogueLength(line_.text.Length);
				animator_.Play("waiting_bottom", 0);
				animator_.Play(line_.speaker.ID() + "_idle", 1);
				break;
			default:
				throw new System.InvalidOperationException();
		}
	}

	// Public interface
	public void Speak(DialogueLine line, Callback callback)
	{
		line_ = line;
		callback_ = callback;
		TransitionState(State.Speaking);
	}
	public void Hide()
	{
		TransitionState(State.Inactive);
	}

	// Private interface
	private void SetDialogueLength(int length)
	{
		dialogueLength_ = Mathf.Min(line_.text.Length, length);
		text.text = line_.text.Substring(0, dialogueLength_).Replace("`", "");
	}

	// Update
	public void Update()
	{
		switch (state_)
		{
			case State.Inactive:
				break;
			case State.Speaking:
				timer_ -= Time.deltaTime;
				if (timer_ <= 0.0f)
				{
					SetDialogueLength(dialogueLength_ + 1);
					if (dialogueLength_ >= line_.text.Length)
						TransitionState(State.Waiting);
					else
						timer_ = line_.letterPause;
				}

				if (Input.GetKeyDown(KeyCode.Return))
					TransitionState(State.Waiting);
				break;
			case State.Waiting:
				if (Input.GetKeyDown(KeyCode.Return))
					callback_();
				break;
			default:
				throw new System.InvalidOperationException();
		}
	}
}