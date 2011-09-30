namespace KinectAET
{
    partial class ExportConfiguration
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
            this.checkedListBoxUserSelection = new System.Windows.Forms.CheckedListBox();
            this.buttonStartExport = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.labelUserSelection = new System.Windows.Forms.Label();
            this.labelFramesSelected = new System.Windows.Forms.Label();
            this.checkBoxExportBackground = new System.Windows.Forms.CheckBox();
            this.checkBoxDrawSkeleton = new System.Windows.Forms.CheckBox();
            this.checkBoxDrawHighlight = new System.Windows.Forms.CheckBox();
            this.groupBoxImageExport = new System.Windows.Forms.GroupBox();
            this.checkBoxUserLabel = new System.Windows.Forms.CheckBox();
            this.radioButtonDepth = new System.Windows.Forms.RadioButton();
            this.radioButtonImage = new System.Windows.Forms.RadioButton();
            this.groupBoxDimension = new System.Windows.Forms.GroupBox();
            this.checkBoxRelativePath = new System.Windows.Forms.CheckBox();
            this.buttonBatchExport = new System.Windows.Forms.Button();
            this.checkBox2D = new System.Windows.Forms.CheckBox();
            this.checkBox3D = new System.Windows.Forms.CheckBox();
            this.groupBoxImageExport.SuspendLayout();
            this.groupBoxDimension.SuspendLayout();
            this.SuspendLayout();
            // 
            // checkedListBoxUserSelection
            // 
            this.checkedListBoxUserSelection.CheckOnClick = true;
            this.checkedListBoxUserSelection.FormattingEnabled = true;
            this.checkedListBoxUserSelection.Location = new System.Drawing.Point(17, 59);
            this.checkedListBoxUserSelection.Name = "checkedListBoxUserSelection";
            this.checkedListBoxUserSelection.Size = new System.Drawing.Size(216, 199);
            this.checkedListBoxUserSelection.TabIndex = 0;
            this.checkedListBoxUserSelection.ThreeDCheckBoxes = true;
            // 
            // buttonStartExport
            // 
            this.buttonStartExport.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonStartExport.Location = new System.Drawing.Point(131, 278);
            this.buttonStartExport.Name = "buttonStartExport";
            this.buttonStartExport.Size = new System.Drawing.Size(102, 23);
            this.buttonStartExport.TabIndex = 1;
            this.buttonStartExport.Text = "Start Export";
            this.buttonStartExport.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(263, 278);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(102, 23);
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // labelUserSelection
            // 
            this.labelUserSelection.AutoSize = true;
            this.labelUserSelection.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelUserSelection.Location = new System.Drawing.Point(12, 43);
            this.labelUserSelection.Name = "labelUserSelection";
            this.labelUserSelection.Size = new System.Drawing.Size(169, 13);
            this.labelUserSelection.TabIndex = 3;
            this.labelUserSelection.Text = "Select User Annotations to Export:";
            // 
            // labelFramesSelected
            // 
            this.labelFramesSelected.AutoSize = true;
            this.labelFramesSelected.Location = new System.Drawing.Point(12, 11);
            this.labelFramesSelected.Name = "labelFramesSelected";
            this.labelFramesSelected.Size = new System.Drawing.Size(0, 13);
            this.labelFramesSelected.TabIndex = 4;
            // 
            // checkBoxExportBackground
            // 
            this.checkBoxExportBackground.AutoSize = true;
            this.checkBoxExportBackground.Checked = true;
            this.checkBoxExportBackground.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxExportBackground.Location = new System.Drawing.Point(6, 58);
            this.checkBoxExportBackground.Name = "checkBoxExportBackground";
            this.checkBoxExportBackground.Size = new System.Drawing.Size(112, 17);
            this.checkBoxExportBackground.TabIndex = 9;
            this.checkBoxExportBackground.Text = "Draw Background";
            this.checkBoxExportBackground.UseVisualStyleBackColor = true;
            // 
            // checkBoxDrawSkeleton
            // 
            this.checkBoxDrawSkeleton.AutoSize = true;
            this.checkBoxDrawSkeleton.Location = new System.Drawing.Point(6, 81);
            this.checkBoxDrawSkeleton.Name = "checkBoxDrawSkeleton";
            this.checkBoxDrawSkeleton.Size = new System.Drawing.Size(93, 17);
            this.checkBoxDrawSkeleton.TabIndex = 10;
            this.checkBoxDrawSkeleton.Text = "DrawSkeleton";
            this.checkBoxDrawSkeleton.UseVisualStyleBackColor = true;
            // 
            // checkBoxDrawHighlight
            // 
            this.checkBoxDrawHighlight.AutoSize = true;
            this.checkBoxDrawHighlight.Location = new System.Drawing.Point(6, 105);
            this.checkBoxDrawHighlight.Name = "checkBoxDrawHighlight";
            this.checkBoxDrawHighlight.Size = new System.Drawing.Size(120, 17);
            this.checkBoxDrawHighlight.TabIndex = 11;
            this.checkBoxDrawHighlight.Text = "Draw User Highlight";
            this.checkBoxDrawHighlight.UseVisualStyleBackColor = true;
            // 
            // groupBoxImageExport
            // 
            this.groupBoxImageExport.Controls.Add(this.checkBoxUserLabel);
            this.groupBoxImageExport.Controls.Add(this.radioButtonDepth);
            this.groupBoxImageExport.Controls.Add(this.radioButtonImage);
            this.groupBoxImageExport.Controls.Add(this.checkBoxDrawSkeleton);
            this.groupBoxImageExport.Controls.Add(this.checkBoxExportBackground);
            this.groupBoxImageExport.Controls.Add(this.checkBoxDrawHighlight);
            this.groupBoxImageExport.Location = new System.Drawing.Point(263, 109);
            this.groupBoxImageExport.Name = "groupBoxImageExport";
            this.groupBoxImageExport.Size = new System.Drawing.Size(200, 149);
            this.groupBoxImageExport.TabIndex = 12;
            this.groupBoxImageExport.TabStop = false;
            this.groupBoxImageExport.Text = "Configure the Image Export";
            // 
            // checkBoxUserLabel
            // 
            this.checkBoxUserLabel.AutoSize = true;
            this.checkBoxUserLabel.Location = new System.Drawing.Point(6, 128);
            this.checkBoxUserLabel.Name = "checkBoxUserLabel";
            this.checkBoxUserLabel.Size = new System.Drawing.Size(110, 17);
            this.checkBoxUserLabel.TabIndex = 14;
            this.checkBoxUserLabel.Text = "Draw User Labels";
            this.checkBoxUserLabel.UseVisualStyleBackColor = true;
            // 
            // radioButtonDepth
            // 
            this.radioButtonDepth.AutoSize = true;
            this.radioButtonDepth.Location = new System.Drawing.Point(100, 20);
            this.radioButtonDepth.Name = "radioButtonDepth";
            this.radioButtonDepth.Size = new System.Drawing.Size(90, 17);
            this.radioButtonDepth.TabIndex = 13;
            this.radioButtonDepth.Text = "Depth Sensor";
            this.radioButtonDepth.UseVisualStyleBackColor = true;
            // 
            // radioButtonImage
            // 
            this.radioButtonImage.AutoSize = true;
            this.radioButtonImage.Checked = true;
            this.radioButtonImage.Location = new System.Drawing.Point(7, 20);
            this.radioButtonImage.Name = "radioButtonImage";
            this.radioButtonImage.Size = new System.Drawing.Size(87, 17);
            this.radioButtonImage.TabIndex = 12;
            this.radioButtonImage.TabStop = true;
            this.radioButtonImage.Text = "ImageSensor";
            this.radioButtonImage.UseVisualStyleBackColor = true;
            // 
            // groupBoxDimension
            // 
            this.groupBoxDimension.Controls.Add(this.checkBox3D);
            this.groupBoxDimension.Controls.Add(this.checkBox2D);
            this.groupBoxDimension.Controls.Add(this.checkBoxRelativePath);
            this.groupBoxDimension.Location = new System.Drawing.Point(263, 43);
            this.groupBoxDimension.Name = "groupBoxDimension";
            this.groupBoxDimension.Size = new System.Drawing.Size(200, 44);
            this.groupBoxDimension.TabIndex = 13;
            this.groupBoxDimension.TabStop = false;
            this.groupBoxDimension.Text = "Select an Annotation Data Format";
            // 
            // checkBoxRelativePath
            // 
            this.checkBoxRelativePath.AutoSize = true;
            this.checkBoxRelativePath.Checked = true;
            this.checkBoxRelativePath.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxRelativePath.Location = new System.Drawing.Point(107, 20);
            this.checkBoxRelativePath.Name = "checkBoxRelativePath";
            this.checkBoxRelativePath.Size = new System.Drawing.Size(90, 17);
            this.checkBoxRelativePath.TabIndex = 8;
            this.checkBoxRelativePath.Text = "Relative Path";
            this.checkBoxRelativePath.UseVisualStyleBackColor = true;
            // 
            // buttonBatchExport
            // 
            this.buttonBatchExport.Location = new System.Drawing.Point(17, 278);
            this.buttonBatchExport.Name = "buttonBatchExport";
            this.buttonBatchExport.Size = new System.Drawing.Size(108, 23);
            this.buttonBatchExport.TabIndex = 14;
            this.buttonBatchExport.Text = "Batch Export";
            this.buttonBatchExport.UseVisualStyleBackColor = true;
            this.buttonBatchExport.Click += new System.EventHandler(this.buttonBatchExport_Click);
            // 
            // checkBox2D
            // 
            this.checkBox2D.AutoSize = true;
            this.checkBox2D.Location = new System.Drawing.Point(7, 20);
            this.checkBox2D.Name = "checkBox2D";
            this.checkBox2D.Size = new System.Drawing.Size(40, 17);
            this.checkBox2D.TabIndex = 9;
            this.checkBox2D.Text = "2D";
            this.checkBox2D.UseVisualStyleBackColor = true;
            // 
            // checkBox3D
            // 
            this.checkBox3D.AutoSize = true;
            this.checkBox3D.Location = new System.Drawing.Point(53, 20);
            this.checkBox3D.Name = "checkBox3D";
            this.checkBox3D.Size = new System.Drawing.Size(40, 17);
            this.checkBox3D.TabIndex = 10;
            this.checkBox3D.Text = "3D";
            this.checkBox3D.UseVisualStyleBackColor = true;
            // 
            // ExportConfiguration
            // 
            this.AcceptButton = this.buttonStartExport;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(486, 313);
            this.Controls.Add(this.buttonBatchExport);
            this.Controls.Add(this.groupBoxDimension);
            this.Controls.Add(this.groupBoxImageExport);
            this.Controls.Add(this.labelFramesSelected);
            this.Controls.Add(this.labelUserSelection);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonStartExport);
            this.Controls.Add(this.checkedListBoxUserSelection);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ExportConfiguration";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Configure Export";
            this.groupBoxImageExport.ResumeLayout(false);
            this.groupBoxImageExport.PerformLayout();
            this.groupBoxDimension.ResumeLayout(false);
            this.groupBoxDimension.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckedListBox checkedListBoxUserSelection;
        private System.Windows.Forms.Button buttonStartExport;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Label labelUserSelection;
        private System.Windows.Forms.Label labelFramesSelected;
        private System.Windows.Forms.CheckBox checkBoxExportBackground;
        private System.Windows.Forms.CheckBox checkBoxDrawSkeleton;
        private System.Windows.Forms.CheckBox checkBoxDrawHighlight;
        private System.Windows.Forms.GroupBox groupBoxImageExport;
        private System.Windows.Forms.RadioButton radioButtonDepth;
        private System.Windows.Forms.RadioButton radioButtonImage;
        private System.Windows.Forms.GroupBox groupBoxDimension;
        private System.Windows.Forms.CheckBox checkBoxUserLabel;
        private System.Windows.Forms.CheckBox checkBoxRelativePath;
        private System.Windows.Forms.Button buttonBatchExport;
        private System.Windows.Forms.CheckBox checkBox3D;
        private System.Windows.Forms.CheckBox checkBox2D;
    }
}