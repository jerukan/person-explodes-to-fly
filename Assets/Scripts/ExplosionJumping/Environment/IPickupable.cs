using UnityEngine;

namespace ExplosionJumping.Environment {
    public interface IPickupable {

        void OnPickup(GameObject pickedUpBy);
    }
}
