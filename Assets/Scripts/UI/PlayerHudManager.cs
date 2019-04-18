using UnityEngine;
using System.Collections;
using ExplosionJumping.PlayerControl;
using UnityEngine.UI;

namespace ExplosionJumping.UI {
    public class PlayerHudManager : MonoBehaviour {

        public PlayerController playerController;

        private Slider healthSlider;
        private Text healthNumber;
        private PlayerRespawner playerRespawner;
        private GameObject respawnPanel;
        private Text respawnTimerText;


        // Use this for initialization
        void Start() {
            healthSlider = GameObject.Find("HealthBar").GetComponent<Slider>();
            healthSlider.minValue = 0;
            healthSlider.maxValue = playerController.maxHealth;
            healthNumber = GameObject.Find("HealthNumber").GetComponent<Text>();
            playerRespawner = playerController.GetComponent<PlayerRespawner>();
            respawnPanel = GameObject.Find("RespawnPanel");
            respawnTimerText = GameObject.Find("RespawnTimerText").GetComponent<Text>();
        }

        // Update is called once per frame
        void Update() {
            healthSlider.value = playerController.CurrentHealth;
            healthNumber.text = ((int)healthSlider.value).ToString();
            if (playerController.Dead) {
                respawnTimerText.text = playerRespawner.TimeUntilRespawn().ToString();
            }
            respawnPanel.SetActive(playerController.Dead);
        }
    }
}