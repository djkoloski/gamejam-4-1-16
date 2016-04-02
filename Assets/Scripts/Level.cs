using UnityEngine;
using System.Collections.Generic;

public class Level : MonoBehaviour
{
	// Public variables
	public DialogueBox dialogueBox;
	public List<DialogueLine> dialogue;

	// Private variables
	private int dialogueIndex_;

	// Initialization
	public void Awake()
	{
		dialogueIndex_ = 0;
	}

	// Update
	public void Update()
	{
		if (Input.GetKeyDown(KeyCode.A))
		{
			dialogueIndex_ = 0;
			SpeakNextLine();
		}
	}

	// Callbacks
	public void SpeakNextLine()
	{
		if (dialogueIndex_ >= dialogue.Count)
			dialogueBox.Hide();
		else
			dialogueBox.Speak(DialogueSpeaker.Grim, dialogue[dialogueIndex_++], SpeakNextLine);
	}
}