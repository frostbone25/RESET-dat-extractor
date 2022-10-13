using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESET_Tools.Experimental.Bnk
{
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
}
