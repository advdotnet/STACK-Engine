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
			var analyzer = new BuildContent(contentDirectory, outputDirectory);

			return analyzer.CreateBuildScript();
		}

		public string CreateBuildScript()
		{
			var builder = new StringBuilder();

			foreach (var contentType in ContentTypes)
			{
				foreach (var fileName in contentType.EnumerateFiles(ContentDirectory, BuildDirectory))
				{
					var withoutContentDir = RemoveContentDir(fileName);
					var buildCommand = contentType.BuildAction.CreateBuildCommand(withoutContentDir, RemoveContentDir(BuildDirectory), StripExtension(withoutContentDir));
					builder.AppendLine(buildCommand);
				}
			}

			return builder.ToString();
		}
	}
}
