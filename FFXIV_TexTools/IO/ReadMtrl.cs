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

using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using FFXIV_TexTools.Helpers;

namespace FFXIV_TexTools.IO
{
    public class ReadMtrl
    {
        FFCRC crc = new FFCRC();
        List<string> textureNames = new List<string>();
        Dictionary<string, string> textureOffsets = new Dictionary<string, string>();
        byte[] colorMapBytes;
        FindOffset fo;

        /// <summary>
        /// Reads the MTRL file
        /// </summary>
        /// <param name="datNum">The dat the file is located in</param>
        /// <param name="mtrlOffset">Offset for MTRL file</param>
        /// <param name="childNode">Name of item</param>
        /// <param name="race">Currently selcted race</param>
        public ReadMtrl(int datNum, int mtrlOffset, string childNode, string race)
        {
            byte[] decompBytes;

            using (BinaryReader br = new BinaryReader(File.OpenRead(Properties.Settings.Default.DefaultDir + "/040000.win32.dat" + datNum)))
            {
                if (datNum == 3)
                {
                    mtrlOffset = mtrlOffset + 88;
                }
                else if (datNum == 2)
                {
                    mtrlOffset = mtrlOffset + 104;
                }
                else if (datNum == 1)
                {
                    mtrlOffset = mtrlOffset + 120;
                }
                else
                {
                    mtrlOffset = mtrlOffset + 136;
                }

                br.BaseStream.Seek(mtrlOffset, SeekOrigin.Begin);

                int dataSize = br.ReadInt32();
                int decompDataSize = br.ReadInt32();

                byte[] dataArray = br.ReadBytes(dataSize);

                decompBytes = new byte[decompDataSize];

                using (MemoryStream ms = new MemoryStream(dataArray))
                {
                    using (DeflateStream ds = new DeflateStream(ms, CompressionMode.Decompress))
                    {
                        int count = ds.Read(decompBytes, 0, decompDataSize);
                    }
                }
            }

            using (BinaryReader br1 = new BinaryReader(new MemoryStream(decompBytes)))
            {
                br1.BaseStream.Seek(6, SeekOrigin.Begin);
                short clrSize = br1.ReadInt16();
                short texNameSize = br1.ReadInt16();
                br1.ReadBytes(2);
                byte texNum = br1.ReadByte();
                byte mapNum = br1.ReadByte();
                byte clrNum = br1.ReadByte();
                byte unkNum = br1.ReadByte();
                br1.ReadBytes(4);

                if (texNum == 3)
                {
                    int tex1;
                    if (childNode.Equals(Resources.strings.Body))
                    {
                        texNum = (byte)(texNum - 1);
                        tex1 = br1.ReadInt16();
                        br1.ReadBytes(10);
                    }
                    else
                    {
                        tex1 = br1.ReadInt16();
                        br1.ReadBytes(10);
                    }

                    br1.ReadBytes(mapNum * 4);
                    byte[] bitName;
                    string name, sName;

                    for (int i = 0; i < texNum; i++)
                    {
                        bitName = br1.ReadBytes(tex1);
                        name = Encoding.ASCII.GetString(bitName);
                        name = name.Replace("\0", "");
                        sName = name.Substring(name.LastIndexOf("/") + 1);
                        if (childNode.Equals(Resources.strings.Body) || childNode.Equals(Resources.strings.Face) || childNode.Equals(Resources.strings.Tail))
                        {
                            if (Properties.Settings.Default.DXVer.Equals("dx11"))
                            {
                                sName = "--" + sName;
                            }
                        }
                        addTextures(name, sName);
                    }
                }
                else if (texNum == 2)
                {
                    int tex1 = br1.ReadInt16();
                    br1.ReadBytes(6);
                    br1.ReadBytes(mapNum * 4);

                    byte[] bitName;
                    string name, sName;

                    for (int i = 0; i < 2; i++)
                    {
                        bitName = br1.ReadBytes(tex1);
                        name = Encoding.ASCII.GetString(bitName);
                        name = name.Replace("\0", "");
                        sName = name.Substring(name.LastIndexOf("/") + 1);

                        if (childNode.Equals(Resources.strings.Hair) || childNode.Equals(Resources.strings.Tail))
                        {
                            if (Properties.Settings.Default.DXVer.Equals("dx11"))
                            {
                                sName = "--" + sName;
                            }
                        }

                        addTextures(name, sName);
                    }
                }
                else
                {
                    int tex1 = br1.ReadInt32();
                    br1.ReadBytes(mapNum * 4);
                    string name, sName;

                    byte[] bitName = br1.ReadBytes(tex1);
                    name = Encoding.ASCII.GetString(bitName);
                    name = name.Replace("\0", "");
                    sName = name.Substring(name.LastIndexOf("/") + 1);

                    addTextures(name, sName);
                }

                if (clrNum > 0 && clrSize > 0)
                {
                    br1.BaseStream.Seek(20 + 4 * (texNum + mapNum + clrNum) + texNameSize, SeekOrigin.Begin);

                    colorMapBytes = br1.ReadBytes(clrSize).ToArray();
                    textureOffsets.Add("ColorSet1", "");
                    textureNames.Add("_ColorSet1.tex");
                }
            }
        }

        /// <summary>
        /// Adds the texture names to dictionary
        /// </summary>
        /// <param name="name">full name of texture</param>
        /// <param name="sName">texture name substring</param>
        private void addTextures(string name, string sName)
        {
            if (name.Contains("_s.tex"))
            {
                fo = new FindOffset(sName);
                if (!fo.getFileOffset().Equals("0"))
                {
                    textureOffsets.Add("Specular", fo.getFileOffset());
                    textureNames.Add(sName);
                }
                else
                {
                    fo = new FindOffset(sName.Substring(2));
                    textureOffsets.Add("Specular", fo.getFileOffset());
                    textureNames.Add(sName);
                }
            }
            else if (name.Contains("_d.tex"))
            {
                fo = new FindOffset(sName);
                if (!fo.getFileOffset().Equals("0"))
                {
                    textureOffsets.Add("Diffuse", fo.getFileOffset());
                    textureNames.Add(sName);
                }
                else
                {
                    fo = new FindOffset(sName.Substring(2));
                    textureOffsets.Add("Diffuse", fo.getFileOffset());
                    textureNames.Add(sName);
                }
            }
            else if (name.Contains("_n.tex"))
            {
                fo = new FindOffset(sName);
                if (!fo.getFileOffset().Equals("0"))
                {
                    textureOffsets.Add("Normal", fo.getFileOffset());
                    textureNames.Add(sName);
                }
                else
                {
                    fo = new FindOffset(sName.Substring(2));
                    textureOffsets.Add("Normal", fo.getFileOffset());
                    textureNames.Add(sName);
                }
            }
            else if (name.Contains("_m.tex"))
            {
                fo = new FindOffset(sName);
                if (!fo.getFileOffset().Equals("0"))
                {
                    textureOffsets.Add("Mask", fo.getFileOffset());
                    textureNames.Add(sName);
                }
                else
                {
                    fo = new FindOffset(sName.Substring(2));
                    textureOffsets.Add("Mask", fo.getFileOffset());
                    textureNames.Add(sName);
                }

            }
        }

        public Dictionary<string, string> getTextureOffsets()
        {
            return textureOffsets;
        }

        public byte[] getColorMapBytes()
        {
            return colorMapBytes;
        }

        public List<string> getTextureNames()
        {
            return textureNames;
        }
    }
}
