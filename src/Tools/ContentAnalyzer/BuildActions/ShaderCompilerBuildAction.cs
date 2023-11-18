namespace ContentAnalyzer.BuildActions
{
	public class ShaderCompilerBuildAction : IBuildAction
	{
		public string CreateBuildCommand(string fileName, string targetDirectory, string fileNameWithoutExtension)
		{
			return "fxc.exe /T fx_2_0 \"" + fileName + "\" /Fo\"" + targetDirectory + "\\" + fileNameWithoutExtension + ".fxb\"";
		}

		public static IBuildAction Action = new ShaderCompilerBuildAction();
	}
}
