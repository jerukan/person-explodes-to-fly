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

        public ExplosiveProjectileTrigger trigger;
        public Rigidbody rigidbody;

        // Use this for initialization
        void Start() {
            rigidbody = GetComponent<Rigidbody>();
            rigidbody.useGravity = gravity;
        }

        // Update is called once per frame
        void Update() {

        }
    }
}