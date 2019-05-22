using System;
using ExplosionJumping.PlayerControl;
using UnityEngine;

namespace ExplosionJumping.Gamestate.Tutorial {
    public class TutorialSequenceLibrary {

        public TutorialSequence GetSequenceFromName(string name) {
            switch (name) {
                case "BasicMovementTutorial":
                    return BasicMovementTutorial();
                case "BasicRocketTutorial":
                    return BasicRocketTutorial();
                default:
                    return null;
            }
        }

        public TutorialSequence BasicMovementTutorial() {
            TutorialCondition pressAnyMovementKey = new TutorialCondition();
            pressAnyMovementKey.Init(
                "pressAnyMovementKey",
                "Press on movement keys WASD to move.",
                () => {
                    return Math.Abs(Input.GetAxisRaw("Horizontal")) > float.Epsilon || Math.Abs(Input.GetAxisRaw("Vertical")) > float.Epsilon;
                }
            );
            TutorialCondition playerJump = new TutorialCondition();
            playerJump.Init(
                "playerJump",
                "Press Space to jump",
                () => {
                    return GameObject.FindWithTag("Player").GetComponent<FirstPersonControllerCustom>().Jumping;
                }
            );

            TutorialConditionTimed displayAutobunnyhop = new TutorialConditionTimed();
            displayAutobunnyhop.Init("bunnyhopnice", "You can hold down the jump button to jump automatically and preserve momentum!", 5f, false);

            return new TutorialSequence("Basic movement tutorial", pressAnyMovementKey, playerJump, displayAutobunnyhop);
        }

        public TutorialSequence BasicRocketTutorial() {
            TutorialCondition firedRocket = new TutorialCondition();
            firedRocket.Init(
                "firedRocket",
                "Press the left mouse key to fire a rocket.",
                () => {
                    return Input.GetKey(KeyCode.Mouse0);
                }
            );
            TutorialCondition rocketJump = new TutorialCondition();
            rocketJump.Init(
                "rocketJump",
                "Fire a rocket at the ground to propel yourself upwards.",
                () => {
                    return GameObject.FindWithTag("Player").GetComponent<PlayerController>().ExplosiveJumping;
                }
            );
            TutorialCondition goodRocketJump = new TutorialCondition();
            goodRocketJump.Init(
                "goodRocketJump",
                "Jump and fire a rocket at the ground to propel yourself even further.",
                () => {
                    return GameObject.FindWithTag("Player").GetComponent<PlayerController>().ExplosiveJumping && GameObject.FindWithTag("Player").GetComponent<FirstPersonControllerCustom>().Jumping;
                }
            );

            return new TutorialSequence("Basic rocket tutorial", firedRocket, rocketJump, goodRocketJump);
        }
    }
}
