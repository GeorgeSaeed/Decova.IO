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
	}
}
