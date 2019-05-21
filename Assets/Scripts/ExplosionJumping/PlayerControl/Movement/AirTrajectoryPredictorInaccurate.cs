using UnityEngine;
using ExplosionJumping.Util.Paths;

namespace ExplosionJumping.PlayerControl.Movement {

    /// <summary>
    /// No iterative parts in the prediction.
    /// Very inaccurate with complex terrain since it only raycasts once from the player depending on velocity.
    /// </summary>
    [RequireComponent(typeof(RigidbodyFPController))]
    public class AirTrajectoryPredictorInaccurate : MonoBehaviour {

        [Tooltip("The gameobject used to visually indicate where the player will land.")]
        public GameObject landingIndicatorPrefab;
        [Tooltip("The distance the landing indicator itself will be translated up by so it doesn't clip into the ground.")]
        public float verticalIndicatorOffset = 0.05f;

        private Vector3 predictedLandingSpot, colliderBottom;
        private float gravityMultiplier;
        private Collider playerCollider;
        private Rigidbody rigidBody;
        private AirPath currentPath;
        private GameObject landingIndicator;
        private RigidbodyFPController controller;

        public Vector3 PredictedLandingSpot {
            get { return predictedLandingSpot; }
        }

        private void Awake() {
            playerCollider = GetComponent<Collider>();
            rigidBody = GetComponent<Rigidbody>();
            controller = GetComponent<RigidbodyFPController>();
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
            RaycastHit hitinfo;
            // attempts to raycast in the direction the collider is heading at, not straight down for multilevel scenes
            Vector3 raycastDirection;
            // if not past the peak, raycast downwards at a velocity predicted a bit after
            if (currentPath.TimeUntilPeak() > -0.5f) {
                raycastDirection = currentPath.GetVelocity(currentPath.TimeUntilPeak() + 0.5f);
            }
            else { // raycast in the direction of the velocity, which is more accurate the closer it gets
                raycastDirection = velocity;
            }
            //hi my name is people injurer and i like running over homeless people
            Physics.Raycast(colliderBottom, raycastDirection, out hitinfo, 5000, LayerMask.GetMask(controller.groundContactLayerNames), QueryTriggerInteraction.Ignore);
            float timetoground = currentPath.GetTimeToGround((colliderBottom - hitinfo.point).y);
            predictedLandingSpot = currentPath.GetPosition(timetoground);
        }
    }
}