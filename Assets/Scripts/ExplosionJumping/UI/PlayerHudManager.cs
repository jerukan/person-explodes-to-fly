using UnityEngine;
using System.Collections;
using ExplosionJumping.PlayerControl;
using UnityEngine.UI;
using ExplosionJumping.PlayerControl.Movement;

namespace ExplosionJumping.UI {
    /// <summary>
    /// Updates all HUD elements related to displaying information about the player specifically.
    /// </summary>
    public class PlayerHudManager : MonoBehaviour {

        public PlayerController playerController;

        private Slider healthSlider;
        private Text healthNumber;
        private Text speedometer;
        private PlayerRespawner playerRespawner;
        private RigidbodyFPController charController;
        private GameObject respawnPanel;
        private Text respawnTimerText;


        // Use this for initialization
        void Start() {
            healthSlider = GameObject.Find("HealthBar").GetComponent<Slider>();
            healthSlider.minValue = 0;
            healthSlider.maxValue = playerController.maxHealth;
            healthNumber = GameObject.Find("HealthNumber").GetComponent<Text>();
            playerRespawner = playerController.GetComponent<PlayerRespawner>();
            charController = playerController.GetComponent<RigidbodyFPController>();
            respawnPanel = GameObject.Find("RespawnPanel");
            respawnTimerText = GameObject.Find("RespawnTimerText").GetComponent<Text>();
            speedometer = GameObject.Find("Speedometer").GetComponent<Text>();
        }

        // Update is called once per frame
        void Update() {
            healthSlider.value = playerController.CurrentHealth;
            healthNumber.text = ((int)healthSlider.value).ToString();
            if (playerController.Dead) {
                respawnTimerText.text = ((int)playerRespawner.TimeUntilRespawn()).ToString();
            }
            respawnPanel.SetActive(playerController.Dead);
            speedometer.text = $"Speed: {charController.HorizontalVelocity.magnitude}";
        }
    }
}