using System;
using ExplosionJumping.PlayerControl.Movement;
using UnityEngine;

namespace ExplosionJumping.Gamestate.Tutorial {
    public class TutorialSequenceLibrary {

        public TutorialSequence BasicMovementTutorial() {
            TutorialCondition pressAnyMovementKey = new TutorialCondition();
            pressAnyMovementKey.Init(
                "Pressanymovekey",
                "Press on movement keys WASD to move.",
                () => {
                    return Math.Abs(Input.GetAxisRaw("Horizontal")) > float.Epsilon || Math.Abs(Input.GetAxisRaw("Vertical")) > float.Epsilon;
                }
            );
            TutorialCondition playerJump = new TutorialCondition();
            playerJump.Init(
                "Pressjump",
                "Press Space to jump",
                () => {
                    return GameObject.FindWithTag("Player").GetComponent<RigidbodyFPController>().Jumping;
                }
            );

            return new TutorialSequence("Basic movement tutorial", pressAnyMovementKey, playerJump);
        }
    }
}
