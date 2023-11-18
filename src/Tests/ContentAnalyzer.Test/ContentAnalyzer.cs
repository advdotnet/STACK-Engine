using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ContentAnalyzer.Test
{
	[TestClass]
	public class ContentEnumeration
	{
		[TestMethod, TestCategory("FileSystem")]
		public void ContentAnalyzerTests()
		{
			var result = BuildContent.CreateScript("content", "content\\bin");

			Assert.IsTrue(result.Contains("copy \"Skins\\Default.skin\" \"bin\\Skins\\Default.skin\""));
			Assert.IsTrue(result.Contains("fxc.exe /T fx_2_0 \"shaders\\Normalmap.fx\" /Fo\"bin\\shaders\\Normalmap.fxb\""));
			Assert.IsTrue(result.Contains("ContentCompiler.exe stacklogo.png bin\\stacklogo TextureImporter TextureProcessor"));
			Assert.IsTrue(result.Contains("ContentCompiler.exe fonts\\stack.spritefont bin\\fonts\\stack FontDescriptionImporter FontDescriptionProcessor"));
		}

		[TestMethod, TestCategory("FileSystem")]
		public void CreateContentTreeTest()
		{
			var result = ContentTree.Create("content", "content\\bin", "MyNamespace", "content");

			Assert.IsTrue(result.Contains("namespace MyNamespace"));
			Assert.IsTrue(result.Contains("public static partial class content"));

			Assert.IsTrue(result.Contains("public static partial class Skins"));
			Assert.IsTrue(result.Contains("public const string _path_ = \"Skins/\";"));
			Assert.IsTrue(result.Contains("public const string Default = \"Skins/Default\";"));
			Assert.IsTrue(result.Contains("public const string Normalmap = \"shaders/Normalmap\";"));
			Assert.IsTrue(result.Contains("public const string stacklogo = \"stacklogo\";"));
			Assert.IsTrue(result.Contains("public const string stack = \"fonts/stack\";"));
		}
	}
}
