using UnityEngine;
using System.Collections;
using ExplosionJumping.Weapon.Projectile.Explosion;

namespace ExplosionJumping.Weapon.Projectile.Trigger {
    [RequireComponent(typeof(ExplosiveProjectileController))]
    public class ExplosionTrigger : MonoBehaviour {

        protected ExplosionController explosionController;

        // Use this for initialization
        private void Start() {
            explosionController = GetComponent<ExplosionController>();
        }

        // Update is called once per frame
        private void Update() {

        }

        protected void Trigger() {
            explosionController.Explode();
        }
    }
}