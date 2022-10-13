using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using RESET_Tools.Utilities;

namespace RESET_Tools.Experimental.Identify
{
    public class CachedBlobFolder
    {
        public CachedBlobFolder(string directoryPath)
        {
            string convertedRootDirectory = Path.GetDirectoryName(directoryPath) + "/CachedBlobConverted";

            string convertedRootDirectory_physX_mesh = Path.GetDirectoryName(directoryPath) + "/CachedBlobConverted/PhysX_Mesh";
            string convertedRootDirectory_physX_convexMesh = Path.GetDirectoryName(directoryPath) + "/CachedBlobConverted/PhysX_ConvexMesh";
            string convertedRootDirectory_dds = Path.GetDirectoryName(directoryPath) + "/CachedBlobConverted/DDS";
            string convertedRootDirectory_mesh = Path.GetDirectoryName(directoryPath) + "/CachedBlobConverted/Mesh";
            string convertedRootDirectory_xD = Path.GetDirectoryName(directoryPath) + "/CachedBlobConverted/xD";
            string convertedRootDirectory_unsorted = Path.GetDirectoryName(directoryPath) + "/CachedBlobConverted/Unsorted";

            if (Directory.Exists(convertedRootDirectory) == false)
                Directory.CreateDirectory(convertedRootDirectory);

            if (Directory.Exists(convertedRootDirectory_physX_mesh) == false)
                Directory.CreateDirectory(convertedRootDirectory_physX_mesh);

            if (Directory.Exists(convertedRootDirectory_physX_convexMesh) == false)
                Directory.CreateDirectory(convertedRootDirectory_physX_convexMesh);

            if (Directory.Exists(convertedRootDirectory_dds) == false)
                Directory.CreateDirectory(convertedRootDirectory_dds);

            if (Directory.Exists(convertedRootDirectory_mesh) == false)
                Directory.CreateDirectory(convertedRootDirectory_mesh);

            if (Directory.Exists(convertedRootDirectory_xD) == false)
                Directory.CreateDirectory(convertedRootDirectory_xD);

            if (Directory.Exists(convertedRootDirectory_unsorted) == false)
                Directory.CreateDirectory(convertedRootDirectory_unsorted);

            string[] filePaths = Directory.GetFiles(directoryPath);

            int ddsFileCount = 0;
            int xD_fileCount = 0;
            int physXMesh_fileCount = 0;
            int physXCVXM_fileCount = 0;
            int physXNXS_fileCount = 0;
            int mesh_fileCount = 0;

            foreach (string filePath in filePaths)
            {
                using (BinaryReader reader = new BinaryReader(File.OpenRead(filePath)))
                {
                    string firstData = Encoding.ASCII.GetString(reader.ReadBytes(4));
                    reader.BaseStream.Seek(0, SeekOrigin.Begin);
                    uint firstDataInt = reader.ReadUInt32();

                    if (firstData.Equals("DDS "))
                    {
                        //this is a DDS file, so lets save it as one
                        string ddsPath = convertedRootDirectory_dds + "/" + Path.GetFileNameWithoutExtension(filePath) + ".dds";
                        //string pngPath = convertedRootDirectory + "/" + Path.GetFileNameWithoutExtension(filePath) + ".png";

                        File.WriteAllBytes(ddsPath, File.ReadAllBytes(filePath));

                        //Surface surface = Surface.LoadFromFile(filePath);
                        //ImageFormat impFormat = ImageFormat.PNG;
                        //surface.SaveToFile(impFormat, pngPath);

                        ddsFileCount++;
                    }
                    else if (firstData.Remove(3, 1).Equals("NXS"))
                    {
                        //read another 4 bytes
                        string secondData = Encoding.ASCII.GetString(reader.ReadBytes(4));

                        if (secondData.Equals("MESH"))
                        {
                            physXMesh_fileCount++;

                            string newPath = convertedRootDirectory_physX_mesh + "/" + Path.GetFileName(filePath);
                            File.WriteAllBytes(newPath, File.ReadAllBytes(filePath));
                        }
                        else if (secondData.Equals("CVXM"))
                        {
                            physXCVXM_fileCount++;

                            string newPath = convertedRootDirectory_physX_convexMesh + "/" + Path.GetFileName(filePath);
                            File.WriteAllBytes(newPath, File.ReadAllBytes(filePath));
                        }

                        physXNXS_fileCount++;
                    }
                    else if (firstData.Remove(0, 2).Equals("xD"))
                    {
                        xD_fileCount++;

                        string newPath = convertedRootDirectory_xD + "/" + Path.GetFileName(filePath);
                        File.WriteAllBytes(newPath, File.ReadAllBytes(filePath));
                    }
                    else if (firstDataInt == 16) //mesh file?
                    {
                        mesh_fileCount++;

                        string newPath = convertedRootDirectory_mesh + "/" + Path.GetFileName(filePath);
                        File.WriteAllBytes(newPath, File.ReadAllBytes(filePath));
                    }
                    else
                    {
                        string newPath = convertedRootDirectory_unsorted + "/" + Path.GetFileName(filePath);
                        File.WriteAllBytes(newPath, File.ReadAllBytes(filePath));
                    }
                }
            }

            Console.WriteLine("{0} / {1} - DDS Files", ddsFileCount, filePaths.Length);
            Console.WriteLine("{0} / {1} - xD Files", xD_fileCount, filePaths.Length);
            Console.WriteLine("{0} / {1} - Mesh Files", mesh_fileCount, filePaths.Length);
            Console.WriteLine("{0} / {1} - PhysX NXS Files", physXNXS_fileCount, filePaths.Length);
            Console.WriteLine("{0} / {1} - PhysX Mesh Files", physXMesh_fileCount, physXNXS_fileCount);
            Console.WriteLine("{0} / {1} - PhysX Convex Mesh Files", physXCVXM_fileCount, physXNXS_fileCount);

            int totalPhysxFilesFound = physXMesh_fileCount + physXCVXM_fileCount;
            Console.WriteLine("{0} / {1} - PhysX Files Found", totalPhysxFilesFound, physXNXS_fileCount);

            int totalFilesFound = ddsFileCount + xD_fileCount + physXNXS_fileCount + mesh_fileCount;
            Console.WriteLine("{0} / {1} - Files Found", totalFilesFound, filePaths.Length);
        }
    }
}
