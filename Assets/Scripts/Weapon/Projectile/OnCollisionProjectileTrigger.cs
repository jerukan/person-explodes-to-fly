using UnityEngine;

namespace ExplosionJumping.Weapon.Projectile {
    [RequireComponent(typeof(ExplosiveProjectileController))]
    public class OnCollisionProjectileTrigger: ExplosiveProjectileTrigger {
        private void OnCollisionEnter(Collision collision) {
            Trigger();
        }
    }
}
