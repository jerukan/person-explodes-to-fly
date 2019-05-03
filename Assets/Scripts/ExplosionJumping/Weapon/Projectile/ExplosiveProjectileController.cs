using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using ExplosionJumping.PlayerControl;
using ExplosionJumping.Weapon.Projectile.Trigger;
using ExplosionJumping.Weapon.Projectile.Explosion;

namespace ExplosionJumping.Weapon.Projectile {
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(ExplosionTrigger))]
    [RequireComponent(typeof(ExplosionController))]
    public class ExplosiveProjectileController : MonoBehaviour {
        [HideInInspector]public PlayerController projectileOwner;
        public int damage;
        public float speed;
        public float acceleration;
        public float lifeTime;
        public bool usesGravity;

        protected Rigidbody rigidBody;

        private void Awake() {
            rigidBody = GetComponent<Rigidbody>();
            rigidBody.useGravity = usesGravity;
        }

        private void FixedUpdate() {
            rigidBody.AddForce(rigidBody.velocity.normalized * acceleration, ForceMode.Acceleration);
        }
    }
}