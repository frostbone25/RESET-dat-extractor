using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using RESET_Tools.Utilities;

namespace RESET_Tools.Experimental.Bin
{
    public class BinFile
    {
        public BinFile(BinaryReader reader, string path)
        {
            string mainDirectoryName = Path.GetFileNameWithoutExtension(path);
            string archiveRootDirectory = Path.GetDirectoryName(path) + "/" + mainDirectoryName;

            long fileSize = reader.BaseStream.Length;

            if (!Directory.Exists(archiveRootDirectory))
                Directory.CreateDirectory(archiveRootDirectory);

            uint entryCount = reader.ReadUInt32(); //START OF FILE
            Console.WriteLine("Entry Count = {0}", entryCount);

            BinFileEntry[] binFileEntires = new BinFileEntry[entryCount];

            for (uint i = 0; i < entryCount; i++)
            {
                BinFileEntry entry = new BinFileEntry();
                entry.FileName = ByteFunctions.ReadWideString(reader);
                entry.Unknown1 = reader.ReadUInt32();

                binFileEntires[i] = entry;
            }

            for (int i = 0; i < binFileEntires.Length; i++)
            {
                BinFileEntry entry = binFileEntires[i];
                Console.WriteLine("{0}", entry);

                //reader.BaseStream.Seek(entry.FileOffsetStart, SeekOrigin.Begin);
                //entry.FileData = reader.ReadBytes((int)entry.FileDataSize);

                //string newFilePath = archiveRootDirectory + "/" + entry.FileName;

                //if (!Directory.Exists(Path.GetDirectoryName(newFilePath)))
                //    Directory.CreateDirectory(Path.GetDirectoryName(newFilePath));

                //File.WriteAllBytes(newFilePath, entry.FileData);
            }

            if (fileSize == reader.BaseStream.Position)
            {
                ConsoleFunctions.SetConsoleColor(ConsoleColor.Black, ConsoleColor.Green);
                Console.WriteLine("File Size = {0}", fileSize);
                Console.WriteLine("Pointer left off at = {0}", reader.BaseStream.Position);
                Console.ResetColor();
            }
            else
            {
                ConsoleFunctions.SetConsoleColor(ConsoleColor.Black, ConsoleColor.Red);
                Console.WriteLine("File Size = {0}", fileSize);
                Console.WriteLine("Pointer left off at = {0}", reader.BaseStream.Position);
                Console.ResetColor();
            }
        }
    }
}
