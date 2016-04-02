using UnityEngine;
using System.Collections.Generic;

public class RockPicker : MonoBehaviour
{
	// Public variables
	public float grabRadius;

	// Private variables
	private Rock heldRock_;

	// Public interface
	public bool GrabRock()
	{
		Rock rock = Game.instance.board.GetRockNearPoint(transform.position, grabRadius);

		if (rock == null)
			return false;

		if (!Game.instance.CanGrabRock(rock))
			return false;

		heldRock_ = rock;
		return true;
	}
	public bool ReleaseRock()
	{
		if (heldRock_ == null)
			return false;

		Pile pile = Game.instance.board.GetPileUnderPoint(transform.position);

		if (!Game.instance.CanMoveRock(heldRock_, pile))
		{
			heldRock_.MoveToPile();
			heldRock_ = null;
			return false;
		}

		Game.instance.MoveRock(heldRock_, pile);
		heldRock_ = null;
		return true;
	}

	// Update
	public void Update()
	{
		if (heldRock_ != null)
			heldRock_.SetTarget(transform.position);

		// TODO: remove
		transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		if (Input.GetMouseButtonDown(0))
		{
			Debug.Log("Trying to grab a rock");
			GrabRock();
		}
		if (Input.GetMouseButtonUp(0))
		{
			Debug.Log("Trying to release a rock");
			ReleaseRock();
		}
	}
}