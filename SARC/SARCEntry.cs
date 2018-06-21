using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO; 

namespace Xayrga.SARC
{
    public struct SARCEntry
    {

        public uint index;
        public uint section_addr;
        public uint offset;
        public uint offset_absolute;
        public uint size;
        public string name;
    }
}
