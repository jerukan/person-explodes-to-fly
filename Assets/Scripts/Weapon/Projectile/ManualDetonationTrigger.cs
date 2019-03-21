using UnityEngine;
using System.Collections;

namespace ExplosionJumping.Weapon.Projectile {
    public class ManualDetonationTrigger : ExplosiveProjectileTrigger {

        // Update is called once per frame
        void Update() {
            if(Input.GetKey(KeyCode.Mouse1)) {
                Trigger();
            }
        }
    }
}