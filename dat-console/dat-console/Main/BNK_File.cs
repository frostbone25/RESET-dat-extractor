using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using dat_console.Utilities;

namespace dat_console.Main
{
    public class BNK_File
    {
        public struct BKHD
        {
            public string SectionName { get; set; } //4 char section identifer
            public uint SectionLength { get; set; } //length of section
            public uint VersionNumber { get; set; } //version number of this SoundBank
            public uint ID { get; set; } //id of this SoundBank
            public uint Unknown1 { get; set; }
            public uint Unknown2 { get; set; }
            public uint Unknown3 { get; set; }
            public uint Unknown4 { get; set; }
            public uint Unknown5 { get; set; }
            public uint Unknown6 { get; set; }

            public override string ToString()
            {
                return string.Format("[BKHD] SectionLength: {0} VersionNumber: {1} ID: {2} Unknown1: {3} Unknown2: {4} Unknown3: {5} Unknown4: {6} Unknown5: {7} Unknown6: {8}", SectionLength, VersionNumber, ID, Unknown1, Unknown2, Unknown3, Unknown4, Unknown5, Unknown6);
            }
        }

        public struct WEM
        {
            public uint FileID { get; set; }
            public uint DataOffset { get; set; }
            public uint DataSize { get; set; }
            public byte[] Data { get; set; }

            public override string ToString()
            {
                return string.Format("[WEM] FileID: {0} DataOffset: {1} DataSize: {2}", FileID, DataOffset, DataSize);
            }
        }

        public struct DIDX
        {
            public string SectionName { get; set; } //4 char section identifer
            public uint SectionLength { get; set; } //length of section (Each sound file is described with 12 bytes, so you can get the number of embedded files by dividing the section length by 12.)
            public WEM[] WemEntires { get; set; }

            public override string ToString()
            {
                return string.Format("[DIDX] SectionLength: {0} WemEntires: {1} ", SectionLength, WemEntires.Length);
            }
        }

        public BNK_File(BinaryReader reader, string path)
        {
            string mainDirectoryName = Path.GetFileNameWithoutExtension(path);
            string archiveRootDirectory = Path.GetDirectoryName(path) + "/" + mainDirectoryName;

            long fileSize = reader.BaseStream.Length;

            if (!Directory.Exists(archiveRootDirectory))
                Directory.CreateDirectory(archiveRootDirectory);

            //-------------START OF FILE-------------
            //----------------BKHD---------------
            BKHD BKHD_Object = new BKHD()
            {
                SectionName = Encoding.ASCII.GetString(reader.ReadBytes(4)),
                SectionLength = reader.ReadUInt32(),
                VersionNumber = reader.ReadUInt32(),
                ID = reader.ReadUInt32(),
                Unknown1 = reader.ReadUInt32(),
                Unknown2 = reader.ReadUInt32(),
                Unknown3 = reader.ReadUInt32(),
                Unknown4 = reader.ReadUInt32(),
                Unknown5 = reader.ReadUInt32(),
                Unknown6 = reader.ReadUInt32()
            };

            Console.WriteLine("{0}", BKHD_Object.SectionName);
            Console.WriteLine("{0}", BKHD_Object);

            //----------------DIDX---------------
            DIDX DIDX_Object = new DIDX();
            DIDX_Object.SectionName = Encoding.ASCII.GetString(reader.ReadBytes(4));
            DIDX_Object.SectionLength = reader.ReadUInt32();

            //Each sound file is described with 12 bytes, so you can get the number of embedded files by dividing the section length by 12.
            int WemEntriesCount = (int)DIDX_Object.SectionLength / 12;
            DIDX_Object.WemEntires = new WEM[WemEntriesCount];

            Console.WriteLine("{0}", DIDX_Object.SectionName);
            Console.WriteLine("{0}", DIDX_Object);

            for (int i = 0; i < DIDX_Object.WemEntires.Length; i++)
            {
                DIDX_Object.WemEntires[i] = new WEM()
                {
                    FileID = reader.ReadUInt32(),
                    DataOffset = reader.ReadUInt32(),
                    DataSize = reader.ReadUInt32()
                };

                Console.WriteLine("{0}", DIDX_Object.WemEntires[i]);
            }

            //----------------DATA---------------
            string Data_SectionName = Encoding.ASCII.GetString(reader.ReadBytes(4));
            uint Data_SectionLength = reader.ReadUInt32();

            for (int i = 0; i < DIDX_Object.WemEntires.Length; i++)
            {
                //reader.BaseStream.Seek(DIDX_Object.WemEntires[i].DataOffset, SeekOrigin.Begin);
                DIDX_Object.WemEntires[i].Data = reader.ReadBytes((int)DIDX_Object.WemEntires[i].DataSize);

                string newWemFile = archiveRootDirectory + "/" + DIDX_Object.WemEntires[i].FileID + ".wem";
                File.WriteAllBytes(newWemFile, DIDX_Object.WemEntires[i].Data);
            }

            /*
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
            */
        }
    }
}
