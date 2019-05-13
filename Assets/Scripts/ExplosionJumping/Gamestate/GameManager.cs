using UnityEngine;
using System.Collections;

namespace ExplosionJumping.Gamestate {
    public class GameManager : MonoBehaviour {

        private bool levelCompleted;
        private float timeSceneLoaded;
        private float timeLevelCompleted;

        public bool LevelCompleted {
            get {
                return levelCompleted;
            }
            set {
                levelCompleted = value;
                timeLevelCompleted = Time.time;
            }
        }

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
            if(levelCompleted) {
                return timeLevelCompleted - timeSceneLoaded;
            }
            return Time.time - timeSceneLoaded;
        }
    }
}