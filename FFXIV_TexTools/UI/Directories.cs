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
using System.Windows.Forms;

namespace FFXIV_TexTools
{
    public partial class Directories : Form
    {
        /// <summary>
        /// Directories dialog
        /// </summary>
        public Directories()
        {
            InitializeComponent();
            FFXIVDir.Text = Properties.Settings.Default.DefaultDir;
            SaveDir.Text = Properties.Settings.Default.SaveDir;
        }

        private void ConfirmButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Loc_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fDialog = new FolderBrowserDialog();
            var d = fDialog.ShowDialog();
            if (d == DialogResult.OK)
            {
                Properties.Settings.Default.DefaultDir = fDialog.SelectedPath;
                FFXIVDir.Text = fDialog.SelectedPath;
                Properties.Settings.Default.Save();  
            }
          
        }

        private void Loc1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fDialog = new FolderBrowserDialog();
            var d = fDialog.ShowDialog();
            if (d == DialogResult.OK)
            {
                Properties.Settings.Default.SaveDir = fDialog.SelectedPath;
                SaveDir.Text = Properties.Settings.Default.SaveDir;
                Properties.Settings.Default.Save();     
            }
 
        }

        private void Loc2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fDialog = new FolderBrowserDialog();
            var d = fDialog.ShowDialog();
            if (d == DialogResult.OK)
            {
                Properties.Settings.Default.BackupDir = fDialog.SelectedPath;
                Properties.Settings.Default.Save();
            }

        }
    }
}
