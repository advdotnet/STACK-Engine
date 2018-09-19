using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using STACK.Components;
using System.Collections.Generic;

namespace STACK.Test
{
    [TestClass]
    public class TextTest
    {
        public Vector2 MeasureString(string text)
        {
            var Result = Vector2.Zero;

            Result.X = text.Length * 10;
            Result.Y = 20;

            return Result;
        }

        [TestMethod]
        public void WordWrapTest()
        {
            var Entity = new Entity();
            var TextComponent = Text.Create(Entity).SetWidth(100).SetWordWrap(true);
            TextComponent.MeasureStringFn = MeasureString;
            TextComponent.Set("Lorem Ipsum", 0, Vector2.Zero);
            Assert.AreEqual(2, TextComponent.Lines.Count);
        }

        [TestMethod]
        public void NoWordWrapTest()
        {
            var Entity = new Entity();
            var TextComponent = Text.Create(Entity).SetWidth(100).SetWordWrap(false);
            TextComponent.MeasureStringFn = MeasureString;
            TextComponent.Set("Lorem Ipsum", 0, Vector2.Zero);
            Assert.AreEqual(1, TextComponent.Lines.Count);
        }

        [TestMethod]
        public void ConstrainTextTest()
        {
            var Entity = new Entity();
            var Rectangle = new Rectangle(10, 10, 80, 80);
            var TextComponent = Text.Create(Entity).SetWidth(100).SetConstrain(true).SetConstrainingRectangle(Rectangle);
            TextComponent.MeasureStringFn = MeasureString;
            TextComponent.Set("Lorem Ipsum Dolor Donot asdasda das asdasda sdsa dasdsadsa", 0, Vector2.Zero);
            Assert.AreEqual(50, TextComponent.ConstrainOffset.X);
        }

        [TestMethod]
        public void ConstrainRectangleTest()
        {
            var Inner = new Rectangle(10, 10, 80, 80);
            var Outer = new Rectangle(0, 0, 640, 400);

            var Result1 = Text.ConstrainRectangle(Inner, Outer);
            Assert.IsTrue(Result1.Equals(Inner));

            var Result2 = Text.ConstrainRectangle(Outer, Inner);
            Assert.IsTrue(Result2.Equals(Inner));

            var SecondInner = new Rectangle(600, -10, 100, 100);
            var Result3 = Text.ConstrainRectangle(SecondInner, Outer);
            Assert.IsTrue(Result3.Equals(new Rectangle(540, 0, 100, 100)));
        }

        [TestMethod]
        public void TransformReferenceTest()
        {
            var Entity = new Entity();
            var TransformComponent = Transform.Create(Entity);
            var TextComponent = Text.Create(Entity);

            TextComponent.Initialize(false);

            Assert.IsNotNull(TextComponent.Transform);
            Assert.AreEqual(TransformComponent, TextComponent.Transform);
        }

        [TestMethod]
        public void UseTextInfoNoWordWrapTest()
        {
            const string FirstTag = "1";
            const string SecondTag = "2";
            var Entity = new Entity();
            var TextComponent = Text.Create(Entity).SetWidth(100).SetWordWrap(false);
            TextComponent.MeasureStringFn = MeasureString;

            var TextInfos = new List<TextInfo>()
            {
                new TextInfo("Lorem", FirstTag),
                new TextInfo("Ipsum", SecondTag),
            };

            TextComponent.Set(TextInfos, 0, Vector2.Zero);

            Assert.AreEqual(2, TextComponent.Lines.Count);
            Assert.AreEqual(FirstTag, TextComponent.Lines[0].Tag);
            Assert.AreEqual(SecondTag, TextComponent.Lines[1].Tag);
        }

        [TestMethod]
        public void UseTextInfoWordWrapTest()
        {
            const string FirstTag = "1";
            const string SecondTag = "2";
            var Entity = new Entity();
            var TextComponent = Text.Create(Entity).SetWidth(30).SetWordWrap(true);
            TextComponent.MeasureStringFn = MeasureString;

            var TextInfos = new List<TextInfo>()
            {
                new TextInfo("Lor em", FirstTag),
                new TextInfo("Ips um", SecondTag),
            };

            TextComponent.Set(TextInfos, 0, Vector2.Zero);

            Assert.AreEqual(4, TextComponent.Lines.Count);
            Assert.AreEqual(FirstTag, TextComponent.Lines[0].Tag);
            Assert.AreEqual(FirstTag, TextComponent.Lines[1].Tag);
            Assert.AreEqual(SecondTag, TextComponent.Lines[2].Tag);
            Assert.AreEqual(SecondTag, TextComponent.Lines[3].Tag);
        }

        [TestMethod]
        public void DeserializeTest()
        {
            var Entity = new Entity();
            var TransformComponent = Transform.Create(Entity);
            var TextComponent = Text.Create(Entity);

            var Serialized = State.Serialization.SaveState(Entity);

            var DeserializedEntity = State.Serialization.LoadState<Entity>(Serialized);
            DeserializedEntity.Initialize(true);

            var DeserializedTextComponent = DeserializedEntity.Get<Text>();
            var DeserializedTransformComponent = DeserializedEntity.Get<Transform>();

            Assert.IsNotNull(DeserializedTextComponent.Transform);
            Assert.AreEqual(DeserializedTransformComponent, DeserializedTextComponent.Transform);
        }
    }
}
