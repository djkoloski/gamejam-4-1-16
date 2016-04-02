using UnityEngine;
using System.Collections.Generic;

public class Cutscene : MonoBehaviour
{
	public string nextSceneName;
	public Dialogue dialogue;

	private DialogueManager dialogueManager_;

	public void Awake()
	{
		dialogueManager_ = GetComponent<DialogueManager>();
	}
	public void Start()
	{
		dialogueManager_.BeginDialogue(dialogue, GoToNextScene);
	}
	public void GoToNextScene()
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
	}
}