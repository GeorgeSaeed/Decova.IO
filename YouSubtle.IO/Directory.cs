using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Linq;

namespace YouSubtle.IO
{
    public class Directory
    {
		/// <summary>
		/// You doubt that it's not working when the container hierarcy of the required directory is missing
		/// the last one or more parts. Make a check.
		/// </summary>
		/// <param name="dir"></param>
		/// <param name="maxNoOfTrials"></param>
		public static void EnsureDirectory(string directory)
		{
			bool success = false;

			Exception raisedException = null;

			if(System.IO.Directory.Exists(directory))
			{
				success = true;
			}
			else
			{
				for(int x=0; x<30; x++)
				{
					try
					{
						System.IO.Directory.CreateDirectory(directory);
						success = true;
						break;
					}
					catch (Exception expt)
					{
						raisedException = expt;
						Thread.Sleep(300);
						success = false;
					}
				}
			}

			if(! success)
			{
				throw new IOException($"Couldn't ensure directory [{raisedException}]", raisedException);
			}
		}


		/// <summary>
		/// Gets the closest common directory of the supplied file and/or directory paths
		/// </summary>
		/// <param name="fileAndDirectoryPaths"></param>
		/// <returns></returns>
		public static DirectoryInfo ClosestCommonAncestorOf(IEnumerable<string> fileAndDirectoryPaths)
		{
			#region assertions
			if (fileAndDirectoryPaths == null) throw new ArgumentNullException("fileAndDirectoryPaths");
			if (fileAndDirectoryPaths.Any() == false) throw new ArgumentException("fileAndDirectoryPaths cannot be empty.");
			foreach (string fileOrDir in fileAndDirectoryPaths)
			{
				if (!System.IO.Directory.Exists(fileOrDir) && !System.IO.File.Exists(fileOrDir))
				{
					throw new ArgumentException($"The path {fileOrDir} couldn't be recognized as an existing file or directory");
				}
			}
			#endregion

			#region case only one item in the list
			if (fileAndDirectoryPaths.Count() == 1)
			{
				string fileOrDir = fileAndDirectoryPaths.First();
				if (System.IO.Directory.Exists(fileOrDir)) return new DirectoryInfo(fileOrDir).Parent;
				else
				{
					return new FileInfo(fileOrDir).Directory;
				}
#if true

#endif
			}

			#endregion

			#region multiple items in the list
			List<string[]> filesDirParts = fileAndDirectoryPaths.Select(f => f.Split(new char[] { '\\', '/' })).ToList();

			int minLength = filesDirParts.Min(prts => prts.Length);

			StringBuilder path = new StringBuilder();

			List<string> commonParts = new List<string>();

			for (int x = 0; x < minLength; x++)
			{
				var distinct = filesDirParts.Select(itemPrts => itemPrts[x]).Distinct();
				if (distinct.Count() == 1)
				{
					commonParts.Add(distinct.First());
				}
				else
				{
					break;
				}
			}

			if (commonParts.Any() == false)
			{
				return null;
			}
			else
			{
				var dir = new DirectoryInfo(string.Join("\\", commonParts.ToArray()));
				if (dir.Exists == false)
				{
					throw new IOException($"Common directory [{dir}] doesn't exist!");
				}

				return dir;
			} 
			#endregion
		}

    }
}
