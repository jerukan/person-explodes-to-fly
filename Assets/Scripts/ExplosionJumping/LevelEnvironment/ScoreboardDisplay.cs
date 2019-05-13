using UnityEngine;
using System.Collections;
using TMPro;
using ExplosionJumping.Util.SaveData;
using UnityEngine.SceneManagement;
using System;

namespace ExplosionJumping.LevelEnvironment {
    [RequireComponent(typeof(TextMeshPro))]
    public class ScoreboardDisplay : MonoBehaviour {

        public int numberOfScoresToDisplay = 3;

        private TextMeshPro text;
        private string topScoresString;

        private void Awake() {
            text = GetComponent<TextMeshPro>();
            LevelTimeData data = LevelTimeSaver.LoadData(SceneManager.GetActiveScene().name);
            float[] times = data.times.ToArray();
            Array.Sort(times);
            topScoresString = $"Top {numberOfScoresToDisplay} times: {Environment.NewLine}";
            for (int i = 0; i < numberOfScoresToDisplay; i++) {
                if (i >= times.Length) {
                    break;
                }
                if (i != numberOfScoresToDisplay - 1) {
                    topScoresString += $"{i + 1}: {times[i]}{Environment.NewLine}";
                }
                else {
                    topScoresString += $"{i + 1}: {times[i]}";
                }
            }
            text.SetText(topScoresString);
        }

        // Use this for initialization
        void Start() {

        }

        // Update is called once per frame
        void Update() {

        }
    }
}