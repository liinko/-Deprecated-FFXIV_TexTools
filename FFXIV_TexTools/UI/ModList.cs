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
using System.Windows.Forms;
using FFXIV_TexTools.Helpers;
using FFXIV_TexTools.IO;
using Newtonsoft.Json;

namespace FFXIV_TexTools.UI
{
    public partial class ModListForm : Form
    {
        bool canGo = true;
        List<jModEntry> modList = new List<jModEntry>();

        /// <summary>
        /// Dialog for Mod List
        /// </summary>
        public ModListForm()
        {
            InitializeComponent();
            listView1.View = View.Details;
            listView1.GridLines = true;
            FFCRC crc = new FFCRC();

            Dictionary<string, string> raceIDDict = new Dictionary<string, string>();
            raceIDDict.Add("0101", Resources.strings.Hyur + " " + Resources.strings.Midlander + " " + Resources.strings.Male);
            raceIDDict.Add("0104", Resources.strings.Hyur + " " + Resources.strings.Male + " NPC");
            raceIDDict.Add("0201", Resources.strings.Hyur + " " + Resources.strings.Midlander + " " + Resources.strings.Female);
            raceIDDict.Add("0204", Resources.strings.Hyur + " " + Resources.strings.Female + " NPC");
            raceIDDict.Add("0301", Resources.strings.Hyur + " " + Resources.strings.Midlander + " " + Resources.strings.Male);
            raceIDDict.Add("0401", Resources.strings.Hyur + " " + Resources.strings.Midlander + " " + Resources.strings.Female);
            raceIDDict.Add("0501", Resources.strings.Elezen + " " + Resources.strings.Male);
            raceIDDict.Add("0504", Resources.strings.Elezen + " " + Resources.strings.Male + " NPC");
            raceIDDict.Add("0601", Resources.strings.Elezen + " " + Resources.strings.Female);
            raceIDDict.Add("0604", Resources.strings.Elezen + " " + Resources.strings.Female + " NPC");
            raceIDDict.Add("0701", Resources.strings.Miqote + " " + Resources.strings.Male);
            raceIDDict.Add("0801", Resources.strings.Miqote + " " + Resources.strings.Female);
            raceIDDict.Add("0804", Resources.strings.Miqote + " " + Resources.strings.Female + " NPC");
            raceIDDict.Add("0901", Resources.strings.Roegadyn + " " + Resources.strings.Male);
            raceIDDict.Add("1001", Resources.strings.Roegadyn + " " + Resources.strings.Female);
            raceIDDict.Add("1101", Resources.strings.Lalafell + " " + Resources.strings.Male);
            raceIDDict.Add("1201", Resources.strings.Lalafell + " " + Resources.strings.Female);
            raceIDDict.Add("1301", Resources.strings.Au_Ra + " " + Resources.strings.Male);
            raceIDDict.Add("1401", Resources.strings.Au_Ra + " " + Resources.strings.Female);
            raceIDDict.Add("9104", "NPC " + Resources.strings.Male);
            raceIDDict.Add("9204", "NPC " + Resources.strings.Female);

            string isActive, partName = "", partNum = "", race;

            listView1.Columns.Add("Name", -2, HorizontalAlignment.Left);
            listView1.Columns.Add("Race", -2, HorizontalAlignment.Left);
            listView1.Columns.Add("Map", -2, HorizontalAlignment.Left);
            listView1.Columns.Add("Part", -2, HorizontalAlignment.Left);
            listView1.Columns.Add("Part 2", -2, HorizontalAlignment.Left);
            listView1.Columns.Add("Active", -2, HorizontalAlignment.Left);

            foreach (string line in File.ReadLines(Properties.Settings.Default.DefaultDir + "/040000.modlist"))
            {
                jModEntry modEntry = JsonConvert.DeserializeObject<jModEntry>(line);
                modList.Add(modEntry);

                if (!modEntry.name.Contains("decal") && !modEntry.name.Contains("mt_m") && !modEntry.name.Contains("1_m") && !modEntry.name.Contains("2_m")
                    && !modEntry.name.Contains("1_d"))
                {
                    string sName = modEntry.name;
                    race = raceIDDict[sName.Substring(sName.IndexOf('c') + 1, 4)];
                    if(race == null && sName.Contains("_w"))
                    {
                        race = "ALL";
                    }
                }
                else if (modEntry.name.Contains("decal"))
                {
                    race = "ALL";
                }
                else
                {
                    race = "Monsters";
                }

                if (modEntry.textureName == null)
                {
                    isActive = "Unknown";
                }
                else
                {
                    IOHelper ioHelper = new IOHelper();
                    if (!modEntry.name.Contains("mt_"))
                    {
                        isActive = ioHelper.isActive(crc.text(modEntry.name + ".tex"), modEntry.folder, modEntry.modOffset).ToString();
                    }
                    else
                    {
                        isActive = ioHelper.isActive(crc.text(modEntry.name + ".mtrl"), modEntry.folder, modEntry.modOffset).ToString();
                    }
                    
                }

                if (modEntry.textureName == null)
                {
                    canGo = false;
                    modEntry.textureName = modEntry.name;
                }
                else
                {
                    canGo = true;
                }

                if(modEntry.name.Contains("f0"))
                {
                    string sName = modEntry.name;
                    int val = int.Parse(sName.Substring(sName.IndexOf('f') + 1, 4));
                    partName = val.ToString(); ;
                }
                else if (modEntry.name.Contains("b0"))
                {
                    string sName = modEntry.name;
                    int val = int.Parse(sName.Substring(sName.IndexOf('b') + 1, 4));
                    partName = val.ToString(); ;
                }
                else if (modEntry.name.Contains("h0"))
                {
                    string sName = modEntry.name;
                    int val = int.Parse(sName.Substring(sName.IndexOf('h') + 1, 4));
                    partName = val.ToString(); ;
                }
                else if (modEntry.name.Contains("t0"))
                {
                    string sName = modEntry.name;
                    int val = int.Parse(sName.Substring(sName.IndexOf('t') + 1, 4));
                    partName = val.ToString(); ;
                }
                else if (modEntry.name.Contains("decal"))
                {
                    string sName = modEntry.name;
                    int val = int.Parse(sName.Substring(sName.LastIndexOf('_') + 1));
                    partName = val.ToString(); ;
                }
                else if (modEntry.name.Contains("mt"))
                {
                    string sName = modEntry.name;
                    partName = sName.Substring(sName.LastIndexOf('_') + 1 );
                }
                else if (modEntry.name.Contains("1_d"))
                {
                    string sName = modEntry.name;
                    partName = sName.Substring(sName.LastIndexOf('_') - 3, 3);
                }
                else
                {
                    partName = "a";
                }

                if (modEntry.name.Contains("f0"))
                {
                    if (modEntry.name.Contains("_fac"))
                    {
                        partNum = "Face";
                    }
                    else if (modEntry.name.Contains("_iri"))
                    {
                        partNum = "Iris";
                    }
                    else if (modEntry.name.Contains("_etc"))
                    {
                        partNum = "Etc";
                    }
                }
                else if (modEntry.name.Contains("h0"))
                {
                    if (modEntry.name.Contains("_hir"))
                    {
                        partNum = "Hair";
                    }
                    if (modEntry.name.Contains("_acc"))
                    {
                        partNum = "Accessory";
                    }
                }
                else
                {
                    partNum = "";
                }

                ListViewItem itemsList = new ListViewItem(modEntry.textureName);
                itemsList.SubItems.Add(race);
                itemsList.SubItems.Add(texMap(modEntry.name));
                itemsList.SubItems.Add(partName);
                itemsList.SubItems.Add(partNum);
                itemsList.SubItems.Add(isActive);

                listView1.Items.Add(itemsList);

            }
            modGoToButton.Enabled = false;
            switchButton.Enabled = false;
        }

        private string texMap(string name)
        {
            string map;

            if (name.Contains("_s") && !name.Contains("mt"))
            {
                map = "Specular";
            }
            else if (name.Contains("_n") && !name.Contains("mt"))
            {
                map = "Normal";
            }
            else if (name.Contains("_d") && !name.Contains("mt"))
            {
                map = "Diffuse";
            }
            else if (name.Contains("_m") && !name.Contains("mt"))
            {
                map = "Mask";
            }
            else if (name.Contains("decal") && !name.Contains("mt"))
            {
                map = "Decal";
            }
            else
            {
                map = "ColorSet1";
            }

            return map;
        }

        private void modGoToButton_Click(object sender, EventArgs e)
        {
            if (canGo)
            {
                (Application.OpenForms["Form1"] as Form1).searchGo(listView1.SelectedItems[0].Text);
            }
        }

        private void modCloseButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                listView1.SelectedItems[0].ToString();
                modGoToButton.Enabled = true;
                switchButton.Enabled = true;
            }
            catch
            {
                modGoToButton.Enabled = false;
                switchButton.Enabled = false;
            }
        }

        private void switchButton_Click(object sender, EventArgs e)
        {
            string active = listView1.SelectedItems[0].SubItems[5].Text;
            IOHelper ioHelper = new IOHelper();

            if (active.Equals("False"))
            {
                jModEntry modEntry = modList[listView1.SelectedItems[0].Index];
                string name;

                if (!modEntry.name.Contains("mt_"))
                {
                    name = modEntry.name + ".tex";
                }
                else
                {
                    name = modEntry.name + ".mtrl";
                }

                int modded = int.Parse(modEntry.modOffset, NumberStyles.HexNumber);
                ioHelper.modifyIndexOffset(modded, name, modEntry.folder);

                listView1.SelectedItems[0].SubItems[5].Text = "True";
            }
            else
            {
                jModEntry modEntry = modList[listView1.SelectedItems[0].Index];
                string name;

                if (!modEntry.name.Contains("mt_"))
                {
                    name = modEntry.name + ".tex";
                }
                else
                {
                    name = modEntry.name + ".mtrl";
                }

                int orig = int.Parse(modEntry.origOffset, NumberStyles.HexNumber);
                ioHelper.modifyIndexOffset(orig, name, modEntry.folder);

                listView1.SelectedItems[0].SubItems[5].Text = "False";
            }
        }

        private void modExportButton_Click(object sender, EventArgs e)
        {
            int count = listView1.CheckedItems.Count;

            Directory.CreateDirectory(Directory.GetCurrentDirectory() + "/exported/");

            if (count > 0)
            {
                List<string> exportModList = new List<string>();
                using (FileStream fs = File.Create(Directory.GetCurrentDirectory() + "/exported/xivMods.dat"))
                {
                    using (BinaryWriter bw = new BinaryWriter(fs))
                    {
                        bw.BaseStream.Seek(0, SeekOrigin.Begin);
                        using (BinaryReader br = new BinaryReader(File.OpenRead(Properties.Settings.Default.DefaultDir + "/040000.win32.dat3")))
                        {
                            int nextOffset;
                            foreach (ListViewItem item in listView1.CheckedItems)
                            {
                                jModEntry entry = modList[item.Index];

                                try
                                {
                                    jModEntry nextEntry = modList[item.Index + 1];
                                    nextOffset = int.Parse(nextEntry.modOffset, NumberStyles.HexNumber);
                                }
                                catch
                                {
                                    nextOffset = (int)br.BaseStream.Length;
                                }

                                int size = nextOffset - int.Parse(entry.modOffset, NumberStyles.HexNumber);

                                br.BaseStream.Seek(int.Parse(entry.modOffset, NumberStyles.HexNumber) - 48, SeekOrigin.Begin);

                                entry.modOffset = bw.BaseStream.Position.ToString("X").PadLeft(8, '0');

                                exportModList.Add(JsonConvert.SerializeObject(entry));

                                bw.Write(br.ReadBytes(size));
                            }
                        }
                    }
                }
                File.WriteAllLines(Directory.GetCurrentDirectory() + "/exported/xivMods.modlist", exportModList.ToArray());
            }
        }
    }
}
