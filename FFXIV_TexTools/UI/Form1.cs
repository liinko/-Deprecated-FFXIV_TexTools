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
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using FFXIV_TexTools.Helpers;
using FFXIV_TexTools.IO;
using FFXIV_TexTools.UI;
using Newtonsoft.Json;


namespace FFXIV_TexTools
{

    /// <summary>
    /// Application main code and UI
    /// </summary>
    public partial class Form1 : Form
    {
        bool inModList;
        byte[] decompTexBytes, colorMapBytes;
        string parentNode, childNode, itemVersion, itemID, currTexName, currMap, fileOffset, weaponBody, currMTRLName, folderHex, summonFolder, mountFolder, mountMTRL, minionFolder, minionMTRL, siteURL, folderPath;
        int height, width, currTexType, datNum, newOffset, materialOffset;
        ProgressDialog dialog;
        List<string> currTex;
        Dictionary<int, string> decalList;
        Dictionary<string, string> raceDict, textureOffsets, raceIDDict, eSlotDict, summonDict, nameSlotDict;
        Dictionary<string, int> slotDict;
        Dictionary<string, List<string[]>> mtrlDict;
        Dictionary<string, Items> itemsDict, mountsDict, minionsDict;
        jModEntry modListEntry;
        Helper helper = new Helper();
        Version v;

        /// <summary>
        /// Application initialization
        /// </summary>
        public Form1()
        {
            InitializeComponent();

            if (Properties.Settings.Default.DefaultDir.Equals(""))
            {
                Properties.Settings.Default.SaveDir = Directory.GetCurrentDirectory() + "/Saved";
                Properties.Settings.Default.BackupDir = Directory.GetCurrentDirectory() + "/Backup";
                FolderBrowserDialog folderdialog = new FolderBrowserDialog();
                folderdialog.Description = "Select SquareEnix\\FINAL FANTASY XIV - A Realm Reborn\\game\\sqpack\\ffxiv folder";
                folderdialog.ShowDialog();
                Properties.Settings.Default.DefaultDir = folderdialog.SelectedPath;
                Properties.Settings.Default.Save();
            }

            CultureInfo ci = new CultureInfo(Properties.Settings.Default.Language);
            CultureInfo.DefaultThreadCurrentUICulture = ci;
            CultureInfo.DefaultThreadCurrentCulture = ci;

            if (Properties.Settings.Default.Language.Equals("en"))
            {
                englishToolStripMenuItem.Checked = true;
            }
            else if (Properties.Settings.Default.Language.Equals("ja"))
            {
                japaneseToolStripMenuItem.Checked = true;
            }
            else if (Properties.Settings.Default.Language.Equals("fr"))
            {
                frenchToolStripMenuItem.Checked = true;
            }
            else if (Properties.Settings.Default.Language.Equals("de"))
            {
                germanToolStripMenuItem.Checked = true;
            }

            if (Properties.Settings.Default.DXVer.Equals("dx11"))
            {
                dX11ToolStripMenuItem.Checked = true;
            }
            else if (Properties.Settings.Default.DXVer.Equals("dx9"))
            {
                dX9ToolStripMenuItem.Checked = true;
            }

            string xmlURL = "https://raw.githubusercontent.com/liinko/FFXIVTexToolsWeb/master/version.xml";
            string changeLog = "";
            try
            {
                using (XmlTextReader reader = new XmlTextReader(xmlURL))
                {
                    reader.MoveToContent();
                    string elementName = "";

                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "FFXIVTexTools")
                    {
                        while (reader.Read())
                        {
                            if (reader.NodeType == XmlNodeType.Element)
                            {
                                elementName = reader.Name;
                            }
                            else
                            {
                                if (reader.NodeType == XmlNodeType.Text && reader.HasValue)
                                {
                                    switch (elementName)
                                    {
                                        case "version":
                                            v = new Version(reader.Value);
                                            break;
                                        case "url":
                                            siteURL = reader.Value;
                                            break;
                                        case "log":
                                            changeLog = reader.Value;
                                            break;
                                    }
                                }
                            }
                        }
                    }
                }

                Version curVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

                if (curVersion.CompareTo(v) < 0)
                {

                    Updates up = new Updates();

                    up.Message = "Version " + v.ToString().Substring(0, 3) + "\n\nChange Log:" + changeLog + "\n\nPlease visit the website to download the update.";
                    up.Show(this);
                }
            }
            catch
            {

            }

            initDictionaries();

            save3DButton.Enabled = false;

            //Set Index Changed event handler for combo boxes
            raceComboBox.SelectedIndexChanged += new EventHandler(raceComboBox_SelectedIndexChanged);
            mapComboBox.SelectedIndexChanged += new EventHandler(mapComboBox_SelectedIndexChanged);
            partComboBox.SelectedIndexChanged += new EventHandler(partComboBox_SelectedIndexChanged);
            extraComboBox.SelectedIndexChanged += new EventHandler(extraComboBox_SelectedIndexChanged);

            startFill();

        }

        /// <summary>
        /// starts BackgroundWorker to fill treeview
        /// </summary>
        public void startFill()
        {
            dialog = new ProgressDialog();
            dialog.StartPosition = FormStartPosition.CenterParent;

            BackgroundWorker fillTreeView = new BackgroundWorker();
            fillTreeView.WorkerReportsProgress = true;
            fillTreeView.WorkerSupportsCancellation = true;
            fillTreeView.DoWork += new DoWorkEventHandler(fillTreeView_Work);
            fillTreeView.ProgressChanged += new ProgressChangedEventHandler(fillTreeView_ProgressChanged);
            fillTreeView.RunWorkerCompleted += new RunWorkerCompletedEventHandler(fillTreeView_RunWorkerCompleted);
            fillTreeView.RunWorkerAsync();
        }

        /// <summary>
        /// Sets up dictionaries
        /// </summary>
        public void initDictionaries()
        {
            //Races Dictionary (Race, RaceID)
            raceDict = new Dictionary<string, string>();
            raceDict.Add(Resources.strings.Hyur + " " + Resources.strings.Midlander + " " + Resources.strings.Male, "0101");
            raceDict.Add(Resources.strings.Hyur + " " + Resources.strings.Midlander + " " + Resources.strings.Female, "0201");
            raceDict.Add(Resources.strings.Hyur + " " + Resources.strings.Highlander + " " + Resources.strings.Male, "0301");
            raceDict.Add(Resources.strings.Hyur + " " + Resources.strings.Highlander + " " + Resources.strings.Female, "0401");
            raceDict.Add(Resources.strings.Elezen + " " + Resources.strings.Male, "0501");
            raceDict.Add(Resources.strings.Elezen + " " + Resources.strings.Female, "0601");
            raceDict.Add(Resources.strings.Miqote + " " + Resources.strings.Male, "0701");
            raceDict.Add(Resources.strings.Miqote + " " + Resources.strings.Female, "0801");
            raceDict.Add(Resources.strings.Roegadyn + " " + Resources.strings.Male, "0901");
            raceDict.Add(Resources.strings.Roegadyn + " " + Resources.strings.Female, "1001");
            raceDict.Add(Resources.strings.Lalafell + " " + Resources.strings.Male, "1101");
            raceDict.Add(Resources.strings.Lalafell + " " + Resources.strings.Female, "1201");
            raceDict.Add(Resources.strings.Au_Ra + " " + Resources.strings.Male, "1301");
            raceDict.Add(Resources.strings.Au_Ra + " " + Resources.strings.Female, "1401");
            raceDict.Add("ALL", "ALL");

            //RaceID Dictionary (RaceID, Race)
            raceIDDict = new Dictionary<string, string>();
            raceIDDict.Add("0101", Resources.strings.Hyur + " " + Resources.strings.Midlander + " " + Resources.strings.Male);
            raceIDDict.Add("0104", Resources.strings.Hyur + " " + Resources.strings.Male + " NPC");
            raceIDDict.Add("0201", Resources.strings.Hyur + " " + Resources.strings.Midlander + " " + Resources.strings.Female);
            raceIDDict.Add("0204", Resources.strings.Hyur + " " + Resources.strings.Female + " NPC");
            raceIDDict.Add("0301", Resources.strings.Hyur + " " + Resources.strings.Highlander + " " + Resources.strings.Male);
            raceIDDict.Add("0401", Resources.strings.Hyur + " " + Resources.strings.Highlander + " " + Resources.strings.Female);
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

            //Equipment slot abreviation Dictionary (Slot, Internal slot abreviation)
            eSlotDict = new Dictionary<string, string>();
            eSlotDict.Add(Resources.strings.Head, "met");
            eSlotDict.Add(Resources.strings.Hands, "glv");
            eSlotDict.Add(Resources.strings.Legs, "dwn");
            eSlotDict.Add(Resources.strings.Feet, "sho");
            eSlotDict.Add(Resources.strings.Body, "top");
            eSlotDict.Add(Resources.strings.Ears, "ear");
            eSlotDict.Add(Resources.strings.Neck, "nek");
            eSlotDict.Add(Resources.strings.Rings, "rir");
            eSlotDict.Add(Resources.strings.Wrists, "wrs");
            eSlotDict.Add(Resources.strings.Head_Body, "top");
            eSlotDict.Add(Resources.strings.Body_Hands_Legs_Feet, "top");
            eSlotDict.Add(Resources.strings.Legs_Feet, "top");
            eSlotDict.Add(Resources.strings.Body_Hands_Legs, "top");
            eSlotDict.Add(Resources.strings.Body_Legs_Feet, "top");
            eSlotDict.Add(Resources.strings.All, "top");
            eSlotDict.Add(Resources.strings.Body_Hands, "top");

            //Equipment slot ID dictionary (Slot, SlotID)
            slotDict = new Dictionary<string, int>();
            slotDict.Add(Resources.strings.Main_Hand, 0);
            slotDict.Add(Resources.strings.Off_Hand, 0);
            slotDict.Add(Resources.strings.Head, 0);
            slotDict.Add(Resources.strings.Body, 1);
            slotDict.Add(Resources.strings.Hands, 2);
            slotDict.Add(Resources.strings.Legs, 3);
            slotDict.Add(Resources.strings.Feet, 4);
            slotDict.Add(Resources.strings.Ears, 0);
            slotDict.Add(Resources.strings.Neck, 1);
            slotDict.Add(Resources.strings.Wrists, 2);
            slotDict.Add(Resources.strings.Rings, 3);
            slotDict.Add(Resources.strings.Two_Handed, 0);
            slotDict.Add(Resources.strings.Main_Off, 0);
            slotDict.Add(Resources.strings.Head_Body, 1);
            slotDict.Add(Resources.strings.Body_Hands_Legs, 1);
            slotDict.Add(Resources.strings.Body_Legs_Feet, 1);
            slotDict.Add(Resources.strings.Body_Hands_Legs_Feet, 1);
            slotDict.Add(Resources.strings.Legs_Feet, 3);
            slotDict.Add(Resources.strings.All, 1);
            slotDict.Add("Extra", 0);
            slotDict.Add(Resources.strings.Body_Hands, 1);

            //Summons dictionary (Summon name, Summon ID)
            summonDict = new Dictionary<string, string>();
            summonDict.Add(Resources.strings.Eos, "7001");
            summonDict.Add(Resources.strings.Selene, "7001a");
            summonDict.Add(Resources.strings.Carbuncle, "7002");
            summonDict.Add(Resources.strings.Ifrit_Egi, "7003");
            summonDict.Add(Resources.strings.Titan_Egi, "7004");
            summonDict.Add(Resources.strings.Garuda_Egi, "7005");
            summonDict.Add(Resources.strings.Ramuh_Egi, "7006");
            summonDict.Add(Resources.strings.Rook_Autoturret, "7101");
            summonDict.Add(Resources.strings.Bishop_Autoturret, "7101a");
        }

        private void fillTreeView_Work(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            //First time setup of .dat3 and .modlist
            if (!File.Exists(Properties.Settings.Default.DefaultDir + "/040000.win32.dat3"))
            {
                CreateDat create = new CreateDat();
                create.createModList();
            }

            itemsDict = new Dictionary<string, Items>();
            nameSlotDict = new Dictionary<string, string>();
            mountsDict = new Dictionary<string, Items>();

            //Offsets for Item Exdf files
            string[] itemHex = {Resources.strings.Item_0, Resources.strings.Item_500, Resources.strings.Item_1000, Resources.strings.Item_1500, Resources.strings.Item_2000, Resources.strings.Item_2500,
                Resources.strings.Item_3000, Resources.strings.Item_3500, Resources.strings.Item_4000, Resources.strings.Item_4500, Resources.strings.Item_5000, Resources.strings.Item_5500,
                Resources.strings.Item_6000, Resources.strings.Item_6500, Resources.strings.Item_7000, Resources.strings.Item_7500, Resources.strings.Item_8000, Resources.strings.Item_8500,
                Resources.strings.Item_9000, Resources.strings.Item_9500, Resources.strings.Item_10000, Resources.strings.Item_10500, Resources.strings.Item_11000, Resources.strings.Item_11500,
                Resources.strings.Item_12000, Resources.strings.Item_12500, Resources.strings.Item_13000, Resources.strings.Item_13500, Resources.strings.Item_14000, Resources.strings.Item_14500,
                Resources.strings.Item_15000, Resources.strings.Item_15500, Resources.strings.Item_16000, Resources.strings.Item_16500, Resources.strings.Item_17000, Resources.strings.Item_17500};

            string mountHex = Resources.strings.mountEXD;
            string minionHex = Resources.strings.minionEXD;

            worker.ReportProgress(0);

            foreach (string s in itemHex)
            {
                ExdReader er = new ExdReader("Item", s);
                itemsDict = itemsDict.Union(er.getItemsDict()).ToDictionary(k => k.Key, v => v.Value);
                nameSlotDict = nameSlotDict.Union(er.getNameSlotDict()).ToDictionary(k => k.Key, v => v.Value);
            }

            ExdReader exr = new ExdReader("Mounts", mountHex);
            mountsDict = exr.getMountsDict();

            exr = new ExdReader("Minions", minionHex);
            minionsDict = exr.getMinionsDict();
        }

        private void fillTreeView_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (!dialog.Visible)
            {
                dialog.ShowDialog();
            }
            if (!dialog.Message.Equals("Loading...."))
            {
                dialog.Message = "Loading....";
            }
        }

        private void fillTreeView_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            if (e.Error != null)
            {
                MessageBox.Show("Unable to find file, please check your directories under settings \n\n" + e.Error.Message, "Directory Error");
                treeView1.Nodes.Clear();
            }
            else
            {
                string[] equipSlots = {Resources.strings.Character, Resources.strings.Main_Hand, Resources.strings.Off_Hand, Resources.strings.Head, Resources.strings.Body, Resources.strings.Hands, Resources.strings.Waist, Resources.strings.Legs, Resources.strings.Feet, Resources.strings.Ears, Resources.strings.Neck, Resources.strings.Wrists, Resources.strings.Rings,
                                        Resources.strings.Two_Handed, Resources.strings.Main_Off, Resources.strings.Head_Body, Resources.strings.Body_Hands_Legs_Feet, Resources.strings.Soul_Crystal, Resources.strings.Legs_Feet, Resources.strings.All,
                                        Resources.strings.Body_Hands_Legs, Resources.strings.Body_Legs_Feet, Resources.strings.Pets, Resources.strings.Mounts, Resources.strings.Minions, "Extra"};


                foreach (string s in equipSlots)
                {
                    treeView1.Nodes.Add(s);
                }


                dialog.Message = "Populating List....";
                dialog.ProgressValue = 100;

                List<string> list = nameSlotDict.Keys.ToList();
                list.Sort();

                foreach(string name in list)
                {
                    if(int.Parse(nameSlotDict[name]) != 0)
                    {
                        try
                        {
                            treeView1.Nodes[int.Parse(nameSlotDict[name])].Nodes.Add(name, name);
                        }
                        catch
                        {
                            Console.WriteLine("Failed " + nameSlotDict[name] + " name " + name);
                        }

                    }
                    else
                    {
                        treeView1.Nodes[25].Nodes.Add(name, name);
                    }
                }

                list = mountsDict.Keys.ToList();
                list.Sort();
                foreach(string m in list)
                {
                    treeView1.Nodes[23].Nodes.Add(m, m);
                }

                list = minionsDict.Keys.ToList();
                list.Sort();
                foreach(string m in list)
                {
                    treeView1.Nodes[24].Nodes.Add(m, m);
                }

                nameSlotDict.Clear();
                nameSlotDict = null;

                treeView1.Nodes[0].Nodes.Add(Resources.strings.Body, Resources.strings.Body);
                treeView1.Nodes[0].Nodes.Add(Resources.strings.Face, Resources.strings.Face);
                treeView1.Nodes[0].Nodes.Add(Resources.strings.Hair, Resources.strings.Hair);
                treeView1.Nodes[0].Nodes.Add(Resources.strings.Face_Paint, Resources.strings.Face_Paint);
                treeView1.Nodes[0].Nodes.Add(Resources.strings.Tail, Resources.strings.Tail);
                treeView1.Nodes[0].Nodes.Add("Equipment Decals", "Equipment Decals");
                treeView1.Nodes[22].Nodes.Add(Resources.strings.Eos, Resources.strings.Eos);
                treeView1.Nodes[22].Nodes.Add(Resources.strings.Selene, Resources.strings.Selene);
                treeView1.Nodes[22].Nodes.Add(Resources.strings.Carbuncle, Resources.strings.Carbuncle);
                treeView1.Nodes[22].Nodes.Add(Resources.strings.Ifrit_Egi, Resources.strings.Ifrit_Egi);
                treeView1.Nodes[22].Nodes.Add(Resources.strings.Titan_Egi, Resources.strings.Titan_Egi);
                treeView1.Nodes[22].Nodes.Add(Resources.strings.Garuda_Egi, Resources.strings.Garuda_Egi);
                treeView1.Nodes[22].Nodes.Add(Resources.strings.Ramuh_Egi, Resources.strings.Ramuh_Egi);
                treeView1.Nodes[22].Nodes.Add(Resources.strings.Rook_Autoturret, Resources.strings.Rook_Autoturret);
                treeView1.Nodes[22].Nodes.Add(Resources.strings.Bishop_Autoturret, Resources.strings.Bishop_Autoturret);

            }
            dialog.Close();
        }

        //When a node is selected
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode node = treeView1.SelectedNode;

            //if the node selected is not a parent node
            if (node.Parent != null)
            {
                mtrlDict = new Dictionary<string, List<string[]>>();

                revertButton.Enabled = false;

                parentNode = e.Node.Parent.Text;
                childNode = e.Node.Text;

                raceComboBox.Items.Clear();

                if(parentNode.Equals("Head") || parentNode.Equals("Body") || parentNode.Equals("Hands") || parentNode.Equals("Legs") || parentNode.Equals("Feet"))
                {
                    save3DButton.Enabled = true;
                }
                else
                {
                    save3DButton.Enabled = false;
                }

                if (parentNode.Equals("Extra"))
                {
                    dataGridView1.Visible = false;
                    bigInfoBox.Visible = true;
                    pictureBox1.Visible = false;
                    savePNGButton.Enabled = false;
                    saveDDSButton.Enabled = false;
                    importButton.Enabled = false;
                    save3DButton.Enabled = false;
                    revertButton.Enabled = false;
                    mapComboBox.Enabled = false;
                    raceComboBox.Enabled = false;
                    partComboBox.Enabled = false;
                    extraLabel.Visible = false;
                    extraComboBox.Visible = false;
                    pictureBox1.Image = null;
                    mapComboBox.Items.Clear();
                    partComboBox.Items.Clear();
                    infoLabel.Text = "These seem to be mostly for food, I'll look into them later.";
                    bigInfoBox.Text = "These textures are not supported yet";
                    return;
                }

                dialog = new ProgressDialog();

                BackgroundWorker selectedWorker = new BackgroundWorker();
                selectedWorker.WorkerReportsProgress = true;
                selectedWorker.DoWork += new DoWorkEventHandler(selectedWorker_Work);
                selectedWorker.ProgressChanged += new ProgressChangedEventHandler(selectedWorker_ProgressChanged);
                selectedWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(selectedWorker_RunWorkerCompleted);
                selectedWorker.RunWorkerAsync();
            }
        }

        private void selectedWorker_Work(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            FFCRC crc = new FFCRC();
            Helper helper = new Helper();

            mountFolder = "";

            if (itemsDict.ContainsKey(childNode))
            {
                string[] secondary = new string[3];
                string[] imc_folder, sec_imc_folder;

                Items item = itemsDict[childNode];
                itemID = item.itemID;
                weaponBody = item.weaponBody;
                bool hasSecondary = item.hasSecondary;

                imc_folder = helper.getMaterialFolder(slotDict, parentNode, itemID, item.imcVersion, weaponBody);

                folderPath = imc_folder[1].Substring(0, imc_folder[1].IndexOf("/m")) + "/texture";

                itemVersion = imc_folder[0];
                folderHex = crc.text(imc_folder[1]);

                if (hasSecondary)
                {
                    secondary[0] = item.itemID1;
                    secondary[1] = item.imcVersion1;
                    secondary[2] = item.weaponBody1;

                    sec_imc_folder = helper.getMaterialFolder(slotDict, parentNode, secondary[0], secondary[1], secondary[2]);

                    secondary[1] = crc.text(sec_imc_folder[1]);
                }

                FindAvailable findAvailable = new FindAvailable("Equipment", parentNode, raceDict, itemID, weaponBody, folderHex, hasSecondary, secondary);
                mtrlDict = findAvailable.getMtrlDict();
            }
            else if (parentNode.Equals(Resources.strings.Character) && childNode.Equals(Resources.strings.Body))
            {
                FindAvailable findAvailable = new FindAvailable(Resources.strings.Body, raceIDDict);
                mtrlDict = findAvailable.getMtrlDict();
            }
            else if (parentNode.Equals(Resources.strings.Character) && childNode.Equals(Resources.strings.Face))
            {
                worker.ReportProgress(100);
                FindAvailable findAvailable = new FindAvailable(Resources.strings.Face, raceIDDict);
                mtrlDict = findAvailable.getMtrlDict();
            }
            else if (parentNode.Equals(Resources.strings.Character) && childNode.Equals(Resources.strings.Hair))
            {
                worker.ReportProgress(100);
                FindAvailable findAvailable = new FindAvailable(Resources.strings.Hair, raceDict);
                mtrlDict = findAvailable.getMtrlDict();
            }
            else if (parentNode.Equals(Resources.strings.Character) && childNode.Equals(Resources.strings.Face_Paint))
            {
                int[] locHex = { 0, 0 };
                decalList = new Dictionary<int, string>();

                for (int i = 0; i < 50; i++)
                {
                    worker.ReportProgress(100);
                    locHex = helper.getMTRLoffset("none", crc.text("_decal_" + (i + 1) + ".tex"));
                    decalList.Add((i + 1), locHex[0].ToString("X"));
                }
            }
            else if (parentNode.Equals(Resources.strings.Character) && childNode.Equals("Equipment Decals"))
            {
                int[] locHex = { 0, 0 };
                decalList = new Dictionary<int, string>();

                for (int i = 0; i < 4; i++)
                {
                    locHex = helper.getMTRLoffset("none", crc.text("-decal_" + (i + 1).ToString().PadLeft(3, '0') + ".tex"));
                    decalList.Add((i + 1), locHex[0].ToString("X"));
                }
                for (int i = 10; i < 31; i++)
                {
                    locHex = helper.getMTRLoffset("none", crc.text("-decal_" + (i + 1).ToString().PadLeft(3, '0') + ".tex"));
                    decalList.Add((i + 1), locHex[0].ToString("X"));
                }
                for (int i = 51; i < 55; i++)
                {
                    locHex = helper.getMTRLoffset("none", crc.text("-decal_" + (i + 1).ToString().PadLeft(3, '0') + ".tex"));
                    decalList.Add((i + 1), locHex[0].ToString("X"));
                }
                for (int i = 100; i < 115; i++)
                {
                    locHex = helper.getMTRLoffset("none", crc.text("-decal_" + (i + 1).ToString().PadLeft(3, '0') + ".tex"));
                    decalList.Add((i + 1), locHex[0].ToString("X"));
                }
                locHex = helper.getMTRLoffset("none", crc.text("-decal_151.tex"));
                decalList.Add((151), locHex[0].ToString("X"));
                for (int i = 195; i < 205; i++)
                {
                    worker.ReportProgress(100);
                    locHex = helper.getMTRLoffset("none", crc.text("-decal_" + (i + 1).ToString().PadLeft(3, '0') + ".tex"));
                    decalList.Add((i + 1), locHex[0].ToString("X"));
                }
                locHex = helper.getMTRLoffset("none", crc.text("-decal_255.tex"));
                decalList.Add((255), locHex[0].ToString("X"));
            }
            else if (parentNode.Equals(Resources.strings.Character) && childNode.Equals(Resources.strings.Tail))
            {
                FindAvailable findAvailable = new FindAvailable(Resources.strings.Tail, raceIDDict);
                mtrlDict = findAvailable.getMtrlDict();
                summonFolder = findAvailable.getSummonFolder();
            }
            else if (parentNode.Equals(Resources.strings.Pets))
            {
                FindAvailable findAvailable = new FindAvailable(Resources.strings.Pets, summonDict, childNode);
                mtrlDict = findAvailable.getMtrlDict();
                summonFolder = findAvailable.getSummonFolder();
            }
            else if (parentNode.Equals(Resources.strings.Mounts))
            {
                FindAvailable findAvailable = new FindAvailable(Resources.strings.Mounts, mountsDict, childNode);
                mtrlDict = findAvailable.getMtrlDict();
                mountFolder = findAvailable.getMountFolder();
                mountMTRL = findAvailable.getMountMtrl();
            }
            else if (parentNode.Equals(Resources.strings.Minions))
            {
                FindAvailable findAvailable = new FindAvailable(Resources.strings.Minions, minionsDict, childNode);
                mtrlDict = findAvailable.getMtrlDict();
                minionFolder = findAvailable.getMinionFolder();
                minionMTRL = findAvailable.getMinionMtrl();
            }
        }

        private void selectedWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            dialog.Message = "Loading Textures...";
            if (!dialog.Visible)
            {
                dialog.ShowDialog();
            }
        }

        private void selectedWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            dialog.ProgressValue = 100;
            if (itemsDict.ContainsKey(childNode))
            {
                mapComboBox.Enabled = true;
                extraLabel.Visible = false;
                extraComboBox.Visible = false;
                PartLabel.Text = "Part:";

                raceComboBox.Items.Clear();

                foreach (string k in mtrlDict.Keys)
                {
                    raceComboBox.Items.Add(k);
                }

                raceComboBox.SelectedIndex = 0;

                if (mtrlDict.Count == 1)
                {
                    raceComboBox.Enabled = false;
                }
                else
                {
                    raceComboBox.Enabled = true;
                }

                getTexFiles();
            }
            else if (parentNode.Equals(Resources.strings.Character) && childNode.Equals(Resources.strings.Body))
            {
                raceComboBox.Enabled = true;
                mapComboBox.Enabled = true;
                extraLabel.Visible = false;
                extraComboBox.Visible = false;
                PartLabel.Text = "Num:";

                raceComboBox.Items.Clear();
                raceComboBox.Items.Add(raceDict.Keys.ElementAt(0));
                raceComboBox.Items.Add(Resources.strings.Hyur + " " + Resources.strings.Male + " NPC");
                raceComboBox.Items.Add(raceDict.Keys.ElementAt(1));
                raceComboBox.Items.Add(raceDict.Keys.ElementAt(2));
                raceComboBox.Items.Add(raceDict.Keys.ElementAt(3));
                raceComboBox.Items.Add(Resources.strings.Miqote + " " + Resources.strings.Female + " NPC");
                raceComboBox.Items.Add(raceDict.Keys.ElementAt(8));
                raceComboBox.Items.Add(raceDict.Keys.ElementAt(10));
                raceComboBox.Items.Add(raceDict.Keys.ElementAt(12));
                raceComboBox.Items.Add(raceDict.Keys.ElementAt(13));

                raceComboBox.SelectedIndex = 0;

                getTexFiles();
            }
            else if (parentNode.Equals(Resources.strings.Character) && childNode.Equals(Resources.strings.Face))
            {
                raceComboBox.Enabled = true;
                mapComboBox.Enabled = true;
                extraLabel.Visible = true;
                extraComboBox.Visible = true;
                PartLabel.Text = "Num:";
                foreach (string fRace in raceIDDict.Values)
                {
                    raceComboBox.Items.Add(fRace);
                }
                raceComboBox.SelectedIndex = 0;

                getTexFiles();
            }
            else if (parentNode.Equals(Resources.strings.Character) && childNode.Equals(Resources.strings.Hair))
            {
                raceComboBox.Enabled = true;
                mapComboBox.Enabled = true;
                extraLabel.Visible = true;
                extraComboBox.Visible = true;
                PartLabel.Text = "Num:";
                foreach (string hRace in raceDict.Keys)
                {
                    if (!hRace.Equals("ALL"))
                    {
                        raceComboBox.Items.Add(hRace);
                    }
                }
                raceComboBox.SelectedIndex = 0;
                getTexFiles();
            }
            else if (parentNode.Equals(Resources.strings.Character) && childNode.Equals(Resources.strings.Face_Paint))
            {
                raceComboBox.Enabled = false;
                mapComboBox.Enabled = false;
                partComboBox.Enabled = true;
                extraLabel.Visible = false;
                extraComboBox.Visible = false;
                PartLabel.Text = "Num:";
                raceComboBox.Items.Clear();
                mapComboBox.Items.Clear();
                partComboBox.Items.Clear();

                raceComboBox.Items.Add("ALL");
                mapComboBox.Items.Add("Texture");

                for (int i = 0; i < 50; i++)
                {
                    partComboBox.Items.Add(i + 1);
                }

                raceComboBox.SelectedIndex = 0;
                mapComboBox.SelectedIndex = 0;
                partComboBox.SelectedIndex = 0;

                currTexName = "_decal_1.tex";

                displayTex(decalList[1].PadLeft(8, '0'), 0);
            }
            else if (parentNode.Equals(Resources.strings.Character) && childNode.Equals("Equipment Decals"))
            {
                raceComboBox.Enabled = false;
                mapComboBox.Enabled = false;
                partComboBox.Enabled = true;
                extraLabel.Visible = false;
                extraComboBox.Visible = false;
                PartLabel.Text = "Num:";
                raceComboBox.Items.Clear();
                mapComboBox.Items.Clear();
                partComboBox.Items.Clear();

                raceComboBox.Items.Add("ALL");
                mapComboBox.Items.Add("Texture");

                for (int i = 0; i < 4; i++)
                {
                    partComboBox.Items.Add(i + 1);
                }
                for (int i = 10; i < 31; i++)
                {
                    partComboBox.Items.Add(i + 1);
                }
                for (int i = 51; i < 55; i++)
                {
                    partComboBox.Items.Add(i + 1);
                }
                for (int i = 100; i < 115; i++)
                {
                    partComboBox.Items.Add(i + 1);
                }
                partComboBox.Items.Add(151);
                for (int i = 195; i < 205; i++)
                {
                    partComboBox.Items.Add(i + 1);
                }
                partComboBox.Items.Add(255);

                raceComboBox.SelectedIndex = 0;
                mapComboBox.SelectedIndex = 0;
                partComboBox.SelectedIndex = 0;

                currTexName = "-decal_001.tex";

                displayTex(decalList[1].PadLeft(8, '0'), 0);
            }
            else if (parentNode.Equals(Resources.strings.Character) && childNode.Equals(Resources.strings.Tail))
            {
                mapComboBox.Enabled = true;
                extraLabel.Visible = false;
                extraComboBox.Visible = false;
                raceComboBox.Enabled = true;

                foreach (string k in mtrlDict.Keys)
                {
                    raceComboBox.Items.Add(k);
                }
                raceComboBox.SelectedIndex = 0;
                getTexFiles();
            }
            else if (parentNode.Equals(Resources.strings.Pets))
            {
                raceComboBox.Enabled = false;
                mapComboBox.Enabled = true;
                extraLabel.Visible = false;
                extraComboBox.Visible = false;
                raceComboBox.Items.Add("Monsters");

                raceComboBox.SelectedIndex = 0;

                getTexFiles();
            }
            else if (parentNode.Equals(Resources.strings.Mounts) || parentNode.Equals(Resources.strings.Minions))
            {
                raceComboBox.Enabled = false;
                mapComboBox.Enabled = true;
                extraLabel.Visible = false;
                extraComboBox.Visible = false;
                raceComboBox.Items.Add("Monsters");

                raceComboBox.SelectedIndex = 0;

                getTexFiles();
            }
            dialog.Close();
        }

        private void raceComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = (ComboBox)sender;

            if (!childNode.Equals(Resources.strings.Face_Paint) && !childNode.Equals("Equipment Decals"))
            {
                partComboBox.Items.Clear();
                extraComboBox.Items.Clear();
            }

            if (parentNode.Equals(Resources.strings.Head_Body) || parentNode.Equals(Resources.strings.Body_Hands_Legs_Feet) || parentNode.Equals(Resources.strings.Legs_Feet) || parentNode.Equals(Resources.strings.Character) ||
                parentNode.Equals(Resources.strings.Pets) || parentNode.Equals(Resources.strings.Main_Hand) || parentNode.Equals(Resources.strings.Off_Hand) || parentNode.Equals(Resources.strings.Two_Handed) || 
                parentNode.Equals(Resources.strings.Rings) || parentNode.Equals(Resources.strings.Mounts) || parentNode.Equals(Resources.strings.Minions))
            {
                if (!childNode.Equals(Resources.strings.Face_Paint) && !childNode.Equals("Equipment Decals"))
                {
                    List<string[]> list;

                    if (parentNode.Equals(Resources.strings.Pets) || parentNode.Equals(Resources.strings.Mounts) || parentNode.Equals(Resources.strings.Minions))
                    {
                        list = mtrlDict[childNode];
                    }
                    else
                    {
                        list = mtrlDict[raceComboBox.SelectedItem.ToString()];
                    }
                    
                    string[] parts = list[0];
                    int partCount = parts.Count();

                    if (partCount > 1)
                    {
                        for (int i = 0; i < partCount; i++)
                        {
                            partComboBox.Enabled = true;
                            partComboBox.Items.Add(parts[i]);
                        }
                    }
                    else
                    {
                        partComboBox.Items.Add("a");
                        partComboBox.Enabled = false;
                    }

                    if (childNode.Equals(Resources.strings.Face) && list[3].Count() > 1)
                    {
                        extraComboBox.Items.Add(Resources.strings.Face);
                        extraComboBox.Items.Add("Iris");
                        extraComboBox.Items.Add("Etc");

                        extraComboBox.SelectedIndex = 0;
                    }

                    if (childNode.Equals(Resources.strings.Hair) && list[3].Count() > 1)
                    {
                        extraComboBox.Items.Add(Resources.strings.Hair);
                        extraComboBox.Items.Add("Accessory");

                        extraComboBox.SelectedIndex = 0;
                    }

                    partComboBox.SelectedIndex = 0;
                }
            }
            else
            {
                partComboBox.Enabled = false;
                partComboBox.Items.Add("a");
                partComboBox.SelectedIndex = 0;
            }

            if (cb.Focused)
            {
                getTexFiles();            
            }
        }

        private void partComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = (ComboBox)sender;
            if (cb.Focused)
            {
                if (!childNode.Equals(Resources.strings.Face_Paint) && !childNode.Equals("Equipment Decals"))
                {
                    if (childNode.Equals(Resources.strings.Face))
                    {
                        extraComboBox.Items.Clear();

                        List<string[]> list = mtrlDict[raceComboBox.SelectedItem.ToString()];
                        if(int.Parse(list[3][partComboBox.SelectedIndex]) > 1)
                        {
                            extraComboBox.Items.Add(Resources.strings.Face);
                            extraComboBox.Items.Add("Iris");
                            extraComboBox.Items.Add("Etc");
                        }
                        else
                        {
                            extraComboBox.Items.Add(Resources.strings.Face);
                        }

                        extraComboBox.SelectedIndex = 0;
                    }
                    else if (childNode.Equals(Resources.strings.Hair))
                    {
                        extraComboBox.Items.Clear();

                        List<string[]> list = mtrlDict[raceComboBox.SelectedItem.ToString()];
                        if (int.Parse(list[3][partComboBox.SelectedIndex]) > 1)
                        {
                            extraComboBox.Items.Add(Resources.strings.Hair);
                            extraComboBox.Items.Add("Accessory");
                        }
                        else
                        {
                            extraComboBox.Items.Add(Resources.strings.Hair);
                        }

                        extraComboBox.SelectedIndex = 0;
                    }
                    getTexFiles();
                }
                else if(childNode.Equals(Resources.strings.Face_Paint))
                {
                    currTexName = "_decal_" + (partComboBox.SelectedIndex + 1) + ".tex";
                    FindOffset fOffset = new FindOffset(currTexName);
                    fileOffset = fOffset.getFileOffset();
                    int loc = ((int.Parse(fileOffset, NumberStyles.HexNumber) / 8) & 0x000f) / 2;
                    displayTex(fileOffset, loc);
                }
                else
                {
                    currTexName = "-decal_" + partComboBox.SelectedItem.ToString().PadLeft(3, '0') + ".tex";
                    FindOffset fOffset = new FindOffset(currTexName);
                    fileOffset = fOffset.getFileOffset();
                    int loc = ((int.Parse(fileOffset, NumberStyles.HexNumber) / 8) & 0x000f) / 2;
                    displayTex(fileOffset, loc);
                }
            }
        }

        private void mapComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = (ComboBox)sender;
            string map = mapComboBox.SelectedItem.ToString();
            currMap = mapComboBox.SelectedItem.ToString();

            if (cb.Focused)
            {
                if (!textureOffsets[map].Equals(""))
                {
                    fileOffset = textureOffsets[map];

                    currTexName = currTex[mapComboBox.SelectedIndex];

                    int loc = ((int.Parse(fileOffset, NumberStyles.HexNumber) / 8) & 0x000f) / 2;

                    displayTex(fileOffset, loc);
                }
                else
                {
                    currMTRLName = getColorMtrlName();

                    currTexName = currMTRLName;

                    displayTex("ColorSet1", 0);
                }
            }
        }

        private void modListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (File.Exists(Properties.Settings.Default.DefaultDir + "/040000.modlist"))
            {
                ModListForm modList = new ModListForm();
                modList.Show(this);
            }
        }

        private void checkForProblemsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (File.Exists(Properties.Settings.Default.DefaultDir + "/040000.win32.dat3"))
            {
                CheckProblems cp = new CheckProblems();
                if (cp.getProb().Equals("num"))
                {
                    ProblemCheckDialog pcd = new ProblemCheckDialog();
                    pcd.MainText = "Problem Found";
                    pcd.LabelText = "Found an issue where there are modified files in the index, but the index cannot access the dat file, "
                        + "which is known to cause game crashes.\n\nYou may choose to Revert all mods or Fix the index";
                    DialogResult dr = pcd.ShowDialog(this);
                    if (dr == DialogResult.No)
                    {
                        revertAll();
                        pcd.Close();
                    }
                    else if (dr == DialogResult.Yes)
                    {
                        cp.reapplyFix();
                        pcd.Close();
                    }
                }
                else if (cp.getProb().Equals("index2"))
                {
                    string message = "Found a .index2 file in the ffxiv folder, this may prevent modified textures from appearing, and can also cause crashes"
                        + "\n\nPress OK to fix the issue.\nThis will rename the index2 file with extension .bak, but does not affect the game in any way";
                    var result = MessageBox.Show(message, "Problem Found", MessageBoxButtons.OKCancel);

                    if(result == DialogResult.OK)
                    {
                        if(File.Exists(Properties.Settings.Default.DefaultDir + "/040000.win32.index2.bak"))
                        {
                            File.Delete(Properties.Settings.Default.DefaultDir + "/040000.win32.index2.bak");
                        }
                        File.Move(Properties.Settings.Default.DefaultDir + "/040000.win32.index2", Properties.Settings.Default.DefaultDir + "/040000.win32.index2.bak");
                    }
                }
                else if (cp.getProb().Equals("dat"))
                {
                    string message = "The game is only reading it's original dat files, ignoring the modified textures dat file."
                        + "\n\nIf you have modified a texture and the game is crashing this may fix the issue. \n\nPress OK to apply fix.";
                    var result = MessageBox.Show(message, "Problem Found", MessageBoxButtons.OKCancel);

                    if (result == DialogResult.OK)
                    {
                        cp.reapplyFix();
                    }
                }
                else
                {
                    MessageBox.Show("No problems found, if you are experiencing any issues or bugs, please report them under Help > Report Bug.", "No problems found");
                }
            }
            else
            {
                MessageBox.Show("The dat used for mods was not found, loading an item from the list will correct this.", "No Mods found");
            }

        }

        private void extraComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = (ComboBox)sender;

            if (cb.Focused)
            {
                getTexFiles();
            }
        }

        private void englishToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Language = "en";
            Properties.Settings.Default.Save();

            englishToolStripMenuItem.Checked = true;
            japaneseToolStripMenuItem.Checked = false;
            frenchToolStripMenuItem.Checked = false;
            germanToolStripMenuItem.Checked = false;

            CultureInfo ci = new CultureInfo(Properties.Settings.Default.Language);
            CultureInfo.DefaultThreadCurrentUICulture = ci;
            CultureInfo.DefaultThreadCurrentCulture = ci;

            treeView1.Nodes.Clear();

            initDictionaries();

            startFill();
        }

        private void save3DButton_Click(object sender, EventArgs e)
        {
            string part;

            if (parentNode.Equals(Resources.strings.Body))
            {
                part = "top";
            }
            else if (parentNode.Equals(Resources.strings.Head))
            {
                part = "met";
            }
            else if (parentNode.Equals(Resources.strings.Legs))
            {
                part = "dwn";
            }
            else if (parentNode.Equals(Resources.strings.Hands))
            {
                part = "glv";
            }
            else
            {
                part = "sho";
            }



            Read3D r3 = new Read3D(itemID, raceDict[raceComboBox.SelectedItem.ToString()], part, parentNode, childNode);
        }

        private void japaneseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Language = "ja";
            Properties.Settings.Default.Save();

            englishToolStripMenuItem.Checked = false;
            japaneseToolStripMenuItem.Checked = true;
            frenchToolStripMenuItem.Checked = false;
            germanToolStripMenuItem.Checked = false;

            CultureInfo ci = new CultureInfo(Properties.Settings.Default.Language);
            CultureInfo.DefaultThreadCurrentUICulture = ci;
            CultureInfo.DefaultThreadCurrentCulture = ci;

            treeView1.Nodes.Clear();

            initDictionaries();

            startFill();
        }

        private void frenchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Language = "fr";
            Properties.Settings.Default.Save();

            englishToolStripMenuItem.Checked = false;
            japaneseToolStripMenuItem.Checked = false;
            frenchToolStripMenuItem.Checked = true;
            germanToolStripMenuItem.Checked = false;

            CultureInfo ci = new CultureInfo(Properties.Settings.Default.Language);
            CultureInfo.DefaultThreadCurrentUICulture = ci;
            CultureInfo.DefaultThreadCurrentCulture = ci;

            treeView1.Nodes.Clear();

            initDictionaries();

            startFill();
        }

        private void germanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Language = "de";
            Properties.Settings.Default.Save();

            englishToolStripMenuItem.Checked = false;
            japaneseToolStripMenuItem.Checked = false;
            frenchToolStripMenuItem.Checked = false;
            germanToolStripMenuItem.Checked = true;

            CultureInfo ci = new CultureInfo(Properties.Settings.Default.Language);
            CultureInfo.DefaultThreadCurrentUICulture = ci;
            CultureInfo.DefaultThreadCurrentCulture = ci;

            treeView1.Nodes.Clear();

            initDictionaries();

            startFill();
        }

        private void dX11ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.DXVer = "dx11";
            Properties.Settings.Default.Save();

            dX9ToolStripMenuItem.Checked = false;
            dX11ToolStripMenuItem.Checked = true;
        }

        private void dX9ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.DXVer = "dx9";
            Properties.Settings.Default.Save();

            dX9ToolStripMenuItem.Checked = true;
            dX11ToolStripMenuItem.Checked = false;
        }

        private void importerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();

            fd.Filter = "FFXIV ModList (*.modlist)|*.modlist";
            fd.InitialDirectory = Directory.GetCurrentDirectory();

            if(fd.ShowDialog() == DialogResult.OK)
            {
                Importer im = new Importer(fd.FileName);
                im.ShowDialog();
            }

        }

        private void reportBugToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://ffxivtextools.dualwield.net/bug_report.html");
        }

        /// <summary>
        /// Get the texture files for selected item
        /// </summary>
        public void getTexFiles()
        {
            int datLoc = 0;
            FFCRC crc = new FFCRC();
            textureOffsets = new Dictionary<string, string>();
            currTex = new List<string>();

            if (!parentNode.Equals(Resources.strings.Pets) && !childNode.Equals(Resources.strings.Face) && !childNode.Equals(Resources.strings.Hair) && !parentNode.Equals(Resources.strings.Mounts) && 
                !parentNode.Equals(Resources.strings.Minions))
            {
                materialOffset = int.Parse(mtrlDict[raceComboBox.SelectedItem.ToString()][1][partComboBox.SelectedIndex]);
            }
            else if (childNode.Equals(Resources.strings.Face))
            {
                if (extraComboBox.SelectedItem.ToString().Equals(Resources.strings.Face))
                {
                    materialOffset = int.Parse(mtrlDict[raceComboBox.SelectedItem.ToString()][1][partComboBox.SelectedIndex]);
                }
                else if (extraComboBox.SelectedItem.ToString().Equals("Iris"))
                {
                    materialOffset = int.Parse(mtrlDict[raceComboBox.SelectedItem.ToString()][3][partComboBox.SelectedIndex]);
                }
                else if (extraComboBox.SelectedItem.ToString().Equals("Etc"))
                {
                    materialOffset = int.Parse(mtrlDict[raceComboBox.SelectedItem.ToString()][5][partComboBox.SelectedIndex]);
                }
            }
            else if (childNode.Equals(Resources.strings.Hair))
            {
                if (extraComboBox.SelectedItem.ToString().Equals(Resources.strings.Hair))
                {
                    materialOffset = int.Parse(mtrlDict[raceComboBox.SelectedItem.ToString()][1][partComboBox.SelectedIndex]);
                }
                else if (extraComboBox.SelectedItem.ToString().Equals("Accessory"))
                {
                    materialOffset = int.Parse(mtrlDict[raceComboBox.SelectedItem.ToString()][3][partComboBox.SelectedIndex]);
                }
            }
            else if (!parentNode.Equals(Resources.strings.Mounts) && !parentNode.Equals(Resources.strings.Minions))
            {
                materialOffset = int.Parse(mtrlDict[childNode][1][partComboBox.SelectedIndex]);
                if (partComboBox.SelectedItem.ToString().Equals("a"))
                {
                    folderHex = crc.text(summonFolder + "1").PadLeft(8, '0');
                }
                else
                {
                    folderHex = crc.text(summonFolder + partComboBox.SelectedItem.ToString()).PadLeft(8, '0');
                }
            }
            else
            {

                if (mountFolder.Contains("demi"))
                {
                    if (mountFolder.Contains("d0002"))
                    {
                        materialOffset = int.Parse(mtrlDict[childNode][1][0]);
                    }
                    else
                    {
                        if (partComboBox.SelectedItem.ToString().Equals("dwn"))
                        {
                            materialOffset = int.Parse(mtrlDict[childNode][1][0]);
                        }
                        else if (partComboBox.SelectedItem.ToString().Equals("met"))
                        {
                            materialOffset = int.Parse(mtrlDict[childNode][1][1]);
                        }
                        else if (partComboBox.SelectedItem.ToString().Equals("top"))
                        {
                            materialOffset = int.Parse(mtrlDict[childNode][1][2]);
                        }
                        else if (partComboBox.SelectedItem.ToString().Equals("sho"))
                        {
                            materialOffset = int.Parse(mtrlDict[childNode][1][3]);
                        }
                    }

                }
                else
                {
                    if (partComboBox.SelectedItem.ToString().Equals("a"))
                    {
                        materialOffset = int.Parse(mtrlDict[childNode][1][0]);
                    }
                    else
                    {
                        materialOffset = int.Parse(mtrlDict[childNode][1][1]);
                    }
                }


                if (parentNode.Equals(Resources.strings.Minions))
                {
                    folderHex = crc.text(minionFolder);
                }
                else
                {
                    folderHex = crc.text(mountFolder);
                }

            }

            int MTRLDatLoc = ((materialOffset / 8) & 0x000f) / 2;

            ReadMtrl readMtrl = new ReadMtrl(MTRLDatLoc, materialOffset, childNode, raceComboBox.Text);

            textureOffsets = readMtrl.getTextureOffsets();
            currTex = readMtrl.getTextureNames();
            colorMapBytes = readMtrl.getColorMapBytes();

            mapComboBox.Items.Clear();

            foreach (var k in textureOffsets)
            {
                mapComboBox.Items.Add(k.Key);
            }
            mapComboBox.SelectedIndex = 0;

            fileOffset = textureOffsets[mapComboBox.SelectedItem.ToString()];
            datLoc = ((int.Parse(fileOffset, NumberStyles.HexNumber) / 8) & 0x000f) / 2;

            string textureName = currTex[mapComboBox.SelectedIndex];

            if (textureName.Contains("ColorSet1"))
            {
                textureName = getColorMtrlName();

                currMTRLName = textureName;
            }

            currTexName = textureName;

            modlistCheck(textureName);

            displayTex(fileOffset, datLoc);
        }

        /// <summary>
        /// Checks if the selected texture exists in the Modlist
        /// </summary>
        /// <param name="textureName">Name of texture</param>
        public void modlistCheck(string textureName)
        {
            foreach (string line in File.ReadLines(Properties.Settings.Default.DefaultDir + "/040000.modlist"))
            {
                jModEntry modEntry = JsonConvert.DeserializeObject<jModEntry>(line);
                if (modEntry.name.Equals(textureName.Substring(0, textureName.LastIndexOf('.'))))
                {
                    modListEntry = modEntry;
                    inModList = true;
                    break;
                }
                else
                {
                    inModList = false;
                }
            }
        }

        /// <summary>
        /// Get the MTRL name of the ColorSet file
        /// </summary>
        /// <returns></returns>
        public string getColorMtrlName()
        {
            string mtrlName = "";

            if (!parentNode.Equals(Resources.strings.Main_Hand) && !parentNode.Equals(Resources.strings.Off_Hand) && !parentNode.Equals(Resources.strings.Two_Handed))
            {
                if (parentNode.Equals(Resources.strings.Head_Body) || parentNode.Equals(Resources.strings.Body_Hands_Legs_Feet) || parentNode.Equals(Resources.strings.Legs_Feet))
                {
                    mtrlName = "mt_c" + raceDict[raceComboBox.SelectedItem.ToString()] + "e" + itemID.PadLeft(4, '0') + "_" + eSlotDict[parentNode] + "_" + partComboBox.SelectedItem.ToString() + ".mtrl";
                }
                else if (parentNode.Equals(Resources.strings.Ears) || parentNode.Equals(Resources.strings.Neck) || parentNode.Equals(Resources.strings.Wrists) || parentNode.Equals(Resources.strings.Rings))
                {
                    mtrlName = "mt_c" + raceDict[raceComboBox.SelectedItem.ToString()] + "a" + itemID.PadLeft(4, '0') + "_" + eSlotDict[parentNode] + "_a.mtrl";
                }
                else if (parentNode.Equals(Resources.strings.Pets))
                {
                    string sName = summonDict[childNode];
                    if (sName.Contains("a"))
                    {
                        mtrlName = "mt_m" + sName.Substring(0, sName.Length - 1) + "b0002_a.mtrl";
                    }
                    else
                    {
                        mtrlName = "mt_m" + sName + "b0001_a.mtrl";
                    }
                }
                else if (parentNode.Equals(Resources.strings.Mounts))
                {
                    mtrlName = mountMTRL.Substring(0, mountMTRL.LastIndexOf('_') + 1) + partComboBox.SelectedItem.ToString() + ".mtrl";
                }
                else if (parentNode.Equals(Resources.strings.Minions))
                {
                    mtrlName = minionMTRL.Substring(0, minionMTRL.LastIndexOf('_') + 1) + partComboBox.SelectedItem.ToString() + ".mtrl";
                }
                else if (childNode.Equals(Resources.strings.Hair))
                {
                    mtrlName = "mt_c" + raceDict[raceComboBox.SelectedItem.ToString()] + "h" + partComboBox.SelectedItem.ToString().PadLeft(4, '0') + "_hir_a.mtrl";
                }
                else
                {
                    mtrlName = "mt_c" + raceDict[raceComboBox.SelectedItem.ToString()] + "e" + itemID.PadLeft(4, '0') + "_" + eSlotDict[parentNode] + "_a.mtrl";
                }
            }
            else
            {
                mtrlName = "mt_w" + itemID + "b" + weaponBody + "_a.mtrl";
            }

            return mtrlName;
        }


        /// <summary>
        /// Display the texture
        /// </summary>
        /// <param name="offset">texture offset</param>
        /// <param name="datLoc">which .dat the texture is located in</param>
        public void displayTex(string offset, int datLoc)
        {
            IOHelper ioHelper = new IOHelper();

            if (ioHelper.ddsExists(parentNode, childNode, currTexName))
            {
                importButton.Enabled = true;
            }
            else
            {
                importButton.Enabled = false;
            }

            if (!offset.Equals("ColorSet1"))
            {
                string fileLoc = Properties.Settings.Default.DefaultDir + "/040000.win32.dat" + datLoc;
                datNum = datLoc;

                TextureReader tr = new TextureReader(offset, fileLoc);

                displayModlistCheck();

                toolStripStatusLabel2.Text = "Loading File...";
                toolStripProgressBar1.Value = 50;

                decompTexBytes = tr.getDecompressedTexture();
                currTexType = tr.getTextureType();
                int[] dimensions = tr.getDimensions();
                width = dimensions[0];
                height = dimensions[1];

                TextureToBitmap tb = new TextureToBitmap(decompTexBytes, currTexType, dimensions);

                Bitmap bmp = tb.getBitmap();
                string typeString = tb.getTypeString();

                groupBox2.Text = "Image";
                dataGridView1.Visible = false;
                bigInfoBox.Visible = false;
                pictureBox1.Visible = true;
                savePNGButton.Enabled = true;

                if(typeString.Equals("DXT1") || typeString.Equals("DXT3") || typeString.Equals("DXT5") || typeString.Equals("32bit A8R8G8B8") || typeString.Equals("64bit A16R16G16B16"))
                {
                    saveDDSButton.Enabled = true;
                }
                else
                {
                    saveDDSButton.Enabled = false;
                }

                if (pictureBox1.Height < height || pictureBox1.Width < width)
                {
                    pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                    pictureBox1.Image = bmp;
                }
                else
                {
                    pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
                    pictureBox1.Image = bmp;
                }

                //display the texture information
                if (!parentNode.Equals(Resources.strings.Character) && !parentNode.Equals(Resources.strings.Pets) && !parentNode.Equals(Resources.strings.Mounts) && !parentNode.Equals(Resources.strings.Minions))
                {
                    infoLabel.Text = "Dimensions\tTexture Type\tModel\tVersion\r\n(" + height + " x " + width + ")\t     " + typeString + "\t\t " + itemID + "\t " + itemVersion +
                        "\r\n\r\nFull Path\r\n" + folderPath + "/" + currTexName;
                }
                else
                {
                    infoLabel.Text = "Dimensions\tTexture Type\r\n(" + height + " x " + width + ")\t" + typeString;
                }

                toolStripStatusLabel2.Text = "File Loaded";
                toolStripProgressBar1.Value = 100;
            }
            else
            {
                displayModlistCheck();

                currTexType = 9312;
                groupBox2.Text = "Image";
                dataGridView1.Visible = false;
                bigInfoBox.Visible = false;
                pictureBox1.Visible = true;
                savePNGButton.Enabled = true;

                Bitmap bmp = helper.readRGBAFImage(colorMapBytes, 4, 16);
                decompTexBytes = colorMapBytes;
                int width = bmp.Width;
                int height = bmp.Height;


                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                Bitmap bmp1 = new Bitmap(bmp.Width * 100, bmp.Height * 100);
                Graphics graphic = Graphics.FromImage(bmp1);
                graphic.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                graphic.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                graphic.DrawImage(bmp, 0, 0, bmp.Width * 100, bmp.Height * 100);
                pictureBox1.Image = bmp1;
                graphic.Dispose();


                if (!parentNode.Equals(Resources.strings.Character) && !parentNode.Equals(Resources.strings.Pets) && !parentNode.Equals(Resources.strings.Mounts) && !parentNode.Equals(Resources.strings.Minions))
                {

                    infoLabel.Text = "Dimensions\t     Texture Type\t\tModel\tVersion\r\n(" + height + " x " + width + ")\t\t64bit A16B16G16R16F\t " + itemID + "\t " + itemVersion +
                        "\r\n\r\nFull Path\r\n" + folderPath + "/" + currTexName;
                }
                else
                {
                    infoLabel.Text = "Dimensions\t     Texture Type\r\n(" + height + " x " + width + ")\t\t64bit A16B16G16R16F";
                }
            }
        }

        public void displayModlistCheck()
        {
            foreach (string line in File.ReadLines(Properties.Settings.Default.DefaultDir + "/040000.modlist"))
            {
                jModEntry modEntry = JsonConvert.DeserializeObject<jModEntry>(line);
                if (modEntry.name.Equals(currTexName.Substring(0, currTexName.LastIndexOf('.'))))
                {
                    revertButton.Enabled = true;
                    modListEntry = modEntry;
                    inModList = true;
                    break;
                }
                else
                {
                    inModList = false;
                    revertButton.Enabled = false;
                }
            }
        }

        //Save PNG button
        private void button1_Click(object sender, EventArgs e)
        {
            Save save = new Save(parentNode, childNode, (Bitmap)pictureBox1.Image);
        }

        //Save DDS button
        private void button3_Click(object sender, EventArgs e)
        {
            Save save = new Save(parentNode, childNode, currTex, mapComboBox.SelectedIndex, currMTRLName, currTexName, decompTexBytes, width, height, currTexType);
            importButton.Enabled = true;
        }

        //Import Button
        private void button2_Click(object sender, EventArgs e)
        {
            BackgroundWorker rWorker = new BackgroundWorker();
            rWorker.WorkerReportsProgress = true;
            rWorker.WorkerSupportsCancellation = true;
            rWorker.DoWork += new DoWorkEventHandler(replacer_work);
            rWorker.ProgressChanged += new ProgressChangedEventHandler(replacer_ProgressChanged);
            rWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(replacer_RunWorkerCompleted);
            rWorker.RunWorkerAsync();
        }

        //Import dowork
        private void replacer_work(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker rWorker = sender as BackgroundWorker;

            string ddsFile = Properties.Settings.Default.SaveDir + "/" + parentNode + "/" + childNode + "/" + currTexName.Substring(0, currTexName.LastIndexOf('.')) + ".dds";
            Import import = new Import(childNode, currTexName, ddsFile, currTexType, inModList, modListEntry, folderHex, currMap, materialOffset, fileOffset, datNum, rWorker, e);
            newOffset = import.getNewOffset();
            colorMapBytes = import.getColorMapBytes();
            e.Result = import.getImportStatus();
        }

        //import progresschanged
        private void replacer_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            toolStripProgressBar1.Value = e.ProgressPercentage;
            toolStripStatusLabel2.Text = (string)e.UserState;
        }

        //import work complete
        private void replacer_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                toolStripProgressBar1.Value = 0;
                toolStripStatusLabel2.Text = "Error, file is not the same format";
            }
            else if ((bool)e.Result == true)
            {
                revertButton.Enabled = true;
                toolStripStatusLabel2.Text = "Done Importing";
                if (!currMap.Equals("ColorSet1"))
                {
                    TextureReader tr = new TextureReader(newOffset.ToString("X").PadLeft(8, '0'), Properties.Settings.Default.DefaultDir + "/040000.win32.dat3");
                    TextureToBitmap tb = new TextureToBitmap(tr.getDecompressedTexture(), tr.getTextureType(), tr.getDimensions());
                    Bitmap bmp = tb.getBitmap();
                    pictureBox1.Image = bmp;
                }
                else
                {
                    Bitmap bmp = helper.readRGBAFImage(colorMapBytes, 4, 16);

                    int width = bmp.Width;
                    int height = bmp.Height;

                    pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                    Bitmap bmp1 = new Bitmap(bmp.Width * 100, bmp.Height * 100);
                    Graphics graphic = Graphics.FromImage(bmp1);
                    graphic.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                    graphic.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                    graphic.DrawImage(bmp, 0, 0, bmp.Width * 100, bmp.Height * 100);
                    pictureBox1.Image = bmp1;
                    graphic.Dispose();
                }
            }
            else if ((bool)e.Result == false)
            {
                toolStripStatusLabel2.Text = "Import Error";
            }
        }

        //Revert Button
        private void button4_Click(object sender, EventArgs e)
        {
            IOHelper ioHelper = new IOHelper();
            int orig = int.Parse(modListEntry.origOffset, NumberStyles.HexNumber);
            ioHelper.modifyIndexOffset(orig, currTexName, folderHex);
            int datLoc = ((orig / 8) & 0x000F) / 2;

            if (!mapComboBox.SelectedItem.ToString().Equals("ColorSet1"))
            {
                displayTex(orig.ToString("X").PadLeft(8, '0'), datLoc);
            }
            else
            {
                displayRevertedColorMap(datLoc, orig);
            }
        }


        public void displayRevertedColorMap(int datNum, int mOffset){
            IOHelper ioHelper = new IOHelper();

            colorMapBytes = ioHelper.getRevertedColorMap(datNum, mOffset);

            Bitmap bmp = helper.readRGBAFImage(colorMapBytes, 4, 16);

            int width = bmp.Width;
            int height = bmp.Height;


            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            Bitmap bmp1 = new Bitmap(bmp.Width * 100, bmp.Height * 100);
            Graphics graphic = Graphics.FromImage(bmp1);
            graphic.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            graphic.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            graphic.DrawImage(bmp, 0, 0, bmp.Width * 100, bmp.Height * 100);
            pictureBox1.Image = bmp1;
            graphic.Dispose();
        }

        //Options Directory Click
        private void fFXIVLocationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Directories dir = new Directories();
            dir.StartPosition = FormStartPosition.CenterParent;
            dir.ShowDialog();
        }

        //About click
        private void fFXIVViewerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 about = new AboutBox1();
            about.ShowDialog();
        }

        //make 3 dots for resizing list
        private void splitContainer1_Paint(object sender, PaintEventArgs e)
        {
            var control = sender as SplitContainer;
            //paint the three dots'
            Point[] points = new Point[3];
            var w = control.Width;
            var h = control.Height;
            var d = control.SplitterDistance;
            var sW = control.SplitterWidth;

            //calculate the position of the points'
            if (control.Orientation == Orientation.Horizontal)
            {
                points[0] = new Point((w / 2), d + (sW / 2));
                points[1] = new Point(points[0].X - 10, points[0].Y);
                points[2] = new Point(points[0].X + 10, points[0].Y);
            }
            else
            {
                points[0] = new Point(d + (sW / 2), (h / 2));
                points[1] = new Point(points[0].X, points[0].Y - 10);
                points[2] = new Point(points[0].X, points[0].Y + 10);
            }

            foreach (Point p in points)
            {
                p.Offset(-2, -2);
                e.Graphics.FillEllipse(SystemBrushes.ControlDark,
                    new Rectangle(p, new Size(3, 3)));

                p.Offset(1, 1);
                e.Graphics.FillEllipse(SystemBrushes.ControlLight,
                    new Rectangle(p, new Size(3, 3)));
            }
        }

        //Search option clicked
        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Find find = new Find(treeView1);
            find.Show(this);
        }

        //Search Go To button
        public void searchGo(string s)
        {
            TreeNode[] t = treeView1.Nodes.Find(s, true);
            treeView1.SelectedNode = t[0];
            treeView1.SelectedNode.Expand();
        }

        //Revert all option clicked
        private void revertAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result =  MessageBox.Show("Are you sure you want to revert all modded files?", "Revert All", MessageBoxButtons.YesNo);

            if (result == DialogResult.Yes)
            {
                revertAll();
            }
        }

        /// <summary>
        /// Reverts all modifications
        /// </summary>
        public void revertAll()
        {
            IOHelper ioHelper = new IOHelper();
            try
            {
                foreach (string line in File.ReadLines(Properties.Settings.Default.DefaultDir + "/040000.modlist"))
                {
                    jModEntry modEntry = JsonConvert.DeserializeObject<jModEntry>(line);
                    int orig = int.Parse(modEntry.origOffset, NumberStyles.HexNumber);
                    if (modEntry.name.Contains("mt_"))
                    {
                        folderHex = modEntry.folder;
                        ioHelper.modifyIndexOffset(orig, modEntry.name + ".mtrl", folderHex);
                    }
                    else
                    {
                        ioHelper.modifyIndexOffset(orig, modEntry.name + ".tex", "");
                    }

                }

                using (BinaryWriter bw = new BinaryWriter(File.OpenWrite(Properties.Settings.Default.DefaultDir + "/040000.win32.index")))
                {
                    bw.BaseStream.Seek(1104, SeekOrigin.Begin);
                    byte b = 3;
                    bw.Write(b);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not find modlist in ffxiv directory \n\n" + ex.Message, "Revert All Error");
            }
        }

        //Reapply all option clicked
        private void reapplyAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IOHelper ioHelper = new IOHelper();
            DialogResult result = MessageBox.Show("Are you sure you want to reapply all modded files?", "Reapply All", MessageBoxButtons.YesNo);

            if (result == DialogResult.Yes)
            {
                try
                {
                    foreach (string line in File.ReadLines(Properties.Settings.Default.DefaultDir + "/040000.modlist"))
                    {
                        jModEntry modEntry = JsonConvert.DeserializeObject<jModEntry>(line);
                        int orig = int.Parse(modEntry.modOffset, NumberStyles.HexNumber);
                        if (modEntry.name.Contains("mt_"))
                        {
                            folderHex = modEntry.folder;
                            ioHelper.modifyIndexOffset(orig, modEntry.name + ".mtrl", folderHex);
                        }
                        else
                        {
                            ioHelper.modifyIndexOffset(orig, modEntry.name + ".tex", "");
                        }
                    }

                    using (BinaryWriter bw = new BinaryWriter(File.OpenWrite(Properties.Settings.Default.DefaultDir + "/040000.win32.index")))
                    {
                        bw.BaseStream.Seek(1104, SeekOrigin.Begin);
                        byte b = 4;
                        bw.Write(b);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Could not find modlist in ffxiv directory \n\n" + ex.Message, "Reapply All Error");
                }

            }
        }
    }
}