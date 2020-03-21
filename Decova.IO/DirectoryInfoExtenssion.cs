using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Decova
{
	public static class DirectoryInfoExtenssion
	{
        #region GetClosestAncestor()
        //####################################################################
        /// <summary>
        /// Gets the closest ancestor that satisfies the dirPicker checker. If withinDirectory provided
        /// the method stops traversing as soon as it reaches a directory that satisfies it.
        /// </summary>
        /// <param name="_this"></param>
        /// <param name="ancestorPicker">The target directory condition checker</param>
        /// <param name="withinDirectory">The function keep searches for a directory matches the "ancestorPicker" checker until either find it or the "withinDirectory" reached and doesn't match as well whichever is earlier. </param>
        /// <returns></returns>
        public static DirectoryInfo GetClosestAncestor(this DirectoryInfo _this,
                                                            Func<DirectoryInfo, bool> ancestorPicker,
                                                            Func<DirectoryInfo, bool> withinDirectory = null)
        {
            if (ancestorPicker == null)
                throw new ArgumentNullException(nameof(ancestorPicker));



            var stepParent = _this.Parent;
            do
            {
                if (withinDirectory != null && withinDirectory(stepParent)) return null;

                if (ancestorPicker(stepParent))
                {
                    return stepParent;
                }
                else
                {
                    stepParent = stepParent.Parent;
                }
            }
            while (stepParent != null);

            return null;
        }
        //####################################################################
        #endregion

        #region GetDescendantFiles()
        //####################################################################
        private static void GetDescendantFiles(this DirectoryInfo dir,
                                               List<FileInfo> output,
                                               Func<FileInfo, bool> pickFileIf = null,
                                               Func<DirectoryInfo, bool> ignoreWithDescendantsIf = null
                                               )
        {
            bool isPickable(FileInfo f)
            {
                try
                {
                    return ((pickFileIf == null) || pickFileIf(f));
                }
                catch (Exception)
                {
                    throw;
                }
            }
            bool isIgnoredAsAWhole(DirectoryInfo d)
            {
                try
                {
                    if (ignoreWithDescendantsIf == null) return false;
                    else return ignoreWithDescendantsIf(d);
                }
                catch (Exception)
                {
                    throw;
                }
            }

            try
            {
                output.AddRange(dir.GetFiles().Where(isPickable));

                foreach (var childDir in dir.GetDirectories())
                {
                    if (isIgnoredAsAWhole(childDir)) continue;

                    GetDescendantFiles(childDir, output, pickFileIf, ignoreWithDescendantsIf);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        /// <summary>
        /// Returns the descendant files with filteration if any specified.
        /// </summary>
        /// <param name="_this"></param>
        /// <param name="pickFileIf">A file filter.</param>
        /// <param name="ignoreDirectoryIf">If provided and matched a descendant directory, the whole content of the directory will be ignored with all of its descendants.</param>
        /// <returns></returns>
        public static IEnumerable<FileInfo> GetDescendantFiles(this DirectoryInfo _this,
                                                                Func<FileInfo, bool> pickFileIf = null,
                                                                Func<DirectoryInfo, bool> ignoreDirectoryIf = null
                                                                )
        {
            List<FileInfo> output = new List<FileInfo>();

            GetDescendantFiles(_this, output, pickFileIf, ignoreDirectoryIf);

            return output;
        }
        //####################################################################
        #endregion

        #region GetDescendentDirectories()
        //####################################################################
        private static void GetDescendantDirectories(this DirectoryInfo dir,
                                      List<DirectoryInfo> output,
                                      Func<DirectoryInfo, bool> pickDirectoryIf = null,
                                      Func<DirectoryInfo, bool> ignoreWithDescendantsIf = null)
        {
            #region simplifying checkers
            //####################################################################
            bool isPickable(DirectoryInfo d)
            {
                return ((pickDirectoryIf == null) || pickDirectoryIf(d));
            }
            bool isIgnoredAsAWhole(DirectoryInfo d)
            {
                if (ignoreWithDescendantsIf == null) return false;
                else return ignoreWithDescendantsIf(d);
            }
            //####################################################################
            #endregion

            if (isIgnoredAsAWhole(dir)) return;

            if (isPickable(dir))
            {
                output.Add(dir);
            }

            foreach (var child in dir.GetDirectories())
            {
                GetDescendantDirectories(child, output, pickDirectoryIf, ignoreWithDescendantsIf);
            }
        }

        /// <summary>
        /// Returns all the descendant directories the satisfy all arguments passed. 
        /// </summary>
        /// <param name="_this"></param>
        /// <param name="pickDirectoryIf">If provided the directory is picked only if evaluates to true. If evaluates to false the directory will not be picked but its descendants are still applicable for picking.</param>
        /// <param name="ignoreWithDescendantsIf">If provided and matched a directory, the whole content of the directory will be ignored. This doesn't apply on the provided search root "dir".</param>
        /// <returns></returns>
        public static IEnumerable<DirectoryInfo> GetDescendantDirectories(this DirectoryInfo _this,
                                                                          Func<DirectoryInfo, bool> pickDirectoryIf = null,
                                                                          Func<DirectoryInfo, bool> ignoreWithDescendantsIf = null)
        {
            List<DirectoryInfo> output = new List<DirectoryInfo>();
            GetDescendantDirectories(_this, output, pickDirectoryIf, ignoreWithDescendantsIf);
            return output;
        }

        //####################################################################
        #endregion

        #region BelongsToMe()
        //####################################################################
        /// <summary>
        /// Returns true if the item (file or directory path) exists and it's a Descendant of this directory.
        /// </summary>
        /// <param name="_this"></param>
        /// <param name="fileOrDirectoryPath"></param>
        /// <returns></returns>
        public static bool BelongsToMe(this DirectoryInfo _this, string fileOrDirectoryPath)
        {
            if (fileOrDirectoryPath.Length < _this.FullName.Length)
            {
                return false;
            }
            else if (fileOrDirectoryPath.Substring(0, _this.FullName.Length) != _this.FullName)
            {
                return false;
            }
            else
            {
                return File.Exists(fileOrDirectoryPath)
                    || Directory.Exists(fileOrDirectoryPath);
            }
        }
        //####################################################################
        #endregion

        #region Ensure()
        //####################################################################
        /// <summary>
        /// Ensures the existence of the directory. Only the drive pre-existence is required. All parts of hierarchy
        /// will be created if not existing.
        /// </summary>
        /// <param name="directory"></param>
        public static void Ensure(this DirectoryInfo directory)
        {
            if (directory == null) throw new ArgumentNullException(nameof(directory));

            bool success = false;

            Exception raisedException = null;

            if (directory.Exists)
            {
                success = true;
            }
            else
            {
                for (int x = 0; x < 30; x++)
                {
                    try
                    {
                        directory.Create();
                        directory.Refresh();
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

            if (!success)
            {
                throw new IOException($"Couldn't ensure directory [{directory.FullName}]", raisedException);
            }
        }

        //####################################################################
        #endregion
    }
}
