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
	public Rock GetRandomRockInPile()
	{
		return rocks_[Mathf.FloorToInt(Random.value * rocks_.Count)];
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
}