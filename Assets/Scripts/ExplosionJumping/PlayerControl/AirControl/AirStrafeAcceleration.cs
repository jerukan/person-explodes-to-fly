using UnityEngine;
using System;
using ExplosionJumping.Util;

namespace ExplosionJumping.PlayerControl.AirControl {
    /// <summary>
    /// This version of air control is taken straight from how the Source Engine handles air movement.
    /// </summary>
    [AddComponentMenu("Player Control/Air Control/Air Strafe Acceleration")]
    public class AirStrafeAcceleration: PlayerAirController {

        [Tooltip("The max speed allowed for the player to accelerate up to in each cardinal direction relative to the direction the player is looking. " +
                 "By rotating player view and accelerating in the one correct direction, air speed can increased quickly.")]
        public float linearAccelerationSpeedLimit = 1f;

        [Tooltip("The rate of acceleration for the player in the air, in m/s/s.")]
        public float linearAcceleration = 50f;

        public override void AirStafe(Vector2 input) {
            // linear movement calculations
            Vector3 velocityRelative = transform.InverseTransformDirection(rigidBody.velocity);
            velocityRelative.y = 0;

            Vector3 accelVector = new Vector3(input.normalized.x * linearAcceleration, 0, input.normalized.y * linearAcceleration) * Time.deltaTime;
            Vector3 predictedVelocity = velocityRelative + accelVector;

            // do not accelerate in the specified direction under the following conditions:
            // - the predicted velocity x goes over the acceleration limit
            // - the predicted velocity x goes over the current velocity x
            // - the current velocity x is already over the acceleration limit
            // - both the predicted velocity x and current velocity x are the same sign (needed since absolute values are used)
            // same conditions on the y axis
            if(Math.Abs(predictedVelocity.x) > linearAccelerationSpeedLimit) {
                if (Math.Abs(predictedVelocity.x) >= Math.Abs(velocityRelative.x) && 
                    Math.Abs(velocityRelative.x) > linearAccelerationSpeedLimit && 
                    Math.Sign(predictedVelocity.x) == Math.Sign(velocityRelative.x)) {
                    accelVector.x = 0;
                } else {
                    accelVector.x = linearAccelerationSpeedLimit * Math.Sign(predictedVelocity.x) - velocityRelative.x;
                }
            }
            if(Math.Abs(predictedVelocity.z) > linearAccelerationSpeedLimit) {
                if (Math.Abs(predictedVelocity.z) >= Math.Abs(velocityRelative.z) && 
                    Math.Abs(velocityRelative.z) > linearAccelerationSpeedLimit && 
                    Math.Sign(predictedVelocity.z) == Math.Sign(velocityRelative.z)) {
                    accelVector.z = 0;
                } else {
                    accelVector.z = linearAccelerationSpeedLimit * Math.Sign(predictedVelocity.z) - velocityRelative.z;
                }
            }
            rigidBody.AddForce(transform.TransformDirection(accelVector), ForceMode.VelocityChange);
        }
    }
}