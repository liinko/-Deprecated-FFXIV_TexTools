namespace FFXIV_TexTools
{
    partial class Directories
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Directories));
            this.FFXIVDir = new System.Windows.Forms.Label();
            this.SaveDir = new System.Windows.Forms.Label();
            this.Loc = new System.Windows.Forms.Button();
            this.Loc1 = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.ConfirmButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // FFXIVDir
            // 
            this.FFXIVDir.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.FFXIVDir.AutoSize = true;
            this.FFXIVDir.BackColor = System.Drawing.SystemColors.Window;
            this.FFXIVDir.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.FFXIVDir.Location = new System.Drawing.Point(3, 28);
            this.FFXIVDir.Name = "FFXIVDir";
            this.FFXIVDir.Size = new System.Drawing.Size(2, 15);
            this.FFXIVDir.TabIndex = 0;
            // 
            // SaveDir
            // 
            this.SaveDir.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.SaveDir.AutoSize = true;
            this.SaveDir.BackColor = System.Drawing.SystemColors.Window;
            this.SaveDir.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.SaveDir.Location = new System.Drawing.Point(3, 80);
            this.SaveDir.Name = "SaveDir";
            this.SaveDir.Size = new System.Drawing.Size(2, 15);
            this.SaveDir.TabIndex = 1;
            // 
            // Loc
            // 
            this.Loc.Location = new System.Drawing.Point(266, 23);
            this.Loc.Name = "Loc";
            this.Loc.Size = new System.Drawing.Size(24, 23);
            this.Loc.TabIndex = 3;
            this.Loc.Text = "...";
            this.Loc.UseVisualStyleBackColor = true;
            this.Loc.Click += new System.EventHandler(this.Loc_Click);
            // 
            // Loc1
            // 
            this.Loc1.Location = new System.Drawing.Point(266, 75);
            this.Loc1.Name = "Loc1";
            this.Loc1.Size = new System.Drawing.Size(24, 23);
            this.Loc1.TabIndex = 5;
            this.Loc1.Text = "...";
            this.Loc1.UseVisualStyleBackColor = true;
            this.Loc1.Click += new System.EventHandler(this.Loc1_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 88.5906F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 11.4094F));
            this.tableLayoutPanel1.Controls.Add(this.ConfirmButton, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.SaveDir, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.Loc1, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.FFXIVDir, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.Loc, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 2);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(9, 8);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(298, 140);
            this.tableLayoutPanel1.TabIndex = 7;
            // 
            // ConfirmButton
            // 
            this.ConfirmButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.ConfirmButton.Location = new System.Drawing.Point(94, 114);
            this.ConfirmButton.Name = "ConfirmButton";
            this.ConfirmButton.Size = new System.Drawing.Size(75, 23);
            this.ConfirmButton.TabIndex = 6;
            this.ConfirmButton.Text = "OK";
            this.ConfirmButton.UseVisualStyleBackColor = true;
            this.ConfirmButton.Click += new System.EventHandler(this.ConfirmButton_Click);
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(94, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "FFXIV Data Folder";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 55);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Save Folder";
            // 
            // Directories
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(315, 158);
            this.ControlBox = false;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Directories";
            this.Text = "Directories";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label FFXIVDir;
        private System.Windows.Forms.Label SaveDir;
        private System.Windows.Forms.Button Loc;
        private System.Windows.Forms.Button Loc1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button ConfirmButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}