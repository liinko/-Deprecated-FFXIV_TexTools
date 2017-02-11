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
using System.IO;
using System.Windows.Forms;
using FFXIV_TexTools.Helpers;
using Newtonsoft.Json;

namespace FFXIV_TexTools.IO
{
    class Import
    {
        bool importStatus;
        int newOffset;
        byte[] colorMapBytes;

        /// <summary>
        /// Imports a DDS file
        /// </summary>
        /// <param name="childNode">Name of item</param>
        /// <param name="currTexName">Name of texture</param>
        /// <param name="ddsFile">DDS file directory</param>
        /// <param name="filetype">DDS file type</param>
        /// <param name="inModList">Included in Mod List boolean</param>
        /// <param name="modListEntry">Modlist entry</param>
        /// <param name="mFolderHex">Folder CRC</param>
        /// <param name="currMap">Currently selected texture map</param>
        /// <param name="MTRLOffset">Offset of MTRL file</param>
        /// <param name="fileHex">File CRC</param>
        /// <param name="datLoc">Dat number file is located in</param>
        /// <param name="worker">BackgroundWorker</param>
        /// <param name="e">BackgroundWorker event args</param>
        public Import(string childNode, string currTexName, string ddsFile, int filetype, bool inModList, jModEntry modListEntry, string mFolderHex, string currMap, int MTRLOffset, string fileHex, int datLoc, BackgroundWorker worker, DoWorkEventArgs e)
        {
            IOHelper ioHelper = new IOHelper();
            importStatus = false;
            uint typecheck;

            try
            {
                using (BinaryReader b1 = new BinaryReader(File.OpenRead(ddsFile)))
                {
                    b1.BaseStream.Seek(84, SeekOrigin.Begin);
                    typecheck = b1.ReadUInt32();
                }

                if (typecheck == 827611204)
                {
                    typecheck = 13344;
                }
                else if (typecheck == 894720068)
                {
                    typecheck = 13361;
                }
                else if (typecheck == 861165636)
                {
                    typecheck = 13360;
                }
                else if (typecheck == 113)
                {
                    typecheck = 9312;
                }
                else if (typecheck == 0)
                {
                    typecheck = 5200;
                }

                if (typecheck == filetype)
                {
                    if (inModList)
                    {
                        string moddedOffset = modListEntry.modOffset;
                        DatWriter dw = new DatWriter(inModList, currMap, ddsFile, modListEntry, moddedOffset, MTRLOffset, datLoc);
                        newOffset = dw.getNewDatOffset() + 48;
                        colorMapBytes = dw.getColorMapBytes();
                        ioHelper.modifyIndexOffset(newOffset, currTexName, mFolderHex);
                        importStatus = true;
                    }
                    else
                    {
                        DatWriter dw = new DatWriter(inModList, currMap, ddsFile, modListEntry, fileHex, MTRLOffset, datLoc);
                        newOffset = dw.getNewDatOffset() + 48;
                        colorMapBytes = dw.getColorMapBytes();

                        jModEntry newEntry;
                        if (currMap.Equals("ColorSet1"))
                        {
                            newEntry = new jModEntry(childNode, currTexName.Substring(0, currTexName.LastIndexOf('.')), MTRLOffset.ToString("X").PadLeft(8, '0'), newOffset.ToString("X").PadLeft(8, '0'), mFolderHex);
                        }
                        else
                        {
                            newEntry = new jModEntry(childNode, currTexName.Substring(0, currTexName.LastIndexOf('.')), fileHex, newOffset.ToString("X").PadLeft(8, '0'), "");
                        }

                        string nEntry = JsonConvert.SerializeObject(newEntry);

                        using (StreamWriter file = new StreamWriter(Properties.Settings.Default.DefaultDir + "/040000.modlist", true))
                        {
                            file.BaseStream.Seek(0, SeekOrigin.End);
                            file.WriteLine(nEntry);
                        }
                        ioHelper.modifyIndexOffset(newOffset, currTexName, mFolderHex);
                        importStatus = true;
                    }
                }
                else
                {
                    worker.CancelAsync();
                    e.Cancel = true;
                }
            }
            catch (Exception ex)
            {
                importStatus = false;
                MessageBox.Show("Cannot import while game is running. \n\n" + ex.Message, "Import Error");
            }
        }

        /// <summary>
        /// Imports shared mods
        /// </summary>
        /// <param name="importDatDirectory">Directory being imported from</param>
        /// <param name="modEntries">The mod entries</param>
        public Import(string importDatDirectory, jModEntry[] modEntries)
        {
            IOHelper ioHelper = new IOHelper();
            List<byte> modBytes = new List<byte>();

            string[] currentMods = File.ReadAllLines(Properties.Settings.Default.DefaultDir + "/040000.modlist");

            using (BinaryReader br = new BinaryReader(File.OpenRead(importDatDirectory)))
            {
                using (BinaryWriter bw = new BinaryWriter(File.OpenWrite(Properties.Settings.Default.DefaultDir + "/040000.win32.dat3")))
                {
                    for(int i = 0; i < modEntries.Length; i++)
                    {
                        modBytes.Clear();
                        int size;
                        bool inModList = false; 

                        try
                        {
                            size = int.Parse(modEntries[i + 1].modOffset, System.Globalization.NumberStyles.HexNumber) - int.Parse(modEntries[i].modOffset, System.Globalization.NumberStyles.HexNumber);
                        }
                        catch
                        {
                            size = (int)br.BaseStream.Length - int.Parse(modEntries[i].modOffset, System.Globalization.NumberStyles.HexNumber);
                        }

                        br.BaseStream.Seek(int.Parse(modEntries[i].modOffset, System.Globalization.NumberStyles.HexNumber), SeekOrigin.Begin);

                        bw.BaseStream.Seek(0, SeekOrigin.End);

                        foreach (string line in currentMods)
                        {
                            jModEntry modEntry = JsonConvert.DeserializeObject<jModEntry>(line);

                            if (modEntries[i].name.Equals(modEntry.name)){
                                bw.BaseStream.Seek(int.Parse(modEntry.modOffset, System.Globalization.NumberStyles.HexNumber) - 48, SeekOrigin.Begin);
                                inModList = true;
                                break;
                            }
                        }

                        modBytes.AddRange(br.ReadBytes(size));

                        newOffset = (int)bw.BaseStream.Position;

                        if (!inModList)
                        {
                            int lastPos = (int)bw.BaseStream.Position + size;
                            byte[] padto = new byte[16];

                            while ((lastPos & 0xff) != 0)
                            {
                                modBytes.AddRange(padto);
                                lastPos = lastPos + 16;
                            }

                            newOffset = (int)bw.BaseStream.Position + 48;
                        }

                        bw.Write(modBytes.ToArray());

                        string name;
                        if (!modEntries[i].name.Contains("mt_"))
                        {
                            name = modEntries[i].name + ".tex";
                        }
                        else
                        {
                            name = modEntries[i].name + ".mtrl";
                        }

                        if (!inModList)
                        {
                            modEntries[i].modOffset = newOffset.ToString("X").PadLeft(8, '0');
                            string nEntry = JsonConvert.SerializeObject(modEntries[i]);

                            using (StreamWriter file = new StreamWriter(Properties.Settings.Default.DefaultDir + "/040000.modlist", true))
                            {
                                file.BaseStream.Seek(0, SeekOrigin.End);
                                file.WriteLine(nEntry);
                            }
                        }


                        if (!inModList)
                        {
                            ioHelper.modifyIndexOffset(newOffset, name, modEntries[i].folder);
                        }
                        else
                        {
                            ioHelper.modifyIndexOffset(newOffset + 48, name, modEntries[i].folder);
                        }
                    }
                }
            }
        }

        public bool getImportStatus()
        {
            return importStatus;
        }

        public int getNewOffset()
        {
            return newOffset;
        }

        public byte[] getColorMapBytes()
        {
            return colorMapBytes;
        }
    }
}
