using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

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
            Console.WriteLine("Successful connection attempts: ");
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
            // Only check OpenVPN config files
            if (Path.GetExtension(path) == ".ovpn")
            {
                // Start the openvpn process and output files names where connection was successful
                System.Diagnostics.Process p = new System.Diagnostics.Process();

                p.StartInfo.FileName = "openvpn.exe";
                p.StartInfo.Arguments = path;

                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;

                p.OutputDataReceived += new DataReceivedEventHandler((sender, e) =>
                {
                    if (e.Data != null && e.Data.Contains("VERIFY OK: depth=0,"))
                    {
                        Console.WriteLine(path);
                    }
                });

                try
                {
                    p.Start();
                    p.BeginOutputReadLine();

                    // Kill after 1.3s
                    System.Threading.Thread.Sleep(1300);
                    p.Kill();
                }
                catch (Exception)
                {
                    throw new Exception("OpenVPN not installed or not in PATH environment variable.");
                }  
            }
        }
    }
}
