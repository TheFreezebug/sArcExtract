using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO; 

namespace Xayrga.SARC
{
    class SARCSection
    {
        public uint data_offset;
        public uint baseaddr;
        public uint index;
        public uint recordcount;

        public SARCEntry[] Entries;

        private BinaryReader breader;
        public SARCSection(BinaryReader bread,  uint idx,uint count,uint bddr,uint doffset)
        {
            Entries = new SARCEntry[count];
            index = idx;
            recordcount = count;
            baseaddr = bddr;
            data_offset = doffset;
            breader = bread;
        } 

        public void nibbleEntries()
        {
            breader.BaseStream.Seek(baseaddr + 32, 0);
            for (uint i = 0; i < recordcount; i++)
            {


                var SEnt = new SARCEntry
                {
                    section_addr = baseaddr,
                    index = breader.ReadUInt32(),
                    size = breader.ReadUInt32(),
                    offset = breader.ReadUInt32()
                };

                breader.ReadUInt32(); // padding i guess
                SEnt.offset_absolute = SEnt.offset + baseaddr;
                Entries[i] = SEnt;
            }
            StringBuilder sb = new StringBuilder();
            byte lastread = 0xFF;
            for (uint i = 0; i < recordcount; i++)
            {
                
              
                breader.ReadByte(); // Skip the 0xA
                sb.Clear();
                while ((lastread = breader.ReadByte()) != 0x00)
                {
                    sb.Append(Convert.ToChar(lastread));
                }
                Entries[i].name = sb.ToString();
             

            }
            
        }
    }
}
