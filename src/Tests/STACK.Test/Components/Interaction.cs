using Microsoft.VisualStudio.TestTools.UnitTesting;
using STACK.Components;
using System.Collections;

namespace STACK.Test
{
    [TestClass]
    public class InteractionTest
    {
        static Verb TestVerb = Verb.Create("use");

        public class TestEntity : Entity
        {
            object PlayerA, PlayerB;

            public TestEntity(object playerA, object playerB)
            {
                Interaction
                    .Create(this)
                    .SetGetInteractionsFn(GetInteractions);

                PlayerA = playerA;
                PlayerB = playerB;
            }

            Interactions GetInteractions()
            {
                return Interactions
                    .Create()
                    .For(PlayerA)
                        .Add(TestVerb, PlayerScript())
                    .For(PlayerB)
                        .Add(TestVerb, ItemScript());
            }

            IEnumerator ItemScript()
            {
                yield return 1;
            }

            IEnumerator PlayerScript()
            {
                yield return 1;
            }
        }


        [TestMethod]
        public void DefaultScriptStartTestForCurrent()
        {
            var PlayerA = new Entity("PlayerA");
            Scripts.Create(PlayerA);

            var PlayerB = new Entity("PlayerB");
            Scripts.Create(PlayerB);

            var InteractionEntity = new TestEntity(PlayerA, PlayerB);
            var Interaction = InteractionEntity.Get<Interaction>();
            var InteractionsForPlayerA = Interaction.GetInteractions().GetFor(PlayerA);
            var InteractionsForPlayerB = Interaction.GetInteractions().GetFor(PlayerB);

            InteractionFn Fn;

            var HasInteraction = InteractionsForPlayerA.TryGetValue(TestVerb, out Fn);

            Assert.IsTrue(HasInteraction);
            Fn(new InteractionContext(PlayerA, null, null, TestVerb));
            Assert.IsTrue(PlayerA.Get<Scripts>().HasScript(Interactions.DEFAULTSCRIPTNAME));

            HasInteraction = InteractionsForPlayerB.TryGetValue(TestVerb, out Fn);

            Assert.IsTrue(HasInteraction);
            Fn(new InteractionContext(PlayerB, null, null, TestVerb));
            Assert.IsTrue(PlayerB.Get<Scripts>().HasScript(Interactions.DEFAULTSCRIPTNAME));
        }
    }
}
