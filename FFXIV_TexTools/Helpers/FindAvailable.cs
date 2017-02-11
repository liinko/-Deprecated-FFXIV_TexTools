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
using FFXIV_TexTools.IO;

namespace FFXIV_TexTools.Helpers
{
    public class FindAvailable
    {
        Dictionary<string, string> raceDict, raceIDDict, summonDict;
        Dictionary<string, List<string[]>> mtrlDict;
        Dictionary<string, Items> mountsDict;
        string itemID, parentNode, childNode, bodyVer, folderHex, summonFolder, mountFolder, mountMtrl, minionFolder, minionMTRL, textureFolder;
        bool hasSecondary;
        string[] secondary;

        FFCRC crc = new FFCRC();
        Helper helper = new Helper();

        /// <summary>
        /// Used to find the available textures
        /// </summary>
        public FindAvailable(string type, string parentNode, Dictionary<string, string> raceDict, string itemID, string bodyVer, string folderHex, bool hasSecondary, string[] secondary)
        {
            this.raceDict = raceDict;
            this.itemID = itemID;
            this.parentNode = parentNode;
            this.bodyVer = bodyVer;
            this.folderHex = folderHex;
            this.hasSecondary = hasSecondary;
            this.secondary = secondary;

            equipmentMtrlMaker();
        }

        /// <summary>
        /// Finds what races the texture is avaialble for and creates the MTRL directory string
        /// </summary>
        /// <param name="type">string that determines which method to run</param>
        /// <param name="raceIDDict">Dictionary for available races</param>
        public FindAvailable(string type, Dictionary<string,string> raceIDDict)
        {
            this.raceIDDict = raceIDDict;

            if(type.Equals(Resources.strings.Body))
            {
                bodyMtrlMaker();
            }
            else if(type.Equals(Resources.strings.Face))
            {
                faceMtrlMaker();
            }
            else if (type.Equals(Resources.strings.Hair))
            {
                raceDict = raceIDDict;
                hairMtrlMaker();
            }
            else if (type.Equals(Resources.strings.Tail))
            {
                tailsMtrlMaker();
            }
        }

        /// <summary>
        /// Find avaialble textures for summons and creates the MTRL directory string
        /// </summary>
        /// <param name="type"></param>
        /// <param name="summonDict"></param>
        /// <param name="childNode"></param>
        public FindAvailable(string type, Dictionary<string,string> summonDict, string childNode)
        {
            this.summonDict = summonDict;
            this.childNode = childNode;

            if (type.Equals(Resources.strings.Pets))
            {
                summonsMtrlMaker();
            }
        }

        /// <summary>
        /// Find avaialable textures for mounts and minions and creates MTRL directory string
        /// </summary>
        /// <param name="type"></param>
        /// <param name="mountsDict"></param>
        /// <param name="childNode"></param>
        public FindAvailable(string type, Dictionary<string, Items> mountsDict, string childNode)
        {
            this.mountsDict = mountsDict;
            this.childNode = childNode;

            if (type.Equals(Resources.strings.Mounts))
            {
                mountssMtrlMaker();
            }
            else if (type.Equals(Resources.strings.Minions))
            {
                minionMtrlMaker();
            }


        }

        private void minionMtrlMaker()
        {
            int[] locHex = { 0, 0 };

            string minionFolderHex = "";
            string minionFileHex = "";

            List<string> minioncrc = new List<string>();
            List<string> minionnum = new List<string>();
            List<string[]> minionList = new List<string[]>();

            mtrlDict = new Dictionary<string, List<string[]>>();

            Items minion = mountsDict[childNode];

            string materialFolder, minionMTRLFile, minionMTRLFile1 = "";

            materialFolder = "chara/monster/m" + minion.itemID.PadLeft(4, '0') + "/obj/body/b000" + minion.weaponBody + "/material/v000" + minion.imcVersion;
            textureFolder = "chara/monster/m" + minion.itemID.PadLeft(4, '0') + "/obj/body/b000" + minion.weaponBody + "/texture";
            minionMTRLFile = "mt_m" + minion.itemID.PadLeft(4, '0') + "b000" + minion.weaponBody + "_a.mtrl";
            minionMTRLFile1 = "mt_m" + minion.itemID.PadLeft(4, '0') + "b000" + minion.weaponBody + "_b.mtrl";

            minionFolder = materialFolder;

            minionFolderHex = crc.text(materialFolder);
            minionFileHex = crc.text(minionMTRLFile);

            locHex = helper.getMTRLoffset(minionFolderHex, minionFileHex);

            minioncrc.Add(locHex[0].ToString());
            minionnum.Add("a");

            if (!minionMTRLFile1.Equals(""))
            {
                minionFileHex = crc.text(minionMTRLFile1);
                locHex = helper.getMTRLoffset(minionFolderHex, minionFileHex);

                if (locHex[0] != 0)
                {
                    minioncrc.Add(locHex[0].ToString());
                    minionnum.Add("b");
                }
            }

            minionMTRL = minionMTRLFile;

            minionList.Add(minionnum.ToArray());
            minionList.Add(minioncrc.ToArray());
            mtrlDict.Add(childNode, minionList);
        }

        private void mountssMtrlMaker()
        {
            int[] locHex = { 0, 0 };

            string mountFolderHex = "";
            string mountFileHex = "";

            List<string> mountcrc = new List<string>();
            List<string> mountnum = new List<string>();
            List<string[]> mountList = new List<string[]>();

            mtrlDict = new Dictionary<string, List<string[]>>();

            Items mount = mountsDict[childNode];

            string materialFolder, mountMTRLFile, mountMTRLFile1 = "", demiMTRLFile = "", demiMTRLFile1 = "", demiMTRLFile2 = "";
            if (childNode.ToLower().Contains(Resources.strings.Warsteed) || childNode.Contains(Resources.strings.Company_Chocobo) || childNode.Contains(Resources.strings.Legacy_Chocobo) || 
                childNode.Contains(Resources.strings.Draught_Chocobo) || childNode.Contains(Resources.strings.Black_Chocobo) || childNode.Contains(Resources.strings.Ceremony_Chocobo))
            {
                materialFolder = "chara/demihuman/d" + mount.itemID.PadLeft(4, '0') + "/obj/equipment/e000" + mount.weaponBody + "/material/v000" + mount.imcVersion;
                textureFolder = "chara/demihuman/d" + mount.itemID.PadLeft(4, '0') + "/obj/equipment/e000" + mount.weaponBody + "/texture";
                mountMTRLFile = "mt_d" + mount.itemID.PadLeft(4, '0') + "e000" + mount.weaponBody + "_dwn_a.mtrl";
                demiMTRLFile = "mt_d" + mount.itemID.PadLeft(4, '0') + "e000" + mount.weaponBody + "_met_a.mtrl";
                demiMTRLFile1 = "mt_d" + mount.itemID.PadLeft(4, '0') + "e000" + mount.weaponBody + "_top_a.mtrl";
                demiMTRLFile2 = "mt_d" + mount.itemID.PadLeft(4, '0') + "e000" + mount.weaponBody + "_sho_a.mtrl";
            }
            else if (childNode.Contains(Resources.strings.Magitek) || childNode.Contains(Resources.strings.White_Devil) || childNode.Contains(Resources.strings.Red_Baron))
            {
                materialFolder = "chara/demihuman/d" + mount.itemID.PadLeft(4, '0') + "/obj/equipment/e000" + mount.weaponBody + "/material/v000" + mount.imcVersion;
                textureFolder = "chara/demihuman/d" + mount.itemID.PadLeft(4, '0') + "/obj/equipment/e000" + mount.weaponBody + "/texture";
                mountMTRLFile = "mt_d" + mount.itemID.PadLeft(4, '0') + "e000" + mount.weaponBody + "_top_a.mtrl";
            }
            else
            {
                if (childNode.Contains(Resources.strings.Twintania))
                {
                    mount.imcVersion = "1";
                }
                materialFolder = "chara/monster/m" + mount.itemID.PadLeft(4, '0') + "/obj/body/b000" + mount.weaponBody + "/material/v000" + mount.imcVersion;
                textureFolder = "chara/monster/m" + mount.itemID.PadLeft(4, '0') + "/obj/body/b000" + mount.weaponBody + "/texture";
                mountMTRLFile = "mt_m" + mount.itemID.PadLeft(4, '0') + "b000" + mount.weaponBody + "_a.mtrl";
                mountMTRLFile1 = "mt_m" + mount.itemID.PadLeft(4, '0') + "b000" + mount.weaponBody + "_b.mtrl";
            }

            mountFolder = materialFolder;

            mountFolderHex = crc.text(materialFolder);
            mountFileHex = crc.text(mountMTRLFile);

            locHex = helper.getMTRLoffset(mountFolderHex, mountFileHex);

            if (demiMTRLFile.Equals(""))
            {
                mountcrc.Add(locHex[0].ToString());

                if (materialFolder.Contains("d0002"))
                {
                    mountnum.Add("top");
                }
                else
                {
                    mountnum.Add("a");
                }


                if (!mountMTRLFile1.Equals(""))
                {
                    mountFileHex = crc.text(mountMTRLFile1);
                    locHex = helper.getMTRLoffset(mountFolderHex, mountFileHex);

                    if (locHex[0] != 0)
                    {
                        mountcrc.Add(locHex[0].ToString());
                        mountnum.Add("b");
                    }
                }
            }
            else
            {
                mountcrc.Add(locHex[0].ToString());
                mountnum.Add("dwn");

                mountFileHex = crc.text(demiMTRLFile);
                locHex = helper.getMTRLoffset(mountFolderHex, mountFileHex);

                if (locHex[0] != 0)
                {
                    mountcrc.Add(locHex[0].ToString());
                    mountnum.Add("met");
                }

                mountFileHex = crc.text(demiMTRLFile1);
                locHex = helper.getMTRLoffset(mountFolderHex, mountFileHex);

                if (locHex[0] != 0)
                {
                    mountcrc.Add(locHex[0].ToString());
                    mountnum.Add("top");
                }

                mountFileHex = crc.text(demiMTRLFile2);
                locHex = helper.getMTRLoffset(mountFolderHex, mountFileHex);

                if (locHex[0] != 0)
                {
                    mountcrc.Add(locHex[0].ToString());
                    mountnum.Add("sho");
                }
            }

            mountMtrl = mountMTRLFile;

            mountList.Add(mountnum.ToArray());
            mountList.Add(mountcrc.ToArray());
            mtrlDict.Add(childNode, mountList);

        }

        public void equipmentMtrlMaker()
        {
            string mtrlFileName, mtrlFileName_b, mtrlFileName_c;
            int i = 0;
            string[] mtrlHexA = new string[15];
            string[] mtrlHexB = new string[15];
            string[] mtrlHexC = new string[15];

            foreach (var c in raceDict)
            {
                mtrlFileName = "mt_c" + c.Value + "e" + itemID;

                if (parentNode.Equals(Resources.strings.Body))
                {
                    mtrlFileName = mtrlFileName + "_top_a.mtrl";
                }
                else if (parentNode.Equals(Resources.strings.Legs))
                {
                    mtrlFileName = mtrlFileName + "_dwn_a.mtrl";
                }
                else if (parentNode.Equals(Resources.strings.Head))
                {
                    mtrlFileName = mtrlFileName + "_met_a.mtrl";
                }
                else if (parentNode.Equals(Resources.strings.Hands))
                {
                    mtrlFileName = mtrlFileName + "_glv_a.mtrl";
                }
                else if (parentNode.Equals(Resources.strings.Feet))
                {
                    mtrlFileName = mtrlFileName + "_sho_a.mtrl";
                }
                else if (parentNode.Equals(Resources.strings.Head_Body))
                {
                    mtrlFileName_c = mtrlFileName + "_top_c.mtrl";
                    mtrlFileName_b = mtrlFileName + "_top_b.mtrl";
                    mtrlFileName = mtrlFileName + "_top_a.mtrl";

                    mtrlHexB[i] = crc.text(mtrlFileName_b);
                    mtrlHexC[i] = crc.text(mtrlFileName_c);
                }
                else if (parentNode.Equals(Resources.strings.Body_Hands_Legs_Feet))
                {
                    mtrlFileName_b = mtrlFileName + "_top_b.mtrl";
                    mtrlFileName = mtrlFileName + "_top_a.mtrl";

                    mtrlHexB[i] = crc.text(mtrlFileName_b);
                }
                else if (parentNode.Equals(Resources.strings.Legs_Feet))
                {
                    mtrlFileName_b = mtrlFileName + "_top_b.mtrl";
                    mtrlFileName = mtrlFileName + "_top_a.mtrl";

                    mtrlHexB[i] = crc.text(mtrlFileName_b);
                }
                else if (parentNode.Equals(Resources.strings.All) || parentNode.Equals(Resources.strings.Body_Hands_Legs) || parentNode.Equals(Resources.strings.Body_Legs_Feet))
                {
                    mtrlFileName = mtrlFileName + "_top_a.mtrl";
                }
                else if (parentNode.Equals(Resources.strings.Ears))
                {
                    mtrlFileName = "mt_c" + c.Value + "a" + itemID;
                    mtrlFileName = mtrlFileName + "_ear_a.mtrl";
                }
                else if (parentNode.Equals(Resources.strings.Neck))
                {
                    mtrlFileName = "mt_c" + c.Value + "a" + itemID;
                    mtrlFileName = mtrlFileName + "_nek_a.mtrl";
                }
                else if (parentNode.Equals(Resources.strings.Wrists))
                {
                    mtrlFileName = "mt_c" + c.Value + "a" + itemID;
                    mtrlFileName = mtrlFileName + "_wrs_a.mtrl";
                }
                else if (parentNode.Equals(Resources.strings.Rings))
                {
                    mtrlFileName = "mt_c" + c.Value + "a" + itemID;
                    mtrlFileName = mtrlFileName + "_rir_a.mtrl";
                    mtrlFileName_b = mtrlFileName + "_ril_a.mtrl";

                    mtrlHexB[i] = crc.text(mtrlFileName_b);
                }
                else if (parentNode.Equals(Resources.strings.Main_Hand) || parentNode.Equals(Resources.strings.Off_Hand) || parentNode.Equals(Resources.strings.Two_Handed))
                {
                    mtrlFileName = "mt_w" + itemID + "b" + bodyVer + "_a.mtrl";

                    if (hasSecondary)
                    {
                        mtrlFileName_b = "mt_w" + secondary[0] + "b" + secondary[2] + "_a.mtrl";
                        mtrlHexB[i] = crc.text(mtrlFileName_b);
                    }
                }

                mtrlHexA[i] = crc.text(mtrlFileName);
                i++;
            }
            getAvailRaces(mtrlHexA, mtrlHexB, mtrlHexC);
        }

        public void getAvailRaces(string[] mtrlHexA, string[] mtrlHexB, string[] mtrlHexC)
        {
            mtrlDict = new Dictionary<string, List<string[]>>();

            int[] locHex = { 0, 0 };
            int[] locHex1 = { 0, 0 };
            int[] locHex2 = { 0, 0 };

            for (int k = 0; k < 15; k++)
            {
                List<string> mtrls = new List<string>();
                List<string> part = new List<string>();
                List<string[]> mtrlParts = new List<string[]>();

                if (parentNode.Equals(Resources.strings.Main_Hand) || parentNode.Equals(Resources.strings.Off_Hand) || parentNode.Equals(Resources.strings.Two_Handed))
                {
                    locHex = helper.getMTRLoffset(folderHex, mtrlHexA[k]);
                    mtrls.Add(locHex[0].ToString());
                    part.Add("a");
                    if (!hasSecondary)
                    {
                        mtrlParts.Add(part.ToArray());
                        mtrlParts.Add(mtrls.ToArray());
                        mtrlDict.Add(raceDict.Keys.ElementAt(14), mtrlParts);
                    }
                    else
                    {
                        locHex = helper.getMTRLoffset(secondary[1], mtrlHexB[k]);
                        mtrls.Add(locHex[0].ToString());
                        part.Add("b");
                        mtrlParts.Add(part.ToArray());
                        mtrlParts.Add(mtrls.ToArray());
                        mtrlDict.Add(raceDict.Keys.ElementAt(14), mtrlParts);
                    }
                    break;
                }

                locHex = helper.getMTRLoffset(folderHex, mtrlHexA[k]);

                if (locHex[0] != 0)
                {
                    mtrls.Add(locHex[0].ToString());
                    part.Add("a");

                    if (!parentNode.Equals(Resources.strings.Head_Body) && !parentNode.Equals(Resources.strings.Body_Hands_Legs_Feet) && !parentNode.Equals(Resources.strings.Legs_Feet) && !parentNode.Equals(Resources.strings.Rings))
                    {
                        mtrlParts.Add(part.ToArray());
                        mtrlParts.Add(mtrls.ToArray());
                        mtrlDict.Add(raceDict.Keys.ElementAt(k), mtrlParts);
                    }

                    if (parentNode.Equals(Resources.strings.Head_Body) || parentNode.Equals(Resources.strings.Body_Hands_Legs_Feet) || parentNode.Equals(Resources.strings.Legs_Feet) || parentNode.Equals(Resources.strings.Rings))
                    {
                        locHex1 = helper.getMTRLoffset(folderHex, mtrlHexB[k]);
                        locHex2 = helper.getMTRLoffset(folderHex, mtrlHexC[k]);
                        if (locHex1[0] != 0)
                        {
                            part.Add("b");
                            mtrls.Add(locHex1[0].ToString());
                        }

                        if (locHex2[0] != 0)
                        {
                            part.Add("c");
                            mtrls.Add(locHex2[0].ToString());
                        }

                        mtrlParts.Add(part.ToArray());
                        mtrlParts.Add(mtrls.ToArray());
                        mtrlDict.Add(raceDict.Keys.ElementAt(k), mtrlParts);
                    }
                }
            }
        }

        public void bodyMtrlMaker()
        {
            int[] locHex = { 0, 0 };

            mtrlDict = new Dictionary<string, List<string[]>>();

            string[] bodyCNum = { "0101", "0104", "0201", "0301", "0401", "0804", "0901", "1101", "1301", "1401" };

            foreach (string s in bodyCNum)
            {
                List<string> bodycrc = new List<string>();
                List<string> bodynum = new List<string>();
                List<string[]> bodList = new List<string[]>();

                string bodyMtrlFile = "mt_c" + s + "b0001_a.mtrl";
                locHex = helper.getMTRLoffset("none", crc.text(bodyMtrlFile));
                bodycrc.Add(locHex[0].ToString());
                bodynum.Add("1");

                if (s.Equals("0101") || s.Equals("0201"))
                {
                    bodyMtrlFile = "mt_c" + s + "b0091_a.mtrl";
                    locHex = helper.getMTRLoffset("none", crc.text(bodyMtrlFile));
                    bodycrc.Add(locHex[0].ToString());
                    bodynum.Add("91");

                }

                if (s.Equals("1301") || s.Equals("1401"))
                {
                    bodyMtrlFile = "mt_c" + s + "b0101_a.mtrl";
                    locHex = helper.getMTRLoffset("none", crc.text(bodyMtrlFile));
                    bodycrc.Add(locHex[0].ToString());
                    bodynum.Add("101");

                }

                if (s.Equals("0101"))
                {
                    bodyMtrlFile = "mt_c" + s + "b0250_a.mtrl";
                    locHex = helper.getMTRLoffset("none", crc.text(bodyMtrlFile));
                    bodycrc.Add(locHex[0].ToString());
                    bodynum.Add("250");
                }

                bodList.Add(bodynum.ToArray());
                bodList.Add(bodycrc.ToArray());
                mtrlDict.Add(raceIDDict[s], bodList);
            }
        }

        public void faceMtrlMaker()
        {
            int[] locHex = { 0, 0 };

            mtrlDict = new Dictionary<string, List<string[]>>();

            foreach (string s in raceIDDict.Keys)
            {
                List<string> facecrc = new List<string>();
                List<string> facenum = new List<string>();
                List<string[]> faceList = new List<string[]>();

                List<string> iriscrc = new List<string>();
                List<string> irisnum = new List<string>();

                List<string> etccrc = new List<string>();
                List<string> etcnum = new List<string>();

                string faceMtrlFile = "mt_c" + s + "f";

                switch (s)
                {
                    case "0101":
                        for (int i = 1; i < 7; i++)
                        {
                            locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + i.ToString().PadLeft(4, '0') + "_fac_a.mtrl"));
                            facecrc.Add(locHex[0].ToString());
                            facenum.Add(i.ToString());

                            locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + i.ToString().PadLeft(4, '0') + "_iri_a.mtrl"));
                            iriscrc.Add(locHex[0].ToString());
                            irisnum.Add(i.ToString());

                            locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + i.ToString().PadLeft(4, '0') + "_etc_a.mtrl"));
                            etccrc.Add(locHex[0].ToString());
                            etcnum.Add(i.ToString());

                        }

                        locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "0091_fac_a.mtrl"));
                        facecrc.Add(locHex[0].ToString());
                        facenum.Add("91");

                        locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "0091_iri_a.mtrl"));
                        iriscrc.Add(locHex[0].ToString());
                        irisnum.Add("91");

                        locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "0091_etc_a.mtrl"));
                        etccrc.Add(locHex[0].ToString());
                        etcnum.Add("91");


                        for (int i = 0; i < 11; i++)
                        {
                            locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "02" + i.ToString().PadLeft(2, '0') + "_fac_a.mtrl"));
                            facecrc.Add(locHex[0].ToString());
                            facenum.Add((200 + i).ToString());

                            locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "02" + i.ToString().PadLeft(2, '0') + "_iri_a.mtrl"));
                            iriscrc.Add(locHex[0].ToString());
                            irisnum.Add((200 + i).ToString());

                            locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "02" + i.ToString().PadLeft(2, '0') + "_etc_a.mtrl"));
                            etccrc.Add(locHex[0].ToString());
                            etcnum.Add((200 + i).ToString());
                        }

                        locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "0249_fac_a.mtrl"));
                        facecrc.Add(locHex[0].ToString());
                        facenum.Add("249");

                        locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "0249_iri_a.mtrl"));
                        iriscrc.Add(locHex[0].ToString());
                        irisnum.Add("249");

                        locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "0249_etc_a.mtrl"));
                        etccrc.Add(locHex[0].ToString());
                        etcnum.Add("249");


                        locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "0250_fac_a.mtrl"));
                        facecrc.Add(locHex[0].ToString());
                        facenum.Add("250");

                        locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "0250_iri_a.mtrl"));
                        iriscrc.Add(locHex[0].ToString());
                        irisnum.Add("250");

                        locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "0250_etc_a.mtrl"));
                        etccrc.Add(locHex[0].ToString());
                        etcnum.Add("250");
                        break;

                    case "0201":
                        for (int i = 1; i < 6; i++)
                        {
                            locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + i.ToString().PadLeft(4, '0') + "_fac_a.mtrl"));
                            facecrc.Add(locHex[0].ToString());
                            facenum.Add(i.ToString());

                            locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + i.ToString().PadLeft(4, '0') + "_iri_a.mtrl"));
                            iriscrc.Add(locHex[0].ToString());
                            irisnum.Add(i.ToString());

                            locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + i.ToString().PadLeft(4, '0') + "_etc_a.mtrl"));
                            etccrc.Add(locHex[0].ToString());
                            etcnum.Add(i.ToString());
                        }

                        locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "0091_fac_a.mtrl"));
                        facecrc.Add(locHex[0].ToString());
                        facenum.Add("91");

                        locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "0091_iri_a.mtrl"));
                        iriscrc.Add(locHex[0].ToString());
                        irisnum.Add("91");

                        locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "0091_etc_a.mtrl"));
                        etccrc.Add(locHex[0].ToString());
                        etcnum.Add("91");

                        for (int i = 1; i < 11; i++)
                        {
                            locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "02" + i.ToString().PadLeft(2, '0') + "_fac_a.mtrl"));
                            facecrc.Add(locHex[0].ToString());
                            facenum.Add((200 + i).ToString());

                            locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "02" + i.ToString().PadLeft(2, '0') + "_iri_a.mtrl"));
                            iriscrc.Add(locHex[0].ToString());
                            irisnum.Add((200 + i).ToString());

                            locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "02" + i.ToString().PadLeft(2, '0') + "_etc_a.mtrl"));
                            etccrc.Add(locHex[0].ToString());
                            etcnum.Add((200 + i).ToString());
                        }
                        break;

                    case "0301":
                        for (int i = 1; i < 5; i++)
                        {
                            locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "01" + i.ToString().PadLeft(2, '0') + "_fac_a.mtrl"));
                            facecrc.Add(locHex[0].ToString());
                            facenum.Add((100 + i).ToString());

                            locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "01" + i.ToString().PadLeft(2, '0') + "_iri_a.mtrl"));
                            iriscrc.Add(locHex[0].ToString());
                            irisnum.Add((100 + i).ToString());

                            locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "01" + i.ToString().PadLeft(2, '0') + "_etc_a.mtrl"));
                            etccrc.Add(locHex[0].ToString());
                            etcnum.Add((100 + i).ToString());
                        }

                        locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "0201_fac_a.mtrl"));
                        facecrc.Add(locHex[0].ToString());
                        facenum.Add("201");

                        locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "0201_iri_a.mtrl"));
                        iriscrc.Add(locHex[0].ToString());
                        irisnum.Add("201");

                        locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "0201_etc_a.mtrl"));
                        etccrc.Add(locHex[0].ToString());
                        etcnum.Add("201");
                        break;

                    case "0401":
                        for (int i = 1; i < 5; i++)
                        {
                            locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "01" + i.ToString().PadLeft(2, '0') + "_fac_a.mtrl"));
                            facecrc.Add(locHex[0].ToString());
                            facenum.Add((100 + i).ToString());

                            locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "01" + i.ToString().PadLeft(2, '0') + "_iri_a.mtrl"));
                            iriscrc.Add(locHex[0].ToString());
                            irisnum.Add((100 + i).ToString());

                            locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "01" + i.ToString().PadLeft(2, '0') + "_etc_a.mtrl"));
                            etccrc.Add(locHex[0].ToString());
                            etcnum.Add((100 + i).ToString());
                        }
                        break;

                    case "0501":
                        for (int i = 1; i < 5; i++)
                        {
                            locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + i.ToString().PadLeft(4, '0') + "_fac_a.mtrl"));
                            facecrc.Add(locHex[0].ToString());
                            facenum.Add(i.ToString());

                            locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + i.ToString().PadLeft(4, '0') + "_iri_a.mtrl"));
                            iriscrc.Add(locHex[0].ToString());
                            irisnum.Add(i.ToString());

                            locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + i.ToString().PadLeft(4, '0') + "_etc_a.mtrl"));
                            etccrc.Add(locHex[0].ToString());
                            etcnum.Add(i.ToString());
                        }
                        for (int i = 1; i < 5; i++)
                        {
                            locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "01" + i.ToString().PadLeft(2, '0') + "_fac_a.mtrl"));
                            facecrc.Add(locHex[0].ToString());
                            facenum.Add((100 + i).ToString());

                            locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "01" + i.ToString().PadLeft(2, '0') + "_iri_a.mtrl"));
                            iriscrc.Add(locHex[0].ToString());
                            irisnum.Add((100 + i).ToString());

                            locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "01" + i.ToString().PadLeft(2, '0') + "_etc_a.mtrl"));
                            etccrc.Add(locHex[0].ToString());
                            etcnum.Add((100 + i).ToString());
                        }
                        for (int i = 1; i < 9; i++)
                        {
                            locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "02" + i.ToString().PadLeft(2, '0') + "_fac_a.mtrl"));
                            facecrc.Add(locHex[0].ToString());
                            facenum.Add((200 + i).ToString());

                            locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "02" + i.ToString().PadLeft(2, '0') + "_iri_a.mtrl"));
                            iriscrc.Add(locHex[0].ToString());
                            irisnum.Add((200 + i).ToString());

                            locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "02" + i.ToString().PadLeft(2, '0') + "_etc_a.mtrl"));
                            etccrc.Add(locHex[0].ToString());
                            etcnum.Add((200 + i).ToString());
                        }
                        break;

                    case "0601":
                        for (int i = 1; i < 5; i++)
                        {
                            locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + i.ToString().PadLeft(4, '0') + "_fac_a.mtrl"));
                            facecrc.Add(locHex[0].ToString());
                            facenum.Add(i.ToString());

                            locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + i.ToString().PadLeft(4, '0') + "_iri_a.mtrl"));
                            iriscrc.Add(locHex[0].ToString());
                            irisnum.Add(i.ToString());

                            locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + i.ToString().PadLeft(4, '0') + "_etc_a.mtrl"));
                            etccrc.Add(locHex[0].ToString());
                            etcnum.Add(i.ToString());
                        }
                        for (int i = 1; i < 5; i++)
                        {
                            locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "01" + i.ToString().PadLeft(2, '0') + "_fac_a.mtrl"));
                            facecrc.Add(locHex[0].ToString());
                            facenum.Add((100 + i).ToString());

                            locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "01" + i.ToString().PadLeft(2, '0') + "_iri_a.mtrl"));
                            iriscrc.Add(locHex[0].ToString());
                            irisnum.Add((100 + i).ToString());

                            locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "01" + i.ToString().PadLeft(2, '0') + "_etc_a.mtrl"));
                            etccrc.Add(locHex[0].ToString());
                            etcnum.Add((100 + i).ToString());
                        }
                        for (int i = 0; i < 3; i++)
                        {
                            locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "02" + i.ToString().PadLeft(2, '0') + "_fac_a.mtrl"));
                            facecrc.Add(locHex[0].ToString());
                            facenum.Add((200 + i).ToString());

                            locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "02" + i.ToString().PadLeft(2, '0') + "_iri_a.mtrl"));
                            iriscrc.Add(locHex[0].ToString());
                            irisnum.Add((200 + i).ToString());

                            locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "02" + i.ToString().PadLeft(2, '0') + "_etc_a.mtrl"));
                            etccrc.Add(locHex[0].ToString());
                            etcnum.Add((200 + i).ToString());
                        }
                        break;

                    case "0701":
                        for (int i = 1; i < 5; i++)
                        {
                            locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + i.ToString().PadLeft(4, '0') + "_fac_a.mtrl"));
                            facecrc.Add(locHex[0].ToString());
                            facenum.Add(i.ToString());

                            locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + i.ToString().PadLeft(4, '0') + "_iri_a.mtrl"));
                            iriscrc.Add(locHex[0].ToString());
                            irisnum.Add(i.ToString());

                            locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + i.ToString().PadLeft(4, '0') + "_etc_a.mtrl"));
                            etccrc.Add(locHex[0].ToString());
                            etcnum.Add(i.ToString());
                        }
                        for (int i = 1; i < 5; i++)
                        {
                            locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "01" + i.ToString().PadLeft(2, '0') + "_fac_a.mtrl"));
                            facecrc.Add(locHex[0].ToString());
                            facenum.Add((100 + i).ToString());

                            locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "01" + i.ToString().PadLeft(2, '0') + "_iri_a.mtrl"));
                            iriscrc.Add(locHex[0].ToString());
                            irisnum.Add((100 + i).ToString());

                            locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "01" + i.ToString().PadLeft(2, '0') + "_etc_a.mtrl"));
                            etccrc.Add(locHex[0].ToString());
                            etcnum.Add((100 + i).ToString());
                        }

                        locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "0201_fac_a.mtrl"));
                        facecrc.Add(locHex[0].ToString());
                        facenum.Add("201");

                        locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "0201_iri_a.mtrl"));
                        iriscrc.Add(locHex[0].ToString());
                        etcnum.Add("201");

                        locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "0201_etc_a.mtrl"));
                        iriscrc.Add(locHex[0].ToString());
                        etcnum.Add("201");
                        break;

                    case "0801":
                        for (int i = 1; i < 5; i++)
                        {
                            locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + i.ToString().PadLeft(4, '0') + "_fac_a.mtrl"));
                            facecrc.Add(locHex[0].ToString());
                            facenum.Add(i.ToString());

                            locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + i.ToString().PadLeft(4, '0') + "_iri_a.mtrl"));
                            iriscrc.Add(locHex[0].ToString());
                            irisnum.Add(i.ToString());

                            locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + i.ToString().PadLeft(4, '0') + "_etc_a.mtrl"));
                            etccrc.Add(locHex[0].ToString());
                            etcnum.Add(i.ToString());
                        }
                        for (int i = 1; i < 5; i++)
                        {
                            locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "01" + i.ToString().PadLeft(2, '0') + "_fac_a.mtrl"));
                            facecrc.Add(locHex[0].ToString());
                            facenum.Add((100 + i).ToString());

                            locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "01" + i.ToString().PadLeft(2, '0') + "_iri_a.mtrl"));
                            iriscrc.Add(locHex[0].ToString());
                            irisnum.Add((100 + i).ToString());

                            locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "01" + i.ToString().PadLeft(2, '0') + "_etc_a.mtrl"));
                            etccrc.Add(locHex[0].ToString());
                            etcnum.Add((100 + i).ToString());

                        }
                        for (int i = 1; i < 5; i++)
                        {
                            locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "02" + i.ToString().PadLeft(2, '0') + "_fac_a.mtrl"));
                            facecrc.Add(locHex[0].ToString());
                            facenum.Add((200 + i).ToString());

                            locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "02" + i.ToString().PadLeft(2, '0') + "_iri_a.mtrl"));
                            iriscrc.Add(locHex[0].ToString());
                            irisnum.Add((200 + i).ToString());

                            locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "02" + i.ToString().PadLeft(2, '0') + "_etc_a.mtrl"));
                            etccrc.Add(locHex[0].ToString());
                            etcnum.Add((200 + i).ToString());
                        }
                        break;
                    case "0901":
                    case "1001":
                    case "1101":
                    case "1201":
                    case "1401":
                        for (int i = 1; i < 5; i++)
                        {
                            locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + i.ToString().PadLeft(4, '0') + "_fac_a.mtrl"));
                            facecrc.Add(locHex[0].ToString());
                            facenum.Add(i.ToString());

                            locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + i.ToString().PadLeft(4, '0') + "_iri_a.mtrl"));
                            iriscrc.Add(locHex[0].ToString());
                            irisnum.Add(i.ToString());

                            locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + i.ToString().PadLeft(4, '0') + "_etc_a.mtrl"));
                            etccrc.Add(locHex[0].ToString());
                            etcnum.Add(i.ToString());
                        }
                        for (int i = 1; i < 5; i++)
                        {
                            locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "01" + i.ToString().PadLeft(2, '0') + "_fac_a.mtrl"));
                            facecrc.Add(locHex[0].ToString());
                            facenum.Add((100 + i).ToString());

                            locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "01" + i.ToString().PadLeft(2, '0') + "_iri_a.mtrl"));
                            iriscrc.Add(locHex[0].ToString());
                            irisnum.Add((100 + i).ToString());

                            locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "01" + i.ToString().PadLeft(2, '0') + "_etc_a.mtrl"));
                            etccrc.Add(locHex[0].ToString());
                            etcnum.Add((100 + i).ToString());
                        }
                        for (int i = 1; i < 3; i++)
                        {
                            locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "02" + i.ToString().PadLeft(2, '0') + "_fac_a.mtrl"));
                            facecrc.Add(locHex[0].ToString());
                            facenum.Add((200 + i).ToString());

                            locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "02" + i.ToString().PadLeft(2, '0') + "_iri_a.mtrl"));
                            iriscrc.Add(locHex[0].ToString());
                            irisnum.Add((200 + i).ToString());

                            locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "02" + i.ToString().PadLeft(2, '0') + "_etc_a.mtrl"));
                            etccrc.Add(locHex[0].ToString());
                            etcnum.Add((200 + i).ToString());
                        }
                        break;
                    case "1301":
                        for (int i = 1; i < 5; i++)
                        {
                            locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + i.ToString().PadLeft(4, '0') + "_fac_a.mtrl"));
                            facecrc.Add(locHex[0].ToString());
                            facenum.Add(i.ToString());

                            locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + i.ToString().PadLeft(4, '0') + "_iri_a.mtrl"));
                            iriscrc.Add(locHex[0].ToString());
                            irisnum.Add(i.ToString());

                            locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + i.ToString().PadLeft(4, '0') + "_etc_a.mtrl"));
                            etccrc.Add(locHex[0].ToString());
                            etcnum.Add(i.ToString());
                        }
                        for (int i = 1; i < 5; i++)
                        {
                            locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "01" + i.ToString().PadLeft(2, '0') + "_fac_a.mtrl"));
                            facecrc.Add(locHex[0].ToString());
                            facenum.Add((100 + i).ToString());

                            locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "01" + i.ToString().PadLeft(2, '0') + "_iri_a.mtrl"));
                            iriscrc.Add(locHex[0].ToString());
                            irisnum.Add((100 + i).ToString());

                            locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "01" + i.ToString().PadLeft(2, '0') + "_etc_a.mtrl"));
                            etccrc.Add(locHex[0].ToString());
                            etcnum.Add((100 + i).ToString());
                        }
                        break;
                    case "0104":
                    case "0504":
                    case "0604":
                    case "0804":
                        locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "0001_fac_a.mtrl"));
                        facecrc.Add(locHex[0].ToString());
                        facenum.Add("1");

                        locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "0001_iri_a.mtrl"));
                        iriscrc.Add(locHex[0].ToString());
                        irisnum.Add("1");

                        locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "0001_etc_a.mtrl"));
                        etccrc.Add(locHex[0].ToString());
                        etcnum.Add("1");

                        locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "0201_fac_a.mtrl"));
                        facecrc.Add(locHex[0].ToString());
                        facenum.Add("201");

                        locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "0201_iri_a.mtrl"));
                        iriscrc.Add(locHex[0].ToString());
                        irisnum.Add("201");

                        locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "0201_etc_a.mtrl"));
                        etccrc.Add(locHex[0].ToString());
                        etcnum.Add("201");
                        break;

                    case "0204":
                        locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "0001_fac_a.mtrl"));
                        facecrc.Add(locHex[0].ToString());
                        facenum.Add("1");

                        locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "0001_iri_a.mtrl"));
                        iriscrc.Add(locHex[0].ToString());
                        irisnum.Add("1");

                        locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "0001_etc_a.mtrl"));
                        etccrc.Add(locHex[0].ToString());
                        etcnum.Add("1");

                        locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "0201_fac_a.mtrl"));
                        facecrc.Add(locHex[0].ToString());
                        facenum.Add("201");

                        locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "0201_iri_a.mtrl"));
                        iriscrc.Add(locHex[0].ToString());
                        irisnum.Add("201");

                        locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "0201_etc_a.mtrl"));
                        etccrc.Add(locHex[0].ToString());
                        etcnum.Add("201");

                        locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "0202_fac_a.mtrl"));
                        facecrc.Add(locHex[0].ToString());
                        facenum.Add("202");

                        locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "0202_iri_a.mtrl"));
                        iriscrc.Add(locHex[0].ToString());
                        irisnum.Add("202");

                        locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "0202_etc_a.mtrl"));
                        etccrc.Add(locHex[0].ToString());
                        etcnum.Add("202");
                        break;

                    case "9104":
                    case "9204":
                        locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "0001_fac_a.mtrl"));
                        facecrc.Add(locHex[0].ToString());
                        facenum.Add("1");

                        locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "0001_iri_a.mtrl"));
                        iriscrc.Add(locHex[0].ToString());
                        irisnum.Add("1");

                        locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "0001_etc_a.mtrl"));
                        etccrc.Add(locHex[0].ToString());
                        etcnum.Add("1");

                        locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "0002_fac_a.mtrl"));
                        facecrc.Add(locHex[0].ToString());
                        facenum.Add("2");

                        locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "0002_iri_a.mtrl"));
                        iriscrc.Add(locHex[0].ToString());
                        irisnum.Add("2");

                        locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "0002_etc_a.mtrl"));
                        etccrc.Add(locHex[0].ToString());
                        etcnum.Add("2");

                        locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "0201_fac_a.mtrl"));
                        facecrc.Add(locHex[0].ToString());
                        facenum.Add("201");

                        locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "0201_iri_a.mtrl"));
                        iriscrc.Add(locHex[0].ToString());
                        irisnum.Add("201");

                        locHex = helper.getMTRLoffset("none", crc.text(faceMtrlFile + "0201_etc_a.mtrl"));
                        etccrc.Add(locHex[0].ToString());
                        etcnum.Add("201");
                        break;
                }

                faceList.Add(facenum.ToArray());
                faceList.Add(facecrc.ToArray());
                faceList.Add(irisnum.ToArray());
                faceList.Add(iriscrc.ToArray());
                faceList.Add(etcnum.ToArray());
                faceList.Add(etccrc.ToArray());
                mtrlDict.Add(raceIDDict[s], faceList);
            }
        }

        public void hairMtrlMaker()
        {
            uint hIcon;
            int[] locHex = { 0, 0 };

            mtrlDict = new Dictionary<string, List<string[]>>();
            Dictionary<string, string[]> fFileHex = new Dictionary<string, string[]>();

            ExdReader ex = new ExdReader(Resources.strings.Hair, "0379BF00");

            using (BinaryReader br = new BinaryReader(new MemoryStream(ex.getDecompBytes())))
            {
                int initOffset = 0x5176;

                foreach (var race in raceDict)
                {
                    List<string> haircrc = new List<string>();
                    List<string> hairnum = new List<string>();
                    List<string[]> hairList = new List<string[]>();

                    List<string> acccrc = new List<string>();
                    List<string> accnum = new List<string>();

                    string hairMtrlFile = "mt_c" + race.Value + "h";

                    br.BaseStream.Seek(initOffset, 0);
                    if (!race.Key.Equals("ALL"))
                    {
                        while ((hIcon = BitConverter.ToUInt32(br.ReadBytes(4).Reverse().ToArray(), 0)) != 0)
                        {
                            br.ReadBytes(2);
                            byte hID = br.ReadByte();
                            locHex = helper.getMTRLoffset("none", crc.text(hairMtrlFile + hID.ToString().PadLeft(4, '0') + "_hir_a.mtrl"));
                            if (locHex[0] != 0)
                            {
                                haircrc.Add(locHex[0].ToString());
                                hairnum.Add(hID.ToString());
                            }

                            locHex = helper.getMTRLoffset("none", crc.text(hairMtrlFile + hID.ToString().PadLeft(4, '0') + "_acc_b.mtrl"));
                            if (locHex[0] != 0)
                            {
                                acccrc.Add(locHex[0].ToString());
                                accnum.Add(hID.ToString());
                            }
                            br.ReadBytes(9);
                        }
                        hairList.Add(hairnum.ToArray());
                        hairList.Add(haircrc.ToArray());
                        hairList.Add(accnum.ToArray());
                        hairList.Add(acccrc.ToArray());
                        mtrlDict.Add(race.Key, hairList);
                    }
                    initOffset = initOffset + 0x640;
                }
            }
        }

        private void tailsMtrlMaker()
        {
            mtrlDict = new Dictionary<string, List<string[]>>();
            int[] locHex = { 0, 0 };
            string[] tailCNum = { "0701", "0801", "1301", "1401"};

            foreach (string raceID in tailCNum)
            {
                List<string> tailcrc = new List<string>();
                List<string> tailnum = new List<string>();
                List<string[]> tailList = new List<string[]>();

                if (raceID.Equals("0701") || raceID.Equals("0801"))
                {
                    for (int i = 1; i < 9; i++)
                    {
                        string tailMtrlFile = "mt_c" + raceID + "t000" + i +"_a.mtrl";
                        locHex = helper.getMTRLoffset("none", crc.text(tailMtrlFile));
                        tailcrc.Add(locHex[0].ToString());
                        tailnum.Add(i.ToString());
                    }

                }
                else if (raceID.Equals("1301") || raceID.Equals("1401"))
                {
                    for (int i = 1; i < 5; i++)
                    {
                        string tailMtrlFile = "mt_c" + raceID + "t000" + i + "_a.mtrl";
                        locHex = helper.getMTRLoffset("none", crc.text(tailMtrlFile));
                        tailcrc.Add(locHex[0].ToString());
                        tailnum.Add(i.ToString());
                    }
                }

                tailList.Add(tailnum.ToArray());
                tailList.Add(tailcrc.ToArray());
                mtrlDict.Add(raceIDDict[raceID], tailList);
            }
        }

        public void summonsMtrlMaker()
        {
            int[] locHex = { 0, 0 };

            string summonsFolderHex = "";
            string summonsFileHex = "";

            List<string> summoncrc = new List<string>();
            List<string> summonnum = new List<string>();
            List<string[]> summonList = new List<string[]>();

            mtrlDict = new Dictionary<string, List<string[]>>();

            string summonMNum = summonDict[childNode];

            string s1 = "";
            int bodnum = 1;
            if (summonMNum.Contains("a"))
            {
                s1 = summonMNum.Substring(0, summonMNum.Length - 1);
                bodnum = 2;
            }
            else
            {
                s1 = summonMNum;
            }

            string materialFolder = "chara/monster/m" + s1 + "/obj/body/b000" + bodnum + "/material/v000";
            textureFolder = "chara/monster/m" + s1 + "/obj/body/b000" + bodnum + "/texture";
            string summonMTRLFile = "mt_m" + s1 + "b000" + bodnum + "_a.mtrl";

            summonFolder = materialFolder;

            summonsFolderHex = crc.text(materialFolder + "1");
            summonsFileHex = crc.text(summonMTRLFile);

            locHex = helper.getMTRLoffset(summonsFolderHex, summonsFileHex);
            summoncrc.Add(locHex[0].ToString());
            summonnum.Add("1");

            if (summonMNum.Equals("7002"))
            {
                for (int x = 2; x < 7; x++)
                {
                    summonsFolderHex = crc.text(materialFolder + x);
                    summonsFileHex = crc.text(summonMTRLFile);

                    locHex = helper.getMTRLoffset(summonsFolderHex, summonsFileHex);
                    summoncrc.Add(locHex[0].ToString());
                    summonnum.Add(x.ToString());
                }
            }
            else if (summonMNum.Equals("7003") || summonMNum.Equals("7004"))
            {
                summonsFolderHex = crc.text(materialFolder + "2");
                summonsFileHex = crc.text(summonMTRLFile);

                locHex = helper.getMTRLoffset(summonsFolderHex, summonsFileHex);
                summoncrc.Add(locHex[0].ToString());
                summonnum.Add("2");
            }

            summonList.Add(summonnum.ToArray());
            summonList.Add(summoncrc.ToArray());
            mtrlDict.Add(childNode, summonList);
        }

        public Dictionary<string, List<string[]>> getMtrlDict()
        {
            return mtrlDict;
        }

        public string getSummonFolder()
        {
            return summonFolder;
        }

        public string getMountFolder()
        {
            return mountFolder;
        }

        public string getMountMtrl()
        {
            return mountMtrl;
        }

        public string getMinionFolder()
        {
            return minionFolder;
        }

        public string getMinionMtrl()
        {
            return minionMTRL;
        }
    }
}
