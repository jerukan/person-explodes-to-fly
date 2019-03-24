using UnityEngine;
using System.Collections;
using ExplosionJumping.Util;
using System;

namespace ExplosionJumping.PlayerControl.AirControl {
    /// <summary>
    /// This version of air control is taken straight from how the Source Engine handles air movement.
    /// </summary>
    public class AirStrafeAcceleration: PlayerAirController {

        [Tooltip("The max speed allowed for the player to accelerate up to in each cardinal direction relative to the direction the player is looking. " +
                 "By rotating player view and accelerating in the one correct direction, air speed can increased quickly.")]
        public float linearAccelerationLimit = 1f;

        [Tooltip("The rate of acceleration for the player in the air. Setting it higher than the linear acceleration limit will cause strange behaviour. " +
                 "It is suggested to set this value at half of the linear acceleration limit.")]
        public float linearAcceleration = 0.5f;

        public override void AirStafe(Vector2 input) {
            // linear movement calculations
            // all angle calculations use bearing, so clockwise is positive angle.
            Vector3 velocityWorld = rigidBody.velocity;
            velocityWorld.y = 0;
            Vector3 velocityRelative = transform.InverseTransformDirection(velocityWorld);
            Vector3 inputRelative = new Vector3(input.x, 0, input.y);
            //Debug.DrawRay(transform.position, velocityVec3World);
            //Debug.DrawRay(transform.position, transform.forward * 10, Color.magenta);

            Vector3 accelVector = new Vector3(inputRelative.normalized.x * linearAcceleration, 0, inputRelative.normalized.z * linearAcceleration);
            Vector3 predictedVelocity = velocityRelative + accelVector;

            if(Math.Abs(predictedVelocity.x) > linearAccelerationLimit) {
                if (Math.Abs(predictedVelocity.x) >= Math.Abs(velocityRelative.x) && 
                    Math.Sign(predictedVelocity.x) == Math.Sign(velocityRelative.x)) {
                    accelVector.x = 0;
                } else {
                    accelVector.x = linearAccelerationLimit * Math.Sign(predictedVelocity.x) - velocityRelative.x;
                }
            }
            if(Math.Abs(predictedVelocity.z) > linearAccelerationLimit) {
                if (Math.Abs(predictedVelocity.z) >= Math.Abs(velocityRelative.z) && 
                    Math.Sign(predictedVelocity.z) == Math.Sign(velocityRelative.z)) {
                    accelVector.z = 0;
                } else {
                    accelVector.z = linearAccelerationLimit * Math.Sign(predictedVelocity.z) - velocityRelative.z;
                }
            }

            rigidBody.AddForce(transform.TransformDirection(accelVector), ForceMode.VelocityChange);
        }
    }
}