using UnityEngine;
using System.Collections;
using ExplosionJumping.PlayerControl;

namespace ExplosionJumping.Weapon.Projectile {
    public class StickybombProjectileController : ExplosiveProjectileController {

        void OnCollisionEnter(Collision collision) {
            rigidBody.velocity = Vector3.zero;
            rigidBody.angularVelocity = Vector3.zero;
            rigidBody.Sleep();
        }
    }
}