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
using System.Globalization;
using System.IO;
using System.Linq;
using FFXIV_TexTools.IO;

namespace FFXIV_TexTools.Helpers
{
    public class Helper
    {
        public Helper()
        { }

        public int getDatLoc(byte loc) 
        {
            return loc & 0x000F;
        }

        /// <summary>
        /// Reads an items ColorSet data
        /// </summary>
        /// <param name="data">ColorSet data array</param>
        /// <param name="w">width</param>
        /// <param name="h">height</param>
        /// <returns>Colorset as a Bitmap</returns>
        public Bitmap readRGBAFImage(byte[] data, int w, int h)
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
                            int i = ((y * w) + x) * 4;

                            Half h1 = Half.ToHalf(r.ReadBytes(2).ToArray(), 0);
                            if (h1 > 1)
                            {
                                h1 = 1;
                            }
                            else if (h1 < 0)
                            {
                                h1 = 0;
                            }

                            Half h2 = Half.ToHalf(r.ReadBytes(2).ToArray(), 0);
                            if (h2 > 1)
                            {
                                h2 = 1;
                            }
                            else if (h2 < 0)
                            {
                                h2 = 0;
                            }

                            Half h3 = Half.ToHalf(r.ReadBytes(2).ToArray(), 0);
                            if (h3 > 1)
                            {
                                h3 = 1;
                            }
                            else if (h3 < 0)
                            {
                                h3 = 0;
                            }

                            Half h4 = Half.ToHalf(r.ReadBytes(2).ToArray(), 0);
                            if (h4 > 1)
                            {
                                h4 = 1;
                            }
                            else if (h4 < 0)
                            {
                                h4 = 0;
                            }
                            Color c = Color.FromArgb(255, (int)(h1 * 255), (int)(h2 * 255), (int)(h3 * 255));
                            res.SetPixel(x, y, c);
                        }
                    }
                }
            }
            return res;
        }

        /// <summary>
        /// String builder for material folder
        /// </summary>
        /// <param name="slotDict"></param>
        /// <param name="parentNode"></param>
        /// <param name="itemID"></param>
        /// <param name="imcVariant"></param>
        /// <param name="bodyVer"></param>
        /// <returns></returns>
        public string[] getMaterialFolder(Dictionary<string, int> slotDict, string parentNode, string itemID, string imcVariant, string bodyVer)
        {
            string imcVersion, materialFolder;
            string[] imc_folder = new string[2];

            if (parentNode.Equals(Resources.strings.Ears) || parentNode.Equals(Resources.strings.Neck) || parentNode.Equals(Resources.strings.Wrists) || parentNode.Equals(Resources.strings.Rings))
            {
                imcVersion = getVarientFromImc(slotDict, parentNode, itemID, imcVariant, "a", "").PadLeft(4, '0');
                materialFolder = "chara/accessory/a" + itemID + "/material/v" + imcVersion;
            }
            else if (parentNode.Equals(Resources.strings.Main_Hand) || parentNode.Equals(Resources.strings.Off_Hand) || parentNode.Equals(Resources.strings.Two_Handed))
            {
                imcVersion = getVarientFromImc(slotDict, parentNode, itemID, imcVariant, "w", bodyVer).PadLeft(4, '0');
                materialFolder = "chara/weapon/w" + itemID + "/obj/body/b" + bodyVer + "/material/v" + imcVersion;
            }
            else
            {
                imcVersion = getVarientFromImc(slotDict, parentNode, itemID, imcVariant, "e", "").PadLeft(4, '0');
                materialFolder = "chara/equipment/e" + itemID + "/material/v" + imcVersion;
            }

            imc_folder[0] = imcVersion;
            imc_folder[1] = materialFolder;

            return imc_folder;
        }

        /// <summary>
        /// Obtains the Variant from the IMC file
        /// </summary>
        /// <param name="slotDict"></param>
        /// <param name="parentNode"></param>
        /// <param name="itemID"></param>
        /// <param name="imcVariant"></param>
        /// <param name="type"></param>
        /// <param name="bodyVer"></param>
        /// <returns></returns>
        public string getVarientFromImc(Dictionary<string, int> slotDict, string parentNode, string itemID, string imcVariant, string type, string bodyVer)
        {
            FFCRC crc = new FFCRC();
            int[] imcHex;

            if (type.Equals("a"))
            {
                imcHex = getMTRLoffset(crc.text("chara/accessory/" + type + itemID), crc.text(type + itemID + ".imc"));
            }
            else if (type.Equals("w"))
            {
                imcHex = getMTRLoffset(crc.text("chara/weapon/" + type + itemID + "/obj/body/b" + bodyVer), crc.text("b" + bodyVer + ".imc"));
            }
            else
            {
                imcHex = getMTRLoffset(crc.text("chara/equipment/" + type + itemID), crc.text(type + itemID + ".imc"));
            }

            if(imcHex[0] != 0)
            {
                int datNum = imcHex[1];

                FindVariant findVariant = new FindVariant(datNum, imcHex[0], slotDict[parentNode], type, imcVariant);

                return findVariant.getVariant();
            }
            else
            {
                return "1";
            }

        }


        /// <summary>
        /// Gets Offset of MTRL file
        /// </summary>
        /// <param name="folderHex"></param>
        /// <param name="fileHex"></param>
        /// <returns></returns>
        public int[] getMTRLoffset(string folderHex, string fileHex)
        {
            int[] locHex = new int[2];

            FindOffset findOffset = new FindOffset(fileHex, folderHex);
            locHex[0] = int.Parse(findOffset.getFileOffset(), NumberStyles.HexNumber);
            locHex[1] = ((locHex[0] / 8) & 0x000f) / 2;

            return locHex;
        }
    }
}
