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
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace FFXIV_TexTools.IO
{
    public class Save
    {
        Bitmap bmp;
        string parentNode, childNode, currMTRLName, currTexName;
        int selectedMapIndex, fileType, height, width;
        byte[] decompressedBytes;
        List<string> currTex;
        int[] dimensions;

        /// <summary>
        /// Saves the texture as a PNG file
        /// </summary>
        /// <param name="parentNode">Equipment slot name</param>
        /// <param name="childNode">Item name</param>
        /// <param name="bmp">Texture Bitmap</param>
        public Save(string parentNode, string childNode, Bitmap bmp)
        {
            this.parentNode = parentNode;
            this.childNode = childNode;
            this.bmp = bmp;
            savePNG(bmp);
        }

        /// <summary>
        /// Saves the texture as a DDS file
        /// </summary>
        /// <param name="parentNode">Equipment slot name</param>
        /// <param name="childNode">Item name</param>
        /// <param name="currTex">Currently selected texture</param>
        /// <param name="selectedMapIndex">Currently selected texture map</param>
        /// <param name="currMTRLName">MTRL file name of currently selcted texture</param>
        /// <param name="currTexName">Currently selected texture file name</param>
        /// <param name="decompressedBytes">Decompressed texture data array</param>
        /// <param name="width">Texture width</param>
        /// <param name="height">Texture height</param>
        /// <param name="fileType">DXT type</param>
        public Save(string parentNode, string childNode, List<string> currTex, int selectedMapIndex, string currMTRLName, string currTexName, byte[] decompressedBytes, int width, int height, int fileType)
        {
            this.parentNode = parentNode;
            this.childNode = childNode;
            this.selectedMapIndex = selectedMapIndex;
            this.currTex = currTex;
            this.currMTRLName = currMTRLName;
            this.currTexName = currTexName;
            this.decompressedBytes = decompressedBytes;
            this.fileType = fileType;
            this.width = width;
            this.height = height;

            saveDDS();
        }

        /// <summary>
        /// Creates the PNG file
        /// </summary>
        /// <param name="bmp">Bitmap</param>
        public void savePNG(Bitmap bmp)
        {
            string saveDir;
            string directory = Properties.Settings.Default.SaveDir + "/" + parentNode + "/" + childNode;
            Directory.CreateDirectory(directory);

            saveDir = Path.Combine(directory, (childNode + ".png"));

            bmp.Save(saveDir, ImageFormat.Png);
        }

        /// <summary>
        /// Creates the .DDS file
        /// </summary>
        public void saveDDS()
        {
            //create DDS header and attach to decompbytes
            string name = "";
            bool isColor = false;
            byte[] colorFooter = new byte[32];

            string directory = Properties.Settings.Default.SaveDir + "/" + parentNode + "/" + childNode;
            string directory1;
            Directory.CreateDirectory(directory);

            if (!childNode.Equals(Resources.strings.Face_Paint) && !childNode.Equals("Equipment Decals"))
            {
                name = currTex[selectedMapIndex];
                if (name.Contains("ColorSet1"))
                {
                    name = currMTRLName;
                    isColor = true;
                }
            }
            else
            {
                name = currTexName;
            }

            List<byte> DDS = new List<byte>();
            if (isColor)
            {
                DDS.AddRange(createColorDDSHeader());

                Array.Copy(decompressedBytes, decompressedBytes.Length - 32, colorFooter, 0, 32);
                File.WriteAllBytes(directory + "/dye.dat", colorFooter);
            }
            else
            {
                DDS.AddRange(createDDSHeader());
            }
            DDS.AddRange(decompressedBytes);


            name = name.Substring(0, name.LastIndexOf('.')) + ".dds";
            directory1 = Path.Combine(directory, name);
            File.WriteAllBytes(directory1, DDS.ToArray());
        }

        /// <summary>
        /// Creates the header for the Texture DDS file
        /// </summary>
        /// <returns>header as a byte array</returns>
        public byte[] createDDSHeader()
        {
            uint m_linearsize;
            uint m_pflags;
            List<byte> header = new List<byte>();
            //DDS
            uint m_magic = 0x20534444;
            header.AddRange(BitConverter.GetBytes(m_magic));
            //header size
            uint m_size = 124;
            header.AddRange(BitConverter.GetBytes(m_size));
            //Flags (DDSD_CAPS, DDSD_HEIGHT, DDSD_WIDTH, DDSD_PIXELFORMAT, DDSD_LINEARSIZE);
            uint m_flags = 528391;
            header.AddRange(BitConverter.GetBytes(m_flags));
            //height
            uint m_height = (uint)height;
            header.AddRange(BitConverter.GetBytes(m_height));
            //width
            uint m_width = (uint)width;
            header.AddRange(BitConverter.GetBytes(m_width));
            //Linearsize
            if (fileType == 9312)
            {
                m_linearsize = 512;
            }
            else if (fileType == 5200)
            {
                m_linearsize = (uint)((m_height * m_width) * 4);
            }
            else
            {
                m_linearsize = (uint)(m_height * m_width);
            }
            header.AddRange(BitConverter.GetBytes(m_linearsize));
            //depth
            uint m_depth = 0;
            header.AddRange(BitConverter.GetBytes(m_depth));
            //mipmap count
            uint m_mipmap = 0;
            header.AddRange(BitConverter.GetBytes(m_mipmap));
            //blank
            byte[] blank = new byte[44];
            Array.Clear(blank, 0, 44);
            header.AddRange(blank);
            //pixelformat size
            uint m_psize = 32;
            header.AddRange(BitConverter.GetBytes(m_psize));
            //pixelformat flags (DDPF_FOURCC)
            if (fileType == 5200)
            {
                m_pflags = 65;
            }
            else
            {
                m_pflags = 4;
            }

            header.AddRange(BitConverter.GetBytes(m_pflags));
            //pixelformat dwFourCC
            if (fileType == 13344)
            {
                uint m_filetype = 0x31545844;
                header.AddRange(BitConverter.GetBytes(m_filetype));
            }
            else if (fileType == 13361)
            {
                uint m_filetype = 0x35545844;
                header.AddRange(BitConverter.GetBytes(m_filetype));
            }
            else if (fileType == 13360)
            {
                uint m_filetype = 0x33545844;
                header.AddRange(BitConverter.GetBytes(m_filetype));
            }
            else if (fileType == 9312)
            {
                uint m_filetype = 0x71;
                header.AddRange(BitConverter.GetBytes(m_filetype));
            }
            else if (fileType == 5200)
            {
                uint m_filetype = 0;
                header.AddRange(BitConverter.GetBytes(m_filetype));
            }
            else
            {
                return null;
            }

            if (fileType == 5200)
            {
                uint m_bpp = 32;
                header.AddRange(BitConverter.GetBytes(m_bpp));
                uint m_red = 16711680;
                header.AddRange(BitConverter.GetBytes(m_red));
                uint m_green = 65280;
                header.AddRange(BitConverter.GetBytes(m_green));
                uint m_blue = 255;
                header.AddRange(BitConverter.GetBytes(m_blue));
                uint m_alpha = 4278190080;
                header.AddRange(BitConverter.GetBytes(m_alpha));
                uint m_dwCaps = 4096;
                header.AddRange(BitConverter.GetBytes(m_dwCaps));

                byte[] blank1 = new byte[16];
                Array.Clear(blank, 0, 16);
                header.AddRange(blank1);

            }
            else
            {
                //blank1
                byte[] blank1 = new byte[40];
                Array.Clear(blank, 0, 40);
                header.AddRange(blank1);
            }

            return header.ToArray();
        }


        /// <summary>
        /// Creates a header for Color Map DDS file
        /// </summary>
        /// <returns>header as a byte array</returns>
        public byte[] createColorDDSHeader()
        {
            uint m_linearsize;
            List<byte> header = new List<byte>();
            //DDS
            uint m_magic = 0x20534444;
            header.AddRange(BitConverter.GetBytes(m_magic));
            //header size
            uint m_size = 124;
            header.AddRange(BitConverter.GetBytes(m_size));
            //Flags (DDSD_CAPS, DDSD_HEIGHT, DDSD_WIDTH, DDSD_PIXELFORMAT, DDSD_LINEARSIZE);
            uint m_flags = 528399;
            header.AddRange(BitConverter.GetBytes(m_flags));
            //height
            uint m_height = 16;
            header.AddRange(BitConverter.GetBytes(m_height));
            //width
            uint m_width = 4;
            header.AddRange(BitConverter.GetBytes(m_width));
            //Linearsize
            m_linearsize = 512;
            header.AddRange(BitConverter.GetBytes(m_linearsize));
            //depth
            uint m_depth = 0;
            header.AddRange(BitConverter.GetBytes(m_depth));
            //mipmap count
            uint m_mipmap = 0;
            header.AddRange(BitConverter.GetBytes(m_mipmap));
            //blank
            byte[] blank = new byte[44];
            Array.Clear(blank, 0, 44);
            header.AddRange(blank);
            //pixelformat size
            uint m_psize = 32;
            header.AddRange(BitConverter.GetBytes(m_psize));
            //pixelformat flags (DDPF_FOURCC)
            uint m_pflags = 4;
            header.AddRange(BitConverter.GetBytes(m_pflags));
            //pixelformat dwFourCC
            uint m_filetype = 0x71;
            header.AddRange(BitConverter.GetBytes(m_filetype));
            //blank1
            byte[] blank1 = new byte[40];
            Array.Clear(blank, 0, 40);
            header.AddRange(blank1);

            return header.ToArray();
        }
    }
}
