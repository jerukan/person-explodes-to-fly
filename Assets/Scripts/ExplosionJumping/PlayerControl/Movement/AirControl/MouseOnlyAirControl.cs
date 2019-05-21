using UnityEngine;

namespace ExplosionJumping.PlayerControl.Movement.AirControl {
    /// <summary>
    /// Player velocity is set to face whatever direction is being faced.
    /// </summary>
    [AddComponentMenu("Player Control/Air Control/Mouse Only Air Control")]
    public class MouseOnlyAirControl : PlayerAirController {

        public override void AirStafe(Vector2 input) {
            Vector3 resultVector = cam.transform.forward;
            resultVector.y = 0;
            Vector2 horizontalSpeed = new Vector2(rigidBody.velocity.x, rigidBody.velocity.z);
            resultVector = resultVector.normalized * horizontalSpeed.magnitude;
            rigidBody.velocity = new Vector3(resultVector.x, rigidBody.velocity.y, resultVector.z);
        }
    }
}