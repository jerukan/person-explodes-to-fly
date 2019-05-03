using UnityEngine;
using System.Collections;

namespace ExplosionJumping.Weapon.Projectile {
    public class RocketProjectileController : ExplosiveProjectileController {

        public ParticleSystem trailParticles;

        private void OnCollisionEnter(Collision collision) {
            trailParticles.Stop();
        }
    }
}