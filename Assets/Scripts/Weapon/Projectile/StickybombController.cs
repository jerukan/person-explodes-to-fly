using UnityEngine;
using System.Collections;

namespace ExplosionJumping.Weapon.Projectile {
    public class StickybombController : ExplosiveProjectileController {

        void OnCollisionEnter(Collision collision) {
            rigidBody.velocity = Vector3.zero;
            rigidBody.angularVelocity = Vector3.zero;
            rigidBody.Sleep();
        }
    }
}