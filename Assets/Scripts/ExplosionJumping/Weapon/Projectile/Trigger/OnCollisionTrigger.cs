using UnityEngine;

namespace ExplosionJumping.Weapon.Projectile.Trigger {
    public class OnCollisionTrigger: ExplosionTrigger {
        private void OnCollisionEnter(Collision collision) {
            Trigger();
        }
    }
}
