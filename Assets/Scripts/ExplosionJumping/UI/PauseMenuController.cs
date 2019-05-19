using System.Collections;
using System.Collections.Generic;
using ExplosionJumping.Util;
using UnityEngine;

namespace ExplosionJumping.UI {
    public class PauseMenuController : MonoBehaviour {

        public KeyCode pauseButton = KeyCode.Escape;
        public GameObject pauseMenu;
        public bool paused;

        private void Awake() {
            SetPause(paused);
        }

        // Update is called once per frame
        void Update() {
            if(Input.GetKeyDown(pauseButton)) {
                SetPause(!paused);
            }
        }

        public void SetPause(bool pause) {
            paused = pause;
            pauseMenu.SetActive(pause);
            if (pause) {
                Time.timeScale = 0f;
            }
            else {
                Time.timeScale = 1f;
            }
            InputUtils.LockCursor(!paused);
        }
    }
}