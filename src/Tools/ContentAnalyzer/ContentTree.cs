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
		string[] _lastPathParts = null;
		bool _first = true;

		ContentTree(string contentDirectory = "content", string buildDirectory = "content\\bin") : base(contentDirectory, buildDirectory) { }

		public static string Create(string contentDirectory, string outputDirectory, string @namespace, string classname)
		{
			var analyzer = new ContentTree(contentDirectory, outputDirectory);

			return analyzer.CreateContentTree(@namespace, classname);
		}

		private string Indent(int count)
		{
			return string.Empty.PadLeft(4 * (count), ' ');
		}

		private string CreateContentTree(string @namespace, string classname)
		{
			var treeElements = new Dictionary<string, List<string>>();

			var builder = new StringBuilder();
			builder.AppendLine($"{Indent(0)}namespace {@namespace}");
			builder.AppendLine($"{Indent(0)}{{");
			builder.AppendLine($"{Indent(1)}public static partial class {classname}");
			builder.AppendLine($"{Indent(1)}{{");

			foreach (var contentType in ContentTypes)
			{
				foreach (var fileName in contentType.EnumerateFiles(ContentDirectory, BuildDirectory))
				{
					var withoutContentDirAndExtension = StripExtension(RemoveContentDir(fileName));
					var normalizedPath = withoutContentDirAndExtension.Replace("\\", "/");
					var parts = withoutContentDirAndExtension.Split('\\');
					var pathParts = parts.Take(parts.Length - 1).ToArray();
					var name = parts.Last();
					var relevant = string.Join(".", pathParts);

					if (!treeElements.ContainsKey(relevant))
					{
						treeElements[relevant] = new List<string>();
					}


					if (int.TryParse(name, out var integer))
					{
						name = $"_{name}";
					}

					var content = $"public const string {name} = {ToLiteral(normalizedPath)};";

					treeElements[relevant].Add(content);
				}
			}

			foreach (var entry in treeElements.OrderBy(entry => entry.Key))
			{
				var pathParts = entry.Key.Split('.');

				ClosePath(builder, _lastPathParts, pathParts);
				if (!_first)
				{
					builder.AppendLine();
				}
				OpenPath(builder, pathParts, _lastPathParts);

				foreach (var content in entry.Value)
				{
					builder.AppendLine(Indent(2 + (IsRoot(pathParts) ? 0 : pathParts.Count())) + content);
				}

				_first = false;
				_lastPathParts = pathParts;
			}

			ClosePath(builder, _lastPathParts, null);

			builder.AppendLine($"{Indent(1)}}}");
			builder.AppendLine($"{Indent(0)}}}");
			return builder.ToString();
		}

		private void OpenPath(StringBuilder builder, string[] pathParts, string[] lastPathParts)
		{
			if (!IsRoot(pathParts))
			{
				for (var i = 0; i < pathParts.Count(); i++)
				{
					if (lastPathParts != null && lastPathParts.ElementAtOrDefault(i) != null && lastPathParts[i] == pathParts[i])
					{
						continue;
					}

					builder.AppendLine($"{Indent(2 + i)}public static partial class {pathParts[i]}");
					builder.AppendLine($"{Indent(2 + i)}{{");
					var currentPath = string.Join("/", pathParts.Take(i + 1)) + "/";
					builder.AppendLine($"{Indent(3 + i)}public const string _path_ = {ToLiteral(currentPath)};");
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
				for (var i = lastPathParts.Count() - 1; i >= 0; i--)
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
			var builder = new StringBuilder();

			foreach (var contentType in ContentTypes)
			{
				foreach (var fileName in contentType.EnumerateFiles(ContentDirectory, BuildDirectory))
				{
					var withoutContentDir = RemoveContentDir(fileName);
					var buildCommand = contentType.BuildAction.CreateBuildCommand(withoutContentDir, RemoveContentDir(BuildDirectory), StripExtension(withoutContentDir));
					builder.AppendLine(buildCommand);
				}
			}

			return builder.ToString();
		}
	}
}
