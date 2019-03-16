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
            [Tooltip("The rate the player will speed up or slow down.")]
            [Range(0f, 1f)]
            public float groundAccelerationRate = 0.2f;
            [Tooltip("How much the player speed will be multiplied by when crouching.")]
            public float crouchMultiplier = 0.5f;
            internal bool crouching;
            public KeyCode crouchKey = KeyCode.LeftControl;
            //public AnimationCurve slopeCurveModifier = new AnimationCurve(new Keyframe(-90.0f, 1.0f), new Keyframe(0.0f, 1.0f), new Keyframe(90.0f, 0.0f));
            [Tooltip("The max possible speed the player can move at, even in the air.")]
            public float maxSpeed = 400f;
            public float gravityMultiplier = 1f;

            public float bunnyHopWindow = 0.1f;
            public bool autoBunnyHop;
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
                if (Input.GetKey(crouchKey)) {
                    currentTargetSpeed *= crouchMultiplier;
                    crouching = true;
                }
                else {
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

        //public float playerHeight = 1.6f;
        //public float playerRadius = 0.5f;
        public float maxSlopeAllowed = 45;
        public float requiredVelocityToSlide = 10;
        public float requiredAngleToSlide = 5; // in degrees

        private Rigidbody rigidBody;
        private CapsuleCollider capsuleCollider;
        private AirStrafeController airStrafeController;
        private float yRotation;
        private Vector3 groundContactNormal;
        private bool jump, grounded;
        private AnimationCurve slideCurveModifier = new AnimationCurve(new Keyframe(0, 1), new Keyframe(45, 1), new Keyframe(90, 0));

        private int ticksOnGround; // time the player has spend on the ground in the duration of being grounded.
        private int totalTicksInAir; // total time player has spend in the air (persistent between jumps).
        private int ticksWhenJumpedInAir; // when the player pressed the jump button while still in the air.

        public Vector3 Velocity {
            get { return rigidBody.velocity; }
        }

        public bool Grounded {
            get { return grounded; }
        }

        private void Awake() {
            rigidBody = GetComponent<Rigidbody>();
            rigidBody.drag = 0;
            rigidBody.angularDrag = 0;
            rigidBody.useGravity = false;
            rigidBody.constraints = RigidbodyConstraints.FreezeRotation;
            rigidBody.isKinematic = false;
            capsuleCollider = GetComponent<CapsuleCollider>();
            //capsuleCollider.radius = playerRadius;
            //capsuleCollider.height = playerHeight;
            capsuleCollider.isTrigger = false;
            airStrafeController = GetComponent<AirStrafeController>();
        }

        private void Start() {
            cam = Camera.main;
            mouseLook.Init(transform, cam.transform);
        }

        private void Update() {
            RotateView();

            if (movementSettings.autoBunnyHop) {
                jump = CrossPlatformInputManager.GetButton("Jump");
            }
            else if (CrossPlatformInputManager.GetButtonDown("Jump")) {
                jump = true;
                if (!grounded) {
                    ticksWhenJumpedInAir = totalTicksInAir;
                }
            }
        }

        private void FixedUpdate() {
            GroundCheck();
            //Debug.Log("Grounded: " + grounded);
            Vector2 input = GetInput();
            if (grounded) {
                // todo make totalTicksInAir not actually total ticks
                if((totalTicksInAir - ticksWhenJumpedInAir) * Time.fixedDeltaTime < movementSettings.bunnyHopWindow / 2) {
                    jump = true;
                    //ticksWhenJumpedInAir = 0;
                }
                ticksOnGround++;
                //ticksInAir = 0;
                if (jump) {
                    rigidBody.velocity = new Vector3(rigidBody.velocity.x, 0f, rigidBody.velocity.z);
                    rigidBody.AddForce(new Vector3(0f, movementSettings.jumpForce, 0f), ForceMode.VelocityChange);
                }
                else if(ticksOnGround * Time.fixedDeltaTime > movementSettings.bunnyHopWindow / 2f) { // when completely grounded, ignore gravity and stick to ground
                    // also prevents normal ground movement until the bunnyhop window goes past.
                    AccelerateToSpeed(input);
                }
            }
            else {
                ticksOnGround = 0;
                totalTicksInAir++;
                rigidBody.AddForce(new Vector3(0f, Physics.gravity.y * movementSettings.gravityMultiplier * rigidBody.mass, 0f), ForceMode.Force);
                airStrafeController.AirStafe(input);
            }
            jump = false;
        }

        //private float SlopeMultiplier() {
        //    float angle = Vector3.Angle(groundContactNormal, Vector3.up);
        //    return movementSettings.slopeCurveModifier.Evaluate(angle);
        //}

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

        /// <summary>
        /// Sphere cast down just beyond the bottom of the capsule to see if the capsule is colliding round the bottom
        /// Sphere radius is slightly smaller than capsule radius to prevent collision with vertical walls.
        /// </summary>
        private void GroundCheck() {
            RaycastHit hitInfo;

            if (Physics.SphereCast(transform.position, capsuleCollider.radius * 0.99f, Vector3.down, out hitInfo,
                                   ((capsuleCollider.height / 2f) - capsuleCollider.radius) + advancedSettings.groundCheckDistance, Physics.AllLayers, QueryTriggerInteraction.Ignore)) {
                groundContactNormal = hitInfo.normal;
                if (CanSlide() || Vector3.Angle(groundContactNormal, Vector3.up) > maxSlopeAllowed) {
                    grounded = false;
                }
                else {
                    grounded = true;
                }
            }
            else {
                grounded = false;
                groundContactNormal = Vector3.up;
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
            if (Math.Abs(multiplier) < float.Epsilon) {
                return movementSettings.maxSpeed + 1;
            }
            //Debug.Log(requiredVelocityToSlide / multiplier);
            return requiredVelocityToSlide / multiplier;
        }
    }
}