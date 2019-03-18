using UnityEngine;

namespace ExplosionJumping {
    public class ProjectileTriggerRocket: ExplosiveProjectileTrigger {
        private void OnCollisionEnter(Collision collision) {
            Trigger();
        }
    }
}
