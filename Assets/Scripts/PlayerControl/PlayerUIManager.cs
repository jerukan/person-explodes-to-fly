using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace ExplosionJumping.PlayerControl {

    /// <summary>
    /// Will be stuck into the actual UI layer since that's more clean.
    /// </summary>
    [RequireComponent(typeof(PlayerController))]
    public class PlayerUIManager : MonoBehaviour {

        public Slider healthSlider;
        public Text healthText;
        public GameObject respawnDisplay;
        public Text respawnTimerText;

        private PlayerController playerController;
        private PlayerRespawner playerRespawner;

        void Awake() {
            playerController = GetComponent<PlayerController>();
            healthSlider.minValue = 0;
            healthSlider.maxValue = playerController.maxHealth;
            playerRespawner = GetComponent<PlayerRespawner>();
        }

        void Update() {
            healthSlider.value = playerController.CurrentHealth;
            healthText.text = ((int)healthSlider.value).ToString();
            if (playerController.Dead) {
                respawnTimerText.text = playerRespawner.TimeUntilRespawn().ToString();
            }
            respawnDisplay.SetActive(playerController.Dead);
        }
    }
}