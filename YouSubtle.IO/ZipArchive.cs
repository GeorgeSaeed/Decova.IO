using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace YouSubtle.IO
{
    public class ZipArchive
    {
		private string _filePath;

		public ZipArchive(string filePath)
		{
			this._filePath = filePath;
		}

		private static string ReadZipTextFile(string archivePath, Func<ZipArchiveEntry, bool> singleFileSelector)
		{
			FileStream file = new FileStream(archivePath, FileMode.Open, FileAccess.Read, FileShare.None, 4 * 1024 * 1024, FileOptions.Asynchronous);

			var zipArch = new System.IO.Compression.ZipArchive(file);
			ZipArchiveEntry entry = zipArch.Entries.Single(e => singleFileSelector(e));

			Stream stream = entry.Open();
			byte[] buffer = new byte[entry.Length];
			var reader = new StreamReader(stream);
			return reader.ReadToEnd();
		}

		private static string ReadZipTextFile(string archivePath, string fileName)
		{
			return ReadZipTextFile(archivePath, entry => entry.Name == fileName);
		}


		/// <summary>
		/// Reads a text file from archive without extraction to disk.
		/// </summary>
		/// <param name="zipFileName">The path inside the archive. Separator is '/'.</param>
		/// <returns>The file content</returns>
		public string ReadTextFile(string fileName)
		{
			return ReadZipTextFile(this._filePath, fileName);
		}

		/// <summary>
		/// Reads a text file from archive without extraction to disk.
		/// </summary>
		/// <param name="singleFileSelector">A single file selector. If the selector matched more than one file it throws an exception.</param>
		/// <returns>The file content</returns>
		public string ReadTextFile(Func<ZipArchiveEntry, bool> singleFileSelector)
		{
			return ReadZipTextFile(this._filePath, singleFileSelector);
		}
    }
}
