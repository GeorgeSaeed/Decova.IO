using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using YouSubtle;

namespace YouSubtle.IO
{
	public class PathComposer
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
	}
}
