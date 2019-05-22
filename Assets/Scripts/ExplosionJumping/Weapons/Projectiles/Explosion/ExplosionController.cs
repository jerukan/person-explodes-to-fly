using UnityEngine;

namespace ExplosionJumping.Weapons.Projectiles.Explosion {

    [RequireComponent(typeof(ExplosiveProjectileController))]
    public abstract class ExplosionController : MonoBehaviour {

        public float explosionForce;
        public float explosionRadius;

        protected ExplosiveProjectileController projectileController;
        protected Rigidbody rigidBody;

        private void Awake() {
            projectileController = GetComponent<ExplosiveProjectileController>();
            rigidBody = GetComponent<Rigidbody>();
        }

        private void Start() {
            Invoke("Explode", projectileController.lifeTime);
        }

        public virtual void Explode() { }
    }
}