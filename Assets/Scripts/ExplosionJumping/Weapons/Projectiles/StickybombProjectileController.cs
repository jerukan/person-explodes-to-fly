using UnityEngine;

namespace ExplosionJumping.Weapons.Projectiles {
    public class StickybombProjectileController : ExplosiveProjectileController {

        void OnCollisionEnter(Collision collision) {
            rigidBody.velocity = Vector3.zero;
            rigidBody.angularVelocity = Vector3.zero;
            rigidBody.Sleep();
        }
    }
}