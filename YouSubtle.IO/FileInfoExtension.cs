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
        /// Gets the closest ancestor that satisfies the dirPicker checker. If withinDirectory provided
        /// the method stops traversing as soon as it reaches a directory that satisfies it.
        /// </summary>
        /// <param name="_this"></param>
        /// <param name="dirPicker">The target directory condition checker</param>
        /// <param name="withinDirectory">A ceiling directory to finish searching at if not found earlier.</param>
        /// <returns></returns>
        public static DirectoryInfo TryGetClosestAncestor(this FileInfo _this, 
                                                          Func<DirectoryInfo, bool> dirPicker, 
                                                          Func<DirectoryInfo, bool> withinDirectory = null)
		{
			var stepParent = _this.Directory;
			do
			{
                if (withinDirectory == null && withinDirectory(stepParent)) return null;

				if(dirPicker(stepParent))
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
