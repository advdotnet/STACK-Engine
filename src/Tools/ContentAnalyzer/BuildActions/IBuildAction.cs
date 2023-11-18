namespace ContentAnalyzer.BuildActions
{
	public interface IBuildAction
	{
		string CreateBuildCommand(string fileName, string targetDirectory, string fileNameWithoutExtension);
	}
}
