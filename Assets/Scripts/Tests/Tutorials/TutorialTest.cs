using NUnit.Framework;
using ExplosionJumping.Util;

namespace ExplosionJumping.Tests {
    public class TutorialTest {

        [Test]
        public void ConditionCheckerTest() {
            ConditionChecker a = new ConditionChecker();
            a.AddCondition(() => true);
            Assert.True(a.CheckConditions());

            int number = 3;
            a.AddCondition(() => number > 2);
            Assert.True(a.CheckConditions());
            number = 1;
            Assert.True(!a.CheckConditions());
        }
    }
}