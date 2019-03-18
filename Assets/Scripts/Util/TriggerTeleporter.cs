using UnityEngine;

namespace ExplosionJumping.Util{
    [RequireComponent(typeof(Collider))]
    public class TriggerTeleporter : MonoBehaviour {

        public Transform targetTransform;

        // Use this for initialization
        void Start() {

        }

        // Update is called once per frame
        void Update() {

        }

        private void OnTriggerEnter(Collider other) {
            if (other.GetComponent<PlayerController>() != null) {
                other.transform.position = targetTransform.position;
            }
        }
    }
}