using ContentAnalyzer.BuildActions;

namespace ContentAnalyzer.ContentTypes
{
    public class SpriteFontContentType : BaseContentType
    {
        public override IBuildAction BuildAction
        {
            get
            {
                return ContentCompilerBuildAction.CreateAction(ContentImporter.FontDescriptionImporter, ContentProcessor.FontDescriptionProcessor);
            }
        }

        public override string FileEnding
        {
            get { return "spritefont"; }
        }
    }
}
