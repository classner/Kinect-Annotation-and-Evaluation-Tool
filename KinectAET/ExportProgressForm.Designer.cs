namespace KinectAET
{
    partial class FormExportProgress
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
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.buttonCancelExport = new System.Windows.Forms.Button();
            this.textBoxProgress = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(13, 13);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(399, 23);
            this.progressBar.TabIndex = 0;
            // 
            // buttonCancelExport
            // 
            this.buttonCancelExport.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancelExport.Location = new System.Drawing.Point(134, 84);
            this.buttonCancelExport.Name = "buttonCancelExport";
            this.buttonCancelExport.Size = new System.Drawing.Size(140, 23);
            this.buttonCancelExport.TabIndex = 1;
            this.buttonCancelExport.Text = "Cancel Export";
            this.buttonCancelExport.UseVisualStyleBackColor = true;
            // 
            // textBoxProgress
            // 
            this.textBoxProgress.BackColor = System.Drawing.SystemColors.Control;
            this.textBoxProgress.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxProgress.Enabled = false;
            this.textBoxProgress.Location = new System.Drawing.Point(13, 42);
            this.textBoxProgress.Name = "textBoxProgress";
            this.textBoxProgress.ReadOnly = true;
            this.textBoxProgress.Size = new System.Drawing.Size(399, 13);
            this.textBoxProgress.TabIndex = 3;
            this.textBoxProgress.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // FormExportProgress
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancelExport;
            this.ClientSize = new System.Drawing.Size(424, 119);
            this.ControlBox = false;
            this.Controls.Add(this.textBoxProgress);
            this.Controls.Add(this.buttonCancelExport);
            this.Controls.Add(this.progressBar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormExportProgress";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Exporting...";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.ProgressBar progressBar;
        internal System.Windows.Forms.TextBox textBoxProgress;
        internal System.Windows.Forms.Button buttonCancelExport;
    }
}