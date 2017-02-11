namespace FFXIV_TexTools
{
    partial class ProgressDialog
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
            this.loadingProgress = new System.Windows.Forms.ProgressBar();
            this.loadingText = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // loadingProgress
            // 
            this.loadingProgress.Location = new System.Drawing.Point(12, 30);
            this.loadingProgress.MarqueeAnimationSpeed = 50;
            this.loadingProgress.Name = "loadingProgress";
            this.loadingProgress.Size = new System.Drawing.Size(376, 23);
            this.loadingProgress.Step = 1;
            this.loadingProgress.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.loadingProgress.TabIndex = 0;
            // 
            // loadingText
            // 
            this.loadingText.AutoSize = true;
            this.loadingText.Location = new System.Drawing.Point(13, 11);
            this.loadingText.Name = "loadingText";
            this.loadingText.Size = new System.Drawing.Size(57, 13);
            this.loadingText.TabIndex = 1;
            this.loadingText.Text = "Loading....";
            // 
            // ProgressDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ClientSize = new System.Drawing.Size(400, 65);
            this.ControlBox = false;
            this.Controls.Add(this.loadingText);
            this.Controls.Add(this.loadingProgress);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProgressDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Loading....";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar loadingProgress;
        private System.Windows.Forms.Label loadingText;
    }
}