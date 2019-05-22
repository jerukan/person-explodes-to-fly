using UnityEngine;
using ExplosionJumping.PlayerControl;
using ExplosionJumping.Weapons.Projectiles.Trigger;
using ExplosionJumping.Weapons.Projectiles.Explosion;

namespace ExplosionJumping.Weapons.Projectiles {
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
        public bool affectedByOtherExplosions;

        protected Rigidbody rigidBody;

        private void Awake() {
            rigidBody = GetComponent<Rigidbody>();
            rigidBody.useGravity = usesGravity;
        }

        private void FixedUpdate() {
            rigidBody.AddForce(rigidBody.velocity.normalized * acceleration, ForceMode.Acceleration);
        }

        public void Init(PlayerController owner, Vector3 direction) {
            projectileOwner = owner;
            rigidBody.velocity = direction * speed;
        }
    }
}