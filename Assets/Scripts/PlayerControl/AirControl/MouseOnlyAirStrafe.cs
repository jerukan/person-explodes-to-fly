using UnityEngine;
using System.Collections;

namespace ExplosionJumping.PlayerControl.AirControl {
    public class MouseOnlyAirStrafe : AirStrafeController {

        public override void AirStafe(Vector2 input) {
            Vector3 resultVector = cam.transform.forward;
            resultVector.y = 0;
            Vector2 horizontalSpeed = new Vector2(rigidBody.velocity.x, rigidBody.velocity.z);
            resultVector = resultVector.normalized * horizontalSpeed.magnitude;
            rigidBody.velocity = new Vector3(resultVector.x, rigidBody.velocity.y, resultVector.z);
        }
    }
}