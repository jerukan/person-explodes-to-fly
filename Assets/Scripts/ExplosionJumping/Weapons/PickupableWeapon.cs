using UnityEngine;
using ExplosionJumping.LevelEnvironment;

namespace ExplosionJumping.Weapons {
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