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
using System.IO;
using System.Linq;
using FFXIV_TexTools.Helpers;

namespace FFXIV_TexTools.IO
{
    public class FindOffset
    {
        FFCRC crc = new FFCRC();

        string fileOffset = "0";
        string fileCRC;
        List<int> races;

        /// <summary>
        /// Finds the offset of a texture name
        /// </summary>
        /// <param name="textureName">Name of the texture to find the offset for</param>
        public FindOffset(string textureName)
        {
            
            if (textureName.Contains(".tex") || textureName.Contains(".mdl"))
            {
                fileCRC = crc.text(textureName).PadLeft(8, '0');
            }
            else
            {
                fileCRC = textureName;
            }

            using (BinaryReader br = new BinaryReader(File.OpenRead(Properties.Settings.Default.DefaultDir + "/040000.win32.index")))
            {
                br.BaseStream.Seek(1036, SeekOrigin.Begin);
                int totalFiles = br.ReadInt32();

                br.BaseStream.Seek(2048, SeekOrigin.Begin);
                for (int i = 0; i < totalFiles; br.ReadBytes(4), i += 16)
                {
                    string tempOffset = br.ReadInt32().ToString("X").PadLeft(8, '0');

                    if (tempOffset.Equals(fileCRC))
                    {
                        br.ReadBytes(4);
                        byte[] offset = br.ReadBytes(4);
                        fileOffset = (BitConverter.ToInt32(offset, 0) * 8).ToString("X").PadLeft(8, '0');
                        break;
                    }
                    else
                    {
                        br.ReadBytes(8);
                    }
                }
            }
        }

        /// <summary>
        /// Find the offset of the texture and folder given the offset in hex form
        /// </summary>
        /// <param name="textureHex">hex offset of texture</param>
        /// <param name="folderHex">hex offset for folder</param>
        public FindOffset(string textureHex, string folderHex)
        {
            using (BinaryReader br = new BinaryReader(File.OpenRead(Properties.Settings.Default.DefaultDir + "/040000.win32.index")))
            {
                br.BaseStream.Seek(1036, SeekOrigin.Begin);
                int totalFiles = br.ReadInt32();

                br.BaseStream.Seek(2048, SeekOrigin.Begin);
                for (int i = 0; i < totalFiles; br.ReadBytes(4), i += 16)
                {
                    string tempOffset = br.ReadInt32().ToString("X").PadLeft(8, '0');

                    if (tempOffset.Equals(textureHex))
                    {
                        if (folderHex.Equals("none"))
                        {
                            br.ReadBytes(4);
                            byte[] offset = br.ReadBytes(4);
                            fileOffset = (BitConverter.ToUInt32(offset, 0) * 8).ToString("X").PadLeft(8, '0');
                            break;
                        }

                        string foHex = br.ReadInt32().ToString("X").PadLeft(8, '0');

                        if (foHex.Equals(folderHex))
                        {
                            byte[] offset = br.ReadBytes(4);
                            fileOffset = (BitConverter.ToUInt32(offset, 0) * 8).ToString("X").PadLeft(8, '0');
                            break;
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
            }
        }

        //not currntly in use
        public FindOffset(string[] textureHexs, string folderHex)
        {
            using (BinaryReader br = new BinaryReader(File.OpenRead(Properties.Settings.Default.DefaultDir + "/040000.win32.index")))
            {
                br.BaseStream.Seek(1036, SeekOrigin.Begin);
                int totalFiles = br.ReadInt32();

                br.BaseStream.Seek(2048, SeekOrigin.Begin);
                for (int i = 0; i < totalFiles; br.ReadBytes(4), i += 16)
                {
                    string tempOffset = br.ReadInt32().ToString("X").PadLeft(8, '0');

                    if (textureHexs.Contains(tempOffset))
                    {
                        string foHex = br.ReadInt32().ToString("X").PadLeft(8, '0');

                        if (foHex.Equals(folderHex))
                        {
                            byte[] offset = br.ReadBytes(4);
                            fileOffset = (BitConverter.ToUInt32(offset, 0) * 8).ToString("X").PadLeft(8, '0');
                            races.Add(Array.IndexOf(textureHexs, tempOffset));
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
            }
        }
        public string getFileOffset()
        {
            return fileOffset;
        }

        public int[] getRaces()
        {
            return races.ToArray();
        }
    }
}
