using ContentAnalyzer.BuildActions;

namespace ContentAnalyzer.ContentTypes
{
    public class PathContentType : BaseContentType
    {
        public override IBuildAction BuildAction
        {
            get
            {
                return CopyBuildAction.Action;
            }
        }

        public override string FileEnding
        {
            get
            {
                return "stp";
            }
        }
    }
}
