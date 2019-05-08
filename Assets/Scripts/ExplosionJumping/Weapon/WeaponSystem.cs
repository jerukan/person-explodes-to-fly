using UnityEngine;
using System.Collections;
using ExplosionJumping.PlayerControl;
using ExplosionJumping.Util;
using ExplosionJumping.PlayerControl.Movement;
using System.Collections.Generic;

namespace ExplosionJumping.Weapon {
    [RequireComponent(typeof(PlayerController))]
    public class WeaponSystem : MonoBehaviour {

        public int maxWeapons;
        public WeaponController[] weaponPrefabs;

        private WeaponController[] weapons;
        private int prevWeaponIndex;
        private int currentWeaponIndex;
        public WeaponController CurrentWeapon {
            get { return weapons[currentWeaponIndex]; }
        }

        private void Awake() {
            weapons = new WeaponController[maxWeapons];
        }

        void Start() {
            for (int i = 0; i < weaponPrefabs.Length; i++) {
                AddWeapon(weaponPrefabs[i]);
            }
        }

        void Update() {

        }

        public void CycleWeapon(bool backward) {
            int wanted;
            if (backward) {
                wanted = currentWeaponIndex - 1;
                if (wanted < 0) {
                    wanted = weapons.Length - 1;
                }
                SetCurrentWeapon(wanted);
            }
            else {
                wanted = currentWeaponIndex + 1;
                if (wanted >= weapons.Length) {
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
            if (weapons[index] == null) {
                return;
            }
            var meshRenderers = weapons[index].GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer mr in meshRenderers) {
                mr.enabled = shouldEnable;
            }
        }

        public bool AddWeapon(WeaponController weapon) {
            GameObject weaponObject = weapon.gameObject;
            int indexToAdd = -1;
            for (int i = 0; i < weapons.Length; i++) {
                if (weapons[i] != null) {
                    if (weapons[i].weaponName == weapon.weaponName) {
                        return false;
                    }
                }
                else {
                    Debug.Log(weapons[i]);
                    indexToAdd = i;
                    break;
                }
            }
            if (indexToAdd == -1) {
                return false;
            }

            GameObject weaponInstance = Instantiate(weaponObject);
            weapons[indexToAdd] = weaponInstance.GetComponent<WeaponController>();
            weapons[indexToAdd].owner = GetComponent<PlayerController>();
            weapons[indexToAdd].transform.SetParent(GetComponentInChildren<PlayerHead>().transform);
            weapons[indexToAdd].transform.localPosition = weapons[indexToAdd].weaponOffset;
            weapons[indexToAdd].transform.localEulerAngles = Vector3.zero;

            if(indexToAdd == currentWeaponIndex) {
                SetCurrentWeapon(indexToAdd);
            } else {
                ToggleWeapon(indexToAdd, false);
            }

            return true;
        }
    }
}