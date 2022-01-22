using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace System;

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
    public static async Task GetDescendantFilesAsync(this DirectoryInfo dir,
                                                    Action<FileInfo> onFileMatched,
                                                    Func<FileInfo, bool> pickFileIf = null,
                                                    Func<DirectoryInfo, bool> ignoreEntireDirectoryIf = null,
                                                    CancellationToken? cancellationToken = null
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
                if (ignoreEntireDirectoryIf == null) return false;
                else return ignoreEntireDirectoryIf(d);
            }
            catch (Exception)
            {
                throw;
            }
        }

        try
        {
            dir.GetFiles().Where(isPickable).ToList().ForEach(onFileMatched);
            //output.AddRange(dir.GetFiles().Where(isPickable));

            foreach (var childDir in dir.GetDirectories())
            {
                if (isIgnoredAsAWhole(childDir)) continue;
                if (cancellationToken?.IsCancellationRequested == true)
                {
                    return;
                }
                await GetDescendantFilesAsync(childDir, onFileMatched, pickFileIf, ignoreEntireDirectoryIf);
            }
        }
        catch (UnauthorizedAccessException unauthExpt)
        {
            return;
        }
        catch (Exception)
        {
            throw;
        }
    }

    //####################################################################
    #endregion

    #region GetDescendentDirectories()
    //####################################################################
    public static async Task GetDescendantDirectoriesAsync(this DirectoryInfo dir,
                                  Action<DirectoryInfo> onDirMatched,
                                  Func<DirectoryInfo, bool> pickDirectoryIf = null,
                                  CancellationToken? cancellationToken = null)
    {
        #region simplifying checkers
        //####################################################################
        bool isPickable(DirectoryInfo d)
        {
            return ((pickDirectoryIf == null) || pickDirectoryIf(d));
        }
        //####################################################################
        #endregion

        if (isPickable(dir))
        {
            onDirMatched(dir);
        }

        try
        {
            foreach (var childDir in dir.GetDirectories())
            {
                if (isPickable(childDir)) onDirMatched(childDir);

                if (cancellationToken?.IsCancellationRequested == true)
                {
                    return;
                }
                await GetDescendantDirectoriesAsync(childDir, onDirMatched, pickDirectoryIf);
            }
        }
        catch (UnauthorizedAccessException unauthExpt)
        {
            return;
        }
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
