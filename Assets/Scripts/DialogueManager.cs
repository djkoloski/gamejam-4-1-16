using UnityEngine;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
	// Types
	public delegate void Callback();

	// Public variables
	public DialogueBox dialogueBox;

	// Private variables
	private Dialogue currentDialogue_;
	private int currentLineIndex_;
	private Callback callback_;

	private DialogueLine CurrentLine
	{
		get { return currentDialogue_.lines[currentLineIndex_]; }
	}

	// Public interface
	public void BeginDialogue(Dialogue dialogue, Callback callback)
	{
		currentDialogue_ = dialogue;
		currentLineIndex_ = 0;
		callback_ = callback;

		SpeakNextLine();
	}

	// Private interface
	private void SpeakNextLine()
	{
		if (currentLineIndex_ >= currentDialogue_.lines.Count)
		{
			dialogueBox.Hide();
			callback_();
		}
		else
		{
			dialogueBox.Speak(CurrentLine, SpeakNextLine);
			currentLineIndex_++;
		}
	}
}