using System;
using System.IO;

namespace SimpleImageProcessor
{
	/// <summary>
	/// Watch the configured directory for added files with the specified file extension and handle them.
	/// </summary>
	public class DirectoryWatcher
    {
		/// <summary>
		/// Name of the subdirectory in which a modified copy of the original file is saved
		/// </summary>
		private const string DONE = "done";

		/// <summary>
		/// Name of the subdirectory in which the original, unmodified, files are moved, after the processing to the copy is done 
		/// </summary>
		private const string ORIGINAL = "original";

		/// <summary>
		/// Directory to watch for new files with the specified extension through fileFilter.
		/// </summary>
		private string baseDirectory { get; set; }

		/// <summary>
		/// Specifies the files to watch for, e.g.: *.txt or *.png
		/// </summary>
		private string fileFilter { get; set; }

		/// <summary>
		/// Checks if the specified subdirectory exists. If not, then it is created.
		/// </summary>
		/// <param name="directoryName">Name of the sub directory to check for existance.</param>
		/// <returns><c>True</c> if the specified subdirectory exists or it was successfully created; otherwise <c>false</c></returns>
		private bool subDirectoryExists(string directoryName)
        {
			var success = true;

			var fullPath = Path.Combine(baseDirectory, directoryName);
			if (!Directory.Exists(fullPath))
			{
                try
                {
					Directory.CreateDirectory(fullPath);
                }
                catch (Exception ex)
                {
					Console.WriteLine($"Ugh ... an error occurred while creating {fullPath}: {ex.Message}");
					success = false;
				}
			}

			return success;
        }

		public DirectoryWatcher(string baseDirectory, string fileFilter)
		{
			this.baseDirectory = baseDirectory;
			this.fileFilter = fileFilter;
		}

		/// <summary>
		/// Checks if the directory exists, creates the needed subfolders it they do not exist and starts to watch for files to process. 
		/// </summary>
		public void VerifyInputAndRunImageProcessor()
		{
			if (Directory.Exists(baseDirectory) && subDirectoryExists(DONE) && subDirectoryExists(ORIGINAL))
			{
				watch();
				Console.WriteLine($"Bye bye");
			}
			else 
			{
				Console.WriteLine($"The specified directory '{baseDirectory}' does not exist");
			}
		}

		/// <summary>
		/// Creates, configures and starts a FileSystemWatcher
		/// </summary>
		private void watch()
		{
			using (FileSystemWatcher watcher = new FileSystemWatcher())
			{
				watcher.Path = baseDirectory;
				watcher.NotifyFilter = NotifyFilters.LastAccess
									 | NotifyFilters.LastWrite
									 | NotifyFilters.FileName
									 | NotifyFilters.DirectoryName;

				watcher.Filter = fileFilter;
				watcher.IncludeSubdirectories = false;
				watcher.Created += OnCreated;
				watcher.EnableRaisingEvents = true;

				Console.WriteLine($"Watching '{baseDirectory}' for new '{fileFilter}' images. \r\nPress 'q' to quit.");
				while (Console.Read() != 'q');
			}
		}

		/// <summary>
		/// Handler for the created event. 
		/// </summary>
		/// <param name="source"><c>object</c></param>
		/// <param name="e"><c>FileSystemEventArgs</c></param>
		private void OnCreated(object source, FileSystemEventArgs e)
		{
			Console.WriteLine($"File '{e.FullPath}' added to directory");
			processImage(e.FullPath);
		}

		/// <summary>
		/// Apply the image transformations with the specified image file as source.
		/// </summary>
		/// <param name="file">Full path to the image file that should be processed</param>
		private void processImage(string file)
		{
			var imageProcessor = new ImageProcessor();
			imageProcessor.AddRoundCornersAndSave(file, subDirectory(DONE));
			file.MoveTo(subDirectory(ORIGINAL));
		}

		/// <summary>
		/// Creates a string containing the full path to the specified subdirectory starting from the globaly specified base directory
		/// </summary>
		/// <param name="subDirectory">Name of the sub directory</param>
		/// <returns>String representing the path to the specified subdirectory</returns>
		private string subDirectory(string subDirectory)
		{
			return Path.Combine(baseDirectory, subDirectory);
		}
	}
}
