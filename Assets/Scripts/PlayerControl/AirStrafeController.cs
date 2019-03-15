using System;
using ExplosionJumping.Util;
using UnityEngine;

namespace ExplosionJumping.PlayerControl {
    public class AirStrafeController : MonoBehaviour {

        public float linearAcceleration = 10f;
        public float angularAcceleration = 15f;

        private Rigidbody rigidBody;
        private Camera cam;

        private void Awake() {
            rigidBody = GetComponent<Rigidbody>();
        }

        // Use this for initialization
        private void Start() {
            cam = Camera.main;
        }

        public void AirStafe(Vector2 input) {
            Vector3 velocityVec3 = rigidBody.velocity;
            velocityVec3.y = 0;
            Vector2 velocityVec2 = new Vector2(velocityVec3.x, velocityVec3.z);
            Vector3 inputVec3 = transform.TransformDirection(new Vector3(input.x, 0, input.y));
            Vector2 inputVec2 = new Vector2(inputVec3.x, inputVec3.z);
            Debug.DrawRay(transform.position, velocityVec3);
            inputVec3.Normalize();
            float angleBetweenVelocityInput = Vector3.Angle(velocityVec3, inputVec3);
            float linearForce = linearAcceleration * angleBetweenVelocityInput / 180;
            rigidBody.AddForce(inputVec3 * linearForce, ForceMode.Acceleration);

            Vector3 lookForward = cam.transform.forward;
            lookForward.y = 0;
            int inputDirection = Utils.VectorIsClockwise(velocityVec2, inputVec2);
            float angleBetweenVelocityAndLook = Vector3.Angle(velocityVec3, lookForward);
            Vector3 targetVector;
            if(angleBetweenVelocityAndLook <= 45) {
                targetVector = lookForward;
            } else if(angleBetweenVelocityAndLook >= 135) {
                targetVector = lookForward * -1;
            } else {
                targetVector = cam.transform.right;
                targetVector *= Utils.VectorIsClockwise(new Vector2(lookForward.x, lookForward.z), velocityVec2);
            }
            targetVector.y = 0;
            int lookDirection = Utils.VectorIsClockwise(velocityVec2, new Vector2(targetVector.x, targetVector.z));

            Debug.DrawRay(transform.position, targetVector * 10, Color.green);
            Debug.DrawRay(transform.position, inputVec3 * 10, Color.magenta);

            float angleAccel = angularAcceleration;

            if(inputDirection == lookDirection && inputDirection != 0) {
                angleAccel *= inputDirection;
            } else {
                angleAccel = 0;
            }

            if(Vector3.Angle(velocityVec3, targetVector) < Math.Abs(angleAccel)) {
                angleAccel = Vector3.Angle(velocityVec3, targetVector) * Math.Sign(angleAccel);
            }

            Quaternion velRotation = Quaternion.AngleAxis(angleAccel, Vector3.up);
            rigidBody.velocity = velRotation * rigidBody.velocity;
        }
    }
}