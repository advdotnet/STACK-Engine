using Microsoft.VisualStudio.TestTools.UnitTesting;
using STACK.Components;
using System.Collections;

namespace STACK.Test
{
	[TestClass]
	public class InteractionTest
	{
		private static readonly Verb _testVerb = Verb.Create("use");

		public class TestEntity : Entity
		{
			private readonly object _playerA, _playerB;

			public TestEntity(object playerA, object playerB)
			{
				Interaction
					.Create(this)
					.SetGetInteractionsFn(GetInteractions);

				_playerA = playerA;
				_playerB = playerB;
			}

			private Interactions GetInteractions()
			{
				return Interactions
					.Create()
					.For(_playerA)
						.Add(_testVerb, PlayerScript())
					.For(_playerB)
						.Add(_testVerb, ItemScript());
			}

			private IEnumerator ItemScript()
			{
				yield return 1;
			}

			private IEnumerator PlayerScript()
			{
				yield return 1;
			}
		}


		[TestMethod]
		public void DefaultScriptStartTestForCurrent()
		{
			var playerA = new Entity("PlayerA");
			Scripts.Create(playerA);

			var playerB = new Entity("PlayerB");
			Scripts.Create(playerB);

			var interactionEntity = new TestEntity(playerA, playerB);
			var interaction = interactionEntity.Get<Interaction>();
			var interactionsForPlayerA = interaction.GetInteractions().GetFor(playerA);
			var interactionsForPlayerB = interaction.GetInteractions().GetFor(playerB);
			var hasInteraction = interactionsForPlayerA.TryGetValue(_testVerb, out var fn);

			Assert.IsTrue(hasInteraction);
			fn(new InteractionContext(playerA, null, null, _testVerb));
			Assert.IsTrue(playerA.Get<Scripts>().HasScript(Interactions.DEFAULTSCRIPTNAME));

			hasInteraction = interactionsForPlayerB.TryGetValue(_testVerb, out fn);

			Assert.IsTrue(hasInteraction);
			fn(new InteractionContext(playerB, null, null, _testVerb));
			Assert.IsTrue(playerB.Get<Scripts>().HasScript(Interactions.DEFAULTSCRIPTNAME));
		}
	}
}
