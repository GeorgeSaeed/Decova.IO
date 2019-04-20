using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using YouSubtle.Tasking;

namespace YouSubtle.IO
{
	public class Zip
	{
		private readonly string _filePath;
		public Zip(string filePath)
		{
			this._filePath = filePath;
		}

		public string ReadEntryAllText(Func<ZipArchiveEntry, bool> singleFileSelector, 
                                       Encoding encoding = null)
		{
            if (singleFileSelector == null) throw new ArgumentNullException("singleFileSelector");

            string allText;

			var archive = ZipFile.Open(this._filePath, ZipArchiveMode.Read);
			var archiveEntry = archive.Entries.Single(e => singleFileSelector(e));

			using (var stream = archiveEntry.Open())
				if (encoding == null)
				{
					using (var entryReader = new StreamReader(stream, true))
					{
						allText = entryReader.ReadToEnd();
					}
				}
				else
				{
					using (var entryReader = new StreamReader(stream, encoding))
					{
						allText = entryReader.ReadToEnd();
					}
				}

			return allText;
		}

		public byte[] ReadEntryAllBytes(Func<ZipArchiveEntry, bool> singleFileSelector)
		{
            if (singleFileSelector == null) throw new ArgumentNullException("singleFileSelector");

            string entryEncoded = ReadEntryAllText(singleFileSelector, null);
			return Encoding.Unicode.GetBytes(entryEncoded);
		}

		public void AddFileEntry(string entryfilePath, string entryName)
		{

            if (string.IsNullOrWhiteSpace(entryfilePath)) throw new ArgumentException("entryfilePath cannot be null or white space.");
            if (File.Exists(entryfilePath) == false) throw new Exception($"File [{entryfilePath}] not found.");

			using (var archive = ZipFile.Open(this._filePath, ZipArchiveMode.Update))
			{
				archive.CreateEntryFromFile(entryfilePath, entryName);
			}

		}

		public IEnumerable<string> ExtractFiles(string destinationDirectory,
                                                bool overwriteExistingFiles,
                                                Func<ZipArchiveEntry, bool> entryFilter = null)
		{
			List<string> destinationPaths = new List<string>();
			using (ZipArchive archive = ZipFile.OpenRead(this._filePath))
			{
				foreach (var entry in archive.Entries.FilterIf(entryFilter!=null, e => entryFilter(e)))
				{
					// Gets the full path to ensure that relative segments are removed.
					string destinationPath = System.IO.Path.GetFullPath(Path.Combine(destinationDirectory, entry.FullName));

                    if(overwriteExistingFiles || !File.Exists(destinationPath))
                    {
					    entry.ExtractToFile(destinationPath);
					    destinationPaths.Add(destinationPath);
                    }
				}
			}
			return destinationPaths;
		}

		public void ExtractFile(Func<ZipArchiveEntry, bool> singleFileSelector, string destinationPath)
		{
			using (ZipArchive archive = ZipFile.OpenRead(this._filePath))
			{
				var entry = archive.Entries.Single(e => singleFileSelector(e));
				entry.ExtractToFile(destinationPath);
			}
		}

		public void AddBytesEntry(string entryName, byte[] bytes)
		{
			using (var archive = ZipFile.Open(this._filePath, ZipArchiveMode.Update))
			{
				var entry = archive.CreateEntry(entryName);
				using (var stream = entry.Open())
				{
					stream.Write(bytes, 0, bytes.Length);
					stream.Flush();
				}

			}
		}
	}

}
