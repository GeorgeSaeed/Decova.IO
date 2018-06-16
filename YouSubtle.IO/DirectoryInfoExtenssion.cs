using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace YouSubtle
{
	public static class DirectoryInfoExtenssion
	{
		/// <summary>
		/// Returns the first ancestor found that satisfies the chekcer function
		/// </summary>
		/// <param name="_this"></param>
		/// <param name="checker"></param>
		/// <returns></returns>
		public static DirectoryInfo GetClosestAncestorWhere(this DirectoryInfo _this, Func<DirectoryInfo, bool> checker)
		{
			var stepParent = _this.Parent;
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
		/// Returns all the descendent files the satisfy all arguments passed. If none of the arguments passed, it returns all descendent files.
		/// </summary>
		/// <param name="_this"></param>
		/// <param name="searchPattern">The search string to match against the names of files. This parameter can contain
		//     a combination of valid literal path and wildcard (* and ?) characters, but it
		//     doesn't support regular expressions. The default pattern is "*", which returns
		//     all files.</param>
		/// <param name="ignoreDirectory">A function returns a boolean that determines whether to skip the argument directory or not.</param>
		/// <param name="directParentDirSelector"></param>
		/// <returns></returns>
		public static IEnumerable<FileInfo> GetDescendentFiles( this DirectoryInfo _this, 
																string searchPattern=null, 
																Func<DirectoryInfo, bool> ignoreDirectory = null
																)
		{

			List<FileInfo> files = new List<FileInfo>();

			if (ignoreDirectory != null && ignoreDirectory(_this)) return files;

			foreach(var file in _this.GetFiles(searchPattern))
			{
				files.Add(file);
			}

			foreach(var dir in _this.GetDirectories())
			{
				var childDirFiles = GetDescendentFiles(dir, searchPattern, ignoreDirectory);
				files.AddRange(childDirFiles);
			}

			return files;
		}



		/// <summary>
		/// Returns all the descendent directories the satisfy all arguments passed. If none of the arguments passed, it returns all descendent files.
		/// </summary>
		/// <param name="_this"></param>
		/// <param name="searchPattern">The search string to match against the names of directories. This parameter can contain
		//     a combination of valid literal path and wildcard (* and ?) characters, but it
		//     doesn't support regular expressions. The default pattern is "*", which returns
		//     all files.</param>
		/// <param name="fileSelector"></param>
		/// <param name="directParentDirSelector"></param>
		/// <returns></returns>
		public static IEnumerable<DirectoryInfo> GetDescendentDirectories(this DirectoryInfo _this, string searchPattern=null)
		{
			List<DirectoryInfo> output = new List<DirectoryInfo>();
			foreach(var directory in _this.GetDirectories(searchPattern))
			{
				output.Add(directory);
			}

			foreach(var dir in _this.GetDirectories())
			{
				output.AddRange(GetDescendentDirectories(dir, searchPattern));
			}

			return output;
		}
	
		/// <summary>
		/// Returns true if the item (file or directory path) existis and it's a descendant of this directory.
		/// </summary>
		/// <param name="_this"></param>
		/// <param name="fileOrDirectoryPath"></param>
		/// <returns></returns>
		public static bool DoesItemBelongToMe(this DirectoryInfo _this, string fileOrDirectoryPath)
		{
			if(fileOrDirectoryPath.Length < _this.FullName.Length)
			{
				return false;
			}
			else if(fileOrDirectoryPath.Substring(0, _this.FullName.Length) != _this.FullName)
			{
				return false;
			}
			else
			{
				return File.Exists(fileOrDirectoryPath) || Directory.Exists(fileOrDirectoryPath);
			}
		}
	}
}
