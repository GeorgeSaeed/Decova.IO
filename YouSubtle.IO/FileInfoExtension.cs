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
		/// Returns the nearest ancestor found that satisfies the chekcer function
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
        /// Ensures the existence of the the directory of the file. Only the drive pre-existence is required. All parts of hierarchy
        /// will be created if not existing.
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="maxNoOfTrials"></param>
        public static void EnsureDirectory(this FileInfo _this)
		{
            _this.Directory.Ensure();
		}
	}
}
