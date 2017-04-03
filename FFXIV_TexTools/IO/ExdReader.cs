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
using System.Text;
using System.Threading.Tasks;
using FFXIV_TexTools.Helpers;
using FFXIV_TexTools.Resources;

namespace FFXIV_TexTools.IO
{
    class ExdReader
    {
        Dictionary<string, string> nameSlotDict = new Dictionary<string, string>();
        Dictionary<string, Items> itemsDict = new Dictionary<string, Items>();
        Dictionary<string, Items> mountsDict = new Dictionary<string, Items>();
        Dictionary<string, Items> minionsDict = new Dictionary<string, Items>();
        byte[] decompBytes;

        /// <summary>
        /// Reads and parses the EXD files which contain data for items, hair, mounts and minions.
        /// </summary>
        /// <param name="exd">The type of exd file passed in</param>
        /// <param name="offset">Offset to the exd file</param>
        public ExdReader(string exd, string offset)
        {

            if (exd.Equals("Item"))
            {
                makeItemsList(offset);
            }
            if (exd.Equals(Resources.strings.Hair))
            {
                getDecompressedBytes(offset);
            }
            if (exd.Equals("Mounts"))
            {
                makeMountsList(offset);
            }
            if (exd.Equals("Minions"))
            {
                makeMinionsList(offset);
            }
        }

        /// <summary>
        /// Makes the list of minions 
        /// </summary>
        /// <param name="offset">The offset where the minion list is located</param>
        private void makeMinionsList(string offset)
        {
            byte[] minionsBytes = getDecompressedBytes(offset);
            byte[] modelchara = getDecompressedBytes(strings.Modelchara_0);

            using (BinaryReader br = new BinaryReader(new MemoryStream(minionsBytes)))
            {
                using (BinaryReader br1 = new BinaryReader(new MemoryStream(modelchara)))
                {

                    int duplicateCount = 1;
                    br.ReadBytes(8);
                    int offsetTableSize = BitConverter.ToInt32(br.ReadBytes(4).Reverse().ToArray(), 0);

                    for (int i = 0; i < offsetTableSize; i += 8)
                    {
                        br.BaseStream.Seek(i + 32, SeekOrigin.Begin);
                        int index = BitConverter.ToInt32(br.ReadBytes(4).Reverse().ToArray(), 0);
                        int tableOffset = BitConverter.ToInt32(br.ReadBytes(4).Reverse().ToArray(), 0);

                        br.BaseStream.Seek(tableOffset, SeekOrigin.Begin);

                        br.ReadBytes(13);

                        int firstText = br.ReadByte();

                        if (firstText >= 2)
                        {
                            br.ReadBytes(8);

                            uint modelIndex = BitConverter.ToUInt16(br.ReadBytes(2).Reverse().ToArray(), 0);

                            br.ReadBytes(30);

                            byte[] mountNameBytes = br.ReadBytes(firstText - 1);
                            string mountName = Encoding.UTF8.GetString(mountNameBytes);
                            mountName = mountName.Replace("\0", "");

                            br1.ReadBytes(8);
                            int offsetTableSize1 = BitConverter.ToInt32(br1.ReadBytes(4).Reverse().ToArray(), 0);

                            for (int j = 0; j < offsetTableSize1; j += 8)
                            {
                                br1.BaseStream.Seek(j + 32, SeekOrigin.Begin);

                                uint index1 = BitConverter.ToUInt32(br1.ReadBytes(4).Reverse().ToArray(), 0);

                                if (index1 == modelIndex)
                                {
                                    int tableOffset1 = BitConverter.ToInt32(br1.ReadBytes(4).Reverse().ToArray(), 0);

                                    br1.BaseStream.Seek(tableOffset1, SeekOrigin.Begin);

                                    br1.ReadBytes(6);

                                    int model = BitConverter.ToInt16(br1.ReadBytes(2).Reverse().ToArray(), 0);
                                    br1.ReadBytes(3);
                                    int body = br1.ReadByte();
                                    int variant = br1.ReadByte();

                                    Items item = new Items(model.ToString(), "", "", variant.ToString(), "", body.ToString(), "", false);

                                    if(model != 0)
                                    {
                                        try
                                        {
                                            minionsDict.Add(mountName, item);
                                        }
                                        catch
                                        {
                                            minionsDict.Add(mountName + "_" + duplicateCount, item);
                                            duplicateCount++;
                                        }
                                    }


                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Makes the list of mounts
        /// </summary>
        /// <param name="offset">Offset where the mounts are located</param>
        private void makeMountsList(string offset)
        {
            byte[] mountBytes = getDecompressedBytes(offset);
            byte[] modelchara = getDecompressedBytes(strings.Modelchara_0);

            using (BinaryReader br = new BinaryReader(new MemoryStream(mountBytes)))
            {
                using (BinaryReader br1 = new BinaryReader(new MemoryStream(modelchara)))
                {
                    br.ReadBytes(8);
                    int offsetTableSize = BitConverter.ToInt32(br.ReadBytes(4).Reverse().ToArray(), 0);

                    for (int i = 0; i < offsetTableSize; i += 8)
                    {
                        br.BaseStream.Seek(i + 32, SeekOrigin.Begin);
                        int index = BitConverter.ToInt32(br.ReadBytes(4).Reverse().ToArray(), 0);
                        int tableOffset = BitConverter.ToInt32(br.ReadBytes(4).Reverse().ToArray(), 0);

                        br.BaseStream.Seek(tableOffset, SeekOrigin.Begin);

                        br.ReadBytes(13);

                        int firstText = br.ReadByte();

                        if (firstText >= 2)
                        {
                            br.ReadBytes(70);

                            uint modelIndex = BitConverter.ToUInt16(br.ReadBytes(2).Reverse().ToArray(), 0);

                            br.ReadBytes(40);

                            byte[] mountNameBytes = br.ReadBytes(firstText - 1);
                            string mountName = Encoding.UTF8.GetString(mountNameBytes);
                            mountName = mountName.Replace("\0", "");

                            br1.ReadBytes(8);
                            int offsetTableSize1 = BitConverter.ToInt32(br1.ReadBytes(4).Reverse().ToArray(), 0);

                            for (int j = 0; j < offsetTableSize1; j += 8)
                            {
                                br1.BaseStream.Seek(j + 32, SeekOrigin.Begin);

                                uint index1 = BitConverter.ToUInt32(br1.ReadBytes(4).Reverse().ToArray(), 0);
                                if (index1 == modelIndex)
                                {
                                    int tableOffset1 = BitConverter.ToInt32(br1.ReadBytes(4).Reverse().ToArray(), 0);

                                    br1.BaseStream.Seek(tableOffset1, SeekOrigin.Begin);

                                    br1.ReadBytes(6);

                                    int model = BitConverter.ToInt16(br1.ReadBytes(2).Reverse().ToArray(), 0);
                                    br1.ReadBytes(3);
                                    int body = br1.ReadByte();
                                    int variant = br1.ReadByte();

                                    Items item = new Items(model.ToString(), "", "", variant.ToString(), "", body.ToString(), "", false);

                                    if (!mountsDict.ContainsKey(mountName))
                                    {
                                        mountsDict.Add(mountName, item);
                                    }

                                    break;
                                }
                            }
                        }
                    }
                }
            }


        }

        /// <summary>
        /// Decompresses the byte array 
        /// </summary>
        /// <param name="offset">Offset of the byte list to decompress</param>
        /// <returns></returns>
        public byte[] getDecompressedBytes(string offset)
        {
            int initialOffset = int.Parse(offset, NumberStyles.HexNumber);
            List<byte> byteList = new List<byte>();

            using (BinaryReader br = new BinaryReader(File.OpenRead(Properties.Settings.Default.DefaultDir + "/0a0000.win32.dat0")))
            {
                br.BaseStream.Seek(initialOffset, SeekOrigin.Begin);

                int headerLength = br.ReadInt32();
                int type = br.ReadInt32();
                int decompressedSize = br.ReadInt32();
                br.ReadBytes(8);
                int parts = br.ReadInt32();

                int endOfHeader = initialOffset + headerLength;
                int partStart = initialOffset + 24;

                for (int f = 0, g = 0; f < parts; f++)
                {
                    //read the current parts info
                    br.BaseStream.Seek(partStart + g, SeekOrigin.Begin);
                    int fromHeaderEnd = br.ReadInt32();
                    int partLength = br.ReadInt16();
                    int partSize = br.ReadInt16();
                    int partLocation = endOfHeader + fromHeaderEnd;

                    //go to part data and read its info
                    br.BaseStream.Seek(partLocation, SeekOrigin.Begin);
                    br.ReadBytes(8);
                    int partCompressedSize = br.ReadInt32();
                    int partDecompressedSize = br.ReadInt32();

                    //if data is already uncompressed add to list if not decompress and add to list
                    if (partCompressedSize == 32000)
                    {
                        byte[] forlist = br.ReadBytes(partDecompressedSize);
                        byteList.AddRange(forlist);
                    }
                    else
                    {
                        byte[] forlist = br.ReadBytes(partCompressedSize);
                        byte[] partDecompressedBytes = new byte[partDecompressedSize];

                        using (MemoryStream ms = new MemoryStream(forlist))
                        {
                            using (DeflateStream ds = new DeflateStream(ms, CompressionMode.Decompress))
                            {
                                int count = ds.Read(partDecompressedBytes, 0x00, partDecompressedSize);
                            }
                        }
                        byteList.AddRange(partDecompressedBytes);
                    }
                    g += 8;
                }
            }
            decompBytes = byteList.ToArray();
            return decompBytes;
        }

        /// <summary>
        /// Makes the list of items
        /// </summary>
        /// <param name="offset">Offset of items list</param>
        public void makeItemsList(string offset)
        {
            int initialOffset = int.Parse(offset, NumberStyles.HexNumber);
            List<byte> byteList = new List<byte>();

            //the smallclothes are not on the item list, so they are added manualy
            if (offset.Equals(Resources.strings.Item_0))
            {
                Items item = new Items("0000", "0000", "4", "1", "1", "", "", false);
                itemsDict.Add("SmallClothes Body", item);
                nameSlotDict.Add("SmallClothes Body", "4");

                item = new Items("0000", "0000", "7", "1", "1", "", "", false);
                itemsDict.Add("SmallClothes Legs", item);
                nameSlotDict.Add("SmallClothes Legs", "7");

                item = new Items("0000", "0000", "8", "1", "1", "", "", false);
                itemsDict.Add("SmallClothes Feet", item);
                nameSlotDict.Add("SmallClothes Feet", "8");
            }

            using (BinaryReader br = new BinaryReader(File.OpenRead(Properties.Settings.Default.DefaultDir + "/0a0000.win32.dat0")))
            {
                br.BaseStream.Seek(initialOffset, SeekOrigin.Begin);

                int headerLength = br.ReadInt32();
                int type = br.ReadInt32();
                int decompressedSize = br.ReadInt32();
                br.ReadBytes(8);
                int parts = br.ReadInt32();

                int endOfHeader = initialOffset + headerLength;
                int partStart = initialOffset + 24;

                for (int f = 0, g = 0; f < parts; f++)
                {
                    //read the current parts info
                    br.BaseStream.Seek(partStart + g, SeekOrigin.Begin);
                    int fromHeaderEnd = br.ReadInt32();
                    int partLength = br.ReadInt16();
                    int partSize = br.ReadInt16();
                    int partLocation = endOfHeader + fromHeaderEnd;

                    //go to part data and read its info
                    br.BaseStream.Seek(partLocation, SeekOrigin.Begin);
                    br.ReadBytes(8);
                    int partCompressedSize = br.ReadInt32();
                    int partDecompressedSize = br.ReadInt32();

                    //if data is already uncompressed add to list if not decompress and add to list
                    if (partCompressedSize == 32000)
                    {
                        byte[] forlist = br.ReadBytes(partDecompressedSize);
                        byteList.AddRange(forlist);
                    }
                    else
                    {
                        byte[] forlist = br.ReadBytes(partCompressedSize);
                        byte[] partDecompressedBytes = new byte[partDecompressedSize];

                        using (MemoryStream ms = new MemoryStream(forlist))
                        {
                            using (DeflateStream ds = new DeflateStream(ms, CompressionMode.Decompress))
                            {
                                int count = ds.Read(partDecompressedBytes, 0x00, partDecompressedSize);
                            }
                        }
                        byteList.AddRange(partDecompressedBytes);
                    }
                    g += 8;
                }

                if(byteList.Count < decompressedSize)
                {
                    int difference = decompressedSize - byteList.Count;
                    byte[] padd = new byte[difference];
                    Array.Clear(padd, 0, difference);
                    byteList.AddRange(padd);
                }
            }

            using (BinaryReader br = new BinaryReader(new MemoryStream(byteList.ToArray())))
            {
                br.ReadBytes(8);
                int offsetTableSize = BitConverter.ToInt32(br.ReadBytes(4).Reverse().ToArray(), 0);

                for(int i = 0; i < offsetTableSize; i += 8)
                {
                    br.BaseStream.Seek(i + 32, SeekOrigin.Begin);
                    int index = BitConverter.ToInt32(br.ReadBytes(4).Reverse().ToArray(), 0);
                    int tableOffset = BitConverter.ToInt32(br.ReadBytes(4).Reverse().ToArray(), 0);

                    br.BaseStream.Seek(tableOffset, SeekOrigin.Begin);
                    int entrySize = BitConverter.ToInt32(br.ReadBytes(4).Reverse().ToArray(), 0);
                    br.ReadBytes(17);
                    //int lastText = BitConverter.ToInt16(br.ReadBytes(4).ToArray(), 0);
                    int lastText = br.ReadByte();
                    br.ReadBytes(3);

                    string imcVersion = "0", imcVersion1 = "0", weaponBody = "0", weaponBody1 = "0", itemID = "0", itemID1 = "0";
                    if(lastText > 10)
                    {
                        bool hasSecondary = false;
                        br.ReadBytes(7);
                        byte[] textureDetails = br.ReadBytes(4).ToArray();
                        int itemCheck = textureDetails[3];
                        //if item has textureDetails
                        if(itemCheck != 0)
                        {
                            int weaponCheck = textureDetails[1];
                            if (weaponCheck == 0)
                            {
                                //not a weapon
                                imcVersion = textureDetails[3].ToString().PadLeft(2, '0');
                            }
                            else
                            {
                                //is a weapon
                                imcVersion = weaponCheck.ToString().PadLeft(2, '0');
                                weaponBody = textureDetails[3].ToString().PadLeft(4, '0');
                            }

                            itemID = BitConverter.ToInt16(br.ReadBytes(2).Reverse().ToArray(), 0).ToString().PadLeft(4, '0');
                            br.ReadBytes(2);

                            textureDetails = br.ReadBytes(4).ToArray();
                            int secondaryCheck = textureDetails[3];
                            if (secondaryCheck != 0)
                            {
                                //Secondary textureDetails
                                hasSecondary = true;
                                weaponCheck = textureDetails[1];
                                if (weaponCheck == 0)
                                {
                                    //not a weapon
                                    imcVersion1 = textureDetails[3].ToString().PadLeft(2, '0');
                                }
                                else
                                {
                                    //is a weapon
                                    imcVersion1 = weaponCheck.ToString().PadLeft(2, '0');
                                    weaponBody1 = textureDetails[3].ToString().PadLeft(4, '0');
                                }

                                itemID1 = BitConverter.ToInt16(br.ReadBytes(2).Reverse().ToArray(), 0).ToString().PadLeft(4, '0');
                                br.ReadBytes(2);
                            }

                            if (!hasSecondary)
                            {
                                br.ReadBytes(118);
                            }
                            else
                            {
                                br.ReadBytes(114);
                            }


                            byte[] slotBytes = br.ReadBytes(4).ToArray();
                            string equipSlot = slotBytes[0].ToString();

                            br.ReadBytes(lastText);

                            byte[] itemNameBytes = br.ReadBytes(entrySize - (lastText + 160));
                            //string itemName = Encoding.ASCII.GetString(itemNameBytes);
                            string itemName = Encoding.UTF8.GetString(itemNameBytes);
                            itemName = itemName.Replace("\0", "");

                            
                            try
                            {
                                nameSlotDict.Add(itemName, equipSlot);
                            }
                            catch(Exception e)
                            {
                            }
                            Items items = new Items(itemID, itemID1, equipSlot, imcVersion, imcVersion1, weaponBody, weaponBody1, hasSecondary);

                            try
                            {
                                itemsDict.Add(itemName, items);
                            }
                            catch(Exception e)
                            {
                            }
                        }

                    }
                }

            }
        }

        public Dictionary<string, string> getNameSlotDict()
        {
            return nameSlotDict;
        }

        public Dictionary<string, Items> getItemsDict()
        {
            return itemsDict;
        }

        public Dictionary<string, Items> getMountsDict()
        {
            return mountsDict;
        }

        public Dictionary<string, Items> getMinionsDict()
        {
            return minionsDict;
        }

        public byte[] getDecompBytes()
        {
            return decompBytes;
        }
    }
}
