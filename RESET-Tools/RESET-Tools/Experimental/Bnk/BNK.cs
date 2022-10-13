using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using RESET_Tools.Utilities;

namespace RESET_Tools.Experimental.Bnk
{
    public class BNK
    {
        public BNK(BinaryReader reader, string path)
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
