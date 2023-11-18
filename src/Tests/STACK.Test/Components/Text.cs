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
			var result = Vector2.Zero;

			result.X = text.Length * 10;
			result.Y = 20;

			return result;
		}

		[TestMethod]
		public void WordWrapTest()
		{
			var entity = new Entity();
			var textComponent = Text.Create(entity).SetWidth(100).SetWordWrap(true);
			textComponent.MeasureStringFn = MeasureString;
			textComponent.Set("Lorem Ipsum", 0, Vector2.Zero);
			Assert.AreEqual(2, textComponent.Lines.Count);
		}

		[TestMethod]
		public void NoWordWrapTest()
		{
			var entity = new Entity();
			var textComponent = Text.Create(entity).SetWidth(100).SetWordWrap(false);
			textComponent.MeasureStringFn = MeasureString;
			textComponent.Set("Lorem Ipsum", 0, Vector2.Zero);
			Assert.AreEqual(1, textComponent.Lines.Count);
		}

		[TestMethod]
		public void ConstrainTextTest()
		{
			var entity = new Entity();
			var rectangle = new Rectangle(10, 10, 80, 80);
			var textComponent = Text.Create(entity).SetWidth(100).SetConstrain(true).SetConstrainingRectangle(rectangle);
			textComponent.MeasureStringFn = MeasureString;
			textComponent.Set("Lorem Ipsum Dolor Donot asdasda das asdasda sdsa dasdsadsa", 0, Vector2.Zero);
			Assert.AreEqual(50, textComponent.ConstrainOffset.X);
		}

		[TestMethod]
		public void ConstrainRectangleTest()
		{
			var inner = new Rectangle(10, 10, 80, 80);
			var outer = new Rectangle(0, 0, 640, 400);

			var result1 = Text.ConstrainRectangle(inner, outer);
			Assert.IsTrue(result1.Equals(inner));

			var result2 = Text.ConstrainRectangle(outer, inner);
			Assert.IsTrue(result2.Equals(inner));

			var secondInner = new Rectangle(600, -10, 100, 100);
			var result3 = Text.ConstrainRectangle(secondInner, outer);
			Assert.IsTrue(result3.Equals(new Rectangle(540, 0, 100, 100)));
		}

		[TestMethod]
		public void TransformReferenceTest()
		{
			var entity = new Entity();
			var transformComponent = Transform.Create(entity);
			var textComponent = Text.Create(entity);

			textComponent.Initialize(false);

			Assert.IsNotNull(textComponent.Transform);
			Assert.AreEqual(transformComponent, textComponent.Transform);
		}

		[TestMethod]
		public void UseTextInfoNoWordWrapTest()
		{
			const string firstTag = "1";
			const string secondTag = "2";
			var entity = new Entity();
			var textComponent = Text.Create(entity).SetWidth(100).SetWordWrap(false);
			textComponent.MeasureStringFn = MeasureString;

			var textInfos = new List<TextInfo>()
			{
				new TextInfo("Lorem", firstTag),
				new TextInfo("Ipsum", secondTag),
			};

			textComponent.Set(textInfos, 0, Vector2.Zero);

			Assert.AreEqual(2, textComponent.Lines.Count);
			Assert.AreEqual(firstTag, textComponent.Lines[0].Tag);
			Assert.AreEqual(secondTag, textComponent.Lines[1].Tag);
		}

		[TestMethod]
		public void UseTextInfoWordWrapTest()
		{
			const string firstTag = "1";
			const string secondTag = "2";
			var entity = new Entity();
			var textComponent = Text.Create(entity).SetWidth(30).SetWordWrap(true);
			textComponent.MeasureStringFn = MeasureString;

			var textInfos = new List<TextInfo>()
			{
				new TextInfo("Lor em", firstTag),
				new TextInfo("Ips um", secondTag),
			};

			textComponent.Set(textInfos, 0, Vector2.Zero);

			Assert.AreEqual(4, textComponent.Lines.Count);
			Assert.AreEqual(firstTag, textComponent.Lines[0].Tag);
			Assert.AreEqual(firstTag, textComponent.Lines[1].Tag);
			Assert.AreEqual(secondTag, textComponent.Lines[2].Tag);
			Assert.AreEqual(secondTag, textComponent.Lines[3].Tag);
		}

		[TestMethod]
		public void TextLineCopyFunctionsCopyTag()
		{
			var textLine = new TextLine("Text", Vector2.Zero, Vector2.Zero, Rectangle.Empty, Color.White, "Tag");

			Assert.AreEqual("Tag", textLine.ChangeColor(Color.Blue).Tag);
			Assert.AreEqual("Tag", textLine.ChangeHitbox(Rectangle.Empty).Tag);
			Assert.AreEqual("Tag", textLine.Move(Vector2.One).Tag);
		}

		[TestMethod]
		public void DeserializeTest()
		{
			var entity = new Entity();
			_ = Transform.Create(entity);
			_ = Text.Create(entity);

			var serialized = State.Serialization.SaveState(entity);

			var deserializedEntity = State.Serialization.LoadState<Entity>(serialized);
			deserializedEntity.Initialize(true);

			var deserializedTextComponent = deserializedEntity.Get<Text>();
			var deserializedTransformComponent = deserializedEntity.Get<Transform>();

			Assert.IsNotNull(deserializedTextComponent.Transform);
			Assert.AreEqual(deserializedTransformComponent, deserializedTextComponent.Transform);
		}
	}
}
