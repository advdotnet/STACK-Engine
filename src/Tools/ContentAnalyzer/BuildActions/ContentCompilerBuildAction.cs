namespace ContentAnalyzer.BuildActions
{
    public class ContentCompilerBuildAction : IBuildAction
    {
        public ContentImporter Importer { get; set; }
        public ContentProcessor Processor { get; set; }

        public string CreateBuildCommand(string fileName, string targetDirectory, string fileNameWithoutExtension)
        {
            return "ContentCompiler.exe " + fileName + " " + targetDirectory + "\\" + fileNameWithoutExtension + " " + Importer.ToString() + " " + Processor.ToString() + " true";
        }

        public static IBuildAction CreateAction(ContentImporter importer, ContentProcessor processor)
        {
            return new ContentCompilerBuildAction()
            {
                Importer = importer,
                Processor = processor
            };
        }
    }
}
