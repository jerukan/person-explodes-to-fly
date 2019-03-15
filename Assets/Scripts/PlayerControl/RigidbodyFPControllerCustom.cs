using System;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using UnityStandardAssets.CrossPlatformInput;

namespace ExplosionJumping.PlayerControl {
    /// <summary>
    /// Ripped straight from RigidbodyFirstPersonController from the standard assets with some modifications.
    /// Inspired by Source engine physics.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(AirStrafeController))]
    public class RigidbodyFPControllerCustom : MonoBehaviour {
        [Serializable]
        public class MovementSettings {
            public float forwardSpeed = 6.0f;   // Speed when walking forward
            public float backwardSpeed = 4.0f;  // Speed when walking backwards
            public float strafeSpeed = 6.0f;    // Speed when walking sideways
            public float jumpForce = 5f;
            public float groundAccelerationRate = 0.2f;
            public float airAcceleration = 1f;
            public float crouchMultiplier = 0.5f;
            internal bool crouching;
            public KeyCode crouchKey = KeyCode.LeftControl;
            public AnimationCurve slopeCurveModifier = new AnimationCurve(new Keyframe(-90.0f, 1.0f), new Keyframe(0.0f, 1.0f), new Keyframe(90.0f, 0.0f));
            public float maxSpeed = 400;
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
        }

        public Camera cam;
        public MovementSettings movementSettings = new MovementSettings();
        public MouseLook mouseLook = new MouseLook();
        public AdvancedSettings advancedSettings = new AdvancedSettings();

        public float height = 1.6f;
        public float radius = 0.5f;
        public float maxSlopeAllowed = 45;
        public float requiredVelocityToSlide = 10;
        public float requiredAngleToSlide = 5; // in degrees

        private Rigidbody rigidBody;
        private CapsuleCollider capsuleCollider;
        private AirStrafeController airStrafeController;
        private float yRotation;
        private Vector3 groundContactNormal;
        private bool jump, previouslyGrounded, jumping, grounded;
        private Dictionary<Collider, Vector3> normalCollisions = new Dictionary<Collider, Vector3>();
        private AnimationCurve slideCurveModifier = new AnimationCurve(new Keyframe(0, 1), new Keyframe(45, 1), new Keyframe(90, 0));

        public Vector3 Velocity {
            get { return rigidBody.velocity; }
        }

        public bool Grounded {
            get { return grounded; }
        }

        public bool Jumping {
            get { return jumping; }
        }

        private void Awake() {
            rigidBody = GetComponent<Rigidbody>();
            rigidBody.drag = 0;
            rigidBody.useGravity = false;
            capsuleCollider = GetComponent<CapsuleCollider>();
            capsuleCollider.radius = radius;
            capsuleCollider.height = height;
            airStrafeController = GetComponent<AirStrafeController>();
        }

        private void Start() {
            cam = Camera.main;
            mouseLook.Init(transform, cam.transform);
        }

        private void Update() {
            RotateView();

            if (CrossPlatformInputManager.GetButtonDown("Jump") && !jump) {
                jump = true;
            }
        }

        /// <summary>
        /// the way gravity is handled is really jank for this controller.
        /// it's impossible to keep still on a slope with gravity without turning up the friction for the material, but that's even more annoying.
        /// depending on the situation, gravity will either be on or off since I'm using Unity's built in physics
        /// </summary>
        private void FixedUpdate() {
            GroundCheck();
            //Debug.Log("Grounded: " + grounded);
            Vector2 input = GetInput();
            if (grounded) {
                if (jump) {
                    rigidBody.useGravity = true;
                    rigidBody.velocity = new Vector3(rigidBody.velocity.x, 0f, rigidBody.velocity.z);
                    rigidBody.AddForce(new Vector3(0f, movementSettings.jumpForce, 0f), ForceMode.VelocityChange);
                    jumping = true;
                } else { // when completely grounded, ignore gravity and stick to ground
                    rigidBody.useGravity = false;
                    AccelerateToSpeed(input);
                }
            } else {
                rigidBody.useGravity = true;
                airStrafeController.AirStafe(input);
            }
            jump = false;
        }

        private float SlopeMultiplier() {
            float angle = Vector3.Angle(groundContactNormal, Vector3.up);
            return movementSettings.slopeCurveModifier.Evaluate(angle);
        }

        private void AccelerateToSpeed(Vector2 input) {
            Vector3 target = transform.TransformDirection(new Vector3(input.x, 0, input.y)).normalized;
            target = target * movementSettings.currentTargetSpeed;
            Vector3 delta = target - rigidBody.velocity;
            delta.y = 0;
            Vector3 projectedVelocity = Vector3.ProjectOnPlane(delta, groundContactNormal).normalized * delta.magnitude;
            projectedVelocity = projectedVelocity * movementSettings.groundAccelerationRate;
            rigidBody.AddForce(projectedVelocity, ForceMode.VelocityChange);
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

            if (Physics.SphereCast(transform.position, capsuleCollider.radius * 0.99f, Vector3.down, out hitInfo,
                                   ((capsuleCollider.height / 2f) - capsuleCollider.radius) + advancedSettings.groundCheckDistance, Physics.AllLayers, QueryTriggerInteraction.Ignore)) {
                groundContactNormal = hitInfo.normal;
                if (CanSlide() || Vector3.Angle(groundContactNormal, Vector3.up) > maxSlopeAllowed) {
                    grounded = false;
                } else {
                    grounded = true;
                }
            } else {
                grounded = false;
                groundContactNormal = Vector3.up;
            }
            if (!previouslyGrounded && grounded && jumping) {
                jumping = false;
            }
        }

        /// <summary>
        /// Checks when sliding is appropriate.
        /// When the player wasn't previously grounded, the velocity can overcome the slope, and it meets the required angle to slide.
        /// Angles based on rigidbody velocity.
        /// </summary>
        private bool CanSlide() {
            return !grounded && 
                    rigidBody.velocity.sqrMagnitude > GetRequiredSlideVelocity(rigidBody.velocity, groundContactNormal) && 
                    Vector3.Angle(groundContactNormal, rigidBody.velocity) < requiredAngleToSlide;
        }

        private float GetRequiredSlideVelocity(Vector3 velocity, Vector3 groundNormal) {
            float multiplier = Vector3.Cross(velocity.normalized, groundNormal.normalized).magnitude;
            //Debug.Log(multiplier);
            if(Math.Abs(multiplier) < float.Epsilon) {
                return movementSettings.maxSpeed + 1;
            }
            //Debug.Log(requiredVelocityToSlide / multiplier);
            return requiredVelocityToSlide / multiplier;
        }

        private void OnCollisionEnter(Collision collision) {
            normalCollisions[collision.collider] = collision.contacts[0].normal; // 1 point of contact and its normal is sufficient.
        }

        private void OnCollisionStay(Collision collision) {
            normalCollisions[collision.collider] = collision.contacts[0].normal; // 1 point of contact and its normal is sufficient.
        }

        private void OnCollisionExit(Collision collision) {
            normalCollisions.Remove(collision.collider);
        }
    }
}
