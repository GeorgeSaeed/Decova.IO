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
		private string _filePath;
		public Zip(string filePath)
		{
			this._filePath = filePath;
		}

		public string ReadEntryAllText(Func<ZipArchiveEntry, bool> singleFileSelector, Encoding encoding = null)
		{
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
			string entryEncoded = ReadEntryAllText(singleFileSelector, null);
			return Encoding.Unicode.GetBytes(entryEncoded);
		}

		public void AddFileEntry(string entryfilePath, string entryName)
		{
			using (var archive = ZipFile.Open(this._filePath, ZipArchiveMode.Update))
			{
				archive.CreateEntryFromFile(entryfilePath, entryName);
			}

		}

		public IEnumerable<string> ExtractFiles(Func<ZipArchiveEntry, bool> fileSelector, string destinationDirectory)
		{
			List<string> destinationPaths = new List<string>();
			using (ZipArchive archive = ZipFile.OpenRead(this._filePath))
			{
				foreach (var entry in archive.Entries.Where(e => fileSelector(e)))
				{
					// Gets the full path to ensure that relative segments are removed.
					string destinationPath = Path.GetFullPath(Path.Combine(destinationDirectory, entry.FullName));

					entry.ExtractToFile(destinationPath);
					destinationPaths.Add(destinationPath);
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
