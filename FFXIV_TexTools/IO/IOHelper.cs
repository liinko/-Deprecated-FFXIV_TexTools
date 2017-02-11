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
using System.Linq;
using FFXIV_TexTools.Helpers;

namespace FFXIV_TexTools.IO
{
    public class IOHelper
    {
        public IOHelper()
        {

        }

        /// <summary>
        /// Checks whether the DDS file exists
        /// </summary>
        /// <param name="parentNode">Name of Equipment Slot</param>
        /// <param name="childNode">Name of Item</param>
        /// <param name="currTexName">Currently selected texture name</param>
        /// <returns></returns>
        public bool ddsExists(string parentNode, string childNode, string currTexName)
        {
            return File.Exists(Properties.Settings.Default.SaveDir + "/" + parentNode + "/" + childNode + "/" + currTexName.Substring(0, currTexName.LastIndexOf('.')) + ".dds");
        }

        /// <summary>
        /// Modifies the offset for a particular texture in the .index file
        /// </summary>
        /// <param name="newOffset">The new offset to be writen</param>
        /// <param name="texName">Name of the currently selected texture</param>
        /// <param name="mFolderHex">the Folder Name CRC</param>
        public void modifyIndexOffset(int newOffset, string texName, string mFolderHex)
        {
            FFCRC crc = new FFCRC();
            string fileCRC;

            fileCRC = crc.text(texName).PadLeft(8, '0');

            using (FileStream fs = new FileStream(Properties.Settings.Default.DefaultDir + "/040000.win32.index", FileMode.Open, FileAccess.ReadWrite))
            {
                using (BinaryReader b = new BinaryReader(fs))
                {
                    using (BinaryWriter bw = new BinaryWriter(fs))
                    {
                        b.BaseStream.Seek(1036, SeekOrigin.Begin);
                        int totalFiles = b.ReadInt32();


                        b.BaseStream.Seek(2048, SeekOrigin.Begin);
                        for (int i = 0; i < totalFiles; b.ReadBytes(4), i += 16)
                        {
                            int fileHex1 = b.ReadInt32();
                            string fHex = fileHex1.ToString("X").PadLeft(8, '0');

                            if (fHex.Equals(fileCRC))
                            {
                                if (!texName.Contains("mt_"))
                                {
                                    bw.BaseStream.Seek(2056 + i, SeekOrigin.Begin);
                                    bw.Write(newOffset / 8);
                                    break;
                                }
                                else
                                {
                                    int folderHex = b.ReadInt32();
                                    string fohex = folderHex.ToString("X").PadLeft(8, '0');

                                    if (fohex.Equals(mFolderHex))
                                    {
                                        bw.BaseStream.Seek(2056 + i, SeekOrigin.Begin);
                                        bw.Write(newOffset / 8);
                                        break;
                                    }
                                    b.ReadBytes(4);
                                }
                            }
                            else
                            {
                                b.ReadBytes(8);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Compress a given array of bytes
        /// </summary>
        /// <param name="uncomp">Array of uncompressed bytes</param>
        /// <returns></returns>
        public byte[] compressor(byte[] uncomp)
        {
            using (MemoryStream ms = new MemoryStream(uncomp))
            {
                byte[] compbytes = null;
                using (var cs = new MemoryStream())
                {
                    using (var ds = new DeflateStream(cs, CompressionMode.Compress))
                    {
                        ms.CopyTo(ds);
                        ds.Close();
                        compbytes = cs.ToArray();
                    }
                }
                return compbytes;
            }
        }

        /// <summary>
        /// Used when reverting a Color Map back to its original
        /// </summary>
        /// <param name="datNum">The dat nubmer the file is located in</param>
        /// <param name="offset">The offset of the file</param>
        /// <returns></returns>
        public byte[] getRevertedColorMap(int datNum, int offset)
        {
            byte[] decompBytes;
            using (BinaryReader b = new BinaryReader(File.OpenRead(Properties.Settings.Default.DefaultDir + "/040000.win32.dat" + datNum)))
            {
                if (datNum == 3)
                {
                    b.BaseStream.Seek(offset + 88, SeekOrigin.Begin);
                }
                else if (datNum == 2)
                {
                    b.BaseStream.Seek(offset + 104, SeekOrigin.Begin);
                }
                else if (datNum == 1)
                {
                    b.BaseStream.Seek(offset + 120, SeekOrigin.Begin);
                }
                else
                {
                    b.BaseStream.Seek(offset + 136, SeekOrigin.Begin);
                }

                int dataSize = b.ReadInt32();
                int decompDataSize = b.ReadInt32();

                byte[] dataArray = b.ReadBytes(dataSize);

                decompBytes = new byte[decompDataSize];

                using (MemoryStream ms = new MemoryStream(dataArray))
                {
                    using (DeflateStream ds = new DeflateStream(ms, CompressionMode.Decompress))
                    {
                        int count = ds.Read(decompBytes, 0, decompDataSize);
                    }
                }
            }
            using (BinaryReader b1 = new BinaryReader(new MemoryStream(decompBytes)))
            {
                b1.BaseStream.Seek(6, SeekOrigin.Begin);
                short clrSize = b1.ReadInt16();
                short texNameSize = b1.ReadInt16();
                b1.ReadBytes(2);
                byte texNum = b1.ReadByte();
                byte mapNum = b1.ReadByte();
                byte clrNum = b1.ReadByte();
                byte unkNum = b1.ReadByte();

                b1.BaseStream.Seek(20 + 4 * (texNum + mapNum + clrNum) + texNameSize, SeekOrigin.Begin);

                return b1.ReadBytes(clrSize).ToArray();
            }
        }

        /// <summary>
        /// Checks if a texture is currently modified and active
        /// </summary>
        /// <param name="fileHex">File name CRC</param>
        /// <param name="folderHex">Folder name CRC</param>
        /// <param name="modOffset">Offset of modified texture</param>
        /// <returns></returns>
        public bool isActive(string fileHex, string folderHex, string modOffset)
        {
            using (BinaryReader br = new BinaryReader(File.OpenRead(Properties.Settings.Default.DefaultDir + "/040000.win32.index")))
            {
                br.BaseStream.Seek(1036, SeekOrigin.Begin);
                int totalFiles = br.ReadInt32();

                br.BaseStream.Seek(2048, SeekOrigin.Begin);
                for (int i = 0; i < totalFiles; br.ReadBytes(4), i += 16)
                {
                    string tempOffset = br.ReadInt32().ToString("X").PadLeft(8, '0');

                    if (tempOffset.Equals(fileHex))
                    {
                        if (folderHex.Equals(""))
                        {
                            br.ReadBytes(4);
                            byte[] offset = br.ReadBytes(4);
                            string fileOffset = (BitConverter.ToUInt32(offset, 0) * 8).ToString("X").PadLeft(8, '0');

                            if (fileOffset.Equals(modOffset))
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }

                        string foHex = br.ReadInt32().ToString("X").PadLeft(8, '0');

                        if (foHex.Equals(folderHex))
                        {
                            byte[] offset = br.ReadBytes(4);
                            string fileOffset = (BitConverter.ToUInt32(offset, 0) * 8).ToString("X").PadLeft(8, '0');

                            if (fileOffset.Equals(modOffset))
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            br.ReadBytes(4);
                        }
                    }
                    else
                    {
                        br.ReadBytes(8);
                    }
                }
                return false;
            }
        }
    }
}
