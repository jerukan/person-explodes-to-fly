using System;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using UnityStandardAssets.CrossPlatformInput;

namespace ExplosionJumping {
    /// <summary>
    /// Ripped straight from RigidbodyFirstPersonController from the standard assets with some modifications.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class RigidbodyFPControllerCustom : MonoBehaviour {
        [Serializable]
        public class MovementSettings {
            public float forwardSpeed = 6.0f;   // Speed when walking forward
            public float backwardSpeed = 4.0f;  // Speed when walking backwards
            public float strafeSpeed = 6.0f;    // Speed when walking sideways
            public float jumpForce = 5f;
            public float crouchMultiplier = 0.5f;
            internal bool crouching;
            public KeyCode crouchKey = KeyCode.LeftControl;
            public AnimationCurve slopeCurveModifier = new AnimationCurve(new Keyframe(-90.0f, 1.0f), new Keyframe(0.0f, 1.0f), new Keyframe(90.0f, 0.0f));
            [HideInInspector] public float currentTargetSpeed = 8f;

            public void UpdateDesiredTargetSpeed(Vector2 input) {
                if (input == Vector2.zero) {
                    return;
                }
                if (input.x > 0 || input.x < 0) {
                    //strafe
                    currentTargetSpeed = strafeSpeed;
                }
                if (input.y < 0) {
                    //backwards
                    currentTargetSpeed = backwardSpeed;
                }
                if (input.y > 0) {
                    //forwards
                    //handled last as if strafing and moving forward at the same time forwards speed should take precedence
                    currentTargetSpeed = forwardSpeed;
                }
                if(Input.GetKey(crouchKey)) {
                    currentTargetSpeed *= crouchMultiplier;
                    crouching = true;
                } else {
                    crouching = false;
                }
            }
        }

        [Serializable]
        public class AdvancedSettings {
            public float groundCheckDistance = 0.01f; // distance for checking if the controller is grounded ( 0.01f seems to work best for this )
            public float stickToGroundHelperDistance = 0.5f; // stops the character
            [Tooltip("set it to 0.1 or more if you get stuck in wall")]
            public float shellOffset; //reduce the radius by that ratio to avoid getting stuck in wall (a value of 0.1f is nice)
        }

        public Camera cam;
        public MovementSettings movementSettings = new MovementSettings();
        public MouseLook mouseLook = new MouseLook();
        public AdvancedSettings advancedSettings = new AdvancedSettings();

        public float height = 1.6f;
        public float radius = 0.5f;

        private Rigidbody rigidBody;
        private CapsuleCollider capsuleCollider;
        private float yRotation;
        private Vector3 groundContactNormal;
        private bool jump, previouslyGrounded, jumping, grounded;
        private Dictionary<Collider, Vector3> normalCollisions = new Dictionary<Collider, Vector3>();

        public Vector3 Velocity {
            get { return rigidBody.velocity; }
        }

        public bool Grounded {
            get { return grounded; }
        }

        public bool Jumping {
            get { return jumping; }
        }

        private void Start() {
            rigidBody = GetComponent<Rigidbody>();
            capsuleCollider = GetComponent<CapsuleCollider>();
            cam = Camera.main;
            mouseLook.Init(transform, cam.transform);
            capsuleCollider.radius = radius;
            capsuleCollider.height = height;
        }

        private void Update() {
            RotateView();

            if (CrossPlatformInputManager.GetButtonDown("Jump") && !jump) {
                jump = true;
            }
        }

        private void FixedUpdate() {
            GroundCheck();
            Vector2 input = GetInput();

            if (Mathf.Abs(input.x) > float.Epsilon || Mathf.Abs(input.y) > float.Epsilon) {
                if (grounded) {
                    // always move along the camera forward as it is the direction that it being aimed at
                    Vector3 desiredMove = cam.transform.forward * input.y + cam.transform.right * input.x;
                    desiredMove = Vector3.ProjectOnPlane(desiredMove, groundContactNormal).normalized;

                    desiredMove.x = desiredMove.x * movementSettings.currentTargetSpeed;
                    desiredMove.z = desiredMove.z * movementSettings.currentTargetSpeed;
                    desiredMove.y = desiredMove.y * movementSettings.currentTargetSpeed;
                    if (rigidBody.velocity.sqrMagnitude <
                        (movementSettings.currentTargetSpeed * movementSettings.currentTargetSpeed)) {
                        rigidBody.AddForce(desiredMove * SlopeMultiplier(), ForceMode.VelocityChange);
                    }
                    else {
                        rigidBody.velocity = desiredMove;
                    }
                } else {
                    
                }
            }

            if (grounded) {
                rigidBody.drag = 5f;

                if (jump) {
                    rigidBody.drag = 0f;
                    rigidBody.velocity = new Vector3(rigidBody.velocity.x, 0f, rigidBody.velocity.z);
                    rigidBody.AddForce(new Vector3(0f, movementSettings.jumpForce, 0f), ForceMode.VelocityChange);
                    jumping = true;
                }
            } else {
                rigidBody.drag = 0f;
                if (previouslyGrounded && !jumping) {
                    StickToGroundHelper();
                }
            }
            jump = false;
        }

        private float SlopeMultiplier() {
            float angle = Vector3.Angle(groundContactNormal, Vector3.up);
            return movementSettings.slopeCurveModifier.Evaluate(angle);
        }

        private void StickToGroundHelper() {
            RaycastHit hitInfo;
            if (Physics.SphereCast(transform.position, capsuleCollider.radius * (1.0f - advancedSettings.shellOffset), Vector3.down, out hitInfo,
                                   ((capsuleCollider.height / 2f) - capsuleCollider.radius) +
                                   advancedSettings.stickToGroundHelperDistance, Physics.AllLayers, QueryTriggerInteraction.Ignore)) {
                if (Mathf.Abs(Vector3.Angle(hitInfo.normal, Vector3.up)) < 85f) {
                    rigidBody.velocity = Vector3.ProjectOnPlane(rigidBody.velocity, hitInfo.normal);
                }
            }
        }

        private Vector2 GetInput() {

            // raw axis makes keyboard actually work properly
            Vector2 input = new Vector2 {
                x = CrossPlatformInputManager.GetAxisRaw("Horizontal"),
                y = CrossPlatformInputManager.GetAxisRaw("Vertical")
            };
            movementSettings.UpdateDesiredTargetSpeed(input);
            return input;
        }

        private void RotateView() {
            // avoids the mouse looking if the game is effectively paused
            if (Mathf.Abs(Time.timeScale) < float.Epsilon) return;

            // get the rotation before it's changed
            float oldYRotation = transform.eulerAngles.y;

            mouseLook.LookRotation(transform, cam.transform);

            if (grounded) {
                // Rotate the rigidbody velocity to match the new direction that the character is looking
                Quaternion velRotation = Quaternion.AngleAxis(transform.eulerAngles.y - oldYRotation, Vector3.up);
                rigidBody.velocity = velRotation * rigidBody.velocity;
            }
        }

        /// sphere cast down just beyond the bottom of the capsule to see if the capsule is colliding round the bottom
        private void GroundCheck() {
            previouslyGrounded = grounded;
            RaycastHit hitInfo;
            if (Physics.SphereCast(transform.position, capsuleCollider.radius * (1.0f - advancedSettings.shellOffset), Vector3.down, out hitInfo,
                                   ((capsuleCollider.height / 2f) - capsuleCollider.radius) + advancedSettings.groundCheckDistance, Physics.AllLayers, QueryTriggerInteraction.Ignore)) {
                grounded = true;
                groundContactNormal = hitInfo.normal;
            }
            else {
                grounded = false;
                groundContactNormal = Vector3.up;
            }
            if (!previouslyGrounded && grounded && jumping) {
                jumping = false;
            }
        }

        private void OnCollisionEnter(Collision collision) {
            foreach(ContactPoint p in collision.contacts) {
                Debug.Log(p);
            }
        }

        private void OnCollisionStay(Collision collision) {
            
        }

        private void OnCollisionExit(Collision collision) {
            normalCollisions.Remove(collision.collider);
        }
    }
}
