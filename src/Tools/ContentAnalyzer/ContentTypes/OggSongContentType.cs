using ContentAnalyzer.BuildActions;

namespace ContentAnalyzer.ContentTypes
{
	public class OggSongContentType : BaseContentType
	{
		public override IBuildAction BuildAction => CopyBuildAction.Action;

		public override string FileEnding => "ogg";
	}
}
