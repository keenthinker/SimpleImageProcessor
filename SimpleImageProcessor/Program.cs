using System.Configuration;

namespace SimpleImageProcessor
{
    class Program
    {
        static void Main(string[] args)
        {
            var baseDirectory = ConfigurationManager.AppSettings["baseDirectory"];
            var fileFilter = ConfigurationManager.AppSettings["fileFilter"];

            var watcher = new DirectoryWatcher(baseDirectory, fileFilter);
            watcher.VerifyInputAndRunImageProcessor();
        }
    }
}
