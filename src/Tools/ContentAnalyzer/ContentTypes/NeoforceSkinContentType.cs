using ContentAnalyzer.BuildActions;

namespace ContentAnalyzer.ContentTypes
{
	public class NeoforceSkinContentType : BaseContentType
	{
		public override IBuildAction BuildAction => CopyBuildAction.Action;

		public override string FileEnding => "skin";
	}
}
