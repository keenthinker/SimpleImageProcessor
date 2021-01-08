using System;
using System.IO;

namespace SimpleImageProcessor
{
	/// <summary>
	/// Project extensions
	/// </summary>
    public static class ProjectExtensions
    {
		/// <summary>
		/// Move the specified file to the specified directory.
		/// </summary>
		/// <param name="file">The full path, including name, of the file that should be moved to the specified directory</param>
		/// <param name="directory">The directory in which the file should be moved</param>
		public static void MoveTo(this string file, string directory)
		{
			try
			{
				var filename = (new FileInfo(file)).Name;
				var toNewLocation = Path.Combine(directory, filename);
				File.Move(file, toNewLocation);
				Console.WriteLine($"Moved '{filename}' to '{toNewLocation}'");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Ugh ... an error occurred while moving {file} to {directory}: {ex.Message}");
			}
		}
	}
}
