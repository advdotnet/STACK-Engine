using ContentAnalyzer.BuildActions;

namespace ContentAnalyzer.ContentTypes
{
	public class WaveSoundEffectContentType : BaseContentType
	{
		public override IBuildAction BuildAction => ContentCompilerBuildAction.CreateAction(ContentImporter.WavImporter, ContentProcessor.SoundEffectProcessor);

		public override string FileEnding => "wav";
	}
}
