namespace FFXIV_TexTools.UI
{
    partial class ProblemCheckDialog
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
            this.problemLabel = new System.Windows.Forms.Label();
            this.problemRevert = new System.Windows.Forms.Button();
            this.problemFix = new System.Windows.Forms.Button();
            this.problemCancel = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // problemLabel
            // 
            this.problemLabel.Location = new System.Drawing.Point(13, 13);
            this.problemLabel.Name = "problemLabel";
            this.problemLabel.Size = new System.Drawing.Size(329, 80);
            this.problemLabel.TabIndex = 0;
            this.problemLabel.Text = "Problem";
            // 
            // problemRevert
            // 
            this.problemRevert.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.problemRevert.Location = new System.Drawing.Point(127, 3);
            this.problemRevert.Name = "problemRevert";
            this.problemRevert.Size = new System.Drawing.Size(75, 23);
            this.problemRevert.TabIndex = 2;
            this.problemRevert.TabStop = false;
            this.problemRevert.Text = "Revert";
            this.problemRevert.UseVisualStyleBackColor = true;
            this.problemRevert.Click += new System.EventHandler(this.problemRevert_Click);
            // 
            // problemFix
            // 
            this.problemFix.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.problemFix.Location = new System.Drawing.Point(237, 3);
            this.problemFix.Name = "problemFix";
            this.problemFix.Size = new System.Drawing.Size(75, 23);
            this.problemFix.TabIndex = 3;
            this.problemFix.TabStop = false;
            this.problemFix.Text = "Fix";
            this.problemFix.UseVisualStyleBackColor = true;
            this.problemFix.Click += new System.EventHandler(this.problemFix_Click);
            // 
            // problemCancel
            // 
            this.problemCancel.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.problemCancel.Location = new System.Drawing.Point(17, 3);
            this.problemCancel.Name = "problemCancel";
            this.problemCancel.Size = new System.Drawing.Size(75, 23);
            this.problemCancel.TabIndex = 1;
            this.problemCancel.TabStop = false;
            this.problemCancel.Text = "Cancel";
            this.problemCancel.UseVisualStyleBackColor = true;
            this.problemCancel.Click += new System.EventHandler(this.problemCancel_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.Controls.Add(this.problemCancel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.problemFix, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.problemRevert, 1, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 96);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(330, 30);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // ProblemCheckDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(354, 138);
            this.ControlBox = false;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.problemLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "ProblemCheckDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Form2";
            this.TopMost = true;
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label problemLabel;
        private System.Windows.Forms.Button problemRevert;
        private System.Windows.Forms.Button problemFix;
        private System.Windows.Forms.Button problemCancel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}