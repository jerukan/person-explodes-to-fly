using System;
using ExplosionJumping.Util;
using UnityEngine;

namespace ExplosionJumping.Gamestate.Tutorial {
    public class TutorialCondition {

        public static readonly string DEFAULT_CONDITION_NAME = "Condition";
        public static int idCounter;

        public string conditionName;
        public int id;
        public string description;
        public readonly ConditionChecker conditions = new ConditionChecker();

        protected float timeWhenStarted;
        protected bool initialized;
        protected bool running;

        public void Init(params Func<bool>[] allConditions) {
            Init(DEFAULT_CONDITION_NAME + "_" + idCounter, "", allConditions);
        }

        public void Init(string name, string description, params Func<bool>[] allConditions) {
            initialized = true;
            conditionName = name;
            this.description = description;
            id = idCounter;
            idCounter++;
            conditions.AddConditions(allConditions);
        }

        public bool GetState() {
            if(!initialized) {
                throw new Exception("This condition must be initialized!");
            }
            return conditions.CheckConditions();
        }

        public void Begin() {
            if (!initialized) {
                throw new Exception("This condition must be initialized!");
            }
            running = true;
            timeWhenStarted = Time.time;
        }
    }
}
