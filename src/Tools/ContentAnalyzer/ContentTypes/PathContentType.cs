using ContentAnalyzer.BuildActions;

namespace ContentAnalyzer.ContentTypes
{
	public class PathContentType : BaseContentType
	{
		public override IBuildAction BuildAction => CopyBuildAction.Action;

		public override string FileEnding => "stp";
	}
}
