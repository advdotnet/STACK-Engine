using ContentAnalyzer.ContentTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ContentAnalyzer
{
	/// <summary>
	/// Base class for iterating over a content directory.
	/// </summary>
	public class BaseContentAnalyzer
	{
		protected string ContentDirectory;
		protected string BuildDirectory;
		protected List<BaseContentType> ContentTypes;

		protected BaseContentAnalyzer(string contentDirectory = "content", string buildDirectory = "content\\bin")
		{
			ContentDirectory = contentDirectory;
			BuildDirectory = buildDirectory;

			ContentTypes = new List<BaseContentType>();

			var q = from t in GetLoadableTypes(Assembly.GetExecutingAssembly())
					where t.IsClass && typeof(BaseContentType).IsAssignableFrom(t) && !t.IsAbstract
					select t;

			foreach (var entityType in q)
			{
				var instance = (BaseContentType)Activator.CreateInstance(entityType);
				ContentTypes.Add(instance);
			}
		}

		private IEnumerable<Type> GetLoadableTypes(Assembly assembly)
		{
			if (assembly == null)
			{
				throw new ArgumentNullException(nameof(assembly));
			}

			try
			{
				return assembly.GetTypes();
			}
			catch (ReflectionTypeLoadException e)
			{
				return e.Types.Where(t => t != null);
			}
		}

		protected string RemoveContentDir(string path)
		{
			var result = path.Replace(ContentDirectory, "");
			result = result.TrimStart('\\');

			return result;
		}

		protected string StripExtension(string fileName)
		{
			var fileInfo = new FileInfo(fileName);
			var extension = fileInfo.Extension;

			return fileName.Substring(0, fileName.LastIndexOf(extension));
		}
	}
}
