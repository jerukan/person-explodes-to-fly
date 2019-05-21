using ExplosionJumping.PlayerControl.Movement.AirControl;
using UnityEngine;

[AddComponentMenu("Player Control/Air Control/Simple Linear Acceleration")]
public class SimpleLinearAcceleration : PlayerAirController {

    public float acceleration = 0.2f;

    public override void AirStafe(Vector2 input) {
        rigidBody.AddForce(transform.TransformDirection(input).normalized * acceleration, ForceMode.Acceleration);
    }
}
