using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Xayrga.SARC;

namespace sArcExtract
{
    class Program
    /* sArcExtract 
     * Created by XayrGA  
     * Made for extracting the sound archive from a 3DS game
     * Only tested on mario and luigi  dream team!
     * 
     * 
     * 
     * 
     * 
     */

    /* 
     The structure of an SARC file 
     
        -- SMALL E
        int32 Section Type Archive Type  (confirmed)
	        00 = WAVE 
	        01 = SOUNDEFFECT
	        04 = CUTSCENE
	
        int32 Record Count (Confirmed) 
        int32 Jumpsize to next section from section base (confirmed)
        int32 Jumpsize to data start from section base (confirmed)
        int32 ??? -- Looks like a pointer back to this section? (probably) 
        int32 ??? Maybe a pointer to a section divider? like... idk (dunno)
        int32 ??? Always 0 ? 
        int32 ??? Always 0 ?


        0x20 First record
	        < Record Format > * Record Count 
	        int32 index_number
	        int32 length
	        int32 offset from header end 
	        int32 padding ???  (always 0 ?) alignment
	
        0x20 + ( Records * 16)	Name Index Table 
	        <Name Record> * Record Count 
	        // Each Record in the name table is analog with the index in the order they were defined . 
	        byte 0x0A
	        Cstring name (Null Terminated)
	

    */

    {
        static BinaryReader SARC;
        static SARCSection[] SArr = new SARCSection[64];

        static void Main(string[] args)
        {
         
            try
            {
                string filename = args[0];
                SARC = new BinaryReader(File.Open(filename, FileMode.Open, FileAccess.Read));
                
            } catch
            {
                Console.WriteLine("sArcExtract: Error, supply a file as the first parameter!");
                Console.WriteLine("Example: sArcExtract ./rawr.arc");
                Environment.Exit(-1);
            }
           

            var go = true;
            uint idx = 0; 
            while (go)
            {
                 
                var stype = SARC.ReadUInt32();
                var records = SARC.ReadUInt32();
                var next_sec = SARC.ReadUInt32();
                var next_data = SARC.ReadUInt32(); 
                var sbase = SARC.ReadUInt32();

                SARC.ReadUInt32(); // Don't know what this is, just have to allign. 
                SARC.ReadUInt32(); // Don't know what this is, just have to allign. 
                SARC.ReadUInt32(); // Don't know what this is, just have to allign. 

                Console.WriteLine("{4:X} Section type 0x{0:X} with 0x{1:X} records , data at 0x{2:X}. Next section at {3:X} ",stype,records,next_data,next_sec,sbase);
                SARCSection rawr = new SARCSection(SARC, idx, records, sbase, next_data);
                rawr.nibbleEntries();
                SArr[idx] = rawr;

                SARC.BaseStream.Seek(next_sec + sbase,0);






                if (SARC.BaseStream.Length == SARC.BaseStream.Position)
                {
                    Console.WriteLine("Next section points to EOF -- end sections");
                    go = false;
                    break; 
                }

                idx++;
                
            }

            Directory.CreateDirectory("out");
            for (int i = 0; i < idx; i++) {
                Directory.CreateDirectory("out/" + i); 
                var sect = SArr[i];
                for (int sidx = 0; sidx < sect.recordcount; sidx++)
                {
                    var ent = sect.Entries[sidx];
                    SARC.BaseStream.Seek(ent.offset_absolute, 0);
                    var data = SARC.ReadBytes((int)ent.size);

                    File.WriteAllBytes("out/" + i + "/" + ent.name + ".rsd", data);



                }


            }
            Console.WriteLine("Done extracting!");
            
        }
    }
}
