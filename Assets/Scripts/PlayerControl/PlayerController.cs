﻿using ExplosionJumping.PlayerControl.Movement;
using ExplosionJumping.Util;
using ExplosionJumping.Weapon;
using ExplosionJumping.Weapon.Projectile;
using UnityEngine;
using UnityEngine.UI;

namespace ExplosionJumping.PlayerControl {
    [RequireComponent(typeof(RigidbodyFPControllerCustom))]
    [RequireComponent(typeof(PlayerUIManager))]
    public class PlayerController : MonoBehaviour {

        public int maxHealth;
        [Tooltip("Health regenerated per second.")]
        public int healthRegen;
        public Camera deathCamera;

        private RigidbodyFPControllerCustom charController;
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
            charController = GetComponent<RigidbodyFPControllerCustom>();
            weaponSystem = GetComponent<WeaponSystem>();
            rigidBody = GetComponent<Rigidbody>();
            currentHealth = maxHealth;
            deathCamera.enabled = false;
        }

        private void Update() {
            Transform camTransform = charController.cam.transform;
            if(Input.GetKeyDown(KeyCode.LeftShift)) {
                // todo make it not modified by vertical tilt
                rigidBody.AddForce(camTransform.forward * 40, ForceMode.VelocityChange);
            }

            if (!dead) {
                AddHealth(healthRegen * Time.deltaTime);
            } else {
                deathCamera.transform.position = transform.position - charController.cam.transform.forward * 4;
            }
        }

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
            Ragdoll(!alive);
            dead = !alive;
            deathCamera.enabled = !alive;
            charController.cam.enabled = alive;
            deathCamera.transform.eulerAngles = charController.cam.transform.eulerAngles;
        }

        public void Ragdoll(bool shouldRagdoll) {
            charController.enabled = !shouldRagdoll; // disables player input and movement
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