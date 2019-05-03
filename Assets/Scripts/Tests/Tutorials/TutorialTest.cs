using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
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

        // A UnityTest behaves like a coroutine in PlayMode
        // and allows you to yield null to skip a frame in EditMode
        [UnityTest]
        public IEnumerator TutorialTestWithEnumeratorPasses() {
            // Use the Assert class to test conditions.
            // yield to skip a frame
            yield return null;
        }
    }
}