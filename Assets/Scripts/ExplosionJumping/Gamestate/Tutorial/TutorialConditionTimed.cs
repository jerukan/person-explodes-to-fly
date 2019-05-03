using System;
using UnityEngine;

namespace ExplosionJumping.Gamestate.Tutorial {
    /// <summary>
    /// This is probably full of spaghetti and it's untested so double whammy ay lmao.
    /// </summary>
    public class TutorialConditionTimed : TutorialCondition {

        public float timeToRun;
        public bool failOnOvertime;

        public void Init(string name, string description, float timeToRun, bool failOnOvertime, params Func<bool>[] allConditions) {
            this.timeToRun = timeToRun;
            this.failOnOvertime = failOnOvertime;
            Func<bool> timeCheck = () => {
                if (failOnOvertime) {
                    return Time.time - timeWhenStarted <= timeToRun;
                }
                return Time.time - timeWhenStarted > timeToRun;
            };
            Func<bool>[] copiedArray = new Func<bool>[allConditions.Length + 1];
            allConditions.CopyTo(copiedArray, 1);
            copiedArray[0] = timeCheck;
            Init(name, description, copiedArray);
        }
    }
}
