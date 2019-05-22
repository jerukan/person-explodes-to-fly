using UnityEngine;

namespace ExplosionJumping.Weapons.Projectiles.Trigger {
    public class ManualDetonationTrigger : ExplosionTrigger {

        [Tooltip("Time before the projectile is able to be triggered in seconds.")]
        public float armTime;
        public KeyCode detonationKey = KeyCode.Mouse1;
        private float creationTime;

        private void Start() {
            creationTime = Time.time;
        }

        // Update is called once per frame
        void Update() {
            if(Input.GetKey(detonationKey) && Time.time - creationTime >= armTime) {
                Trigger();
            }
        }
    }
}