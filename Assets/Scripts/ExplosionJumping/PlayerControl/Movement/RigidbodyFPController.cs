using System;
using ExplosionJumping.PlayerControl.Movement.AirControl;
using UnityEngine;

namespace ExplosionJumping.PlayerControl.Movement {
    /// <summary>
    /// Ripped straight from RigidbodyFirstPersonController from the standard assets with many modifications.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(PlayerAirController))]
    [AddComponentMenu("Player Control/Rigidbody First Person Controller")]
    public class RigidbodyFPController : MonoBehaviour {

        [Tooltip("Reference to the gameobject (usually child) that rotates the camera to look around.")]
        public PlayerHead head;
        [Tooltip("Identifies the layers of gameobjects that the player should be allowed to stand on.")]
        public string[] groundContactLayerNames = { "Default" };

        [Header("Basic movement")]

        [Tooltip("Speed when walking forward.")]
        public float forwardSpeed = 4f;   // Speed when walking forward
        [Tooltip("Speed when walking backwards.")]
        public float backwardSpeed = 3.5f;  // Speed when walking backwards
        [Tooltip("Speed when walking sideways. This speed is overrided by forwardSpeed when both strafing and moving forward.")]
        public float strafeSpeed = 4f;    // Speed when walking sideways
        [Tooltip("The rate the player will speed up or slow down while walking.")]
        [Range(0f, 1f)]
        public float groundAccelerationRate = 0.2f;
        [Tooltip("How much the player speed will be multiplied by when crouching.")]
        public float crouchSpeedMultiplier = 0.5f;
        [Tooltip("The max possible horizontal speed the player can move at, even in the air.")]
        public float maxHorizontalSpeed = 100f;
        [Tooltip("The minimum speed the player can be at before being considered at a standstill.")]
        public float lowestSpeedPossible = 0.0001f;
        [Tooltip("The maximum falling vertical speed the player can move at.")]
        public float terminalVelocity = 75f;

        [Header("Vertical movement")]

        [Tooltip("The force to push the player up for jumping. The force will not be affected by mass.")]
        public float jumpForce = 5f;
        [Tooltip("This is multiplied by the Physics Engine's gravitational constant to affect how fast this player falls.")]
        public float gravityMultiplier = 1f;

        [Header("Bunnyhopping")]

        [Tooltip("Window of time in seconds where a premature/late jump will preserve momentum. Setting it too high will make the player too slippery when landing.")]
        public float bunnyHopWindow = 0.2f;
        [Tooltip("Set to true to allow the user to hold down the jump buttom to continuously bunnyhop perfectly.")]
        public bool autoBunnyHop;

        [Header("Misc movement")]

        [Tooltip("How much to multiply the capsule collider's height by to represent crouching.")]
        [Range(0f, 1f)]
        public float crouchHeightMultiplier = 0.5f;
        [Tooltip("Distance for checking if the controller is grounded (0.01f seems to work best for this).")]
        public float groundCheckDistance = 0.01f; // distance for checking if the controller is grounded (0.01f seems to work best for this)
        [Tooltip("The maximum slope the player can stand on before being considered not grounded anymore.")]
        [Range(0f, 90f)]
        public float maxSlopeAllowed = 45f;
        [Tooltip("The minimum speed required start rampsliding behaviour.")]
        public float requiredVelocityToSlide = 10f;
        [Tooltip("The minimum slope of the terrain in degrees required to start rampsliding behaviour.")]
        public float requiredAngleToSlide = 5f; // in degrees
        [Tooltip("Set to true to allow the player to automatically climb up short enough elevations.")]
        public bool enableAutoClimb = true;
        [Tooltip("The maximum height of the an elevation the player can automatically climb up.")]
        public float autoClimbMaxHeight = 0.2f;

        private Rigidbody rigidBody;
        private CapsuleCollider capsuleCollider;
        private PlayerAirController airStrafeController;
        private Vector3 groundContactNormal;
        private bool shouldJump;
        private bool bottomGrounded; // whether the bottom (and only bottom) of the capsule is grounded
        private float startingHeight, startingRadius;

        [HideInInspector] public float currentGroundTargetSpeed;
        [HideInInspector] public Vector2 currentTargetMovementDirection; // local direction
        [HideInInspector] public Vector2 currentTargetLookAngle; // desired local direction for the camera to look at (x and y rot)
        private int ticksOnGround; // time the player has spend on the ground in the duration of being grounded.
        private int totalTicksInAir; // total time player has spend in the air (persistent between jumps).
        private int ticksWhenJumpedInAir; // when the player pressed the jump button while still in the air.

        private int contactLayerMasks; // layers that the player can stand on.

        public Vector3 Velocity {
            get { return rigidBody.velocity; }
        }

        public Vector3 HorizontalVelocity {
            get { return new Vector3(Velocity.x, 0f, Velocity.z); }
        }

        /// <summary>
        /// The bottom of the capsule collider. Allows setting the position of the player through this field.
        /// This position is in world space.
        /// </summary>
        public Vector3 ColliderBottom {
            get {
                return capsuleCollider.bounds.center - new Vector3(0f, capsuleCollider.bounds.extents.y, 0f);
            }
            set {
                transform.position = value + new Vector3(0f, capsuleCollider.height / 2f, 0f);
            }
        }

        /// <summary>
        /// Represents whether the player is in the air through jumping.
        /// </summary>
        /// <value><c>true</c> if the player is in the air because of jumping; otherwise, <c>false</c>.</value>
        public bool Jumping {
            get;
            private set;
        }

        /// <summary>
        /// Represents whether the player is currently crouching.
        /// </summary>
        /// <value><c>true</c> if the player is crouching; otherwise, <c>false</c>.</value>
        public bool Crouching {
            get;
            private set;
        }

        /// <summary>
        /// Represents whether the player is currently sliding (rampsliding).
        /// When the player is on the ground and not considered grounded.
        /// </summary>
        /// <value><c>true</c> if the player is sliding; otherwise, <c>false</c>.</value>
        public bool Sliding {
            get;
            private set;
        }

        /// <summary>
        /// Represents whether the player is on the ground, or touching it in general as long as the player is not sliding.
        /// </summary>
        /// <value><c>true</c> if the player is fully grounded; otherwise, <c>false</c>.</value>
        public bool Grounded {
            get;
            private set;
        }
        /// <summary>
        /// Tells the player to jump, and then processes whether it should jump.
        /// </summary>
        public bool ShouldJump {
            set {
                shouldJump = value;
                // if in the air, logs when the player jumped
                if (!autoBunnyHop && !Grounded && shouldJump) {
                    ticksWhenJumpedInAir = totalTicksInAir;
                }
            }
        }

        /// <summary>
        /// Tells the player to crouch by multiplying its height by some factor.
        /// </summary>
        public bool ShouldCrouch {
            set {
                bool prevState = Crouching;
                Crouching = value;
                if (Crouching) {
                    SetColliderHeight(startingHeight * crouchHeightMultiplier);
                }
                else {
                    SetColliderHeight(startingHeight);
                }
                if (Crouching && !prevState && Grounded) {
                    // if the player is grounded and just crouched, the player should immediately be translated to touch the ground.
                    transform.Translate(new Vector3(0f, -(startingHeight - capsuleCollider.height) / 2f, 0f), Space.World);
                }
            }
        }

        /// <summary>
        /// Initializes component fields, as well as a few other ones.
        /// </summary>
        private void Awake() {
            rigidBody = GetComponent<Rigidbody>();
            capsuleCollider = GetComponent<CapsuleCollider>();
            startingHeight = capsuleCollider.height;
            startingRadius = capsuleCollider.radius;
            airStrafeController = GetComponent<PlayerAirController>();
            contactLayerMasks = LayerMask.GetMask(groundContactLayerNames);
        }

        private void FixedUpdate() {
            GroundCheck();
            UpdateDesiredTargetSpeed();

            if (Grounded) {
                // checks whether the player jumped while still in the air within bunnyHopWindow / 2 seconds of landing.
                shouldJump |= (totalTicksInAir - ticksWhenJumpedInAir) * Time.fixedDeltaTime <= bunnyHopWindow / 2f;
                ticksOnGround++;
                if (shouldJump) {
                    //rigidBody.velocity = new Vector3(rigidBody.velocity.x, 0f, rigidBody.velocity.z);
                    rigidBody.AddForce(new Vector3(0f, jumpForce, 0f), ForceMode.VelocityChange);
                    Jumping = true;
                }
                else if (ticksOnGround * Time.fixedDeltaTime > bunnyHopWindow / 2f) { // when completely grounded, ignore gravity and stick to ground
                    // also prevents normal ground movement until the bunnyhop window goes past.
                    Jumping = false;
                    AccelerateToSpeed();
                    ZeroLowVelocity();
                }
            }
            else {
                ticksOnGround = 0;
                totalTicksInAir++;
                rigidBody.AddForce(new Vector3(0f, Physics.gravity.y * gravityMultiplier, 0f), ForceMode.Acceleration);
                airStrafeController.AirStafe(currentTargetMovementDirection);
            }
            CapVelocity();
            shouldJump = false;
        }

        /// <summary>
        /// Only active when grounded.
        /// Calculates how much to speed up by and accelerate to <see cref="currentGroundTargetSpeed"/> in the <see cref="currentTargetMovementDirection"/> direction.
        /// </summary>
        private void AccelerateToSpeed() {
            Vector3 target = transform.TransformDirection(new Vector3(currentTargetMovementDirection.x, 0f, currentTargetMovementDirection.y)).normalized;
            target = target * currentGroundTargetSpeed;
            Vector3 delta = target - rigidBody.velocity;
            delta.y = 0;
            Vector3 projectedVelocity = Vector3.ProjectOnPlane(delta, groundContactNormal).normalized * delta.magnitude;
            projectedVelocity = projectedVelocity * groundAccelerationRate;
            rigidBody.AddForce(projectedVelocity, ForceMode.VelocityChange);
        }

        /// <summary>
        /// Updates variables <see cref="Grounded"/>, <see cref="Sliding"/>, <see cref="bottomGrounded"/>, and <see cref="groundContactNormal"/> to represent the state of the player.
        /// Spherecast down just beyond the bottom of the capsule to see if the capsule is colliding round the bottom
        /// Spherecast radius is slightly smaller than capsule radius to prevent collision with vertical walls.
        /// </summary>
        private void GroundCheck() {
            if (Physics.SphereCast(transform.position, capsuleCollider.radius * 0.99f, Vector3.down, out RaycastHit hitInfo,
                                   ((capsuleCollider.height / 2f) - capsuleCollider.radius) + groundCheckDistance, contactLayerMasks, QueryTriggerInteraction.Ignore)) {
                groundContactNormal = hitInfo.normal;

                bottomGrounded = Physics.Raycast(transform.position, Vector3.down, out RaycastHit bottomHitInfo,
                                                 capsuleCollider.height / 2f + groundCheckDistance, contactLayerMasks, QueryTriggerInteraction.Ignore);
                if (CanSlide() && !bottomGrounded) {
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
                bottomGrounded = false;
                Sliding = false;
                groundContactNormal = Vector3.up;
            }
        }

        /// <summary>
        /// Updates the target speed, not velocity, based on the desired direction to move in.
        /// </summary>
        private void UpdateDesiredTargetSpeed() {
            if (currentTargetMovementDirection == Vector2.zero) {
                return;
            }
            if (currentTargetMovementDirection.x > 0f || currentTargetMovementDirection.x < 0f) {
                //strafe
                currentGroundTargetSpeed = strafeSpeed;
            }
            if (currentTargetMovementDirection.y < 0f) {
                //backwards
                currentGroundTargetSpeed = backwardSpeed;
            }
            if (currentTargetMovementDirection.y > 0f) {
                //forwards
                //handled last as if strafing and moving forward at the same time forwards speed should take precedence
                currentGroundTargetSpeed = forwardSpeed;
            }
            if (Crouching) {
                currentGroundTargetSpeed *= crouchSpeedMultiplier;
            }
        }

        /// <summary>
        /// Caps the player's velocity with the defined limits <see cref="maxHorizontalSpeed"/> and <see cref="terminalVelocity"/>.
        /// </summary>
        private void CapVelocity() {
            Vector3 velocityNoY = rigidBody.velocity;
            velocityNoY.y = 0f;
            if (velocityNoY.sqrMagnitude > maxHorizontalSpeed * maxHorizontalSpeed) {
                velocityNoY = velocityNoY.normalized * maxHorizontalSpeed;
            }
            float ySpeed = rigidBody.velocity.y;
            if(rigidBody.velocity.y < -terminalVelocity) {
                ySpeed = -terminalVelocity;
            }
            rigidBody.velocity = new Vector3(velocityNoY.x, ySpeed, velocityNoY.z);
        }

        /// <summary>
        /// Zeros the horizontal velocity if the speed is too small according to <see cref="lowestSpeedPossible"/>.
        /// </summary>
        private void ZeroLowVelocity() {
            float velX = rigidBody.velocity.x;
            float velZ = rigidBody.velocity.z;
            if (Math.Abs(rigidBody.velocity.x) < lowestSpeedPossible) {
                velX = 0f;
            }
            if (Math.Abs(rigidBody.velocity.z) < lowestSpeedPossible) {
                velZ = 0f;
            }
            rigidBody.velocity = new Vector3(velX, rigidBody.velocity.y, velZ);
        }

        /// <summary>
        /// Determines whether the player is under the right conditions to "slide".
        /// Basically means being considered not grounded while on surface that is suitable for walking.
        /// </summary>
        /// <returns><c>true</c>, if sliding is possible, <c>false</c> otherwise.</returns>
        private bool CanSlide() {
            return (Vector3.Angle(groundContactNormal, Vector3.up) > maxSlopeAllowed) ||
                (rigidBody.velocity.sqrMagnitude > requiredVelocityToSlide * requiredVelocityToSlide &&
                 Vector3.Angle(groundContactNormal, Vector3.up) > requiredAngleToSlide);
        }

        /// <summary>
        /// Sets the height of the collider while also keeping the measurements valid for a capsule.
        /// Height cannot be less than twice the capsule diameter.
        /// </summary>
        /// <param name="desiredHeight">The height to set the player's capsule collider to.</param>
        private void SetColliderHeight(float desiredHeight) {
            if (Math.Abs(desiredHeight - capsuleCollider.height) < float.Epsilon) {
                return;
            }
            if (desiredHeight < capsuleCollider.radius * 2f) {
                desiredHeight = capsuleCollider.radius * 2f;
            }
            capsuleCollider.height = desiredHeight;
        }

        /// <summary>
        /// If enabled, this method will process the given collision and determine whether it can be autoclimbed.
        /// The autoclimb will also be performed by moving the bottom point of the capsule to the highest viable contact point.
        /// </summary>
        /// <param name="collision">The collision with an object that happened.</param>
        private void PerformAutoClimb(Collision collision) {
            ContactPoint highestContact = collision.contacts[0];
            // find highest point of contact to climb to
            foreach (ContactPoint cp in collision.contacts) {
                if (cp.point.y > highestContact.point.y) {
                    highestContact = cp;
                }
            }
            float heightDifference = highestContact.point.y - ColliderBottom.y;
            Vector3 contactDirection = highestContact.point - ColliderBottom;
            contactDirection.y = 0f;
            // if the point to autoclimb to is part of a walkable slope, do nothing
            if (Physics.Raycast(ColliderBottom, contactDirection, out RaycastHit bottomHit,
                                capsuleCollider.radius, contactLayerMasks, QueryTriggerInteraction.Ignore)) {
                if (Vector3.Angle(Vector3.up, bottomHit.normal) < maxSlopeAllowed) {
                    return;
                }
            }
            Physics.Raycast(highestContact.point + new Vector3(0f, 0.1f, 0f), Vector3.down, out RaycastHit highestContactHit,
                            0.2f, contactLayerMasks, QueryTriggerInteraction.Ignore); // check slope of the point to climb to
            // perform autoclimb if highest point is valid
            if (heightDifference <= autoClimbMaxHeight && heightDifference > 0f && Grounded && Vector3.Angle(Vector3.up, highestContactHit.normal) < maxSlopeAllowed) {
                ColliderBottom = highestContact.point;
                rigidBody.velocity = new Vector3(rigidBody.velocity.x, 0f, rigidBody.velocity.z);
            }
        }

        /// <summary>
        /// Autoclimb is only triggered when walking into a new collider.
        /// </summary>
        private void OnCollisionEnter(Collision collision) {
            if (enableAutoClimb) {
                PerformAutoClimb(collision);
            }
        }
    }
}