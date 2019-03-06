using UnityEngine;
using System.Collections;
using UnityEngine.Events;

namespace ExplosionJumping {
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(ExplosiveProjectileTrigger))]
    public class ExplosiveProjectileController : MonoBehaviour {

        public PlayerController projectileOwner;
        public float explosionForce;
        public float explosionRadius;
        public bool gravity;
        public float speed;
        public float lifeTime;

        private Rigidbody rigidBody;
        private ExplosiveProjectileTrigger projectileTrigger;

        // Use this for initialization
        void Start() {
            rigidBody = GetComponent<Rigidbody>();
            rigidBody.useGravity = gravity;
            Physics.IgnoreLayerCollision(9, 10);
            projectileTrigger = GetComponent<ExplosiveProjectileTrigger>();
            Invoke("Explode", lifeTime);
        }

        // Update is called once per frame
        void Update() {
            
        }

        public void Explode() {
            Vector3 explosionPos = transform.position;
            Collider[] hit = Physics.OverlapSphere(explosionPos, explosionRadius);
            foreach(Collider hitCollider in hit) {
                if(hitCollider.GetComponent<PlayerController>() != null) {
                    Rigidbody hitBody = hitCollider.GetComponent<Rigidbody>();
                    hitBody.AddExplosionForce(explosionForce, explosionPos, explosionRadius, 0f);
                }
            }
            Destroy(gameObject);
        }
    }
}