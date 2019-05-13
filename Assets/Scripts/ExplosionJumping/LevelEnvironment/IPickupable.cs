using UnityEngine;

namespace ExplosionJumping.LevelEnvironment {
    public interface IPickupable {

        bool OnPickup(GameObject pickedUpBy);
    }
}
