using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using dat_console.Utilities;
using dat_console.Main;
using Newtonsoft.Json;

namespace dat_console
{
    class Program
    {
        //private static string datExt = ".bnk";
        private static string datExt = ".bin";
        //private static string datExt = ".dat";
        //private static string directory = "C:/Users/David Matos/Desktop/RESET/copiedInstalledData/Theory Interactive/Reset Greenlight Demo";
        //private static string directory = "C:/Users/David Matos/Desktop/RESET/from-will-germany";
        //private static string directory = "C:/Users/David Matos/Desktop/RESET/from-will-germany/Reset_1";
        //private static string directory = "C:/Users/David Matos/Desktop/RESET/from-will-germany/Reset_0/atmosphere/default";
        //private static string directory = "C:/Users/David Matos/Desktop/RESET/from-will-germany/Reset_0/audio/English(US)";
        private static string directory = "C:/Users/David Matos/Desktop/RESET/copiedInstalledData/Theory Interactive/Reset Greenlight Demo/Reset_1";
        private static string cachedBlobDirectory = "C:/Users/David Matos/Desktop/RESET/copiedInstalledData/Theory Interactive/Reset Greenlight Demo/Reset_0/cached_blob";

        private static void Main(string[] args)
        {
            GetFilesInDirectory();
        }

        private static void GetFilesInDirectory()
        {
            CachedBlobFolder cachedBlobFolder = new CachedBlobFolder(cachedBlobDirectory);

            return;

            //gather the files from the folder path into an array
            List<string> files = new(Directory.GetFiles(directory));

            ConsoleFunctions.SetConsoleColor(ConsoleColor.Black, ConsoleColor.Yellow);
            Console.WriteLine("Filtering Files..."); //notify the user we are filtering the array

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

                ReadFile(file);

                ConsoleFunctions.SetConsoleColor(ConsoleColor.Black, ConsoleColor.Green);
                Console.WriteLine("Finished converting '{0}'...", fileName); //notify the user we finished converting 'x' file.
                ConsoleFunctions.SetConsoleColor(ConsoleColor.Black, ConsoleColor.White);
            }
        }

        public static void ReadFile(string filePath)
        {
            //WalkBoxes_Master walkBoxes = new(filePath);

            //if (writeJSON)
            //    WriteJSON(filePath, walkBoxes);

            using(BinaryReader reader = new BinaryReader(File.OpenRead(filePath)))
            {
                //DAT_File dataFile = new DAT_File(reader, filePath);
                //BIN_File binFile = new BIN_File(reader, filePath);
                //BNK_File bnkFile = new BNK_File(reader, filePath);
            }

        }

        /*
        public static void WriteJSON(string originalFilePath, WalkBoxes_Master walkBoxes)
        {
            string fileExt = Path.GetExtension(originalFilePath);
            string jsonPath = originalFilePath.Remove(originalFilePath.Length - fileExt.Length, fileExt.Length) + ".json";

            if (File.Exists(jsonPath))
                File.Delete(jsonPath);

            //open a stream writer to create the text file and write to it
            using (StreamWriter file = File.CreateText(jsonPath))
            {
                //get our json seralizer
                JsonSerializer serializer = new();

                List<object> serializedObjects = new();
                serializedObjects.Add(walkBoxes.Get_Meta_Object());
                serializedObjects.Add(walkBoxes.walkboxes);

                //seralize the data and write it to the configruation file
                serializer.Formatting = Formatting.Indented;
                serializer.Serialize(file, serializedObjects);
            }
        }
        */
    }
}
