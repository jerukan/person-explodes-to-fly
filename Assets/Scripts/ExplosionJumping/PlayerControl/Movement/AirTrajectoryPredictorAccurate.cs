using UnityEngine;
using System.Collections;
using ExplosionJumping.Util;
using ExplosionJumping.Util.Paths;

namespace ExplosionJumping.PlayerControl.Movement {

    /// <summary>
    /// Uses an Euler approximation to find where the player will land in complex terrain.
    /// Iteratively raycasts along the trajectory so it's slower.
    /// </summary>
    public class AirTrajectoryPredictorAccurate : MonoBehaviour {
        
        public GameObject landingIndicatorPrefab;
        public float dt, maxIterations;

        private Vector3 predictedLandingSpot, colliderBottom;
        private float gravityMultiplier;
        private CapsuleCollider capsuleCollider; // todo make it work with other colliders maybe
        private Rigidbody rigidBody;
        private bool hasRigidBody;
        private AirPath currentPath;
        private GameObject landingIndicator;
        private RigidbodyFPController controller;

        public Vector3 PredictedLandingSpot {
            get { return predictedLandingSpot; }
        }

        private void Awake() {
            capsuleCollider = GetComponent<CapsuleCollider>();
            rigidBody = GetComponent<Rigidbody>();
            hasRigidBody = rigidBody != null;
            controller = GetComponent<RigidbodyFPController>();
            gravityMultiplier = controller.gravityMultiplier;
        }

        void Start() {
            landingIndicator = Instantiate(landingIndicatorPrefab);
        }

        void FixedUpdate() {
            if (!controller.Grounded) {
                Vector3 colliderWorldCenter = transform.TransformPoint(capsuleCollider.center);
                colliderBottom = new Vector3(colliderWorldCenter.x, colliderWorldCenter.y - capsuleCollider.height / 2, colliderWorldCenter.z);
                if (hasRigidBody) {
                    UpdatePredictedLandingSpot(rigidBody.velocity);
                }
                landingIndicator.transform.position = predictedLandingSpot;
                landingIndicator.transform.Translate(new Vector3(0f, 0.05f, 0f), Space.World); // translate it a bit above so it isn't clipping with the ground
            }
        }

        public void UpdatePredictedLandingSpot(Vector3 velocity) {
            currentPath = new AirPath(colliderBottom, velocity, Physics.gravity * gravityMultiplier);
            RaycastHit hitinfo;
            Vector3 raycastDirection;
            float currentTime = 0f;
            for (int i = 0; i < maxIterations; i++) {
                raycastDirection = currentPath.GetVelocity(currentTime);
                if(Physics.Raycast(currentPath.GetPosition(currentTime), raycastDirection, out hitinfo, raycastDirection.magnitude * dt, LayerMask.GetMask("Default"), QueryTriggerInteraction.Ignore)) {
                    float timetoground = currentPath.GetTimeToGround((colliderBottom - hitinfo.point).y);
                    predictedLandingSpot = currentPath.GetPosition(timetoground);
                    break;
                }
                currentTime += dt;
            }
        }
    }
}