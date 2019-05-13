using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using ExplosionJumping.Util.SaveData;
using ExplosionJumping.Gamestate;

namespace ExplosionJumping.Environment {
    [RequireComponent(typeof(Collider))]
    public class SaveTimeOnTrigger : MonoBehaviour {

        private bool completed;

        // Use this for initialization
        void Start() {

        }

        // Update is called once per frame
        void Update() {

        }

        private void OnTriggerEnter(Collider other) {
            if(!completed) {
                GameObject gameManager = GameObject.FindWithTag("GameController");
                LevelTimeData data = LevelTimeSaver.LoadData(SceneManager.GetActiveScene().name);
                data.times.Add(gameManager.GetComponent<GameManager>().GetElapsedTime());
                LevelTimeSaver.SaveData(data, SceneManager.GetActiveScene().name);
                completed = true;
            }
        }
    }
}