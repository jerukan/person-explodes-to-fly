using UnityEngine;
using System.Collections;

namespace ExplosionJumping.Weapon.Projectile.Trigger {
    public class ManualDetonationTrigger : ExplosionTrigger {

        public KeyCode detonationKey = KeyCode.Mouse1;

        // Update is called once per frame
        void Update() {
            if(Input.GetKey(detonationKey)) {
                Trigger();
            }
        }
    }
}