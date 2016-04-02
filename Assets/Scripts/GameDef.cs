using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class GameDef
{
	public GameType gameType;
	public int[] rockCounts;
	public Player startingPlayer;
	public float mistakeChance;
	public List<DialogueHook> dialogueHooks;
	public Dialogue introDialogue;
}