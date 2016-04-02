using UnityEngine;
using System.Collections;

public class PlayerHand : MonoBehaviour
{
	public RockPicker rockPicker;

	private Vector2 goalPos;	 // The target pos of the hand
	public Vector2 pilePos;	 // The pos of the pile where we place rocks

	private Vector2 startPos;   // The off screen position we start at

	public float accel = 0;
	public float maxAccel = 0;
	public float maxVel = 0;

	public float tolerance = 0;

	public float timetoreach = 0;

	public delegate void Callback();

	public enum State
	{
		Inactive,
		Reaching,
		Retracting,
	}

	// Private variables
	private Rigidbody2D rigidbody_;
	private Animator animator_;
	private State state_;
	private Callback callback_;

	private void TransitionState(State newState)
	{
		state_ = newState;
		switch (state_)
		{
			case State.Inactive:
				animator_.Play("open");
				break;
			case State.Reaching:
				animator_.Play("open");
				break;
			case State.Retracting:
				animator_.Play("closed");
				break;
		}
	}

	// Public call to this function to begin reaching toward a rock
	public void Reach(Callback callback)
	{
		Vector3 m2w = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		goalPos = new Vector2(m2w.x, m2w.y);

		TransitionState(State.Reaching);
		callback_ = callback;
	}

	// Retracts the hand back toward the pile of rocks
	public void Retract(Callback callback)
	{
		
		TransitionState(State.Retracting);
		callback_ = callback;
	}

	// function for changing the position to which the hand retracts to
	public void setPilePos(Vector2 pp)
	{
		pilePos = pp;
	}

	public void Inactive()
	{
		TransitionState(State.Inactive);
		callback_ = null;
	}

	// Initialization
	public void Awake()
	{
		rigidbody_ = GetComponent<Rigidbody2D>();
		animator_ = GetComponent<Animator>();
		startPos = transform.position;
		callback_ = null;
	}

	// Update
	public void Update()
	{
		if (Level.instance.Paused)
		{
			MoveUtil.AccelerateClamped2D(rigidbody_, Vector2.zero, accel, maxAccel);
			return;
		}

		Vector3 m2w = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		switch (state_)
		{
			case State.Inactive:
				MoveUtil.AccelerateClampedToward2D(rigidbody_, startPos, accel, maxAccel, maxVel, timetoreach);
				MoveUtil.ClampVelocity2D(rigidbody_, maxVel);
				break;
			case State.Reaching:
				goalPos = new Vector2(m2w.x, m2w.y);

				MoveUtil.AccelerateClampedToward2D(rigidbody_, goalPos, accel, maxAccel, maxVel, timetoreach);
				MoveUtil.ClampVelocity2D(rigidbody_, maxVel);
				
				if (Vector2.Distance(transform.position, goalPos) <= tolerance && Input.GetMouseButtonDown(0) && callback_ != null)
				{
					if (rockPicker.GrabRock())
						callback_();
				}
				break;
			case State.Retracting:
				goalPos = new Vector2(m2w.x, m2w.y);

				MoveUtil.AccelerateClampedToward2D(rigidbody_, goalPos, accel, maxAccel, maxVel, timetoreach);
				MoveUtil.ClampVelocity2D(rigidbody_, maxVel);

				if (Input.GetMouseButtonUp(0) && callback_ != null)
				{
					callback_();
					bool movedRock = rockPicker.ReleaseRock();
				}
				
				break;
		}
	}
}
