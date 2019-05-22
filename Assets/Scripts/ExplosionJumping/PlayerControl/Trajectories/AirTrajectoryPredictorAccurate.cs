using UnityEngine;
using ExplosionJumping.Util.Paths;

namespace ExplosionJumping.PlayerControl.Trajectories {

    /// <summary>
    /// Uses an Euler approximation to find where the player will land in complex terrain.
    /// Iteratively raycasts along the trajectory so it's marginally slower.
    /// </summary>
    [RequireComponent(typeof(FirstPersonControllerCustom))]
    [AddComponentMenu("Player Control/Trajectories/Air Trajectory Predictor Accurate")]
    public class AirTrajectoryPredictorAccurate : MonoBehaviour {

        [Tooltip("The gameobject used to visually indicate where the player will land.")]
        public GameObject landingIndicatorPrefab;
        [Tooltip("The time in seconds the predictor should advance the trajectory prediction by every iteration.")]
        public float dt = 0.3f;
        [Tooltip("How many times the predictor will check for a collision.")]
        public int maxIterations = 100;
        [Tooltip("The distance the landing indicator itself will be translated up by so it doesn't clip into the ground.")]
        public float verticalIndicatorOffset = 0.05f;

        private Vector3 predictedLandingSpot, colliderBottom;
        private float gravityMultiplier;
        private Collider playerCollider;
        private Rigidbody rigidBody;
        private AirPath currentPath;
        private GameObject landingIndicator;
        private FirstPersonControllerCustom controller;

        public Vector3 PredictedLandingSpot {
            get { return predictedLandingSpot; }
        }

        private void Awake() {
            playerCollider = GetComponent<Collider>();
            rigidBody = GetComponent<Rigidbody>();
            controller = GetComponent<FirstPersonControllerCustom>();
            gravityMultiplier = controller.gravityMultiplier;
        }

        void Start() {
            landingIndicator = Instantiate(landingIndicatorPrefab);
        }

        void FixedUpdate() {
            if (!controller.Grounded) {
                landingIndicator.SetActive(true);
                Vector3 colliderWorldCenter = playerCollider.bounds.center;
                colliderBottom = new Vector3(colliderWorldCenter.x, playerCollider.bounds.min.y, colliderWorldCenter.z);
                UpdatePredictedLandingSpot(rigidBody.velocity);
                landingIndicator.transform.position = predictedLandingSpot;
                landingIndicator.transform.Translate(new Vector3(0f, verticalIndicatorOffset, 0f), Space.World);
            }
            else {
                landingIndicator.SetActive(false);
            }
        }

        public void UpdatePredictedLandingSpot(Vector3 velocity) {
            currentPath = new AirPath(colliderBottom, velocity, Physics.gravity * gravityMultiplier);
            Vector3 raycastDirection;
            float currentTime = 0f;
            for (int i = 0; i < maxIterations; i++) {
                raycastDirection = currentPath.GetVelocity(currentTime);
                if (Physics.Raycast(currentPath.GetPosition(currentTime), raycastDirection, out RaycastHit hitinfo,
                                    raycastDirection.magnitude * dt, LayerMask.GetMask(controller.groundContactLayerNames), QueryTriggerInteraction.Ignore)) {
                    float timetoground = currentPath.GetTimeToGround((colliderBottom - hitinfo.point).y);
                    predictedLandingSpot = currentPath.GetPosition(timetoground);
                    break;
                }
                currentTime += dt;
            }
        }
    }
}