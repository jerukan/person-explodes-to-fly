﻿using UnityEngine;
using System.Collections;
using ExplosionJumping.Util;
using ExplosionJumping.UI;
using UnityEngine.UI;

namespace ExplosionJumping.Gamestate.Tutorial {
    public class TutorialManager : MonoBehaviour {

        public string tutorialOnStartup;

        private TutorialSequenceLibrary tutorialLibrary = new TutorialSequenceLibrary();

        private TutorialSequence currentTutorialSequence;
        private Text tutorialText;

        private bool lastState;
        private float timeFinishedSequence;

        void Awake() {
            currentTutorialSequence = tutorialLibrary.GetSequenceFromName(tutorialOnStartup);
        }

        private void Start() {
            tutorialText = GameObject.Find("TutorialPopup").GetComponent<Text>();
            tutorialText.text = "";
        }

        void Update() {
            if (currentTutorialSequence != null) {
                lastState = currentTutorialSequence.Complete;
                if (!currentTutorialSequence.Complete) {
                    currentTutorialSequence.UpdateSequence();
                    tutorialText.text = currentTutorialSequence.CurrentCondition.description;
                }
                if (!lastState) {
                    timeFinishedSequence = Time.time;
                }
                else {
                    if (Time.time - timeFinishedSequence >= 5f) {
                        tutorialText.text = "";
                    }
                    else {
                        tutorialText.text = $"{currentTutorialSequence.name} complete!";
                    }
                }
            }
        }

        public void SetTutorialFromName(string tutorialName) {
            currentTutorialSequence = tutorialLibrary.GetSequenceFromName(tutorialName);
        }
    }
}