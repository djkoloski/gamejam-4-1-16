using UnityEngine;
using System.Collections.Generic;

public class Pile : MonoBehaviour
{
	// Public variables
	public float radius;

	public int NumRocks
	{
		get { return rocks_.Count; }
	}

	// Private variables
	private List<Rock> rocks_;

	// Initialization
	public void Awake()
	{
		rocks_ = new List<Rock>();
	}

	// Public interface
	public void Clear()
	{
		rocks_ = new List<Rock>();
	}
	public void AddRock(Rock rock)
	{
		rocks_.Add(rock);
		rock.Pile = this;
		rock.MoveToPile();
	}
	public void RemoveRock(Rock rock)
	{
		rocks_.Remove(rock);
	}
	public Vector2 GetRandomPointInPile()
	{
		float r = Random.value;
		float theta = Random.value * 2.0f * Mathf.PI;

		return new Vector2(
			transform.position.x + radius * Mathf.Sqrt(r) * Mathf.Cos(theta),
			transform.position.y + radius * Mathf.Sqrt(r) * Mathf.Sin(theta)
		);
	}

	/*
	// Update
	void Update()
	{
		// TODO: move this check into rock
		Vector3 mouse2world = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		Vector2 mouseCheck = new Vector2(mouse2world.x, mouse2world.y);

		// On click, the player cursor must be over the Pile and there must be rocks here
		if (Input.GetMouseButtonDown(0) && this.GetComponent<BoxCollider2D>().OverlapPoint(mouseCheck) && numRocks > 0)
		{
			rockGrabbed = true;
		}

		// Check for leftMouse is released
		if (Input.GetMouseButtonUp(0) && rockGrabbed)
		{
			// If this was released over a player pile
			if (pileCheck(mouseCheck))
			{
				this.removeRocks();
				this.GetComponentInParent<Board>().PlayerPile.GetComponentInParent<PlayerPile>().addRocks();
				this.GetComponentInParent<Board>().setPileLock(this.gameObject);
			}

			rockGrabbed = false;
		}
	}
	*/
}