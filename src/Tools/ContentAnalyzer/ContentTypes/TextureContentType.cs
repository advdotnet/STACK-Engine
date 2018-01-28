using ContentAnalyzer.BuildActions;

namespace ContentAnalyzer.ContentTypes
{
    public class TextureContentType : BaseContentType
    {
        public override IBuildAction BuildAction
        {
            get
            {
                return ContentCompilerBuildAction.CreateAction(ContentImporter.TextureImporter, ContentProcessor.TextureProcessor);
            }
        }

        public override string FileEnding
        {
            get
            {
                return "png";
            }
        }

        public override bool IsContentType(string fileName)
        {
            return !fileName.EndsWith("_BMF.png");
        }
    }
}
