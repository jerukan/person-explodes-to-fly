using ExplosionJumping.PlayerControl.Movement;
using ExplosionJumping.Weapon;
using ExplosionJumping.Weapon.Projectile;
using UnityEngine;
using UnityEngine.UI;

namespace ExplosionJumping.PlayerControl {
    [RequireComponent(typeof(RigidbodyFPControllerCustom))]
    public class PlayerController : MonoBehaviour {

        public int maxHealth;
        [Tooltip("Health regenerated per second.")]
        public int healthRegen;
        public Slider healthSlider;

        private RigidbodyFPControllerCustom charController;
        private float currentHealth;

        private void Awake() {
            charController = GetComponent<RigidbodyFPControllerCustom>();
            healthSlider.minValue = 0;
            healthSlider.maxValue = maxHealth;
            currentHealth = maxHealth;
        }

        private void Update() {
            Transform camTransform = Camera.main.transform;
            if(Input.GetKeyDown(KeyCode.LeftShift)) {
                // todo make it not modified by vertical tilt
                charController.GetComponent<Rigidbody>().AddForce(camTransform.forward * 40, ForceMode.VelocityChange);
            }

            AddHealth(healthRegen * Time.deltaTime);
            healthSlider.value = currentHealth;
        }

        public void AddHealth(float amount) {
            currentHealth += amount;
            if(currentHealth > maxHealth) {
                currentHealth = maxHealth;
            } else if(currentHealth < 0f) {
                currentHealth = 0f;
            }
        }
    }
}