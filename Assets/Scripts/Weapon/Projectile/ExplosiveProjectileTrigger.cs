using UnityEngine;
using System.Collections;

namespace ExplosionJumping.Weapon.Projectile {
    [RequireComponent(typeof(ExplosiveProjectileController))]
    public class ExplosiveProjectileTrigger : MonoBehaviour {

        protected ExplosiveProjectileController projectileController;

        // Use this for initialization
        private void Start() {
            projectileController = GetComponent<ExplosiveProjectileController>();
        }

        // Update is called once per frame
        private void Update() {

        }

        protected void Trigger() {
            projectileController.Explode();
        }
    }
}