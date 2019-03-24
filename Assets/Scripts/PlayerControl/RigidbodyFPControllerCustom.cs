using System;
using ExplosionJumping.PlayerControl.AirControl;
using UnityEngine;

namespace ExplosionJumping.PlayerControl {
    /// <summary>
    /// Ripped straight from RigidbodyFirstPersonController from the standard assets with many modifications.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(PlayerAirController))]
    public class RigidbodyFPControllerCustom : MonoBehaviour {

        public Camera cam;
        public MouseLook mouseLook = new MouseLook();

        [Header("Basic movement")]

        public float forwardSpeed = 4f;   // Speed when walking forward
        public float backwardSpeed = 3.5f;  // Speed when walking backwards
        public float strafeSpeed = 4f;    // Speed when walking sideways
        [Tooltip("The rate the player will speed up or slow down while walking.")]
        [Range(0f, 1f)]
        public float groundAccelerationRate = 0.2f;
        [Tooltip("How much the player speed will be multiplied by when crouching.")]
        public float crouchMultiplier = 0.5f;
        public KeyCode crouchKey = KeyCode.LeftControl;
        [Tooltip("The max possible horizontal speed the player can move at, even in the air.")]
        public float maxSpeed = 400f;
        [Tooltip("The maximum speed the player can be at before being considered at a standstill.")]
        public float lowestSpeedPossible = 0.0001f;

        [Header("Vertical movement")]

        public float jumpForce = 5f;
        public float gravityMultiplier = 1f;

        [Header("Bunnyhopping")]

        [Tooltip("Window of time in seconds where a premature/late jump will preserve momentum.")]
        public float bunnyHopWindow = 0.2f;
        [Tooltip("Allows the user to hold down jump to continuously bunnyhop perfectly.")]
        public bool autoBunnyHop;

        [Header("Misc movement")]

        [Tooltip("Distance for checking if the controller is grounded (0.01f seems to work best for this).")]
        public float groundCheckDistance = 0.01f; // distance for checking if the controller is grounded ( 0.01f seems to work best for this )
        public float maxSlopeAllowed = 45f;
        public float requiredVelocityToSlide = 10f;
        public float requiredAngleToSlide = 5f; // in degrees
        public float autoClimbMaxHeight = 0.05f;
        public AnimationCurve slideFrictionModifier = new AnimationCurve(new Keyframe(-90f, 1f), new Keyframe(-45f, 1f), new Keyframe(0f, 0.95f), new Keyframe(45f, 0.9f), new Keyframe(90f, 0f));

        private Rigidbody rigidBody;
        private CapsuleCollider capsuleCollider;
        private PlayerAirController airStrafeController;
        private Vector3 groundContactNormal;
        private bool jump, grounded, crouching, sliding;
        private bool canAutoClimb;
        private float toClimb;

        private float currentTargetSpeed = 8f;
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
            capsuleCollider.isTrigger = false;
            airStrafeController = GetComponent<PlayerAirController>();
        }

        private void Start() {
            mouseLook.Init(transform, cam.transform);
        }

        private void Update() {
            RotateView();

            if (autoBunnyHop) {
                jump = Input.GetButton("Jump");
            }
            else if (Input.GetButtonDown("Jump")) {
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
                if((totalTicksInAir - ticksWhenJumpedInAir) * Time.fixedDeltaTime < bunnyHopWindow / 2) {
                    jump = true;
                    //ticksWhenJumpedInAir = 0;
                }
                ticksOnGround++;
                //ticksInAir = 0;
                if (jump) {
                    rigidBody.velocity = new Vector3(rigidBody.velocity.x, 0f, rigidBody.velocity.z);
                    rigidBody.AddForce(new Vector3(0f, jumpForce, 0f), ForceMode.VelocityChange);
                }
                else if(ticksOnGround * Time.fixedDeltaTime > bunnyHopWindow / 2f) { // when completely grounded, ignore gravity and stick to ground
                    // also prevents normal ground movement until the bunnyhop window goes past.
                    AccelerateToSpeed(input);
                    ZeroLowVelocity();
                }
            }
            else {
                ticksOnGround = 0;
                totalTicksInAir++;
                rigidBody.AddForce(new Vector3(0f, Physics.gravity.y * gravityMultiplier * rigidBody.mass, 0f), ForceMode.Force);
                airStrafeController.AirStafe(input);
                if(sliding) {
                    //rigidBody.velocity *= GetSlideFriction();
                }
            }
            CapHorizontalVelocity();
            jump = false;
            if(canAutoClimb) {
                transform.Translate(new Vector3(0f, toClimb, 0f));
                canAutoClimb = false;
            }
        }

        private void AccelerateToSpeed(Vector2 input) {
            Vector3 target = transform.TransformDirection(new Vector3(input.x, 0, input.y)).normalized;
            target = target * currentTargetSpeed;
            Vector3 delta = target - rigidBody.velocity;
            delta.y = 0;
            Vector3 projectedVelocity = Vector3.ProjectOnPlane(delta, groundContactNormal).normalized * delta.magnitude;
            projectedVelocity = projectedVelocity * groundAccelerationRate;
            rigidBody.AddForce(projectedVelocity, ForceMode.VelocityChange);
        }

        private Vector2 GetInput() {
            // raw axis makes keyboard actually work properly
            Vector2 input = new Vector2 {
                x = Input.GetAxisRaw("Horizontal"),
                y = Input.GetAxisRaw("Vertical")
            };
            UpdateDesiredTargetSpeed(input);
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
        /// Spherecast down just beyond the bottom of the capsule to see if the capsule is colliding round the bottom
        /// Spherecast radius is slightly smaller than capsule radius to prevent collision with vertical walls.
        /// </summary>
        private void GroundCheck() {
            RaycastHit hitInfo;

            if (Physics.SphereCast(transform.position, capsuleCollider.radius * 1f, Vector3.down, out hitInfo,
                                   ((capsuleCollider.height / 2f) - capsuleCollider.radius) + groundCheckDistance, Physics.AllLayers, QueryTriggerInteraction.Ignore)) {
                groundContactNormal = hitInfo.normal;
                Vector3 topOfCollider = hitInfo.collider.bounds.center + new Vector3(0f, hitInfo.collider.bounds.extents.y, 0f);
                float heightDifference = topOfCollider.y - transform.TransformPoint(capsuleCollider.center - new Vector3(0f, capsuleCollider.height / 2, 0f)).y;
                //Debug.Log($"Height difference: {heightDifference}");
                if(heightDifference < autoClimbMaxHeight && Vector3.Angle(groundContactNormal, Vector3.up) > 70f) {
                    canAutoClimb = true;
                    toClimb = heightDifference;
                } else {
                    canAutoClimb = false;
                }
                //Debug.Log(Vector3.Angle(groundContactNormal, Vector3.up));
                if (Vector3.Angle(groundContactNormal, Vector3.up) > maxSlopeAllowed || CanSlide()) {
                    grounded = false;
                    sliding = true;
                }
                else {
                    grounded = true;
                    sliding = false;
                }
            }
            else {
                grounded = false;
                sliding = false;
                groundContactNormal = Vector3.up;
            }
        }

        private void UpdateDesiredTargetSpeed(Vector2 input) {
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

        private void CapHorizontalVelocity() {
            Vector3 velocityNoY = rigidBody.velocity;
            velocityNoY.y = 0;
            if (velocityNoY.sqrMagnitude > maxSpeed * maxSpeed) {
                velocityNoY = velocityNoY.normalized * maxSpeed;
                rigidBody.velocity = new Vector3(velocityNoY.x, rigidBody.velocity.y, velocityNoY.z);
            }
        }

        private void ZeroLowVelocity() {
            float velX = rigidBody.velocity.x;
            float velZ = rigidBody.velocity.z;
            if (Math.Abs(rigidBody.velocity.x) < lowestSpeedPossible) {
                velX = 0;
            }
            if (Math.Abs(rigidBody.velocity.z) < lowestSpeedPossible) {
                velZ = 0;
            }
            rigidBody.velocity = new Vector3(velX, rigidBody.velocity.y, velZ);
        }

        private bool CanSlide() {
            return !grounded && rigidBody.velocity.sqrMagnitude > requiredVelocityToSlide * requiredVelocityToSlide && Vector3.Angle(groundContactNormal, Vector3.up) > requiredAngleToSlide;
        }

        private float GetSlideFriction() {
            float angle = Vector3.SignedAngle(Vector3.up, groundContactNormal, transform.right);
            Debug.Log($"AGNGLE {angle}");
            return slideFrictionModifier.Evaluate(angle);
        }
    }
}