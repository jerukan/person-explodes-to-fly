using System;
using System.Collections.Generic;

namespace ExplosionJumping.Util {
    public class ConditionChecker {

        private readonly List<Func<bool>> conditions = new List<Func<bool>>();

        public void AddCondition(Func<bool> condition) {
            conditions.Add(condition);
        }

        public void AddConditions(params Func<bool>[] allConditions) {
            conditions.AddRange(allConditions);
        }

        public bool CheckConditions() {
            if(conditions.Count == 0) {
                return false;
            }
            bool result = true;
            foreach(Func<bool> cond in conditions) {
                result &= cond.Invoke();
            }
            return result;
        }
    }
}
