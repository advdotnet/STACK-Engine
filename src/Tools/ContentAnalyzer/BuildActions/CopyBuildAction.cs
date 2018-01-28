namespace ContentAnalyzer.BuildActions
{
    public class CopyBuildAction : IBuildAction
    {
        public string CreateBuildCommand(string fileName, string targetDirectory, string fileNameWithoutExtension)
        {
            return "copy \"" + fileName + "\" \"" + targetDirectory + "\\" + fileName + "\"";
        }

        public static IBuildAction Action = new CopyBuildAction();
    }
}
