namespace FFXIV_TexTools.UI
{
    partial class Updates
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.goWebButton = new System.Windows.Forms.Button();
            this.upCancelButton = new System.Windows.Forms.Button();
            this.versionTextBox = new System.Windows.Forms.RichTextBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.versionTextBox, 0, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 88.1579F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.8421F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(359, 304);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.goWebButton, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.upCancelButton, 1, 0);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 271);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(353, 30);
            this.tableLayoutPanel2.TabIndex = 1;
            // 
            // goWebButton
            // 
            this.goWebButton.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.goWebButton.Location = new System.Drawing.Point(45, 3);
            this.goWebButton.Name = "goWebButton";
            this.goWebButton.Size = new System.Drawing.Size(86, 23);
            this.goWebButton.TabIndex = 0;
            this.goWebButton.Text = "Visit Website";
            this.goWebButton.UseVisualStyleBackColor = true;
            this.goWebButton.Click += new System.EventHandler(this.goWebButton_Click);
            // 
            // upCancelButton
            // 
            this.upCancelButton.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.upCancelButton.Location = new System.Drawing.Point(227, 3);
            this.upCancelButton.Name = "upCancelButton";
            this.upCancelButton.Size = new System.Drawing.Size(75, 23);
            this.upCancelButton.TabIndex = 1;
            this.upCancelButton.Text = "Cancel";
            this.upCancelButton.UseVisualStyleBackColor = true;
            this.upCancelButton.Click += new System.EventHandler(this.upCancelButton_Click);
            // 
            // versionTextBox
            // 
            this.versionTextBox.Location = new System.Drawing.Point(3, 3);
            this.versionTextBox.Name = "versionTextBox";
            this.versionTextBox.ReadOnly = true;
            this.versionTextBox.Size = new System.Drawing.Size(353, 262);
            this.versionTextBox.TabIndex = 2;
            this.versionTextBox.Text = "";
            // 
            // Updates
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(383, 328);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Updates";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "New Version Available!";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button goWebButton;
        private System.Windows.Forms.Button upCancelButton;
        private System.Windows.Forms.RichTextBox versionTextBox;
    }
}