using UnityEngine;

namespace ExplosionJumping.Weapons.Projectiles {
    public class RocketProjectileController : ExplosiveProjectileController {

        public ParticleSystem trailParticles;

        private void OnCollisionEnter(Collision collision) {
            trailParticles.Stop();
        }
    }
}