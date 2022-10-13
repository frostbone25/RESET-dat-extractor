using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESET_Tools.Experimental.Bin
{
    public struct BinFileEntry
    {
        public string FileName { get; set; }
        public uint Unknown1 { get; set; }
        public byte[] FileData { get; set; }

        public override string ToString()
        {
            return string.Format("Entry: {0} [{1}]", FileName, Unknown1);
        }
    }
}
