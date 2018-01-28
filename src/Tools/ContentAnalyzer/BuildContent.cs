using System.Text;

namespace ContentAnalyzer
{
    /// <summary>
    /// Creates a batch file to build all the content files into xnb format.
    /// </summary>
    public class BuildContent : BaseContentAnalyzer
    {
        BuildContent(string contentDirectory = "content", string buildDirectory = "content\\bin") : base(contentDirectory, buildDirectory) { }

        public static string CreateScript(string contentDirectory, string outputDirectory)
        {
            var Analyzer = new BuildContent(contentDirectory, outputDirectory);

            return Analyzer.CreateBuildScript();
        }

        public string CreateBuildScript()
        {
            var Builder = new StringBuilder();

            foreach (var ContentType in ContentTypes)
            {
                foreach (var FileName in ContentType.EnumerateFiles(ContentDirectory, BuildDirectory))
                {
                    var WithoutContentDir = RemoveContentDir(FileName);
                    var BuildCommand = ContentType.BuildAction.CreateBuildCommand(WithoutContentDir, RemoveContentDir(BuildDirectory), StripExtension(WithoutContentDir));
                    Builder.AppendLine(BuildCommand);
                }
            }

            return Builder.ToString();
        }
    }
}
