using ExplosionJumping.PlayerControl.Movement;
using ExplosionJumping.Util;
using ExplosionJumping.Weapon;
using ExplosionJumping.Weapon.Projectile;
using UnityEngine;

namespace ExplosionJumping.PlayerControl {
    [RequireComponent(typeof(RigidbodyFPController))]
    public class PlayerController : MonoBehaviour {

        public int maxHealth;
        [Tooltip("Health regenerated per second.")]
        public int healthRegen;

        private RigidbodyFPController charController;
        private WeaponSystem weaponSystem;
        private Rigidbody rigidBody;
        private float currentHealth;
        private bool dead;

        public float CurrentHealth {
            get {
                return currentHealth;
            }
        }

        public bool Dead {
            get {
                return dead;
            }
        }

        private void Awake() {
            charController = GetComponent<RigidbodyFPController>();
            weaponSystem = GetComponent<WeaponSystem>();
            rigidBody = GetComponent<Rigidbody>();
            currentHealth = maxHealth;
        }

        private void Update() {
            Transform camTransform = charController.cam.transform;

            if (!dead) {
                AddHealth(healthRegen * Time.deltaTime);
            } else {
                // dead
            }
        }

        /// <summary>
        /// The method to modify health while the player is in-game and alive.
        /// This should be used over setting the current health directly.
        /// </summary>
        public void AddHealth(float amount) {
            currentHealth += amount;
            if(currentHealth > maxHealth) {
                currentHealth = maxHealth;
            } else if(currentHealth <= 0f) {
                currentHealth = 0f;
                SetAlive(false);
            }
        }

        public void TakeDamageFromExplosion(Vector3 explosionPos, float explosionRadius, ExplosiveProjectileController projectileController) {
            AddHealth(-(explosionRadius - (explosionPos - GetComponent<CapsuleCollider>().ClosestPoint(explosionPos)).magnitude) / explosionRadius * projectileController.damage);
        }

        public void SetAlive(bool alive) {
            if(alive && dead) {
                currentHealth = maxHealth;
            }
            Ragdoll(!alive);
            dead = !alive;
            if (dead) {
                charController.cam.transform.localPosition = new Vector3(0f, 0f, -5f);
                //charController.head.cameraLook.Init(transform, charController.head.transform);
                charController.cam.cullingMask |= 1 << LayerMask.NameToLayer("PlayerModel");
            }
            else {
                charController.cam.transform.localPosition = Vector3.zero;
                charController.head.ResetHeadRotation();
                charController.cam.cullingMask &= ~(1 << LayerMask.NameToLayer("PlayerModel"));
            }
        }

        private void Ragdoll(bool shouldRagdoll) {
            charController.enabled = !shouldRagdoll; // disables player input and movement
            if(GetComponent<PlayerMovementInput>() != null) {
                GetComponent<PlayerMovementInput>().enabled = !shouldRagdoll;
            }
            rigidBody.useGravity = shouldRagdoll;
            if (shouldRagdoll) {
                rigidBody.constraints = RigidbodyConstraints.None;
                rigidBody.drag = 1f;
                rigidBody.angularDrag = 1f;
            } else {
                rigidBody.constraints = RigidbodyConstraints.FreezeRotation;
                rigidBody.drag = 0f;
                rigidBody.angularDrag = 0f;
                rigidBody.transform.eulerAngles = new Vector3(0f, 0f, 0f);
            }
        }
    }
}