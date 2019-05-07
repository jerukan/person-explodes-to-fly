using System;
using System.Collections.Generic;

namespace ExplosionJumping.Gamestate.Tutorial {
    public class TutorialSequence {

        public readonly string name;
        private bool complete;
        public bool Complete {
            get { return complete; }
        }
        private readonly List<TutorialCondition> conditionSequence = new List<TutorialCondition>();
        private int currentConditionIndex;

        public TutorialCondition CurrentCondition {
            get { return conditionSequence[currentConditionIndex]; }
        }

        public bool CurrentConditionMet {
            get { return CurrentCondition.GetState(); }
        }

        public TutorialSequence(string name, params TutorialCondition[] conditions) {
            this.name = name;
            conditionSequence.AddRange(conditions);
        }

        public void UpdateSequence() {
            if(!complete && CurrentConditionMet) {
                if (currentConditionIndex == conditionSequence.Count - 1) {
                    complete = true;
                }
                else {
                    currentConditionIndex++;
                    CurrentCondition.Begin();
                }
            }
        }
    }
}
