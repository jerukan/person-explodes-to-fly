using UnityEngine;

namespace ExplosionJumping.Gamestate.Tutorial {
    public class TutorialSequenceLibrary {

        public TutorialSequence BasicMovementTutorial() {
            TutorialCondition pressAnyMovementKey = new TutorialCondition();
            pressAnyMovementKey.Init(
                "Pressanymovekey",
                "Press on movement keys WASD to move.",
                () => {
                    return System.Math.Abs(Input.GetAxisRaw("Horizontal")) > float.Epsilon || System.Math.Abs(Input.GetAxisRaw("Vertical")) > float.Epsilon;
                }
            );

            return new TutorialSequence(pressAnyMovementKey);
        }
    }
}
