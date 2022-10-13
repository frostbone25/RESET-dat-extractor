using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESET_Tools.Experimental.Bnk
{
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
}
