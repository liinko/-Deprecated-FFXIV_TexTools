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
    public partial class Find : Form
    {
        TreeView tv;
        TreeNode foundNode;

        /// <summary>
        /// Search
        /// </summary>
        /// <param name="tv">Treeview</param>
        public Find(TreeView tv)
        {
            this.tv = tv;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;
            listBox1.Items.Clear();

            var tn = tv.Nodes;
            foreach(TreeNode node in tn){
                foreach (TreeNode cNode in node.Nodes)
                {
                    if (cNode.Text.ToLower().Contains(textBox1.Text.ToLower()))
                    {
                        listBox1.Items.Add(cNode.Text);
                    }
                }
            }
            listBox1.Sorted = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string s = (string)listBox1.SelectedItem;

            if(Application.OpenForms["Form1"] != null)
            {
                (Application.OpenForms["Form1"] as Form1).searchGo(s);
            }
            else
            {
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
            {
                button2.Enabled = true;
            }
        }
    }
}
