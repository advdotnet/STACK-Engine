using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ContentAnalyzer.Test
{
    [TestClass]
    public class ContentEnumeration
    {
        [TestMethod, TestCategory("FileSystem")]
        public void ContentAnalyzerTests()
        {
            var Result = BuildContent.CreateScript("content", "content\\bin");

            Assert.IsTrue(Result.Contains("copy \"Skins\\Default.skin\" \"bin\\Skins\\Default.skin\""));
            Assert.IsTrue(Result.Contains("fxc.exe /T fx_2_0 \"shaders\\Normalmap.fx\" /Fo\"bin\\shaders\\Normalmap.fxb\""));
            Assert.IsTrue(Result.Contains("ContentCompiler.exe stacklogo.png bin\\stacklogo TextureImporter TextureProcessor"));
            Assert.IsTrue(Result.Contains("ContentCompiler.exe fonts\\stack.spritefont bin\\fonts\\stack FontDescriptionImporter FontDescriptionProcessor"));
        }

        [TestMethod, TestCategory("FileSystem")]
        public void CreateContentTreeTest()
        {
            var Result = ContentTree.Create("content", "content\\bin", "MyNamespace", "content");

            Assert.IsTrue(Result.Contains("namespace MyNamespace"));
            Assert.IsTrue(Result.Contains("public static partial class content"));

            Assert.IsTrue(Result.Contains("public static partial class Skins"));
            Assert.IsTrue(Result.Contains("public const string _path_ = \"Skins\\\\\";"));
            Assert.IsTrue(Result.Contains("public const string Default = \"Skins\\\\Default\";"));
            Assert.IsTrue(Result.Contains("public const string Normalmap = \"shaders\\\\Normalmap\";"));
            Assert.IsTrue(Result.Contains("public const string stacklogo = \"stacklogo\";"));
            Assert.IsTrue(Result.Contains("public const string stack = \"fonts\\\\stack\";"));
        }
    }
}
