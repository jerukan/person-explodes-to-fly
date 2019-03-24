using UnityEngine;
using System.Collections;

namespace ExplosionJumping.Weapon.Projectile.Explosion {

    [RequireComponent(typeof(ExplosiveProjectileController))]
    public abstract class ExplosionController : MonoBehaviour {

        public float explosionForce;
        public float explosionRadius;

        protected ExplosiveProjectileController projectileController;
        protected Rigidbody rigidBody;
        protected ParticleSystem particles;

        private void Start() {
            projectileController = GetComponent<ExplosiveProjectileController>();
            rigidBody = GetComponent<Rigidbody>();
            particles = GetComponent<ParticleSystem>();
            Invoke("Explode", projectileController.lifeTime);
        }

        public virtual void Explode() { }
    }
}