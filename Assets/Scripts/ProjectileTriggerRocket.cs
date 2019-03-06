using UnityEngine;

namespace ExplosionJumping {
    public class ProjectileTriggerRocket: ExplosiveProjectileTrigger {
        private void OnCollisionEnter(Collision collision) {
            //if (collision.gameObject.GetInstanceID() != GetComponent<ExplosiveProjectileController>().projectileOwner.gameObject.GetInstanceID()) {
                Trigger();
            //}
        }
    }
}
