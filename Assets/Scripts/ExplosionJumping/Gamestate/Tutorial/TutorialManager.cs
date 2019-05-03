using UnityEngine;
using System.Collections;
using ExplosionJumping.Util;
using ExplosionJumping.UI;

namespace ExplosionJumping.Gamestate.Tutorial {
    public class TutorialManager : MonoBehaviour {

        private TutorialSequenceLibrary tutorialLibrary = new TutorialSequenceLibrary();

        private TutorialSequence currentTutorialSequence;
        private MessageQueue messageQueue;

        void Awake() {
            currentTutorialSequence = tutorialLibrary.BasicMovementTutorial();
        }

        private void Start() {
            messageQueue = GameObject.Find("IngameHud").GetComponent<MessageQueue>();
        }

        void Update() {
            if(currentTutorialSequence != null && !currentTutorialSequence.Complete) {
                currentTutorialSequence.UpdateSequence();

            }
        }
    }
}