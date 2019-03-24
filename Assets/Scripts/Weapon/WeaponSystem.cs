using UnityEngine;
using System.Collections;
using ExplosionJumping.PlayerControl;
using ExplosionJumping.Util;

namespace ExplosionJumping.Weapon {
    public class WeaponSystem : MonoBehaviour {

        public WeaponController[] weaponPrefabs;

        private WeaponController[] weapons;
        private int prevWeaponIndex;
        private int currentWeaponIndex;
        public WeaponController CurrentWeapon {
            get { return weapons[currentWeaponIndex]; }
        }

        // Use this for initialization
        void Start() {
            weapons = new WeaponController[weaponPrefabs.Length];
            for (int i = 0; i < weaponPrefabs.Length; i++) {
                GameObject weaponInstance = Instantiate(weaponPrefabs[i].gameObject);
                weapons[i] = weaponInstance.GetComponent<WeaponController>();
                weapons[i].owner = GetComponent<PlayerController>();
                ToggleWeapon(i, false);
                weapons[i].transform.SetParent(GetComponentInChildren<Camera>().transform);
                weapons[i].transform.localPosition = weapons[i].weaponOffset;
            }
            currentWeaponIndex = 0;
            ToggleWeapon(0, true);
        }

        // Update is called once per frame
        void Update() {
            float scrolled = Input.GetAxis("Mouse ScrollWheel");
            if(scrolled < 0f) {
                CycleWeapon(true);
            } else if(scrolled > 0f) {
                CycleWeapon(false);
            }
            int numberPressed = Utils.GetNumberPressed();
            SetCurrentWeapon(numberPressed - 1);
        }

        public void CycleWeapon(bool backward) {
            int wanted;
            if(backward) {
                wanted = currentWeaponIndex - 1;
                if(wanted < 0) {
                    wanted = weapons.Length - 1;
                }
                SetCurrentWeapon(wanted);
            } else {
                wanted = currentWeaponIndex + 1;
                if(wanted >= weapons.Length) {
                    wanted = 0;
                }
                SetCurrentWeapon(wanted);
            }
        }

        public void SetCurrentWeapon(int index) {
            if (index >= 0 && index < weapons.Length && index != currentWeaponIndex) {
                prevWeaponIndex = currentWeaponIndex;
                currentWeaponIndex = index;
                ToggleWeapon(prevWeaponIndex, false);
                ToggleWeapon(currentWeaponIndex, true);
            }
        }

        private void ToggleWeapon(int index, bool shouldEnable) {
            var meshRenderers = weapons[index].GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer mr in meshRenderers) {
                mr.enabled = shouldEnable;
            }
            weapons[index].GetComponent<WeaponController>().enabled = shouldEnable;
        }
    }
}