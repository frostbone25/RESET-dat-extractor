using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using RESET_Tools.Utilities;

namespace RESET_Tools.DataArchives
{
    public struct DataArchiveFileEntry
    {
        public string FileName { get; set; }
        public uint FileOffsetStart { get; set; }
        public uint Unknown1 { get; set; }
        public uint FileDataSize { get; set; }
        public uint Unknown2 { get; set; }
        public byte[] FileData { get; set; }

        public DataArchiveFileEntry(BinaryReader reader)
        {
            FileName = ByteFunctions.ReadWideString(reader);
            FileOffsetStart = reader.ReadUInt32();
            Unknown1 = reader.ReadUInt32();
            FileDataSize = reader.ReadUInt32();
            Unknown2 = reader.ReadUInt32();
            FileData = null;
        }

        public void ParseFileData(BinaryReader reader)
        {
            reader.BaseStream.Seek(FileOffsetStart, SeekOrigin.Begin);
            FileData = reader.ReadBytes((int)FileDataSize);
        }
    }
}
