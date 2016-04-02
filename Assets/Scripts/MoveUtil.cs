using UnityEngine;
using System.Collections.Generic;

namespace ergo
{
	public static class MoveUtil
	{
		public static void AccelerateClamped2D(Rigidbody2D rb, Vector2 targetVelocity, float acceleration, float maxAcceleration)
		{
			Vector2 dv = targetVelocity - new Vector2(rb.velocity.x, rb.velocity.y);
			dv = dv.normalized * Mathf.Min(dv.magnitude * acceleration, maxAcceleration);
			rb.AddForce(new Vector2(dv.x, dv.y) * rb.mass, ForceMode2D.Force);
		}

		public static void AccelerateClampedToward2D(Rigidbody2D rb, Vector2 target, float acceleration, float maxAcceleration, float maxVelocity, float timeToReach)
		{
			Vector2 targetVelocity = target - (Vector2)rb.transform.position;
			targetVelocity = targetVelocity.normalized * Mathf.Min(targetVelocity.magnitude / timeToReach, maxVelocity);
			AccelerateClamped2D(rb, targetVelocity, acceleration, maxAcceleration);
		}

		public static void ClampVelocity2D(Rigidbody2D rb, float maxVelocity)
		{
			Vector2 velocity = new Vector2(rb.velocity.x, rb.velocity.y);
			velocity = velocity.normalized * Mathf.Min(velocity.magnitude, maxVelocity);
			velocity.y = rb.velocity.y;
			rb.velocity = velocity;
		}

		public static void ClampVelocity(Rigidbody rb, float maxVelocity)
		{
			rb.velocity = rb.velocity.normalized * Mathf.Min(rb.velocity.magnitude, maxVelocity);
		}
	}
}
