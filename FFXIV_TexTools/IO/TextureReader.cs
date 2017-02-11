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

namespace FFXIV_TexTools.IO
{
    class TextureReader
    {
        byte[] decompressedTexture;
        int textureType;
        int[] dimensions = new int[2];


        /// <summary>
        /// Reads the texture data
        /// </summary>
        /// <param name="offset">Texture offset</param>
        /// <param name="file">dat file texture is located in</param>
        public TextureReader(string offset, string file)
        {
            List <byte> byteList = new List<byte>();

            using (BinaryReader br = new BinaryReader(File.OpenRead(file)))
            {
                int initialOffset = int.Parse(offset, NumberStyles.HexNumber);

                if (file.Contains(".dat1"))
                {
                    initialOffset = initialOffset - 16;
                }
                else if (file.Contains(".dat2"))
                {
                    initialOffset = initialOffset - 32;

                }
                else if (file.Contains(".dat3"))
                {
                    initialOffset = initialOffset - 48;
                }

                br.BaseStream.Seek(initialOffset, SeekOrigin.Begin);

                int headerLength = br.ReadInt32();
                int type = br.ReadInt32();
                int decompSize = br.ReadInt32();
                br.ReadBytes(8);
                int mipMapCount = br.ReadInt32();

                
                int endOfHeader = initialOffset + headerLength;
                int mipMapInfoStart = initialOffset + 24;

                br.BaseStream.Seek(endOfHeader + 4, SeekOrigin.Begin);

                textureType = br.ReadInt32();
                int width = br.ReadInt16();
                int height = br.ReadInt16();
                dimensions[0] = width;
                dimensions[1] = height;

                //Start MipMap Read
                for(int i = 0, j = 0; i < mipMapCount; i++)
                {
                    br.BaseStream.Seek(mipMapInfoStart + j, SeekOrigin.Begin);

                    int offsetFromHeaderEnd = br.ReadInt32();
                    int mipMapLength = br.ReadInt32();
                    int mipMapSize = br.ReadInt32();
                    int mipMapStart = br.ReadInt32();
                    int mipMapParts = br.ReadInt32();

                    int mipMapOffset = endOfHeader + offsetFromHeaderEnd;

                    br.BaseStream.Seek(mipMapOffset, SeekOrigin.Begin);

                    br.ReadBytes(8);
                    int compressedSize = br.ReadInt32();
                    int decompressedSize = br.ReadInt32();

                    if(mipMapParts > 1)
                    {
                        byte[] compressedData = br.ReadBytes(compressedSize);
                        byte[] decompressedData = new byte[decompressedSize];

                        using (MemoryStream ms = new MemoryStream(compressedData))
                        {
                            using (DeflateStream ds = new DeflateStream(ms, CompressionMode.Decompress))
                            {
                                ds.Read(decompressedData, 0x00, decompressedSize);
                            }
                        }

                        byteList.AddRange(decompressedData);

                        //start MipMap Parts Read
                        for (int k = 1; k < mipMapParts; k++)
                        {
                            byte check = br.ReadByte();
                            while (check != 0x10)
                            {
                                check = br.ReadByte();
                            }

                            br.ReadBytes(7);
                            compressedSize = br.ReadInt32();
                            decompressedSize = br.ReadInt32();

                            compressedData = br.ReadBytes(compressedSize);
                            decompressedData = new byte[decompressedSize];

                            using (MemoryStream ms = new MemoryStream(compressedData))
                            {
                                using (DeflateStream ds = new DeflateStream(ms, CompressionMode.Decompress))
                                {
                                    ds.Read(decompressedData, 0x00, decompressedSize);
                                }
                            }

                            byteList.AddRange(decompressedData);
                        }
                    }
                    else
                    {
                        byte[] compressedData, decompressedData;

                        if (compressedSize != 32000)
                        {
                            compressedData = br.ReadBytes(compressedSize);
                            decompressedData = new byte[decompressedSize];

                            using (MemoryStream ms = new MemoryStream(compressedData))
                            {
                                using (DeflateStream ds = new DeflateStream(ms, CompressionMode.Decompress))
                                {
                                    ds.Read(decompressedData, 0x00, decompressedSize);
                                }
                            }

                            byteList.AddRange(decompressedData);
                        }
                        else
                        {
                            decompressedData = br.ReadBytes(decompressedSize);
                            byteList.AddRange(decompressedData);
                        }
                    }
                    j = j + 20;
                }

                if (byteList.Count < decompSize)
                {
                    int difference = decompSize - byteList.Count;
                    byte[] padd = new byte[difference];
                    Array.Clear(padd, 0, difference);
                    byteList.AddRange(padd);
                }

            }
            decompressedTexture = byteList.ToArray();
        }

        public byte[] getDecompressedTexture()
        {
            return decompressedTexture;
        }

        public int getTextureType()
        {
            return textureType;
        }

        public int[] getDimensions()
        {
            return dimensions;
        }
    }
}
