using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ContentAnalyzer
{
    /// <summary>
    /// Intended to use in a T4 template to create a class containing all content files.
    /// </summary>
    public class ContentTree : BaseContentAnalyzer
    {
        string[] LastPathParts = null;
        bool first = true;

        ContentTree(string contentDirectory = "content", string buildDirectory = "content\\bin") : base(contentDirectory, buildDirectory) { }

        public static string Create(string contentDirectory, string outputDirectory, string @namespace, string classname)
        {
            var Analyzer = new ContentTree(contentDirectory, outputDirectory);

            return Analyzer.CreateContentTree(@namespace, classname);
        }

        private string Indent(int count)
        {
            return string.Empty.PadLeft(4 * (count), ' ');
        }

        private string CreateContentTree(string @namespace, string classname)
        {
            Dictionary<string, List<string>> TreeElements = new Dictionary<string, List<string>>();

            var Builder = new StringBuilder();
            Builder.AppendLine(Indent(0) + "namespace " + @namespace);
            Builder.AppendLine(Indent(0) + "{");
            Builder.AppendLine(Indent(1) + "public static partial class " + classname);
            Builder.AppendLine(Indent(1) + "{");

            foreach (var ContentType in ContentTypes)
            {
                foreach (var FileName in ContentType.EnumerateFiles(ContentDirectory, BuildDirectory))
                {
                    var WithoutContentDirAndExtension = StripExtension(RemoveContentDir(FileName));
                    var NormalizedPath = WithoutContentDirAndExtension.Replace("\\", "/");
                    var Parts = WithoutContentDirAndExtension.Split('\\');
                    var PathParts = Parts.Take(Parts.Length - 1).ToArray();
                    var Name = Parts.Last();
                    var Relevant = string.Join(".", PathParts);

                    if (!TreeElements.ContainsKey(Relevant))
                    {
                        TreeElements[Relevant] = new List<string>();
                    }

                    int Integer;

                    if (int.TryParse(Name, out Integer))
                    {
                        Name = "_" + Name;
                    }

                    var Content = "public const string " + Name + " = " + ToLiteral(NormalizedPath) + ";";

                    TreeElements[Relevant].Add(Content);
                }
            }

            foreach (KeyValuePair<string, List<string>> Entry in TreeElements.OrderBy(entry => entry.Key))
            {
                var PathParts = Entry.Key.Split('.');

                ClosePath(Builder, LastPathParts, PathParts);
                if (!first)
                {
                    Builder.AppendLine();
                }
                OpenPath(Builder, PathParts, LastPathParts);

                foreach (var Content in Entry.Value)
                {
                    Builder.AppendLine(Indent(2 + (IsRoot(PathParts) ? 0 : PathParts.Count())) + Content);
                }

                first = false;
                LastPathParts = PathParts;
            }

            ClosePath(Builder, LastPathParts, null);

            Builder.AppendLine(Indent(1) + "}");
            Builder.AppendLine(Indent(0) + "}");
            return Builder.ToString();
        }

        private void OpenPath(StringBuilder builder, string[] pathParts, string[] lastPathParts)
        {
            if (!IsRoot(pathParts))
            {
                for (int i = 0; i < pathParts.Count(); i++)
                {
                    if (lastPathParts != null && lastPathParts.ElementAtOrDefault(i) != null && lastPathParts[i] == pathParts[i])
                    {
                        continue;
                    }

                    builder.AppendLine(Indent(2 + i) + "public static partial class " + pathParts[i]);
                    builder.AppendLine(Indent(2 + i) + "{");
                    var CurrentPath = string.Join("/", pathParts.Take(i + 1)) + "/";
                    builder.AppendLine(Indent(3 + i) + "public const string _path_ = " + ToLiteral(CurrentPath) + ";");
                }
            }
        }

        private void ClosePath(StringBuilder builder, string[] lastPathParts, string[] pathParts)
        {
            if (null == lastPathParts)
            {
                return;
            }

            if (!IsRoot(lastPathParts))
            {
                for (int i = lastPathParts.Count() - 1; i >= 0; i--)
                {
                    if (pathParts != null && pathParts.ElementAtOrDefault(i) != null && pathParts[i] == lastPathParts[i])
                    {
                        continue;
                    }

                    builder.AppendLine(Indent(2 + i) + "}");
                }
            }
        }

        private bool IsRoot(string[] pathParts)
        {
            return pathParts.Count() == 1 && pathParts.First() == string.Empty;
        }

        private string ToLiteral(string input)
        {
            using (var writer = new StringWriter())
            {
                using (var provider = CodeDomProvider.CreateProvider("CSharp"))
                {
                    provider.GenerateCodeFromExpression(new CodePrimitiveExpression(input), writer, null);
                    return writer.ToString();
                }
            }
        }

        public string CreateBuildScript()
        {
            var Builder = new StringBuilder();

            foreach (var ContentType in ContentTypes)
            {
                foreach (var FileName in ContentType.EnumerateFiles(ContentDirectory, BuildDirectory))
                {
                    var WithoutContentDir = RemoveContentDir(FileName);
                    var BuildCommand = ContentType.BuildAction.CreateBuildCommand(WithoutContentDir, RemoveContentDir(BuildDirectory), StripExtension(WithoutContentDir));
                    Builder.AppendLine(BuildCommand);
                }
            }

            return Builder.ToString();
        }
    }
}
