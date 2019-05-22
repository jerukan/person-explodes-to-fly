using ExplosionJumping.PlayerControl.CameraControl;
using UnityEngine;

namespace ExplosionJumping.PlayerControl {
    [RequireComponent(typeof(PlayerController))]
    public class PlayerRespawner : MonoBehaviour {

        public Transform respawnPosition;
        public float respawnTime;
        private float timeOfDeath;
        private bool prevState;
        private PlayerController playerController;
        private FirstPersonControllerCustom charController;

        void Awake() {
            playerController = GetComponent<PlayerController>();
            charController = GetComponent<FirstPersonControllerCustom>();
        }

        void Update() {
            if(!prevState && playerController.Dead) {
                timeOfDeath = Time.time;
            }
            if(prevState) {
                if (Time.time - timeOfDeath >= respawnTime) {
                    GetComponentInChildren<SmootherMouseLook>().shouldRotateCharacterBody = true;
                    playerController.SetAlive(true);
                    playerController.transform.position = respawnPosition.position;
                }
                else {
                    if (!(Mathf.Abs(Time.timeScale) < float.Epsilon)) {
                        GetComponentInChildren<SmootherMouseLook>().shouldRotateCharacterBody = false;
                    }
                }
            }
            prevState = playerController.Dead;
        }

        public float TimeUntilRespawn() {
            return timeOfDeath + respawnTime - Time.time;
        }
    }
}