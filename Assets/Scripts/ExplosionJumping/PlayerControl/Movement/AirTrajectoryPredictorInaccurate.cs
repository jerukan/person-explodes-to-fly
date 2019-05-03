using UnityEngine;
using System.Collections;
using ExplosionJumping.Util;
using ExplosionJumping.Util.Paths;

namespace ExplosionJumping.PlayerControl.Movement {

    /// <summary>
    /// No iterative parts in the prediction.
    /// Inaccurate with complex terrain since it only raycasts once from the player depending on velocity.
    /// </summary>
    public class AirTrajectoryPredictorInaccurate : MonoBehaviour {

        public GameObject landingIndicatorPrefab;

        private Vector3 predictedLandingSpot, colliderBottom;
        private CapsuleCollider capsuleCollider; // todo make it work with other colliders maybe
        private Rigidbody rigidBody;
        private bool hasRigidBody;
        private AirPath currentPath;
        private GameObject landingIndicator;

        public Vector3 PredictedLandingSpot {
            get { return predictedLandingSpot; }
        }

        private void Awake() {
            capsuleCollider = GetComponent<CapsuleCollider>();
            rigidBody = GetComponent<Rigidbody>();
            hasRigidBody = rigidBody != null;
        }

        void Start() {
            landingIndicator = Instantiate(landingIndicatorPrefab);
        }

        void FixedUpdate() {
            Vector3 colliderWorldCenter = transform.TransformPoint(capsuleCollider.center);
            colliderBottom = new Vector3(colliderWorldCenter.x, colliderWorldCenter.y - capsuleCollider.height / 2, colliderWorldCenter.z);
            if(hasRigidBody) {
                UpdatePredictedLandingSpot(rigidBody.velocity);
            }
            landingIndicator.transform.position = predictedLandingSpot;
            landingIndicator.transform.Translate(new Vector3(0f, 0.05f, 0f), Space.World);
        }

        public void UpdatePredictedLandingSpot(Vector3 velocity) {
            currentPath = new AirPath(colliderBottom, velocity);
            RaycastHit hitinfo;
            // attempts to raycast in the direction the collider is heading at, not straight down for multilevel scenes
            Vector3 raycastDirection;
            // if not past the peak, raycast downwards at a velocity predicted a bit after
            if(currentPath.TimeUntilPeak() > -0.5f) {
                raycastDirection = currentPath.GetVelocity(currentPath.TimeUntilPeak() + 0.5f);
            }
            else { // raycast in the direction of the velocity, which is more accurate the closer it gets
                raycastDirection = velocity;
            }
            Debug.DrawRay(transform.position, raycastDirection * 10, Color.magenta);//hi my name is people injurer and i like running over homeless people
            Physics.Raycast(colliderBottom, raycastDirection, out hitinfo, 5000, LayerMask.GetMask("Default"), QueryTriggerInteraction.Ignore);
            float timetoground = currentPath.GetTimeToGround((colliderBottom - hitinfo.point).y);
            //Utils.LogValue("Time until impact", timetoground);
            predictedLandingSpot = currentPath.GetPosition(timetoground);
        }
    }
}