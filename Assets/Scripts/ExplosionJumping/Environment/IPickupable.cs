using UnityEngine;

namespace ExplosionJumping.Environment {
    public interface IPickupable {

        bool OnPickup(GameObject pickedUpBy);
    }
}
