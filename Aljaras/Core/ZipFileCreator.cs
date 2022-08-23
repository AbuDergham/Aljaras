using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aljaras.Core
{
    public class ZipFileCreator
    {
        /// <summary>
        /// Create a ZIP file of the files provided.
        /// </summary>
        /// <param name="destinationFileName">The full path and name to store the ZIP file at.</param>
        /// <param name="sourceFiles">The list of files to be added.</param>
        public void CreateZipFile(string destinationFileName, List<string> sourceFiles)
        {
            // Create and open a new ZIP file
            var zip = ZipFile.Open(destinationFileName, ZipArchiveMode.Create);
            foreach (var file in sourceFiles)
            {
                // Add the entry for each file
                zip.CreateEntryFromFile(file, Path.GetFileName(file), CompressionLevel.Fastest);
            }
            // Dispose of the object when we are done
            zip.Dispose();
        }
    }
}
