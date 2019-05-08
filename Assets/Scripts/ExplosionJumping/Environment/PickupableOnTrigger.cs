using UnityEngine;
using System.Collections;

namespace ExplosionJumping.Environment {
    [RequireComponent(typeof(Collider))]
    public abstract class PickupableOnTrigger : MonoBehaviour, IPickupable {
        
        public abstract void OnPickup(GameObject pickedUpBy);

        private void OnTriggerEnter(Collider other) {
            OnPickup(other.gameObject);
        }
    }
}