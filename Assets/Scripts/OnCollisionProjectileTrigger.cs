using UnityEngine;

namespace ExplosionJumping {
    public class OnCollisionProjectileTrigger: ExplosiveProjectileTrigger {
        private void OnCollisionEnter(Collision collision) {
            Trigger();
        }
    }
}
