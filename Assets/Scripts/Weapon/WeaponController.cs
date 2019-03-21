﻿using UnityEngine;
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

        [HideInInspector]public PlayerController owner;
        private float timeUntilNextShotPrimary;
        private float timeUntilNextShotSecondary;

        // Use this for initialization
        void Start() {

        }

        // Update is called once per frame
        void Update() {
            if(timeUntilNextShotPrimary > 0) {
                timeUntilNextShotPrimary -= Time.deltaTime;
            }
            if(timeUntilNextShotSecondary > 0) {
                timeUntilNextShotSecondary -= Time.deltaTime;
            }
            if(fullAuto) {
                if(Input.GetKey(primaryFireKey) && timeUntilNextShotPrimary <= 0) {
                    OnPrimaryFire();
                    timeUntilNextShotPrimary = fireRatePrimary;
                }
                if(Input.GetKey(secondaryFireKey) && timeUntilNextShotSecondary <= 0) {
                    OnSecondaryFire();
                    timeUntilNextShotSecondary = fireRateSecondary;
                }
            } else {
                if(Input.GetKeyDown(primaryFireKey) && timeUntilNextShotPrimary <= 0) {
                    OnPrimaryFire();
                    timeUntilNextShotPrimary = fireRatePrimary;
                }
                if (Input.GetKeyDown(secondaryFireKey) && timeUntilNextShotSecondary <= 0) {
                    OnSecondaryFire();
                    timeUntilNextShotSecondary = fireRateSecondary;
                }
            }
        }

        public void OnPrimaryFire() {
            Fire();
        }

        public void OnSecondaryFire() {
        }

        public void Fire() {
            Transform projectileSpawn;
            if(firesFromCamera) {
                projectileSpawn = owner.transform.GetChild(0);
            } else {
                projectileSpawn = transform;
            }

            GameObject spawned = Instantiate(projectile, projectileSpawn.position, projectileSpawn.rotation);
            spawned.GetComponent<ExplosiveProjectileController>().projectileOwner = owner;
            spawned.GetComponent<Rigidbody>().velocity = projectileSpawn.transform.forward * spawned.GetComponent<ExplosiveProjectileController>().speed;
        }
    }
}