using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace YouSubtle
{
    public static class FileInfoExtension
    {
		/// <summary>
		/// Returns the first ancestor found that satisfies the chekcer function
		/// </summary>
		/// <param name="_this"></param>
		/// <param name="checker"></param>
		/// <returns></returns>
		public static DirectoryInfo GetClosestAncestorWhere(this FileInfo _this, Func<DirectoryInfo, bool> checker)
		{
			var stepParent = _this.Directory;
			do
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
		/// You doubt that it's not working when the container hierarcy of the required directory is missing
		/// the last one or more parts. Make a check.
		/// </summary>
		/// <param name="dir"></param>
		/// <param name="maxNoOfTrials"></param>
		public static void EnsureDirectory(this FileInfo _this)
		{
			YouSubtle.IO.Directory.EnsureDirectory(_this.Directory.FullName);
		}
	}
}
