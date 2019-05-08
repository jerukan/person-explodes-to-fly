using UnityEngine;
using System.Collections;
using ExplosionJumping.Util;

namespace ExplosionJumping.Weapon {
    [RequireComponent(typeof(WeaponSystem))]
    public class WeaponSystemInput : MonoBehaviour {

        public KeyCode primaryFireButton = KeyCode.Mouse0;
        public KeyCode secondaryFireButton = KeyCode.Mouse1;

        private WeaponSystem weaponSystem;

        private void Awake() {
            weaponSystem = GetComponent<WeaponSystem>();
        }

        // Use this for initialization
        void Start() {

        }

        // Update is called once per frame
        void Update() {
            float scrolled = Input.GetAxis("Mouse ScrollWheel");
            if (scrolled < 0f) {
                weaponSystem.CycleWeapon(true);
            }
            else if (scrolled > 0f) {
                weaponSystem.CycleWeapon(false);
            }
            int numberPressed = InputUtils.GetNumberPressed();
            weaponSystem.SetCurrentWeapon(numberPressed - 1);

            if (weaponSystem.CurrentWeapon != null) {
                if (weaponSystem.CurrentWeapon.GetKeyModified(primaryFireButton)) {
                    weaponSystem.CurrentWeapon.OnPrimaryFire();
                }
                if (weaponSystem.CurrentWeapon.GetKeyModified(secondaryFireButton)) {
                    weaponSystem.CurrentWeapon.OnSecondaryFire();
                }
            }
        }
    }
}