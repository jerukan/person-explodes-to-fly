using System;
using ExplosionJumping.PlayerControl.Movement.AirControl;
using ExplosionJumping.Util;
using UnityEngine;

namespace ExplosionJumping.PlayerControl.Movement {
    /// <summary>
    /// Ripped straight from RigidbodyFirstPersonController from the standard assets with many modifications.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(PlayerAirController))]
    public class RigidbodyFPController : MonoBehaviour {

        public PlayerHead head;
        public Camera cam;

        [Header("Basic movement")]

        public float forwardSpeed = 4f;   // Speed when walking forward
        public float backwardSpeed = 3.5f;  // Speed when walking backwards
        public float strafeSpeed = 4f;    // Speed when walking sideways
        [Tooltip("The rate the player will speed up or slow down while walking.")]
        [Range(0f, 1f)]
        public float groundAccelerationRate = 0.2f;
        [Tooltip("How much the player speed will be multiplied by when crouching.")]
        public float crouchMultiplier = 0.5f;
        [Tooltip("The max possible horizontal speed the player can move at, even in the air.")]
        public float maxHorizontalSpeed = 400f;
        [Tooltip("The maximum speed the player can be at before being considered at a standstill.")]
        public float lowestSpeedPossible = 0.0001f;

        [Header("Vertical movement")]

        public float jumpForce = 5f;
        public float gravityMultiplier = 1f;

        [Header("Bunnyhopping")]

        [Tooltip("Window of time in seconds where a premature/late jump will preserve momentum. Setting it too high will make the player too slippery when landing.")]
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

        private Rigidbody rigidBody;
        private CapsuleCollider capsuleCollider;
        private PlayerAirController airStrafeController;
        private Vector3 groundContactNormal;
        private bool jump;
        private bool bottomGrounded;
        private float height, radius;

        [HideInInspector] public float currentTargetSpeed;
        [HideInInspector] public Vector2 currentTargetDirection; // local direction
        [HideInInspector] public Vector2 currentTargetLookAngle; // desired local direction for the camera to look at (x and y rot)
        private int ticksOnGround; // time the player has spend on the ground in the duration of being grounded.
        private int totalTicksInAir; // total time player has spend in the air (persistent between jumps).
        private int ticksWhenJumpedInAir; // when the player pressed the jump button while still in the air.

        private int contactLayerMask;

        public Vector3 Velocity {
            get { return rigidBody.velocity; }
        }

        public Vector3 HorizontalVelocity {
            get { return new Vector3(Velocity.x, 0f, Velocity.z); }
        }

        public Vector3 ColliderBottom {
            get {
                return capsuleCollider.bounds.center - new Vector3(0f, capsuleCollider.bounds.extents.y, 0f);
            }
            set {
                transform.position = value + new Vector3(0f, capsuleCollider.height / 2f, 0f);
            }
        }

        public bool Jumping {
            get;
            private set;
        }

        public bool Crouching {
            get;
            private set;
        }

        public bool Sliding {
            get;
            private set;
        }

        public bool Grounded {
            get;
            private set;
        }

        public bool Jump {
            set {
                jump = value;
                if (!autoBunnyHop && !Grounded && jump) {
                    ticksWhenJumpedInAir = totalTicksInAir;
                }
            }
        }

        public bool Crouch {
            set {
                bool prevState = Crouching;
                Crouching = value;
                if (Crouching) {
                    SetColliderHeight(height * 0.5f);
                }
                else {
                    SetColliderHeight(height);
                }
                if (Crouching && !prevState && Grounded) {
                    transform.Translate(new Vector3(0f, -(height - capsuleCollider.height) / 2, 0f), Space.World);
                }
            }
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
            height = capsuleCollider.height;
            radius = capsuleCollider.radius;
            airStrafeController = GetComponent<PlayerAirController>();
            contactLayerMask = LayerMask.GetMask("Default");
        }

        private void Start() {
        }

        private void Update() {
        }

        private void FixedUpdate() {
            GroundCheck();
            UpdateDesiredTargetSpeed();

            if (Grounded) {
                if((totalTicksInAir - ticksWhenJumpedInAir) * Time.fixedDeltaTime < bunnyHopWindow / 2) {
                    jump = true;
                }
                ticksOnGround++;
                if (jump) {
                    rigidBody.velocity = new Vector3(rigidBody.velocity.x, 0f, rigidBody.velocity.z);
                    rigidBody.AddForce(new Vector3(0f, jumpForce, 0f), ForceMode.VelocityChange);
                    Jumping = true;
                }
                else if(ticksOnGround * Time.fixedDeltaTime > bunnyHopWindow / 2f) { // when completely grounded, ignore gravity and stick to ground
                    // also prevents normal ground movement until the bunnyhop window goes past.
                    AccelerateToSpeed();
                    ZeroLowVelocity();
                }
            }
            else {
                ticksOnGround = 0;
                totalTicksInAir++;
                rigidBody.AddForce(new Vector3(0f, Physics.gravity.y * gravityMultiplier, 0f), ForceMode.Acceleration);
                airStrafeController.AirStafe(currentTargetDirection);
            }
            CapHorizontalVelocity();
            jump = false;
        }

        private void AccelerateToSpeed() {
            Vector3 target = transform.TransformDirection(new Vector3(currentTargetDirection.x, 0, currentTargetDirection.y)).normalized;
            target = target * currentTargetSpeed;
            Vector3 delta = target - rigidBody.velocity;
            delta.y = 0;
            Vector3 projectedVelocity = Vector3.ProjectOnPlane(delta, groundContactNormal).normalized * delta.magnitude;
            projectedVelocity = projectedVelocity * groundAccelerationRate;
            rigidBody.AddForce(projectedVelocity, ForceMode.VelocityChange);
        }

        /// <summary>
        /// Spherecast down just beyond the bottom of the capsule to see if the capsule is colliding round the bottom
        /// Spherecast radius is slightly smaller than capsule radius to prevent collision with vertical walls.
        /// </summary>
        private void GroundCheck() {
            RaycastHit hitInfo;

            if (Physics.SphereCast(transform.position, capsuleCollider.radius * 0.99f, Vector3.down, out hitInfo,
                                   ((capsuleCollider.height / 2f) - capsuleCollider.radius) + groundCheckDistance, contactLayerMask, QueryTriggerInteraction.Ignore)) {
                groundContactNormal = hitInfo.normal;
                Jumping = false;

                RaycastHit bottomHitInfo;
                bottomGrounded = Physics.Raycast(transform.position, Vector3.down, out bottomHitInfo, capsuleCollider.height / 2f + groundCheckDistance, contactLayerMask, QueryTriggerInteraction.Ignore);
                // TODO sliding machine broke fix pls
                if ((Vector3.Angle(groundContactNormal, Vector3.up) > maxSlopeAllowed || CanSlide()) && !bottomGrounded) {
                    Grounded = false;
                    Sliding = true;
                }
                else {
                    Grounded = true;
                    Sliding = false;
                }
            }
            else {
                Grounded = false;
                Sliding = false;
                groundContactNormal = Vector3.up;
            }
        }

        private void UpdateDesiredTargetSpeed() {
            if (currentTargetDirection == Vector2.zero) {
                return;
            }
            if (currentTargetDirection.x > 0 || currentTargetDirection.x < 0) {
                //strafe
                currentTargetSpeed = strafeSpeed;
            }
            if (currentTargetDirection.y < 0) {
                //backwards
                currentTargetSpeed = backwardSpeed;
            }
            if (currentTargetDirection.y > 0) {
                //forwards
                //handled last as if strafing and moving forward at the same time forwards speed should take precedence
                currentTargetSpeed = forwardSpeed;
            }
            if (Crouching) {
                currentTargetSpeed *= crouchMultiplier;
            }
        }

        private void CapHorizontalVelocity() {
            Vector3 velocityNoY = rigidBody.velocity;
            velocityNoY.y = 0;
            if (velocityNoY.sqrMagnitude > maxHorizontalSpeed * maxHorizontalSpeed) {
                velocityNoY = velocityNoY.normalized * maxHorizontalSpeed;
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
            return !Grounded && rigidBody.velocity.sqrMagnitude > requiredVelocityToSlide * requiredVelocityToSlide && Vector3.Angle(groundContactNormal, Vector3.up) > requiredAngleToSlide;
        }

        private void SetColliderHeight(float desiredHeight) {
            if(Math.Abs(desiredHeight - capsuleCollider.height) < float.Epsilon) { return; }
            if(desiredHeight < capsuleCollider.radius * 2) {
                desiredHeight = capsuleCollider.radius * 2;
            }
            capsuleCollider.height = desiredHeight;
        }

        private void PerformAutoClimb(Collision collision) {
            ContactPoint highestContact = collision.contacts[0];
            // find highest point of contact to climb to
            foreach (ContactPoint cp in collision.contacts) {
                if (cp.point.y > highestContact.point.y) {
                    highestContact = cp;
                }
            }
            float heightDifference = highestContact.point.y - ColliderBottom.y;
            RaycastHit bottomHit;
            Vector3 contactDirection = highestContact.point - ColliderBottom;
            contactDirection.y = 0;
            // if the point to autoclimb to is part of a walkable slope, do nothing
            if (Physics.Raycast(ColliderBottom, contactDirection, out bottomHit, capsuleCollider.radius, contactLayerMask, QueryTriggerInteraction.Ignore)) {
                if (Vector3.Angle(Vector3.up, bottomHit.normal) < maxSlopeAllowed) {
                    return;
                }
            }
            RaycastHit highestContactHit; // check slope of the point to climb to
            Physics.Raycast(highestContact.point + new Vector3(0f, 0.1f, 0f), Vector3.down, out highestContactHit, 0.2f, contactLayerMask, QueryTriggerInteraction.Ignore);
            // perform autoclimb if highest point is valid
            if (heightDifference <= autoClimbMaxHeight && heightDifference > 0f && Grounded && Vector3.Angle(Vector3.up, highestContactHit.normal) < maxSlopeAllowed) {
                ColliderBottom = highestContact.point;
                rigidBody.velocity = new Vector3(rigidBody.velocity.x, 0f, rigidBody.velocity.z);
            }
        }

        private void OnCollisionEnter(Collision collision) {
            PerformAutoClimb(collision);
        }
    }
}