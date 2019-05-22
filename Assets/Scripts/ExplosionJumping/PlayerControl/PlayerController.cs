using ExplosionJumping.PlayerControl.PlayerInput;
using ExplosionJumping.Weapons;
using ExplosionJumping.Weapons.Projectiles;
using UnityEngine;

namespace ExplosionJumping.PlayerControl {
    [RequireComponent(typeof(FirstPersonControllerCustom))]
    public class PlayerController : MonoBehaviour {

        public int maxHealth;
        [Tooltip("Health regenerated per second.")]
        public int healthRegen;
        [Tooltip("When the player is hit by an enemy, health regen will be disabled for this time.")]
        public float healthRegenDelay;

        private FirstPersonControllerCustom charController;
        private WeaponSystem weaponSystem;
        private Rigidbody rigidBody;
        private float currentHealth;
        public float CurrentHealth {
            get {
                return currentHealth;
            }
        }
        private bool healthRegenDisabled;
        private float timeLastDamaged;
        private float timeLastHitByEnemy;
        private bool dead;
        public bool Dead {
            get {
                return dead;
            }
        }
        private bool explosiveJumping;
        public bool ExplosiveJumping {
            get {
                return explosiveJumping;
            }
        }

        private void Awake() {
            charController = GetComponent<FirstPersonControllerCustom>();
            weaponSystem = GetComponent<WeaponSystem>();
            rigidBody = GetComponent<Rigidbody>();
            currentHealth = maxHealth;
        }

        private void Update() {
            Transform camTransform = charController.head.cam.transform;

            if (!dead) {
                if(Input.GetKeyDown(KeyCode.LeftShift)) {
                    rigidBody.AddForce(camTransform.forward * 20f, ForceMode.VelocityChange);
                }
                if (!healthRegenDisabled) {
                    AddHealth(healthRegen * Time.deltaTime);
                } else {
                    if(Time.time - timeLastHitByEnemy >= healthRegenDelay) {
                        healthRegenDisabled = false;
                    }
                }
                if(explosiveJumping && charController.Grounded) {
                    explosiveJumping = false;
                }
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
            timeLastDamaged = Time.time;
            if(projectileController.projectileOwner.GetInstanceID() != GetInstanceID()) {
                healthRegenDisabled = true;
                timeLastHitByEnemy = Time.time;
            }
            explosiveJumping = true;
        }

        public void SetAlive(bool alive) {
            if(alive && dead) {
                currentHealth = maxHealth;
            }
            Ragdoll(!alive);
            dead = !alive;
            if (dead) {
                charController.head.cam.transform.localPosition = new Vector3(0f, 0f, -5f);
                charController.head.cam.cullingMask |= 1 << LayerMask.NameToLayer("PlayerModel");
            }
            else {
                charController.head.cam.transform.localPosition = Vector3.zero;
                charController.head.ResetHeadRotation();
                charController.head.cam.cullingMask &= ~(1 << LayerMask.NameToLayer("PlayerModel"));
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