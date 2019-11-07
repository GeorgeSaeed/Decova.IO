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


		public string ReadEntryAllText(Func<ZipArchiveEntry, bool> singleEntrySelector, 
                                       Encoding encoding = null)
		{
            if (singleEntrySelector == null) throw new ArgumentNullException("singleFileSelector");

            string allText;

			var archive = ZipFile.Open(this._filePath, ZipArchiveMode.Read);
			var archiveEntry = archive.Entries.Single(e => singleEntrySelector(e));

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

                    // empty string is for a folder
                    if(entry.Name != "")
                    {
                        new FileInfo(destinationPath).Directory.Ensure();
                        if (overwriteExistingFiles || !File.Exists(destinationPath))
                        {
                            entry.ExtractToFile(destinationPath);
					        destinationPaths.Add(destinationPath);
                        }

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

        public enum AddEntryFromFileResult
        {
            Exception,
            Failure_SimilarlyEntryAlreadyExists,
            Success_EntryAdded,
            Success_ReplacedSimilarlyNamedTntry
        }
        /// <summary>
        /// Adds a new entry from file. If replaceExistingEntryIfAny argument is passed as true it will replace
        /// an existing entry with the same name if any. 
        /// </summary>
        /// <param name="entryName">The relative file path to archive.</param>
        /// <param name="newFilePath">The new file path.</param>
        /// <param name="replaceExistingEntryIfAny">True if you want to replace an existing entry if any. False to fail if there is an already existing similarly named entity</param>
        /// <param name="exception">The exception raised from underlying libraries. You need to check it only if the return value is Exception</param>
        /// <returns></returns>
        public AddEntryFromFileResult AddFile(string entryName, 
                                              string newFilePath, 
                                              bool replaceExistingEntryIfAny, 
                                              out Exception exception)
        {
            if (string.IsNullOrWhiteSpace(newFilePath)) throw new ArgumentException("newFilePath cannot be null or white space.");
            if (File.Exists(newFilePath) == false) throw new Exception($"File [{newFilePath}] not found.");

            try
            {
                using (var archive = ZipFile.Open(this._filePath, ZipArchiveMode.Update))
                {
                    ZipArchiveEntry oldEntry = archive.GetEntry(entryName);

                    if (oldEntry == null)
                    {
                        archive.CreateEntryFromFile(newFilePath, entryName);
                        exception = null;
                        return AddEntryFromFileResult.Success_EntryAdded;
                    }
                    else
                    {
                        if (replaceExistingEntryIfAny)
                        {
                            if (oldEntry != null) oldEntry.Delete();
                            archive.CreateEntryFromFile(newFilePath, entryName);
                            exception = null;
                            return AddEntryFromFileResult.Success_ReplacedSimilarlyNamedTntry;
                        }
                        else
                        {
                            exception = null;
                            return AddEntryFromFileResult.Failure_SimilarlyEntryAlreadyExists;
                        }
                    }
                }
                    
            }
            catch(Exception expt)
            {
                exception = new Exception("Couldn't add entry to zip archive!", expt);
                return AddEntryFromFileResult.Exception;
            }
        }
	}

}
