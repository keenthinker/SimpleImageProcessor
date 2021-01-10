using System.Configuration;

namespace SimpleImageProcessor
{
    class Program
    {
        static void Main(string[] args)
        {
            var baseDirectory = ConfigurationManager.AppSettings["baseDirectory"];
            var fileFilter = ConfigurationManager.AppSettings["fileFilter"];
            var cornerRadius = ConfigurationManager.AppSettings["cornerRadius"];

            var watcher = new DirectoryWatcher(baseDirectory, fileFilter, cornerRadius);
            watcher.VerifyInputAndRunImageProcessor();
        }
    }
}
