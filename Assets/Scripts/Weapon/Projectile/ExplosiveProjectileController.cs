using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using ExplosionJumping.PlayerControl;
using ExplosionJumping.Weapon.Projectile.Trigger;
using ExplosionJumping.Weapon.Projectile.Explosion;

namespace ExplosionJumping.Weapon.Projectile {
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(ExplosionTrigger))]
    [RequireComponent(typeof(ExplosionController))]
    public class ExplosiveProjectileController : MonoBehaviour {
        [HideInInspector]public PlayerController projectileOwner;
        public bool gravity;
        public float speed;
        public float acceleration;
        public float lifeTime;

        protected Rigidbody rigidBody;

        // Use this for initialization
        void Start() {
            rigidBody = GetComponent<Rigidbody>();
            rigidBody.useGravity = gravity;
        }

        // Update is called once per frame
        void Update() {
            
        }

        private void FixedUpdate() {
            rigidBody.AddForce(rigidBody.velocity.normalized * acceleration, ForceMode.Acceleration);
        }
    }
}