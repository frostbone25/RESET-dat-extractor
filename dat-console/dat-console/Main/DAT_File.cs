using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using dat_console.Utilities;

namespace dat_console.Main
{
    /// <summary>
    /// This is a class that reads, parses, and extracts a Reset .dat file.
    /// Note: This class works fully.
    /// </summary>
    public class DAT_File
    {
        public struct DataFileEntry
        {
            public string FileName { get; set; }
            public uint FileOffsetStart { get; set; }
            public uint Unknown1 { get; set; }
            public uint FileDataSize { get; set; }
            public uint Unknown2 { get; set; }
            public byte[] FileData { get; set; }
        }

        public DAT_File(BinaryReader reader, string path)
        {
            string mainDirectoryName = Path.GetFileNameWithoutExtension(path);
            string archiveRootDirectory = Path.GetDirectoryName(path) + "/" + mainDirectoryName;

            long fileSize = reader.BaseStream.Length;

            if (!Directory.Exists(archiveRootDirectory))
                Directory.CreateDirectory(archiveRootDirectory);

            uint entryCount = reader.ReadUInt32(); //START OF FILE
            Console.WriteLine("Entry Count = {0}", entryCount);

            DataFileEntry[] dataFileEntires = new DataFileEntry[entryCount];

            for(uint i = 0; i < entryCount; i++)
            {
                DataFileEntry entry = new DataFileEntry();
                entry.FileName = ByteFunctions.ReadWideString(reader);
                entry.FileOffsetStart = reader.ReadUInt32();
                entry.Unknown1 = reader.ReadUInt32();
                entry.FileDataSize = reader.ReadUInt32();
                entry.Unknown2 = reader.ReadUInt32();

                dataFileEntires[i] = entry;
            }

            for(int i = 0; i < dataFileEntires.Length; i++)
            {
                DataFileEntry entry = dataFileEntires[i];

                reader.BaseStream.Seek(entry.FileOffsetStart, SeekOrigin.Begin);
                entry.FileData = reader.ReadBytes((int)entry.FileDataSize);

                string newFilePath = archiveRootDirectory + "/" + entry.FileName;

                if (!Directory.Exists(Path.GetDirectoryName(newFilePath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(newFilePath));

                File.WriteAllBytes(newFilePath, entry.FileData);
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
