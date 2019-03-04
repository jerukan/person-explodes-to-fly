using UnityEngine;
using System.Collections;
using UnityEngine.Events;

namespace ExplosionJumping {
    [RequireComponent(typeof(Rigidbody))]
    public class ExplosiveProjectileController : MonoBehaviour {

        public float explosionForce;
        public float explosionRadius;
        public bool gravity;
        public float speed;
        public float lifeTime;

        public ExplosiveProjectileTrigger trigger;
        public Rigidbody rigidbody;

        private float startTime;

        // Use this for initialization
        void Start() {
            rigidbody = GetComponent<Rigidbody>();
            rigidbody.useGravity = gravity;

            startTime = Time.time;
        }

        // Update is called once per frame
        void Update() {
            if(Time.time - startTime >= lifeTime) {
                Destroy(gameObject);
            }
        }
    }
}