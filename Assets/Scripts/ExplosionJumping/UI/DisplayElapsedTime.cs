using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using ExplosionJumping.Gamestate;

namespace ExplosionJumping.UI {
    [RequireComponent(typeof(Text))]
    public class DisplayElapsedTime : MonoBehaviour {

        private Text text;
        private GameManager gameManager;

        private void Awake() {
            text = GetComponent<Text>();
        }

        void Start() {
            gameManager = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
        }

        void Update() {
            text.text = $"Elapsed time: {gameManager.GetElapsedTime()}";
        }
    }
}