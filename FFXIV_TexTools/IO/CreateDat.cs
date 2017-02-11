// FFXIV TexTools
// Copyright © 2017 Rafael Gonzalez - All Rights Reserved
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System.IO;
using System.Security.Cryptography;

namespace FFXIV_TexTools
{
    class CreateDat
    {
        /// <summary>
        /// Creates .dat3 file
        /// </summary>
        public CreateDat()
        {
            string newDat = Properties.Settings.Default.DefaultDir + "/040000.win32.dat3";
            string indexDir = Properties.Settings.Default.DefaultDir + "/040000.win32.index";

            using(FileStream fs = File.Create(newDat)){
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    bw.BaseStream.Seek(0, SeekOrigin.Begin);

                    writeSqPackHeader(bw);
                    writeDatHeader(bw);
                }
            }

            using(BinaryWriter bw = new BinaryWriter(File.OpenWrite(indexDir))){
                bw.BaseStream.Seek(1104, SeekOrigin.Begin);
                byte b = 4;
                bw.Write(b);
            }

        }

        /// <summary>
        /// writes the sqpack to header
        /// </summary>
        /// <param name="bw"></param>
        public void writeSqPackHeader(BinaryWriter bw){
            byte[] header = new byte[1024];

            using (BinaryWriter hw = new BinaryWriter(new MemoryStream(header)))
            {
                hw.BaseStream.Seek(0, SeekOrigin.Begin);

                SHA1Managed shaM = new SHA1Managed();

                hw.Write(1632661843);
                hw.Write(27491);
                hw.Write(0);
                hw.Write(1024);
                hw.Write(1);
                hw.Write(1);
                hw.Seek(8, SeekOrigin.Current);
                hw.Write(-1);
                hw.Seek(960, SeekOrigin.Begin);

                hw.Write(shaM.ComputeHash(header, 0, 959));

                bw.Write(header);
            }
        }

        /// <summary>
        /// Writes the new .dat header
        /// </summary>
        /// <param name="bw"></param>
        public void writeDatHeader(BinaryWriter bw)
        {
            byte[] header = new byte[1024];

            using (BinaryWriter hw = new BinaryWriter(new MemoryStream(header)))
            {
                hw.BaseStream.Seek(0, SeekOrigin.Begin);

                SHA1Managed shaM = new SHA1Managed();

                hw.Write(header.Length);
                hw.Write(0);
                hw.Write(16);
                hw.Write(2048);
                hw.Write(2);
                hw.Write(0);
                hw.Write(2000000000);
                hw.Write(0);
                hw.Seek(960, SeekOrigin.Begin);

                hw.Write(shaM.ComputeHash(header, 0, 959));

                bw.BaseStream.Seek(1024, SeekOrigin.Begin);
                bw.Write(header);
            }
        }

        /// <summary>
        /// Creates the empty modlist file
        /// </summary>
        public void createModList()
        {
            string modList = Properties.Settings.Default.DefaultDir + "/040000.modlist";
            File.Create(modList);
        }
    }
}
