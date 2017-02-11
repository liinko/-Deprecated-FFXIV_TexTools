namespace FFXIV_TexTools.UI
{
    partial class ModListForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.modCloseButton = new System.Windows.Forms.Button();
            this.modGoToButton = new System.Windows.Forms.Button();
            this.listView1 = new System.Windows.Forms.ListView();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.switchButton = new System.Windows.Forms.Button();
            this.modExportButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // modCloseButton
            // 
            this.modCloseButton.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.modCloseButton.Location = new System.Drawing.Point(344, 3);
            this.modCloseButton.Name = "modCloseButton";
            this.modCloseButton.Size = new System.Drawing.Size(75, 23);
            this.modCloseButton.TabIndex = 3;
            this.modCloseButton.Text = "Close";
            this.modCloseButton.UseVisualStyleBackColor = true;
            this.modCloseButton.Click += new System.EventHandler(this.modCloseButton_Click);
            // 
            // modGoToButton
            // 
            this.modGoToButton.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.modGoToButton.Enabled = false;
            this.modGoToButton.Location = new System.Drawing.Point(128, 3);
            this.modGoToButton.Name = "modGoToButton";
            this.modGoToButton.Size = new System.Drawing.Size(75, 23);
            this.modGoToButton.TabIndex = 2;
            this.modGoToButton.Text = "Go To";
            this.modGoToButton.UseVisualStyleBackColor = true;
            this.modGoToButton.Click += new System.EventHandler(this.modGoToButton_Click);
            // 
            // listView1
            // 
            this.listView1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listView1.CheckBoxes = true;
            this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView1.FullRowSelect = true;
            this.listView1.GridLines = true;
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(3, 3);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(437, 232);
            this.listView1.TabIndex = 4;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.listView1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 85.82375F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.17624F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(443, 278);
            this.tableLayoutPanel1.TabIndex = 5;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 4;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 112F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel2.Controls.Add(this.modGoToButton, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.modCloseButton, 3, 0);
            this.tableLayoutPanel2.Controls.Add(this.switchButton, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.modExportButton, 0, 0);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 241);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(436, 34);
            this.tableLayoutPanel2.TabIndex = 5;
            // 
            // switchButton
            // 
            this.switchButton.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.switchButton.Enabled = false;
            this.switchButton.Location = new System.Drawing.Point(229, 3);
            this.switchButton.Name = "switchButton";
            this.switchButton.Size = new System.Drawing.Size(90, 23);
            this.switchButton.TabIndex = 4;
            this.switchButton.Text = "Enable/Disable";
            this.switchButton.UseVisualStyleBackColor = true;
            this.switchButton.Click += new System.EventHandler(this.switchButton_Click);
            // 
            // modExportButton
            // 
            this.modExportButton.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.modExportButton.Location = new System.Drawing.Point(18, 3);
            this.modExportButton.Name = "modExportButton";
            this.modExportButton.Size = new System.Drawing.Size(75, 23);
            this.modExportButton.TabIndex = 5;
            this.modExportButton.Text = "Export";
            this.modExportButton.UseVisualStyleBackColor = true;
            this.modExportButton.Click += new System.EventHandler(this.modExportButton_Click);
            // 
            // ModListForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(443, 278);
            this.ControlBox = false;
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ModListForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ModList";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button modCloseButton;
        private System.Windows.Forms.Button modGoToButton;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button switchButton;
        private System.Windows.Forms.Button modExportButton;
    }
}