using UnityEngine;
using System.Collections;
using ExplosionJumping.Weapon.Projectile;
using ExplosionJumping.PlayerControl;

namespace ExplosionJumping.Weapon {
    public class WeaponController : MonoBehaviour {

        public string weaponName;
        public GameObject primaryProjectile;
        public bool firesFromCamera;
        [Tooltip("The seconds between each shot.")]
        public float fireRatePrimary;
        public float fireRateSecondary;
        public bool fullAuto;

        public Vector3 weaponOffset;

        [HideInInspector] public PlayerController owner;
        private Animator animator;
        private bool primaryFired;
        private bool secondaryFired;
        private float timeUntilNextShotPrimary;
        private float timeUntilNextShotSecondary;

        void Awake() {
            animator = GetComponentInChildren<Animator>();
        }

        void Update() {
            if (timeUntilNextShotPrimary > 0) {
                timeUntilNextShotPrimary -= Time.deltaTime;
            }
            if (timeUntilNextShotSecondary > 0) {
                timeUntilNextShotSecondary -= Time.deltaTime;
            }
            if (timeUntilNextShotPrimary > 0) {                
                if (animator != null && !primaryFired) {
                    animator.SetBool("Fired", false);
                } else {
                    primaryFired = false;
                }
            }
            if (timeUntilNextShotSecondary > 0) {
                if (animator != null && !secondaryFired) {
                    animator.SetBool("Fired", false);
                }
                else {
                    secondaryFired = false;
                }
            }
        }

        public void OnPrimaryFire() {
            if (timeUntilNextShotPrimary <= 0) {
                Fire(primaryProjectile);
                timeUntilNextShotPrimary = fireRatePrimary;
                primaryFired = true;
                if (animator != null) {
                    animator.SetBool("Fired", true);
                }
            }
        }

        public void OnSecondaryFire() {
        }

        public void Fire(GameObject projectileToSpawn) {
            Transform projectileSpawn;
            if (firesFromCamera) {
                projectileSpawn = owner.transform.GetChild(0);
            }
            else {
                projectileSpawn = transform;
            }

            GameObject spawned = Instantiate(projectileToSpawn, projectileSpawn.position, projectileSpawn.rotation);
            spawned.GetComponent<ExplosiveProjectileController>().Init(owner, projectileSpawn.transform.forward);
        }

        public bool GetKeyModified(KeyCode key) {
            if (fullAuto) {
                return Input.GetKey(key);
            }
            return Input.GetKeyDown(key);
        }
    }
}