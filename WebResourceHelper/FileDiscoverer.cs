using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CrmCommandLineHelpersCore.CommandLine;

namespace WebResourceHelper
{
    class WebResourceFile
    {
        public string RelativePath;
        public string Extension;
        public string Base64Content;
    }

    class FileDiscoverer
    {
        private readonly WebResourceHelperOptions webResourceHelperOptions;

        public FileDiscoverer(WebResourceHelperOptions webResourceHelperOptions)
        {
            this.webResourceHelperOptions = webResourceHelperOptions;
        }

        public IEnumerable<WebResourceFile> DiscoverFiles()
        {
            if (webResourceHelperOptions.RootPath == null)
            {
                throw new Exception("The root path must be provided as a command line argument");
            }

            var rootDirectory = new DirectoryInfo(webResourceHelperOptions.RootPath);

            if (!rootDirectory.Exists)
            {
                throw new Exception($"The root directory does not exist ({rootDirectory.FullName})");
            }

            var files = rootDirectory.EnumerateFiles("*", SearchOption.AllDirectories)
                                     .Where(f => WebResourceExtensionUtilities.ValidExtensions.Contains(f.Extension)).ToList();

            ConsoleHelper.WriteLine($"{files.Count} files have been found in the directory with valid file extensions.", logLevel: LogLevel.Debug);
            ConsoleHelper.WriteLine(logLevel: LogLevel.Debug);

            return files.Select(f => FileInfoToWebResourceFileInfo(rootDirectory, f));
        }

        private WebResourceFile FileInfoToWebResourceFileInfo(DirectoryInfo rootDirectory, FileInfo file)
        {
            return new WebResourceFile
                   {
                       RelativePath = GetRelativePath(rootDirectory, file),
                       Extension = file.Extension,
                       Base64Content = GetEncodedFileContents(file)
                   };
        }

        private string GetRelativePath(DirectoryInfo rootDirectory, FileInfo file)
        {
            var fullDirectory = rootDirectory.FullName;
            var fullFile = file.FullName;

            if (!fullFile.StartsWith(fullDirectory))
            {
                throw new Exception($"Could not resolve a relative path for file: {fullFile} from root directory {fullDirectory}");
            }
            // The +1 is to avoid the directory separator
            return fullFile.Substring(fullDirectory.Length + 1);
        }

        private string GetEncodedFileContents(FileInfo file)
        {
            byte[] binaryData;
            using (var fs = file.OpenRead())
            {
                binaryData = new byte[fs.Length];
                fs.Read(binaryData, 0, (int)fs.Length);
            }
            return Convert.ToBase64String(binaryData, 0, binaryData.Length);
        }
    }
}
