using UnityEngine;
using System.Collections.Generic;

public class Rock : MonoBehaviour
{
	// Public variables
	public float radius;
	public float acceleration;
	public float maxAcceleration;
	public float maxVelocity;
	public float timeToReach;

	public Pile Pile
	{
		get { return pile_; }
		set { pile_ = value; }
	}

	// Private variables
	private Rigidbody2D rigidbody_;
	private Pile pile_;
	private Vector2 targetPosition_;

	// Initialization
	public void Awake()
	{
		rigidbody_ = GetComponent<Rigidbody2D>();
	}

	// Public interface
	public void SetTarget(Vector2 position)
	{
		targetPosition_ = position;
	}
	public void MoveToPile()
	{
		SetTarget(pile_.GetRandomPointInPile());
	}
	public bool IsWithinDistanceOfPoint(Vector2 point, float distance)
	{
		return (point - new Vector2(transform.position.x, transform.position.y)).magnitude < radius + distance;
	}

	// Update
	public void Update()
	{
		MoveUtil.AccelerateClampedToward2D(rigidbody_, targetPosition_, acceleration, maxAcceleration, maxVelocity, timeToReach);
	}
}