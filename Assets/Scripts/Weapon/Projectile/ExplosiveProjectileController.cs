using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using ExplosionJumping.PlayerControl;

namespace ExplosionJumping.Weapon.Projectile {
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(ExplosiveProjectileTrigger))]
    public class ExplosiveProjectileController : MonoBehaviour {

        public PlayerController projectileOwner;
        public float explosionForce;
        public float explosionRadius;
        public bool gravity;
        public float speed;
        public float acceleration;
        public float lifeTime;

        private Rigidbody rigidBody;
        private ExplosiveProjectileTrigger projectileTrigger;
        private ParticleSystem particles;

        // Use this for initialization
        void Start() {
            rigidBody = GetComponent<Rigidbody>();
            rigidBody.useGravity = gravity;
            Physics.IgnoreLayerCollision(9, 10);
            Physics.IgnoreLayerCollision(10, 11);
            projectileTrigger = GetComponent<ExplosiveProjectileTrigger>();
            particles = GetComponent<ParticleSystem>();
            Invoke("Explode", lifeTime);
        }

        // Update is called once per frame
        void Update() {
            
        }

        private void FixedUpdate() {
            rigidBody.AddForce(rigidBody.velocity.normalized * acceleration, ForceMode.Acceleration);
        }

        public void Explode() {
            Vector3 explosionPos = transform.position;
            Collider[] hit = Physics.OverlapSphere(explosionPos, explosionRadius);
            foreach(Collider hitCollider in hit) {
                if(hitCollider.GetComponent<PlayerController>() != null) {
                    Rigidbody hitBody = hitCollider.GetComponent<Rigidbody>();
                    hitBody.AddExplosionForce(explosionForce, explosionPos, explosionRadius, 0f, ForceMode.Impulse);
                }
            }
            particles.Play();
            rigidBody.velocity = Vector3.zero;
            GetComponent<Collider>().enabled = false;
            GetComponentInChildren<MeshRenderer>().enabled = false;
            Destroy(gameObject, particles.main.duration);
        }
    }
}