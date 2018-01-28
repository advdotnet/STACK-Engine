using ContentAnalyzer.BuildActions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var FilesMatchingExtension = Directory
                .EnumerateFiles(contentDirectory, "*." + FileEnding, SearchOption.AllDirectories);

            var FilesBeingContentType = FilesMatchingExtension.Where(FileName => IsContentType(FileName));
            var FilesNotInBuildDirectory = FilesBeingContentType.Where(FileName => !FileName.StartsWith(buildDirectory));

            return FilesNotInBuildDirectory;
        }
    }
}
