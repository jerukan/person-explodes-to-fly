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

        [Tooltip("A constant that fixes calculation inaccuracies at linear accelerations higher than the limit. This shouldn't need to exist but it does.")]
        public float highAccelerationInaccuracyModifer = 0.0001f;


        public override void AirStafe(Vector2 input) {
            // linear movement calculations
            // all angle calculations use bearing, so clockwise is positive angle.
            Vector3 velocityVec3World = rigidBody.velocity;
            velocityVec3World.y = 0;
            Vector3 velocityVec3Relative = transform.InverseTransformDirection(velocityVec3World);
            Vector3 inputVec3Relative = new Vector3(input.x, 0, input.y);
            Vector3 inputVec3World = transform.TransformDirection(new Vector3(input.x, 0, input.y));
            //Debug.DrawRay(transform.position, velocityVec3World);
            //Debug.DrawRay(transform.position, transform.forward * 10, Color.magenta);

            float angleBetweenVelocityAndLook = -Vector3.SignedAngle(velocityVec3Relative, Vector3.forward, Vector3.up);

            float velocityXComponentRelative = Mathf.Sin(angleBetweenVelocityAndLook * Mathf.Deg2Rad) * velocityVec3Relative.magnitude;
            float velocityYComponentRelative = Mathf.Cos(angleBetweenVelocityAndLook * Mathf.Deg2Rad) * velocityVec3Relative.magnitude;


            Vector3 accelVector = new Vector3(inputVec3Relative.normalized.x * linearAcceleration, 0, inputVec3Relative.normalized.z * linearAcceleration);
            Vector3 predictedVelocity = new Vector3(velocityVec3Relative.x + accelVector.x, 0,
                                                    velocityVec3Relative.z + accelVector.z);

            float predictedAngleBetweenVelocityAndLook = -Vector3.SignedAngle(predictedVelocity, Vector3.forward, Vector3.up);

            float predictedVelocityXComponentRelative = Mathf.Sin(predictedAngleBetweenVelocityAndLook * Mathf.Deg2Rad) * predictedVelocity.magnitude;
            float predictedVelocityYComponentRelative = Mathf.Cos(predictedAngleBetweenVelocityAndLook * Mathf.Deg2Rad) * predictedVelocity.magnitude;

            if(Math.Abs(predictedVelocityXComponentRelative) > linearAccelerationLimit) {
                if (Math.Abs(predictedVelocityXComponentRelative) >= Math.Abs(velocityXComponentRelative) - highAccelerationInaccuracyModifer && 
                    Math.Sign(predictedVelocityXComponentRelative) == Math.Sign(velocityXComponentRelative)) {
                    accelVector.x = 0;
                } else {
                    accelVector.x = linearAccelerationLimit * Math.Sign(predictedVelocityXComponentRelative) - velocityXComponentRelative;
                }
            }
            if(Math.Abs(predictedVelocityYComponentRelative) > linearAccelerationLimit) {
                if (Math.Abs(predictedVelocityYComponentRelative) >= Math.Abs(velocityYComponentRelative) - highAccelerationInaccuracyModifer && 
                    Math.Sign(predictedVelocityYComponentRelative) == Math.Sign(velocityYComponentRelative)) {
                    accelVector.z = 0;
                } else {
                    accelVector.z = linearAccelerationLimit * Math.Sign(predictedVelocityYComponentRelative) - velocityYComponentRelative;
                }
            }

            Vector3 worldForce = new Vector3(accelVector.x / (Time.fixedDeltaTime * Time.fixedDeltaTime), 0, accelVector.z / (Time.fixedDeltaTime * Time.fixedDeltaTime));
            rigidBody.AddForce(transform.TransformDirection(accelVector), ForceMode.VelocityChange);
        }
    }
}