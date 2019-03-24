using UnityEngine;
using System.Collections;
using ExplosionJumping.Weapon.Projectile;
using ExplosionJumping.PlayerControl;

namespace ExplosionJumping.Weapon {
    public class WeaponController : MonoBehaviour {

        public GameObject projectile;
        public bool firesFromCamera;
        public float fireRatePrimary;
        public float fireRateSecondary;
        public bool fullAuto;

        public Vector3 weaponOffset;

        public KeyCode primaryFireKey = KeyCode.Mouse0;
        public KeyCode secondaryFireKey = KeyCode.Mouse1;

        [HideInInspector] public PlayerController owner;
        private Animator animator;
        private float timeUntilNextShotPrimary;
        private float timeUntilNextShotSecondary;

        // Use this for initialization
        void Start() {
            animator = GetComponentInChildren<Animator>();
        }

        // Update is called once per frame
        void Update() {
            if (timeUntilNextShotPrimary > 0) {
                timeUntilNextShotPrimary -= Time.deltaTime;
            }
            if (timeUntilNextShotSecondary > 0) {
                timeUntilNextShotSecondary -= Time.deltaTime;
            }
            if (GetKeyModified(primaryFireKey) && timeUntilNextShotPrimary <= 0) {
                OnPrimaryFire();
                timeUntilNextShotPrimary = fireRatePrimary;
                if (animator != null) {
                    animator.SetBool("Fired", true);
                }
            }
            else {
                if (animator != null) {
                    animator.SetBool("Fired", false);
                }
            }
            if (GetKeyModified(secondaryFireKey) && timeUntilNextShotSecondary <= 0) {
                OnSecondaryFire();
                timeUntilNextShotSecondary = fireRateSecondary;
            }
        }

        public void OnPrimaryFire() {
            Fire();
        }

        public void OnSecondaryFire() {
        }

        public void Fire() {
            Transform projectileSpawn;
            if (firesFromCamera) {
                projectileSpawn = owner.transform.GetChild(0);
            }
            else {
                projectileSpawn = transform;
            }

            GameObject spawned = Instantiate(projectile, projectileSpawn.position, projectileSpawn.rotation);
            spawned.GetComponent<ExplosiveProjectileController>().projectileOwner = owner;
            spawned.GetComponent<Rigidbody>().velocity = projectileSpawn.transform.forward * spawned.GetComponent<ExplosiveProjectileController>().speed;
        }

        private bool GetKeyModified(KeyCode key) {
            if (fullAuto) {
                return Input.GetKey(key);
            }
            return Input.GetKeyDown(key);
        }
    }
}