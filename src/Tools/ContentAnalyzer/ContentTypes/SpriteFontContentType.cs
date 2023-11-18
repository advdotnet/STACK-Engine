using ContentAnalyzer.BuildActions;

namespace ContentAnalyzer.ContentTypes
{
	public class SpriteFontContentType : BaseContentType
	{
		public override IBuildAction BuildAction => ContentCompilerBuildAction.CreateAction(ContentImporter.FontDescriptionImporter, ContentProcessor.FontDescriptionProcessor);

		public override string FileEnding => "spritefont";
	}
}
