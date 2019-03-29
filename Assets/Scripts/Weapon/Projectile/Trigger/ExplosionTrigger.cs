using UnityEngine;
using System.Collections;
using ExplosionJumping.Weapon.Projectile.Explosion;

namespace ExplosionJumping.Weapon.Projectile.Trigger {
    [RequireComponent(typeof(ExplosiveProjectileController))]
    public class ExplosionTrigger : MonoBehaviour {

        protected ExplosionController explosionController;
        private bool triggered;

        private void Awake() {
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