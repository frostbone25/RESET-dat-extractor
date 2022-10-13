using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using RESET_Tools.Utilities;
using RESET_Tools.DataArchives;

namespace RESET_Tools
{
    class Program
    {
        private static string datExt = ".dat";

        private static void Main(string[] args)
        {
            //if there are no arguments, then tell the user how ot use it
            if(args.Length <= 0)
            {
                Console.WriteLine("Usage: [inputDirectory]");
                Console.WriteLine("Searches the directory for .dat files and automatically extracts all of them.");

                return;
            }

            //get the first argument given
            string firstArguemnt = args[0];

            //if the directory exists
            if(Directory.Exists(firstArguemnt))
            {
                //extract the dat files in the directory
                ExtractFilesInDirectory(firstArguemnt);
            }
            else
            {
                Console.WriteLine("Input directory doesn't exist...");
            }
        }

        /// <summary>
        /// Searches the directory for .dat files and extracts them.
        /// </summary>
        /// <param name="directoryPath"></param>
        private static void ExtractFilesInDirectory(string directoryPath)
        {
            //gather the files from the folder path into an array
            List<string> files = new List<string>(Directory.GetFiles(directoryPath));

            //filter the array so we only get .d3dtx files
            files = IOManagement.FilterFiles(files, datExt);

            //if no d3dtx files were found, abort the program from going on any further (we don't have any files to convert!)
            if (files.Count < 1)
            {
                ConsoleFunctions.SetConsoleColor(ConsoleColor.Black, ConsoleColor.Red);
                Console.WriteLine("No {0} files were found, aborting.", datExt);
                Console.ResetColor();
                return;
            }

            ConsoleFunctions.SetConsoleColor(ConsoleColor.Black, ConsoleColor.Green);
            Console.WriteLine("Found {0} Dat files.", files.Count.ToString()); //notify the user we found x amount of files in the array
            Console.WriteLine("Starting...");//notify the user we are starting
            Console.ResetColor();

            //run a loop through each of the found textures and convert each one
            foreach (string file in files)
            {
                //build the path for the resulting file
                string fileName = Path.GetFileName(file); //get the file name of the file + extension

                ConsoleFunctions.SetConsoleColor(ConsoleColor.Black, ConsoleColor.White);
                Console.WriteLine("||||||||||||||||||||||||||||||||");
                ConsoleFunctions.SetConsoleColor(ConsoleColor.Black, ConsoleColor.Blue);
                Console.WriteLine("Converting '{0}'...", fileName); //notify the user are converting 'x' file.
                Console.ResetColor();

                //extract the current file
                ExtractFile(file);

                ConsoleFunctions.SetConsoleColor(ConsoleColor.Black, ConsoleColor.Green);
                Console.WriteLine("Finished converting '{0}'...", fileName); //notify the user we finished converting 'x' file.
                ConsoleFunctions.SetConsoleColor(ConsoleColor.Black, ConsoleColor.White);
            }
        }

        /// <summary>
        /// Extracts a dat file on the disk.
        /// </summary>
        /// <param name="filePath"></param>
        public static void ExtractFile(string filePath)
        {
            DataArchive dataFile = null;

            //parse the data archive file
            using (BinaryReader reader = new BinaryReader(File.OpenRead(filePath)))
            {
                dataFile = new DataArchive(reader);
            }

            //if its valid, then write the file entires to the disk.
            if(dataFile != null)
                dataFile.WriteEntriesToDisk(filePath);
        }
    }
}
