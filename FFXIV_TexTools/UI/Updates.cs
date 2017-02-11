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

namespace FFXIV_TexTools.UI
{
    public partial class Updates : Form
    {

        /// <summary>
        /// Checks for updates to application
        /// </summary>
        public Updates()
        {
            InitializeComponent();
        }

        public string Message
        {
            set { versionTextBox.Text = value; }
            get { return versionTextBox.Text; }
        }

        private void goWebButton_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://ffxivtextools.dualwield.net");
        }

        private void upCancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
