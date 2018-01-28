using ContentAnalyzer.BuildActions;

namespace ContentAnalyzer.ContentTypes
{
    public class WaveSoundEffectContentType : BaseContentType
    {
        public override IBuildAction BuildAction
        {
            get
            {
                return ContentCompilerBuildAction.CreateAction(ContentImporter.WavImporter, ContentProcessor.SoundEffectProcessor);
            }
        }

        public override string FileEnding
        {
            get
            {
                return "wav";
            }
        }
    }
}
