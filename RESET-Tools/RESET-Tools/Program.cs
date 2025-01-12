//#define DEBUG_EXTRACTION_MODE
#define DEBUG_REPACK_MODE
//#define DEBUG_WORLD_PARSE

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using RESET_Tools.Utilities;
using RESET_Tools.ResetDataArchives;
using System.Text.RegularExpressions;

namespace RESET_Tools
{
    class Program
    {
        private static string datExt = ".dat";
        private static int modeType = -1;

        private static void Main(string[] args)
        {

#if DEBUG_WORLD_PARSE
            ParseWorldFile("D:/ResetGame/Reset_1/world.bin");
            Console.ReadLine();
            return;
#endif

#if (!DEBUG_EXTRACTION_MODE && !DEBUG_REPACK_MODE)
            //if there are no arguments, then tell the user how to use it
            if (args.Length != 2)
            {
                ConsoleWriter.WriteLine("Usage For Extraction: extract [inputDirectory]");
                ConsoleWriter.WriteLine("Usage For Repacking: repack [inputDirectory]");
                ConsoleWriter.WriteLine("Searches the directory for .dat files and automatically extracts all of them.");

                return;
            }

            //get the first argument given
            string firstArguement = args[0];
            string secondArguement = args[1];
#else

#if DEBUG_EXTRACTION_MODE
            string firstArguement = "extract";
    string secondArguement = "D:/ResetGame";
#endif

#if DEBUG_REPACK_MODE
    string firstArguement = "repack";
    string secondArguement = "D:/ResetGame";
#endif

#endif

            if (string.IsNullOrEmpty(firstArguement) || string.IsNullOrWhiteSpace(firstArguement))
            {
                ConsoleWriter.WriteErrorLine("First argument unrecognized!");
                return;
            }

            if(string.Equals(firstArguement, "extract", StringComparison.OrdinalIgnoreCase))
            {
                ConsoleWriter.WriteLine("Extraction Mode!");
                modeType = 0;
            }
            else if (string.Equals(firstArguement, "repack", StringComparison.OrdinalIgnoreCase))
            {
                ConsoleWriter.WriteLine("Repack Mode!");
                modeType = 1;
            }
            else
            {
                ConsoleWriter.WriteErrorLine("Mode not recognized! Must be either 'extract' or 'repack'");
                return;
            }

            if (!Directory.Exists(secondArguement))
            {
                ConsoleWriter.WriteErrorLine("Input directory doesn't exist...");
                return;
            }

            switch (modeType)
            {
                case 0:
                    ExtractFilesInDirectory(secondArguement); //extract the dat files in the directory
                    break;
                case 1:
                    RepackDirectoriesInDirectory(secondArguement);
                    break;
            }

            ConsoleWriter.WriteSuccessLine("Application End!");

#if (DEBUG_EXTRACTION_MODE || DEBUG_REPACK_MODE)
            Console.ReadLine();
#endif
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

            //if no dat files were found, abort the program from going on any further (we don't have any files to convert!)
            if (files.Count < 1)
            {
                ConsoleWriter.WriteErrorLine(string.Format("No {0} files were found, aborting extraction.", datExt));
                return;
            }

            ConsoleWriter.WriteInfoLine(string.Format("Found {0} Dat files in... {1}", files.Count.ToString(), directoryPath)); //notify the user we found x amount of files in the array
            ConsoleWriter.WriteLine("Starting...");//notify the user we are starting

            //run a loop through each of the found textures and convert each one
            foreach (string file in files)
            {
                //build the path for the resulting file
                string fileName = Path.GetFileName(file); //get the file name of the file + extension

                ConsoleWriter.WriteLine("||||||||||||||||||||||||||||||||");
                ConsoleWriter.WriteInfoLine(string.Format("Reading '{0}'...", fileName)); //notify the user we are reading 'x' file.

                //extract the current file
                ExtractFile(file);

                ConsoleWriter.WriteSuccessLine(string.Format("Finished reading '{0}'...", fileName)); //notify the user we finished reading 'x' file.
            }
        }

        /// <summary>
        /// Extracts a dat file on the disk.
        /// </summary>
        /// <param name="filePath"></param>
        public static void ExtractFile(string filePath)
        {
            ResetDataArchive dataFile = null;

            //parse the data archive file
            using (BinaryReader reader = new BinaryReader(File.OpenRead(filePath)))
            {
                dataFile = new ResetDataArchive(reader);
            }

            //if its valid, then write the file entires to the disk.
            if(dataFile != null)
                dataFile.ExtractEntriesToDisk(filePath);
        }

        private static void RepackDirectoriesInDirectory(string directoryPath)
        {
            List<string> folders = new List<string>(Directory.GetDirectories(directoryPath));

            //if no folders were found, abort the program from going on any further (we don't have any folders to repack!)
            if (folders.Count < 1)
            {
                ConsoleWriter.WriteErrorLine(string.Format("No folders were found, aborting repack."));
                return;
            }

            ConsoleWriter.WriteInfoLine(string.Format("Found {0} folders in... {1}", folders.Count.ToString(), directoryPath)); //notify the user we found x amount of folders 
            ConsoleWriter.WriteLine("Starting...");//notify the user we are starting

            for (int i = 0; i < folders.Count; i++)
            {
                ConsoleWriter.WriteInfoLine(string.Format("Packing Folder... {0}", folders[i]));

                string currentFolderPath = folders[i];

                ResetDataArchive newDataArchive = new ResetDataArchive();

                string[] filesInFolder = Directory.GetFiles(currentFolderPath, "*.*", SearchOption.AllDirectories);

                //NOTE: In reset archive, files are sorted alphabetically, as for file sizes... not sure
                Array.Sort(filesInFolder);

                newDataArchive.DataArchiveFileEntries = new ResetDataArchiveFileEntry[filesInFolder.Length];

                for (int j = 0; j < filesInFolder.Length; j++)
                {
                    string currentFileInFolderPath = filesInFolder[j];
                    string simplifiedFilePath = currentFileInFolderPath.Remove(0, currentFolderPath.Length + 1);
                    byte[] currentFileData = File.ReadAllBytes(currentFileInFolderPath);

                    simplifiedFilePath = ReformatPathStringWithForwardSlashes(simplifiedFilePath);

                    newDataArchive.DataArchiveFileEntries[j] = new ResetDataArchiveFileEntry()
                    {
                        FileName = simplifiedFilePath,
                        FileData = currentFileData,
                        FileDataSize = (ulong)currentFileData.LongLength,
                    };

                    ConsoleWriter.WriteInfoLine(string.Format("({0, -5}/{1, -5}) Packed... {2}", j + 1, filesInFolder.Length, simplifiedFilePath));
                }

                ConsoleWriter.WriteInfoLine("Recalculating Offsets...");

                ulong entryTableEndOffset = newDataArchive.GetTableEndOffset() + 4;

                for (int j = 0; j < newDataArchive.DataArchiveFileEntries.Length; j++)
                {
                    newDataArchive.DataArchiveFileEntries[j].FileOffsetStart = entryTableEndOffset;
                    entryTableEndOffset += newDataArchive.DataArchiveFileEntries[j].FileDataSize;
                }

                ConsoleWriter.WriteInfoLine("Writing new archive to disk...");

                string newDataArchivePath = currentFolderPath + ".dat";
                newDataArchive.WriteArchiveToDisk(newDataArchivePath);
            }
        }

        private static void ParseWorldFile(string filePath)
        {
            using (BinaryReader reader = new BinaryReader(File.OpenRead(filePath)))
            {
                uint entries = reader.ReadUInt32();

                ConsoleWriter.WriteInfoLine(string.Format("{0} Entries...", entries));

                for (uint i = 0; i < entries; i++)
                {
                    string wideString = ByteFunctions.ReadWideString(reader);
                    uint unknown1 = reader.ReadUInt32();

                    ConsoleWriter.WriteInfoLine(string.Format("{0} {1}", wideString, unknown1));
                }

                ByteFunctions.ReachedEndOfFile((uint)reader.BaseStream.Position, (uint)reader.BaseStream.Length);
            }
        }

        private static string ReformatPathStringWithForwardSlashes(string initalString)
        {
            string reformattedString = initalString.Replace("\\", "/").Replace("//", "/");
            return Regex.Replace(reformattedString, "/{2,}", "/");
        }
    }
}
