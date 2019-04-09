using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace ExplosionJumping.UI {
    public class SceneLoader : MonoBehaviour {

        public string sceneName;

        public void LoadScene() {
            SceneManager.LoadScene(sceneName);
        }
    }
}