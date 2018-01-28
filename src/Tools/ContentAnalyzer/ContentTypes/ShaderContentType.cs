using ContentAnalyzer.BuildActions;

namespace ContentAnalyzer.ContentTypes
{
    public class ShaderContentType : BaseContentType
    {
        public override IBuildAction BuildAction
        {
            get
            {
                return ShaderCompilerBuildAction.Action;
            }
        }

        public override string FileEnding
        {
            get
            {
                return "fx";
            }
        }
    }
}
