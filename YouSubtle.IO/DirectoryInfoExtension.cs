using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace YouSubtle.IO
{
	public static class Directory
	{
		/// <summary>
		/// Returns the first ancestor found that satisfies the chekcer function
		/// </summary>
		/// <param name="_this"></param>
		/// <param name="checker"></param>
		/// <returns></returns>
		public static DirectoryInfo GetFirstAncestor(this DirectoryInfo _this, Func<DirectoryInfo, bool> checker)
		{
			var stepParent = _this.Parent;
			{
				if(checker(stepParent))
				{
					return stepParent;
				}
				else
				{
					stepParent = stepParent.Parent;
				}
			}
			while (stepParent != null) ;

			return null;
		}


		/// <summary>
		/// Returns all the descendent files the satisfies all arguments passed. If none of arguments passed, it returns all descendent files.
		/// </summary>
		/// <param name="_this"></param>
		/// <param name="searchPattern">The search string to match against the names of files. This parameter can contain
		//     a combination of valid literal path and wildcard (* and ?) characters, but it
		//     doesn't support regular expressions. The default pattern is "*", which returns
		//     all files.</param>
		/// <param name="fileSelector"></param>
		/// <param name="directParentDirSelector"></param>
		/// <returns></returns>
		public static IEnumerable<FileInfo> GetDescendentFiles( this DirectoryInfo _this, 
																string searchPattern=null, 
																Func<FileInfo, bool> fileSelector = null, 
																Func<DirectoryInfo, bool> directParentDirSelector = null)
		{
			List<FileInfo> files = new List<FileInfo>();

			foreach(var file in _this.GetFiles(searchPattern))
			{
				if(fileSelector == null || fileSelector(file))
				{
					files.Add(file);
				}
			}

			foreach(var dir in _this.GetDirectories())
			{
				if(directParentDirSelector != null && directParentDirSelector(dir) == false)	
				{
					continue;
				}

				var childDirFiles = GetDescendentFiles(dir, searchPattern, fileSelector, directParentDirSelector);
				files.AddRange(childDirFiles);
			}

			return files;
		}


		/// <summary>
		/// You doubt that it's not working when the container hierarcy of the required directory is missing
		/// the last one or more parts. Make a check.
		/// </summary>
		/// <param name="dir"></param>
		/// <param name="maxNoOfTrials"></param>
		public static void EnsureDirectory(this DirectoryInfo dir, int maxNoOfTrials = 5)
		{
			bool success = false;

			Exception raisedException = null;

			if(dir.Exists)
			{
				success = true;
			}
			else
			{
				for(int x=0; x<maxNoOfTrials; x++)
				{
					try
					{
						dir.Create();
						success = true;
						break;
					}
					catch (Exception expt)
					{
						raisedException = expt;
						success = false;
					}
				}
			}

			if(! success)
			{
				throw new IOException($"Couldn't ensure directory [{raisedException}]", raisedException);
			}
		}

	
	}
}
