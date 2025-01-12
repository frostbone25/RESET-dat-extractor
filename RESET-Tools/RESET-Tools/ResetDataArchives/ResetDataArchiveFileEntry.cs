using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using RESET_Tools.Utilities;

namespace RESET_Tools.ResetDataArchives
{
    public struct ResetDataArchiveFileEntry
    {
        public string FileName { get; set; }
        public ulong FileOffsetStart { get; set; }
        public ulong FileDataSize { get; set; }
        public byte[] FileData { get; set; }

        public ResetDataArchiveFileEntry(BinaryReader reader)
        {
            FileName = ByteFunctions.ReadWideString(reader);
            FileOffsetStart = reader.ReadUInt64();
            FileDataSize = reader.ReadUInt64();
            FileData = null;

            ConsoleWriter.WriteInfoLine(string.Format("[FileName]:{0} ", FileName));
            ConsoleWriter.WriteInfoLine(string.Format("[FileOffsetStart]: {0}", FileOffsetStart));
            ConsoleWriter.WriteInfoLine(string.Format("[FileDataSize]: {0}", FileDataSize));
        }

        public int GetEntryByteSize()
        {
            int result = 0;

            result = 4; //FileName String Length
            result += FileName.Length * 2; //FileName
            result += 8; //FileOffsetStart
            result += 8; //FileDataSize

            return result;
        }

        public void ParseFileData(BinaryReader reader)
        {
            reader.BaseStream.Seek((long)FileOffsetStart, SeekOrigin.Begin);
            FileData = reader.ReadBytes((int)FileDataSize);
        }

        public void WriteTableEntry(BinaryWriter writer)
        {
            ByteFunctions.WriteWideString(writer, FileName);
            writer.Write(FileOffsetStart);
            writer.Write(FileDataSize);
        }

        public void WriteFileData(BinaryWriter writer)
        {
            writer.Write(FileData);
        }
    }
}
