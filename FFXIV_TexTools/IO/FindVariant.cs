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

using System;
using System.IO;
using System.IO.Compression;

namespace FFXIV_TexTools.IO
{
    public class FindVariant
    {
        string variant;

        /// <summary>
        /// Finds the vairant of the texture that is associated with that particular item
        /// </summary>
        /// <param name="datNum">.dat the file is located in</param>
        /// <param name="imcOffset">Offset of IMC file</param>
        /// <param name="slot">Equipment slot</param>
        /// <param name="type">Type of equipment</param>
        /// <param name="ver">Item version</param>
        public FindVariant(int datNum, int imcOffset, int slot, string type, string ver)
        {
            int variantLoc;
            byte[] decompBytes;

            imcOffset = imcOffset - (16 * datNum);

            using (BinaryReader b = new BinaryReader(File.OpenRead(Properties.Settings.Default.DefaultDir + "/040000.win32.dat" + datNum)))
            {
                b.BaseStream.Seek(imcOffset, SeekOrigin.Begin);

                int headerLength = b.ReadInt32();

                b.ReadBytes(headerLength + 4);

                int compressedSize = b.ReadInt32();
                int uncompressedSize = b.ReadInt32();

                byte[] data = b.ReadBytes(compressedSize);
                decompBytes = new byte[uncompressedSize];

                using (MemoryStream ms = new MemoryStream(data))
                {
                    using (DeflateStream ds = new DeflateStream(ms, CompressionMode.Decompress))
                    {
                        int count = ds.Read(decompBytes, 0, uncompressedSize);
                    }
                }
            }

            using (BinaryReader b1 = new BinaryReader(new MemoryStream(decompBytes)))
            {
                if (type.Equals("w"))
                {
                    variantLoc = 4 + (int.Parse(ver) * 6);
                }
                else
                {
                    variantLoc = 4 + (int.Parse(ver) * 30) + (6 * slot);
                }

                b1.BaseStream.Seek(variantLoc, SeekOrigin.Begin);

                int v = b1.ReadInt16();
                if (v <= 0)
                {
                    variant = "1";
                }
                else if( v > 100)
                {
                    variant = "1";
                }
                else
                {
                    variant = v.ToString();
                }
            }
        }

        public string getVariant()
        {
            return variant;
        }
    }
}
