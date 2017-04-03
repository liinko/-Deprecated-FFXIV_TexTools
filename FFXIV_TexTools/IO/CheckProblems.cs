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

using System.IO;
using FFXIV_TexTools.Helpers;
using Newtonsoft.Json;

namespace FFXIV_TexTools.IO
{
    /// <summary>
    /// Class to find and correct problems that may arise when modifying textures
    /// 
    /// checks for existance of index2
    /// checks if mod is showing active but index is not reflecting change
    /// checks for number of dat files
    /// 
    /// </summary>
    class CheckProblems
    {
        string indexDir = Properties.Settings.Default.DefaultDir + "/040000.win32.index";
        string prob = "None";
        bool isActive = false;
        FFCRC crc = new FFCRC();

        public CheckProblems()
        {
            int check; 

            using (BinaryReader br = new BinaryReader(File.OpenRead(indexDir)))
            {
                br.BaseStream.Seek(1104, SeekOrigin.Begin);

                check = br.ReadInt16();
            }

            bool index2Exists = File.Exists(Properties.Settings.Default.DefaultDir + "/040000.win32.index2");
            if (index2Exists)
            {
                prob = "index2";
            }
            else if (check != 4)
            {
                prob = "dat";
            }
            else if (!index2Exists)
            {
                foreach (string line in File.ReadLines(Properties.Settings.Default.DefaultDir + "/040000.modlist"))
                {
                    jModEntry modEntry = JsonConvert.DeserializeObject<jModEntry>(line);

                    IOHelper ioHelper = new IOHelper();
                    if (!modEntry.name.Contains("mt_"))
                    {
                        isActive = ioHelper.isActive(crc.text(modEntry.name + ".tex"), modEntry.folder, modEntry.modOffset);
                    }
                    else
                    {
                        isActive = ioHelper.isActive(crc.text(modEntry.name + ".mtrl"), modEntry.folder, modEntry.modOffset);
                    }

                    if(isActive == true)
                    {
                        prob = "num";
                        break;
                    }
                }
            }
            else
            {
                prob = "None";
            }
        }

        public void reapplyFix()
        {
            using (BinaryWriter bw = new BinaryWriter(File.OpenWrite(indexDir)))
            {
                bw.BaseStream.Seek(1104, SeekOrigin.Begin);
                byte b = 4;
                bw.Write(b);
            }
        }

        public string getProb()
        {
            return prob;
        }
    }
}
