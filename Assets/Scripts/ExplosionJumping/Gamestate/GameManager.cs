using UnityEngine;
using System.Collections;

namespace ExplosionJumping.Gamestate {
    public class GameManager : MonoBehaviour {

        private float timeSceneLoaded;

        private void Awake() {
            Time.timeScale = 1f;
            timeSceneLoaded = Time.time;
        }

        void Start() {

        }

        // Update is called once per frame
        void Update() {

        }

        public float GetElapsedTime() {
            return Time.time - timeSceneLoaded;
        }
    }
}