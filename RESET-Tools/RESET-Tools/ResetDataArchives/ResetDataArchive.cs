using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using RESET_Tools.Utilities;

namespace RESET_Tools.ResetDataArchives
{
    /// <summary>
    /// This is a class that reads, parses, and extracts a Reset .dat file.
    /// Note: This class works fully.
    /// </summary>
    public class ResetDataArchive
    {
        public ResetDataArchiveFileEntry[] DataArchiveFileEntries { get; set; }

        public ResetDataArchive()
        {

        }

        public ResetDataArchive(BinaryReader reader)
        {
            //start of the file
            uint entryCount = reader.ReadUInt32();

            ConsoleWriter.WriteInfoLine(string.Format("{0} file entries...", entryCount));

            //Build entry table
            DataArchiveFileEntries = new ResetDataArchiveFileEntry[entryCount];

            //parse each entry
            for (uint i = 0; i < entryCount; i++)
            {
                DataArchiveFileEntries[i] = new ResetDataArchiveFileEntry(reader);
            }

            //go back and parse the file data for each entry
            for (int i = 0; i < DataArchiveFileEntries.Length; i++)
            {
                DataArchiveFileEntries[i].ParseFileData(reader);
            }
        }

        public void ExtractEntriesToDisk(string originalDatPath)
        {
            //build the data archive directory on the disk so we can extract files to it
            string mainDirectoryName = Path.GetFileNameWithoutExtension(originalDatPath);
            string archiveRootDirectory = Path.GetDirectoryName(originalDatPath) + "/" + mainDirectoryName;

            //create the directory if it doesn't exist
            if (!Directory.Exists(archiveRootDirectory))
                Directory.CreateDirectory(archiveRootDirectory);

            //write each extracted file data to the disk with their respective file names
            for (int i = 0; i < DataArchiveFileEntries.Length; i++)
            {
                //build the path
                string newFilePath = archiveRootDirectory + "/" + DataArchiveFileEntries[i].FileName;

                //if the directory doesn't exist for the path then create one
                if (!Directory.Exists(Path.GetDirectoryName(newFilePath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(newFilePath));

                //write the file to the disk.
                File.WriteAllBytes(newFilePath, DataArchiveFileEntries[i].FileData);

                ConsoleWriter.WriteSuccessLine(string.Format("Extracted file to disk! {0}", newFilePath));
            }
        }

        public ulong GetTableEndOffset()
        {
            ulong entryTableEndOffset = 0;

            for(int i = 0; i < DataArchiveFileEntries.Length; i++)
            {
                entryTableEndOffset += (ulong)DataArchiveFileEntries[i].GetEntryByteSize();
            }

            return entryTableEndOffset;
        }

        public void WriteArchiveToDisk(string newArchivePath)
        {
            if(File.Exists(newArchivePath))
                File.Delete(newArchivePath);

            using (BinaryWriter writer = new BinaryWriter(File.Create(newArchivePath)))
            {
                writer.Write(DataArchiveFileEntries.Length);

                for(int i = 0; i < DataArchiveFileEntries.Length; i++)
                {
                    DataArchiveFileEntries[i].WriteTableEntry(writer);
                }

                for (int i = 0; i < DataArchiveFileEntries.Length; i++)
                {
                    DataArchiveFileEntries[i].WriteFileData(writer);
                }
            }
        }
    }
}
