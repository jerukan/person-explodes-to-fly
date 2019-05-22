using UnityEngine;

namespace ExplosionJumping.Weapons.Projectiles.Trigger {
    public class OnCollisionTrigger: ExplosionTrigger {
        private void OnCollisionEnter(Collision collision) {
            Trigger();
        }
    }
}
