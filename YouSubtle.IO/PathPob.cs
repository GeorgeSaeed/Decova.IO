using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using YouSubtle;
using System.IO;

namespace YouSubtle.IO
{
	public class PathPob
	{
		/// <summary>
		/// Combines the provided segments of the desired path taking care of normalizing extra path separators
		/// '\' or '/' according the host platform.
		/// </summary>
		/// <param name="segments"></param>
		/// <returns></returns>
		public static string Combine(params string[] segments)
		{
			string pathSeparator = Environment.OSVersion.Platform == PlatformID.Win32NT ? "\\" : "/";
			segments = segments.Select(s => s.TrimSpecificSubstring(pathSeparator)).ToArray();
			return string.Join(pathSeparator, segments);
		}

        /// <summary>
        /// Gets the path separator according to the environment platform.
        /// </summary>
        private static char PathSeparator
        {
            get
            {
                switch (Environment.OSVersion.Platform)
                {
                    case PlatformID.Win32NT:
                        return '\\';
                    case PlatformID.Unix:
                    default:
                        return '/';
                }
            }
        }


        /// <summary>
        /// 1. In case path is an absolute one and belongs to the reference directory it returns the relative path (with no preceding path separator character).
        /// 2. In case path is an absolute one but doesn't belong to the reference directory it throws an InvalidOperationException.
        /// 3. In case path is already a relative path it returns it and removes the starting path separator character if any.
        /// </summary>
        /// <param name="referenceDir"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string EnsureRelativeTo(string referenceDir, string path)
		{
			if(path.Contains(":")) // absolute
			{
				if (path.StartsWith(referenceDir) == false)
				{
					throw new InvalidOperationException($"path [{path}] cannot be relative to [{referenceDir}]");
				}
				else
				{
					// ommit the reference dir.
					string relativePath = path.Substring(referenceDir.Length);
					if(relativePath.StartsWith("/") || relativePath.StartsWith("\\"))
					{
						relativePath = relativePath.Substring(1);
					}
					return relativePath;
				}
			}
			else // relative
			{
				return path;
			}
		}

		/// <summary>
		/// 1. In case path is an absolute one and belongs to the reference directory it returns it as is.
		/// 2. In case path is an absolute one but doesn't belong to the reference directory it throws an InvalidOperationException.
		/// 3. In case path is a relative path it returns the absolute path based on the reference directory.
		/// </summary>
		/// <param name="referenceDir"></param>
		/// <param name="path"></param>
		/// <returns></returns>
		public static string EnsureAbsolutePath(string referenceDir, string path)
		{
			if (path.Contains(":")) // absolute
			{
				if (path.StartsWith(referenceDir) == false)
				{
					throw new InvalidOperationException($"path [{path}] cannot be relative to [{referenceDir}]");
				}

				return path;
			}
			else // relative
			{
				return Combine(referenceDir, path);
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
