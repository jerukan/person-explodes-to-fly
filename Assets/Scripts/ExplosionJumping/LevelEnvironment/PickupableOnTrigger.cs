using UnityEngine;
using System.Collections;
using UnityEngine.Events;

namespace ExplosionJumping.LevelEnvironment {
    [RequireComponent(typeof(Collider))]
    public abstract class PickupableOnTrigger : MonoBehaviour, IPickupable {

        public UnityEvent invokeOnSuccessfulPickup;
        
        public abstract bool OnPickup(GameObject pickedUpBy);

        private void OnTriggerEnter(Collider other) {
            if (OnPickup(other.gameObject)) {
                invokeOnSuccessfulPickup.Invoke();
                Destroy(gameObject);
            }
        }
    }
}