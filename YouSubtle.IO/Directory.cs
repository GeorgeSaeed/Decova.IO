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


		public static DirectoryInfo ClosestCommonAncestorOf(IEnumerable<string> fileAndDirectoryPaths)
		{
			List<string[]> filesDirParts = fileAndDirectoryPaths.Select(f => f.Split(new char[] { '\\', '/' })).ToList();

			int minLength = filesDirParts.Min(prts => prts.Length);

			StringBuilder path = new StringBuilder();

			List<string> commonParts = new List<string>();

			for(int x=0; x<minLength; x++)
			{
				var distinct = filesDirParts.Select(itemPrts => itemPrts[x]).Distinct();
				if(distinct.Count() == 1)
				{
					commonParts.Add(distinct.First());
				}
				else 
				{
					break;
				}
			}

			if(commonParts.Any() == false)
			{
				return null;
			}
			else
			{
				var dir = new DirectoryInfo(string.Join("\\", commonParts.ToArray()));
				if(dir.Exists == false)
				{
					throw new IOException($"Common directory [{dir}] doesn't exist!");
				}

				return dir;
			}
		}

    }
}
