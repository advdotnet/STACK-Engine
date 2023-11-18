using ContentAnalyzer.BuildActions;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ContentAnalyzer.ContentTypes
{
	public abstract class BaseContentType
	{
		abstract public string FileEnding { get; }
		abstract public IBuildAction BuildAction { get; }

		public virtual bool IsContentType(string fileName)
		{
			return true;
		}

		public IEnumerable<string> EnumerateFiles(string contentDirectory, string buildDirectory)
		{
			var filesMatchingExtension = Directory
				.EnumerateFiles(contentDirectory, $"*.{FileEnding}", SearchOption.AllDirectories);

			var filesBeingContentType = filesMatchingExtension.Where(fileName => IsContentType(fileName));
			var filesNotInBuildDirectory = filesBeingContentType.Where(fileName => !fileName.StartsWith(buildDirectory));

			return filesNotInBuildDirectory;
		}
	}
}
