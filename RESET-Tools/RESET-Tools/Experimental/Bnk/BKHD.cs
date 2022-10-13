using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESET_Tools.Experimental.Bnk
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
}
