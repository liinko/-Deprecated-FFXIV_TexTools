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
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using FFXIV_TexTools.Helpers;

namespace FFXIV_TexTools.IO
{
    class DatWriter
    {
        int datOffset;
        byte[] colorMapBytes;


        /// <summary>
        /// Writes the modified texture into the .dat file
        /// </summary>
        /// <param name="inModList">Mod list presense check</param>
        /// <param name="selectedMap">Currently selected texture map</param>
        /// <param name="ddsFile">DDS file directory</param>
        /// <param name="modListEntry">Modlist entry</param>
        /// <param name="fileOffset">Offset of texture file</param>
        /// <param name="mMtrlOffset">Offset of MTRL file</param>
        /// <param name="datLoc">Which dat the file is located in</param>
        public DatWriter(bool inModList, string selectedMap, string ddsFile, jModEntry modListEntry, string fileOffset, int mMtrlOffset, int datLoc)
        {
            List<byte> modBytes = new List<byte>();
            List<byte> recomp = new List<byte>();
            List<byte> nSize = new List<byte>();
            string originalOffset = fileOffset;
            string fileDirectory;
            short nPadSize;
            int partPSize = 0;
            int partPSize2 = 0;
            int partSum = 0;
            int datNum = 0;

            IOHelper ioHelper = new IOHelper();

            if (inModList)
            {
                datNum = ((int.Parse(modListEntry.origOffset, NumberStyles.HexNumber) / 8) & 0x000f) /2; 
                fileOffset = modListEntry.origOffset;
            }
            else if (selectedMap.Equals("ColorSet1"))
            {
                datNum = ((mMtrlOffset / 8) & 0x000f)/2;
                fileOffset = mMtrlOffset.ToString("X").PadLeft(8, '0');
            }
            else
            {
                datNum = datLoc;
            }


            if (selectedMap.Equals("ColorSet1"))
            {
                try
                {
                    List<byte> colorFix;
                    using (BinaryReader br = new BinaryReader(File.OpenRead(ddsFile)))
                    {
                        string dye = ddsFile.Substring(0, ddsFile.LastIndexOf('/')) + "/dye.dat";
                        using (BinaryReader br1 = new BinaryReader(File.OpenRead(dye)))
                        {
                            colorFix = new List<byte>();

                            br.BaseStream.Seek(0, SeekOrigin.Begin);
                            br1.BaseStream.Seek(0, SeekOrigin.Begin);
                            colorFix.AddRange(br.ReadBytes(640).ToArray());
                            colorFix.AddRange(br1.ReadBytes(32).ToArray());
                        }
                    }

                    File.WriteAllBytes(ddsFile, colorFix.ToArray());
                }
                catch(Exception e)
                {
                }
            }

            fileDirectory = Properties.Settings.Default.DefaultDir + "/040000.win32.dat" + datNum;

            using (BinaryReader br = new BinaryReader(File.OpenRead(fileDirectory)))
            {
                using (BinaryReader br1 = new BinaryReader(File.OpenRead(ddsFile)))
                {
                    using (BinaryWriter bw = new BinaryWriter(File.OpenWrite(Properties.Settings.Default.DefaultDir + "/040000.win32.dat3")))
                    {
                        br1.BaseStream.Seek(128, SeekOrigin.Begin);
                        int initialOffset = int.Parse(fileOffset, NumberStyles.HexNumber);
                        if (fileDirectory.Contains(".dat1"))
                        {
                            initialOffset = initialOffset - 16;
                        }
                        else if (fileDirectory.Contains(".dat2"))
                        {
                            initialOffset = initialOffset - 32;
                        }
                        else if (fileDirectory.Contains(".dat3"))
                        {
                            initialOffset = initialOffset - 48;
                        }
                        br.BaseStream.Seek(initialOffset, SeekOrigin.Begin);

                        byte[] b_header = br.ReadBytes(24);
                        uint hLength = BitConverter.ToUInt32(b_header, 0x00);
                        uint type = BitConverter.ToUInt32(b_header, 0x04);

                        if (type == 4)
                        {
                            uint numMipMaps = BitConverter.ToUInt32(b_header, 0x14);
                            int endHeader = initialOffset + (int)hLength;
                            uint toInitPartInfo = (uint)(initialOffset + 24 + (numMipMaps * 20));
                            int[] nOffsetsSizes = new int[numMipMaps];

                            modBytes.AddRange(b_header);

                            for (int i = 0; i < numMipMaps; i++)
                            {
                                partSum = 0;
                                int nMipPadSize = 0;
                                uint toPartInfo = (uint)(initialOffset + 24 + (numMipMaps * 20) + partPSize2);

                                br.BaseStream.Seek(initialOffset + 24 + (i * 20), SeekOrigin.Begin);

                                byte[] mipPart = br.ReadBytes(20);
                                uint fromEnd = BitConverter.ToUInt32(mipPart, 0x00);
                                uint mipLength = BitConverter.ToUInt32(mipPart, 0x04);
                                uint partsToRead = BitConverter.ToUInt32(mipPart, 0x10);
                                partPSize = (int)partsToRead * 2;
                                partPSize2 = partPSize2 + partPSize;

                                //get padded part sizes including 16byte header
                                br.BaseStream.Seek(toPartInfo, SeekOrigin.Begin);
                                byte[] mipPartPadSizes = br.ReadBytes(partPSize);

                                //read mip1 header
                                int mip = endHeader + (int)fromEnd;
                                br.BaseStream.Seek(mip, SeekOrigin.Begin);
                                byte[] mipHeader = br.ReadBytes(16);
                                uint compSize = BitConverter.ToUInt32(mipHeader, 0x08);
                                uint uncompSize = BitConverter.ToUInt32(mipHeader, 0x0C);
                                recomp.AddRange(mipHeader.Take(8));

                                if (partsToRead > 1)
                                {
                                    mipPart = ioHelper.compressor(br1.ReadBytes((int)uncompSize));
                                    int count = mipPart.Length;
                                    recomp.AddRange(BitConverter.GetBytes(count));
                                    recomp.AddRange(mipHeader.Skip(12));
                                    recomp.AddRange(mipPart);

                                    if (count % 16 != 0)
                                    {
                                        int difference = 16 - ((int)count % 16);
                                        byte[] padd = new byte[difference];
                                        Array.Clear(padd, 0, difference);
                                        recomp.AddRange(padd);
                                        //padded size of compressed data for header
                                        nPadSize = (short)(16 + count + difference);
                                        nSize.AddRange(BitConverter.GetBytes(nPadSize));
                                        nMipPadSize = nMipPadSize + nPadSize;
                                    }
                                    else
                                    {
                                        nPadSize = (short)(16 + count);
                                        nSize.AddRange(BitConverter.GetBytes(nPadSize));
                                        nMipPadSize = nMipPadSize + nPadSize;
                                    }

                                    for (int j = 1; j < partsToRead; j++)
                                    {
                                        partSum = partSum + BitConverter.ToUInt16(mipPartPadSizes, ((j - 1) * 2));
                                        br.BaseStream.Seek(mip + partSum, SeekOrigin.Begin);
                                        mipHeader = br.ReadBytes(16);
                                        compSize = BitConverter.ToUInt32(mipHeader, 0x08);
                                        uncompSize = BitConverter.ToUInt32(mipHeader, 0x0C);
                                        recomp.AddRange(mipHeader.Take(8));

                                        mipPart = ioHelper.compressor(br1.ReadBytes((int)uncompSize));
                                        count = mipPart.Length;
                                        recomp.AddRange(BitConverter.GetBytes(count));
                                        recomp.AddRange(mipHeader.Skip(12));
                                        recomp.AddRange(mipPart);

                                        if (count % 16 != 0)
                                        {
                                            int difference = 16 - ((int)count % 16);
                                            byte[] padd = new byte[difference];
                                            Array.Clear(padd, 0, difference);
                                            recomp.AddRange(padd);
                                            nPadSize = (short)(16 + count + difference);
                                            nSize.AddRange(BitConverter.GetBytes(nPadSize));
                                            nMipPadSize = nMipPadSize + nPadSize;
                                        }
                                        else
                                        {
                                            nPadSize = (short)(16 + count);
                                            nSize.AddRange(BitConverter.GetBytes(nPadSize));
                                            nMipPadSize = nMipPadSize + nPadSize;
                                        }
                                    }
                                    nOffsetsSizes[i] = nMipPadSize;
                                }
                                else
                                {
                                    byte[] padd;
                                    int count;
                                    br.BaseStream.Seek(mip, SeekOrigin.Begin);
                                    mipPart = ioHelper.compressor(br1.ReadBytes((int)uncompSize));
                                    count = mipPart.Length;
                                    recomp.AddRange(BitConverter.GetBytes(count));
                                    recomp.AddRange(mipHeader.Skip(12));
                                    recomp.AddRange(mipPart);

                                    if (count % 16 != 0)
                                    {
                                        int difference = 16 - ((int)count % 16);
                                        padd = new byte[difference];
                                        Array.Clear(padd, 0, difference);
                                        recomp.AddRange(padd);
                                        nPadSize = (short)(16 + count + difference);
                                        nSize.AddRange(BitConverter.GetBytes(nPadSize));
                                        nMipPadSize = nMipPadSize + nPadSize;
                                    }
                                    else
                                    {
                                        nPadSize = (Int16)(16 + count);
                                        nSize.AddRange(BitConverter.GetBytes(nPadSize));
                                        nMipPadSize = nMipPadSize + nPadSize;
                                    }
                                    nOffsetsSizes[i] = nMipPadSize;
                                }
                            }

                            int noffset = 80;


                            for (int i = 0; i < numMipMaps; i++)
                            {
                                br.BaseStream.Seek((initialOffset + 32) + (i * 20), SeekOrigin.Begin);
                                if (i == 0)
                                {
                                    modBytes.AddRange(BitConverter.GetBytes(80));
                                }
                                else
                                {
                                    noffset = noffset + nOffsetsSizes[i - 1];
                                    modBytes.AddRange(BitConverter.GetBytes(noffset));
                                }
                                modBytes.AddRange(BitConverter.GetBytes(nOffsetsSizes[i]));
                                modBytes.AddRange(br.ReadBytes(12));
                            }

                            modBytes.AddRange(nSize.ToArray());
                            int remaining = (int)hLength - modBytes.Count;
                            for (int i = 0; i < remaining; i++)
                            {
                                modBytes.Add((byte)0);
                            }

                            br.BaseStream.Seek(endHeader, SeekOrigin.Begin);
                            modBytes.AddRange(br.ReadBytes(80));
                            modBytes.AddRange(recomp.ToArray());

                            byte pad = 0;
                            for (int i = 0; i < 1024; i++)
                            {
                                modBytes.Add(pad);
                            }

                            if (inModList)
                            {
                                bw.BaseStream.Seek(int.Parse(originalOffset, NumberStyles.HexNumber) - 48, SeekOrigin.Begin);
                            }
                            else
                            {
                                bw.BaseStream.Seek(0, SeekOrigin.End);
                                int lastPos = (int)bw.BaseStream.Position + modBytes.Count;
                                byte[] padto = new byte[16];

                                while ((lastPos & 0xff) != 0)
                                {
                                    modBytes.AddRange(padto);
                                    lastPos = lastPos + 16;
                                }

                            }


                            datOffset = (int)bw.BaseStream.Position;
                            bw.Write(modBytes.ToArray());
                        }
                        else
                        {
                            List<byte> uncompMTRLbytes = new List<byte>();
                            int endHeader = initialOffset + (int)hLength;

                            modBytes.AddRange(b_header);
                            modBytes.AddRange(br.ReadBytes(112).ToArray());

                            uint compSize = br.ReadUInt32();
                            uint uncompSize = br.ReadUInt32();
                            byte[] forlist = br.ReadBytes((int)compSize);
                            byte[] uncompressedbytes = new byte[uncompSize];

                            using (MemoryStream ms = new MemoryStream(forlist))
                            {
                                using (DeflateStream ds = new DeflateStream(ms, CompressionMode.Decompress))
                                {
                                    int count = ds.Read(uncompressedbytes, 0x00, (int)uncompSize);
                                }
                            }

                            using (BinaryReader b2 = new BinaryReader(new MemoryStream(uncompressedbytes)))
                            {
                                b2.BaseStream.Seek(4, SeekOrigin.Begin);
                                short fileSize = b2.ReadInt16();
                                short clrSize = b2.ReadInt16();
                                short texNameSize = b2.ReadInt16();
                                b2.ReadBytes(2);
                                byte texNum = b2.ReadByte();
                                byte mapNum = b2.ReadByte();
                                byte clrNum = b2.ReadByte();
                                byte unkNum = b2.ReadByte();
                                b2.ReadBytes(4);

                                int mHeaderLength = 20 + 4 * (texNum + mapNum + clrNum) + texNameSize;
                                b2.BaseStream.Seek(0, SeekOrigin.Begin);

                                uncompMTRLbytes.AddRange(b2.ReadBytes(mHeaderLength).ToArray());

                                byte[] comp = br1.ReadBytes(clrSize);

                                colorMapBytes = comp;

                                uncompMTRLbytes.AddRange(comp);

                                int headerColorSum = mHeaderLength + clrSize;
                                b2.BaseStream.Seek(headerColorSum, SeekOrigin.Begin);

                                uncompMTRLbytes.AddRange(b2.ReadBytes(fileSize - headerColorSum));
                            }

                            byte[] compMTRL = ioHelper.compressor(uncompMTRLbytes.ToArray());

                            modBytes.AddRange(BitConverter.GetBytes(compMTRL.Length));
                            modBytes.AddRange(BitConverter.GetBytes(uncompSize));
                            modBytes.AddRange(compMTRL);
                            int mCount = compMTRL.Length;

                            if (mCount % 16 != 0)
                            {
                                int difference = 16 - (mCount % 16);
                                byte[] padd = new byte[difference];
                                Array.Clear(padd, 0, difference);
                                modBytes.AddRange(padd);
                            }

                            byte pad = 0;
                            for (int i = 0; i < 1024; i++)
                            {
                                modBytes.Add(pad);
                            }

                            if (inModList)
                            {
                                bw.BaseStream.Seek(int.Parse(originalOffset, NumberStyles.HexNumber) - 48, SeekOrigin.Begin);
                            }
                            else
                            {
                                bw.BaseStream.Seek(0, SeekOrigin.End);
                                int lastPos = (int)bw.BaseStream.Position + modBytes.Count;
                                byte[] padto = new byte[16];

                                while ((lastPos & 0xff) != 0)
                                {
                                    modBytes.AddRange(padto);
                                    lastPos = lastPos + 16;
                                }
                            }

                            datOffset = (int)bw.BaseStream.Position;
                            bw.Write(modBytes.ToArray());
                        }
                    }
                }
            }
        }

        public int getNewDatOffset()
        {
            return datOffset;
        }

        public byte[] getColorMapBytes()
        {
            return colorMapBytes;
        }
    }
}
