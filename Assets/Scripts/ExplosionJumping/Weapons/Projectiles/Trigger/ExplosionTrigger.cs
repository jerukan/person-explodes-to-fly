using UnityEngine;
using ExplosionJumping.Weapons.Projectiles.Explosion;

namespace ExplosionJumping.Weapons.Projectiles.Trigger {
    [RequireComponent(typeof(ExplosiveProjectileController))]
    public class ExplosionTrigger : MonoBehaviour {

        protected ExplosiveProjectileController projectileController;
        protected ExplosionController explosionController;
        private bool triggered;

        private void Awake() {
            projectileController = GetComponent<ExplosiveProjectileController>();
            explosionController = GetComponent<ExplosionController>();
        }

        protected void Trigger() {
            if (!triggered) {
                triggered = true;
                explosionController.Explode();
            }
        }
    }
}