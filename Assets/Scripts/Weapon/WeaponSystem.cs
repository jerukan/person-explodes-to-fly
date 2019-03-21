using UnityEngine;
using System.Collections;
using ExplosionJumping.PlayerControl;

namespace ExplosionJumping.Weapon {
    public class WeaponSystem : MonoBehaviour {

        public WeaponController[] weaponPrefabs;

        private WeaponController[] weapons;
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
                weapons[i].gameObject.SetActive(false);
                weapons[i].transform.SetParent(GetComponentInChildren<Camera>().transform);
                weapons[i].transform.localPosition = weapons[i].weaponOffset;
            }
            currentWeaponIndex = 0;
            CurrentWeapon.gameObject.SetActive(true);
        }

        // Update is called once per frame
        void Update() {
            float scrolled = Input.GetAxis("Mouse ScrollWheel");
            if(scrolled < 0f) {
                CycleWeapon(true);
            } else if(scrolled > 0f) {
                CycleWeapon(false);
            }
        }

        public void CycleWeapon(bool backward) {
            int prevWeapon = currentWeaponIndex;
            if(backward) {
                currentWeaponIndex--;
                if(currentWeaponIndex < 0) {
                    currentWeaponIndex = weapons.Length - 1;
                }
                weapons[prevWeapon].gameObject.SetActive(false);
                weapons[currentWeaponIndex].gameObject.SetActive(true);
            } else {
                currentWeaponIndex++;
                if(currentWeaponIndex >= weapons.Length) {
                    currentWeaponIndex = 0;
                }
                weapons[prevWeapon].gameObject.SetActive(false);
                weapons[currentWeaponIndex].gameObject.SetActive(true);
            }
        }

        public void SetWeapon(int index) {
            if (index >= 0 && index < weapons.Length) {
                currentWeaponIndex = index;
            }
        }
    }
}