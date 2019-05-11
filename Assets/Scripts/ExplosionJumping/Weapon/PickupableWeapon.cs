using UnityEngine;
using System.Collections;
using ExplosionJumping.Environment;

namespace ExplosionJumping.Weapon {
    public class PickupableWeapon : PickupableOnTrigger {

        public WeaponController associatedWeapon;

        public override bool OnPickup(GameObject pickedUpBy) {
            WeaponSystem weaponSystem = pickedUpBy.GetComponent<WeaponSystem>();
            if (weaponSystem != null) {
                return weaponSystem.AddWeapon(associatedWeapon);
            }
            return false;
        }
    }
}