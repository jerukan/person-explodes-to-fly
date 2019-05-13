using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using ExplosionJumping.Util.SaveData;
using ExplosionJumping.Gamestate;

namespace ExplosionJumping.LevelEnvironment {
    [RequireComponent(typeof(Collider))]
    public class SaveTimeOnTrigger : MonoBehaviour {
        
        void Start() {

        }

        void Update() {

        }

        private void OnTriggerEnter(Collider other) {
            GameObject gameManager = GameObject.FindWithTag("GameController");
            if(!gameManager.GetComponent<GameManager>().LevelCompleted) {
                LevelTimeData data = LevelTimeSaver.LoadData(SceneManager.GetActiveScene().name);
                data.times.Add(gameManager.GetComponent<GameManager>().GetElapsedTime());
                LevelTimeSaver.SaveData(data, SceneManager.GetActiveScene().name);
                gameManager.GetComponent<GameManager>().LevelCompleted = true;
            }
        }
    }
}