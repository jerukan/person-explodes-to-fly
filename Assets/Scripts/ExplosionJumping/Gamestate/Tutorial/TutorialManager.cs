using UnityEngine;
using System.Collections;
using ExplosionJumping.Util;
using ExplosionJumping.UI;
using UnityEngine.UI;

namespace ExplosionJumping.Gamestate.Tutorial {
    public class TutorialManager : MonoBehaviour {

        private TutorialSequenceLibrary tutorialLibrary = new TutorialSequenceLibrary();

        private TutorialSequence currentTutorialSequence;
        private Text tutorialText;

        private bool lastState;
        private float timeFinishedSequence;

        void Awake() {
            currentTutorialSequence = tutorialLibrary.BasicMovementTutorial();
        }

        private void Start() {
            tutorialText = GameObject.Find("TutorialPopup").GetComponent<Text>();
        }

        void Update() {
            if(currentTutorialSequence != null && !currentTutorialSequence.Complete) {
                currentTutorialSequence.UpdateSequence();
            }
            if (currentTutorialSequence.Complete) {
                if(!lastState) {
                    timeFinishedSequence = Time.time;
                }
                if (Time.time - timeFinishedSequence >= 5f) {
                    tutorialText.text = "";
                }
                else {
                    tutorialText.text = $"{currentTutorialSequence.name} complete!";
                }
            }
            else {
                tutorialText.text = currentTutorialSequence.CurrentCondition.description;
            }
            lastState = currentTutorialSequence.Complete;
        }
    }
}