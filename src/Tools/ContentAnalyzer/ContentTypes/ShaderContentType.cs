using ContentAnalyzer.BuildActions;

namespace ContentAnalyzer.ContentTypes
{
	public class ShaderContentType : BaseContentType
	{
		public override IBuildAction BuildAction => ShaderCompilerBuildAction.Action;

		public override string FileEnding => "fx";
	}
}
