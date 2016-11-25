using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace OpenVPNConfigFileChecker
{
    // For Directory.GetFiles and Directory.GetDirectories
    // For File.Exists, Directory.Exists
    using System;
    using System.IO;
    using System.Collections;

    public class RecursiveFileProcessor
    {
        public static void Main(string[] args)
        {
            foreach (string path in args)
            {
                if (File.Exists(path))
                {
                    // This path is a file
                    ProcessFile(path);
                }
                else if (Directory.Exists(path))
                {
                    // This path is a directory
                    ProcessDirectory(path);
                }
                else
                {
                    Console.WriteLine("{0} is not a valid file or directory.", path);
                }
            }
        }


        // Process all files in the directory passed in, recurse on any directories 
        // that are found, and process the files they contain.
        public static void ProcessDirectory(string targetDirectory)
        {
            // Process the list of files found in the directory.
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            foreach (string fileName in fileEntries)
                ProcessFile(fileName);

            // Recurse into subdirectories of this directory.
            string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
            foreach (string subdirectory in subdirectoryEntries)
                ProcessDirectory(subdirectory);
        }

        // Insert logic for processing found files here.
        public static void ProcessFile(string path)
        {
            Regex ipRegex = new Regex(@"((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)");
            // Only check OpenVPN config files that contain an IP address in the file name
            if (Path.GetExtension(path) == ".ovpn" && ipRegex.Match(path).Success)
            {
                string ipAddress = ipRegex.Match(path).Value;
                Console.WriteLine(ipAddress);
                //Console.WriteLine("Processed file '{0}'.", path);
            }
        }
    }
}
