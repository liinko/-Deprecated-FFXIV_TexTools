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
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework.Graphics;

namespace FFXIV_TexTools.Helpers
{
    class TextureToBitmap
    {
        Bitmap bmp;
        string typeString;

        public TextureToBitmap(byte[] decompressedData, int textureType, int[] dimensions)
        {
            byte[] decompressedTexture;

            switch (textureType)
            {
                //DXT1
                case 13344:
                    typeString = "DXT1";
                    decompressedTexture = DxtUtil.DecompressDxt1(decompressedData, dimensions[0], dimensions[1]);
                    bmp = readLinearImage(decompressedTexture, dimensions[0], dimensions[1]);
                    break;
                //DXT3
                case 13360:
                    typeString = "DXT3";
                    decompressedTexture = DxtUtil.DecompressDxt3(decompressedData, dimensions[0], dimensions[1]);
                    bmp = readLinearImage(decompressedTexture, dimensions[0], dimensions[1]);
                    break;
                //DXT5	
                case 13361:
                    typeString = "DXT5";
                    decompressedTexture = DxtUtil.DecompressDxt5(decompressedData, dimensions[0], dimensions[1]);
                    bmp = readLinearImage(decompressedTexture, dimensions[0], dimensions[1]);
                    break;
                //8-bit image	
                case 4401:
                case 4400:
                    typeString = "8bit";
                    bmp = read8bitImage(decompressedData, dimensions[0], dimensions[1]);
                    break;
                //16-bit image in RGB4444 format	
                case 5184:
                    typeString = "16bit R4G4B4A4";
                    bmp = read4444Image(decompressedData, dimensions[0], dimensions[1]);
                    break;
                //16-bit image in RGB5551 format
                case 5185:
                    typeString = "16bit R5G5B5A1";
                    bmp = read5551Image(decompressedData, dimensions[0], dimensions[1]);
                    break;
                //32-bit A8R8G8B8 image
                case 5200:
                case 4440:
                case 5201:
                    typeString = "32bit A8R8G8B8";
                    bmp = new Bitmap(dimensions[0], dimensions[1], dimensions[0] * 4, System.Drawing.Imaging.PixelFormat.Format32bppArgb, Marshal.UnsafeAddrOfPinnedArrayElement(decompressedData, 0));
                    break;
                //64-bit A16R16G16B16 image	
                case 9312:
                    typeString = "64bit A16R16G16B16";
                    bmp = readRGBAFImage(decompressedData, dimensions[0], dimensions[1]);
                    break;
            }
        }

        public Bitmap getBitmap()
        {
            return bmp;
        }

        public string getTypeString()
        {
            return typeString;
        }

        private Bitmap readLinearImage(byte[] data, int w, int h)
        {
            Bitmap res = new Bitmap(w, h);
            using (MemoryStream ms = new MemoryStream(data))
            {
                using (BinaryReader r = new BinaryReader(ms))
                {
                    for (int y = 0; y < h; y++)
                    {
                        for (int x = 0; x < w; x++)
                        {
                            byte red = r.ReadByte();
                            byte green = r.ReadByte();
                            byte blue = r.ReadByte();
                            byte alpha = r.ReadByte();

                            res.SetPixel(x, y, Color.FromArgb(alpha, red, green, blue));
                        }
                    }
                }
            }
            return res;
        }

        //read RGBAF to bitmap
        private Bitmap readRGBAFImage(byte[] data, int w, int h)
        {
            Bitmap res = new Bitmap(w, h);
            using (MemoryStream ms = new MemoryStream(data))
            {
                using (BinaryReader r = new BinaryReader(ms))
                {
                    for (int y = 0; y < h; y++)
                    {
                        for (int x = 0; x < w; x++)
                        {
                            ushort sred = BitConverter.ToUInt16(r.ReadBytes(2), 0);
                            Half h1 = Half.ToHalf(sred);
                            ushort sgreen = BitConverter.ToUInt16(r.ReadBytes(2), 0);
                            Half h2 = Half.ToHalf(sgreen);
                            ushort sblue = BitConverter.ToUInt16(r.ReadBytes(2), 0);
                            Half h3 = Half.ToHalf(sblue);
                            ushort salpha = BitConverter.ToUInt16(r.ReadBytes(2), 0);
                            Half h4 = Half.ToHalf(salpha);
                            if (h1 > 1)
                            {
                                h1 = 1;
                            }
                            else if (h1 < 0)
                            {
                                h1 = 0;
                            }
                            if (h2 > 1)
                            {
                                h2 = 1;
                            }
                            else if (h2 < 0)
                            {
                                h2 = 0;
                            }
                            if (h3 > 1)
                            {
                                h3 = 1;
                            }
                            else if (h3 < 0)
                            {
                                h3 = 0;
                            }
                            if (h4 > 1)
                            {
                                h4 = 1;
                            }
                            else if (h4 < 0)
                            {
                                h4 = 0;
                            }

                            res.SetPixel(x, y, Color.FromArgb(255, (int)(h1 * 255), (int)(h2 * 255), (int)(h3 * 255)));
                        }
                    }
                }
            }
            return res;
        }

        //16-bit RGBA4444 to bitmap
        private Bitmap read4444Image(byte[] data, int w, int h)
        {
            Bitmap res = new Bitmap(w, h);
            using (MemoryStream ms = new MemoryStream(data))
            {
                using (BinaryReader r = new BinaryReader(ms))
                {
                    for (int y = 0; y < h; y++)
                    {
                        for (int x = 0; x < w; x++)
                        {
                            int pixel = r.ReadUInt16() & 0xffff;
                            int red = ((pixel & 0xF)) * 16;
                            int green = ((pixel & 0xF0 >> 4)) * 16;
                            int blue = ((pixel & 0xF00 >> 8)) * 16;
                            int alpha = ((pixel & 0xF000 >> 12)) * 16;

                            res.SetPixel(x, y, Color.FromArgb(alpha, red, green, blue));
                        }
                    }
                }
            }
            return res;
        }

        //RGBA5551 to bitmap
        private Bitmap read5551Image(byte[] data, int w, int h)
        {
            Bitmap res = new Bitmap(w, h);
            using (MemoryStream ms = new MemoryStream(data))
            {
                using (BinaryReader r = new BinaryReader(ms))
                {
                    for (int y = 0; y < h; y++)
                    {
                        for (int x = 0; x < w; x++)
                        {
                            int pixel = r.ReadUInt16() & 0xffff;
                            int red = ((pixel & 0x7e00 >> 10)) * 8;
                            int green = ((pixel & 0x3E0 >> 5)) * 8;
                            int blue = ((pixel & 0x1F)) * 8;
                            int alpha = ((pixel & 0x80000 >> 15)) * 255;

                            res.SetPixel(x, y, Color.FromArgb(alpha, red, green, blue));
                        }
                    }
                }
            }
            return res;
        }

        //8-bit to bitmap
        private Bitmap read8bitImage(byte[] data, int w, int h)
        {
            Bitmap res = new Bitmap(w, h);

            using (MemoryStream ms = new MemoryStream(data))
            {
                using (BinaryReader r = new BinaryReader(ms))
                {
                    for (int y = 0; y < h; y++)
                    {
                        for (int x = 0; x < w; x++)
                        {
                            int pixel = r.ReadByte() & 0xff;

                            int red = pixel;
                            int green = pixel;
                            int blue = pixel;
                            byte alpha = 255;

                            res.SetPixel(x, y, Color.FromArgb(alpha, red, green, blue));
                        }
                    }
                }
            }
            return res;
        }
    }
}
