namespace KinectAET
{
    partial class MainWindow
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.tabControlVideoSources = new System.Windows.Forms.TabControl();
            this.tabPageCapture = new System.Windows.Forms.TabPage();
            this.labelCaptureSubfolderContent = new System.Windows.Forms.Label();
            this.labelCaptureSubfolder = new System.Windows.Forms.Label();
            this.buttonStartCapturing = new System.Windows.Forms.Button();
            this.tabPageExport = new System.Windows.Forms.TabPage();
            this.buttonEvaluation = new System.Windows.Forms.Button();
            this.textBoxExport = new System.Windows.Forms.TextBox();
            this.buttonExport = new System.Windows.Forms.Button();
            this.buttonResetMarkers = new System.Windows.Forms.Button();
            this.buttonMarkFrame = new System.Windows.Forms.Button();
            this.trackBarVideo = new MediaSlider.MediaSlider();
            this.buttonPlay = new System.Windows.Forms.Button();
            this.listBoxAvailableTakes = new System.Windows.Forms.ListBox();
            this.buttonDisplayShortcuts = new System.Windows.Forms.Button();
            this.labelProjectDirectory = new System.Windows.Forms.Label();
            this.buttonChangeProjectDirectory = new System.Windows.Forms.Button();
            this.textBoxProjectDirectory = new System.Windows.Forms.TextBox();
            this.folderBrowserDialogProjectDirectory = new System.Windows.Forms.FolderBrowserDialog();
            this.fileSystemWatcherProjectDir = new System.IO.FileSystemWatcher();
            this.tabControlVideoSources.SuspendLayout();
            this.tabPageCapture.SuspendLayout();
            this.tabPageExport.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcherProjectDir)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControlVideoSources
            // 
            this.tabControlVideoSources.Controls.Add(this.tabPageCapture);
            this.tabControlVideoSources.Controls.Add(this.tabPageExport);
            this.tabControlVideoSources.Location = new System.Drawing.Point(13, 493);
            this.tabControlVideoSources.Name = "tabControlVideoSources";
            this.tabControlVideoSources.SelectedIndex = 0;
            this.tabControlVideoSources.Size = new System.Drawing.Size(674, 201);
            this.tabControlVideoSources.TabIndex = 1;
            this.tabControlVideoSources.Selecting += new System.Windows.Forms.TabControlCancelEventHandler(this.tabControlVideoSources_Selecting);
            // 
            // tabPageCapture
            // 
            this.tabPageCapture.Controls.Add(this.labelCaptureSubfolderContent);
            this.tabPageCapture.Controls.Add(this.labelCaptureSubfolder);
            this.tabPageCapture.Controls.Add(this.buttonStartCapturing);
            this.tabPageCapture.Location = new System.Drawing.Point(4, 22);
            this.tabPageCapture.Name = "tabPageCapture";
            this.tabPageCapture.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageCapture.Size = new System.Drawing.Size(666, 175);
            this.tabPageCapture.TabIndex = 0;
            this.tabPageCapture.Text = "Capture";
            this.tabPageCapture.UseVisualStyleBackColor = true;
            // 
            // labelCaptureSubfolderContent
            // 
            this.labelCaptureSubfolderContent.AutoSize = true;
            this.labelCaptureSubfolderContent.Location = new System.Drawing.Point(131, 37);
            this.labelCaptureSubfolderContent.Name = "labelCaptureSubfolderContent";
            this.labelCaptureSubfolderContent.Size = new System.Drawing.Size(70, 13);
            this.labelCaptureSubfolderContent.TabIndex = 2;
            this.labelCaptureSubfolderContent.Text = "Not Available";
            // 
            // labelCaptureSubfolder
            // 
            this.labelCaptureSubfolder.AutoSize = true;
            this.labelCaptureSubfolder.Location = new System.Drawing.Point(7, 37);
            this.labelCaptureSubfolder.Name = "labelCaptureSubfolder";
            this.labelCaptureSubfolder.Size = new System.Drawing.Size(116, 13);
            this.labelCaptureSubfolder.TabIndex = 1;
            this.labelCaptureSubfolder.Text = "Last Capture subfolder:";
            // 
            // buttonStartCapturing
            // 
            this.buttonStartCapturing.Location = new System.Drawing.Point(7, 7);
            this.buttonStartCapturing.Name = "buttonStartCapturing";
            this.buttonStartCapturing.Size = new System.Drawing.Size(213, 23);
            this.buttonStartCapturing.TabIndex = 0;
            this.buttonStartCapturing.Text = "Start Capturing";
            this.buttonStartCapturing.UseVisualStyleBackColor = true;
            this.buttonStartCapturing.Click += new System.EventHandler(this.buttonStartCapturing_Click);
            // 
            // tabPageExport
            // 
            this.tabPageExport.Controls.Add(this.buttonEvaluation);
            this.tabPageExport.Controls.Add(this.textBoxExport);
            this.tabPageExport.Controls.Add(this.buttonExport);
            this.tabPageExport.Controls.Add(this.buttonResetMarkers);
            this.tabPageExport.Controls.Add(this.buttonMarkFrame);
            this.tabPageExport.Controls.Add(this.trackBarVideo);
            this.tabPageExport.Controls.Add(this.buttonPlay);
            this.tabPageExport.Controls.Add(this.listBoxAvailableTakes);
            this.tabPageExport.Location = new System.Drawing.Point(4, 22);
            this.tabPageExport.Name = "tabPageExport";
            this.tabPageExport.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageExport.Size = new System.Drawing.Size(666, 175);
            this.tabPageExport.TabIndex = 1;
            this.tabPageExport.Text = "Export and Evaluate";
            this.tabPageExport.UseVisualStyleBackColor = true;
            // 
            // buttonEvaluation
            // 
            this.buttonEvaluation.Enabled = false;
            this.buttonEvaluation.Location = new System.Drawing.Point(279, 117);
            this.buttonEvaluation.Name = "buttonEvaluation";
            this.buttonEvaluation.Size = new System.Drawing.Size(159, 23);
            this.buttonEvaluation.TabIndex = 8;
            this.buttonEvaluation.Text = "Show Evaluation Data";
            this.buttonEvaluation.UseVisualStyleBackColor = true;
            this.buttonEvaluation.Click += new System.EventHandler(this.buttonEvaluation_Click);
            // 
            // textBoxExport
            // 
            this.textBoxExport.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxExport.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxExport.Location = new System.Drawing.Point(6, 122);
            this.textBoxExport.Name = "textBoxExport";
            this.textBoxExport.ReadOnly = true;
            this.textBoxExport.Size = new System.Drawing.Size(267, 13);
            this.textBoxExport.TabIndex = 7;
            // 
            // buttonExport
            // 
            this.buttonExport.Enabled = false;
            this.buttonExport.Location = new System.Drawing.Point(279, 144);
            this.buttonExport.Name = "buttonExport";
            this.buttonExport.Size = new System.Drawing.Size(159, 23);
            this.buttonExport.TabIndex = 6;
            this.buttonExport.Text = "Export";
            this.buttonExport.UseVisualStyleBackColor = true;
            this.buttonExport.Click += new System.EventHandler(this.buttonExport_Click);
            // 
            // buttonResetMarkers
            // 
            this.buttonResetMarkers.Enabled = false;
            this.buttonResetMarkers.Location = new System.Drawing.Point(151, 144);
            this.buttonResetMarkers.Name = "buttonResetMarkers";
            this.buttonResetMarkers.Size = new System.Drawing.Size(122, 23);
            this.buttonResetMarkers.TabIndex = 5;
            this.buttonResetMarkers.Text = "Reset Markers";
            this.buttonResetMarkers.UseVisualStyleBackColor = true;
            this.buttonResetMarkers.Click += new System.EventHandler(this.buttonResetMarkers_Click);
            // 
            // buttonMarkFrame
            // 
            this.buttonMarkFrame.Enabled = false;
            this.buttonMarkFrame.Location = new System.Drawing.Point(6, 144);
            this.buttonMarkFrame.Name = "buttonMarkFrame";
            this.buttonMarkFrame.Size = new System.Drawing.Size(139, 23);
            this.buttonMarkFrame.TabIndex = 4;
            this.buttonMarkFrame.Text = "Mark Frame";
            this.buttonMarkFrame.UseVisualStyleBackColor = true;
            this.buttonMarkFrame.Click += new System.EventHandler(this.buttonMarkFrame_Click);
            // 
            // trackBarVideo
            // 
            this.trackBarVideo.Animated = false;
            this.trackBarVideo.AnimationSize = 0.2F;
            this.trackBarVideo.AnimationSpeed = MediaSlider.MediaSlider.AnimateSpeed.Normal;
            this.trackBarVideo.AutoScrollMargin = new System.Drawing.Size(0, 0);
            this.trackBarVideo.AutoScrollMinSize = new System.Drawing.Size(0, 0);
            this.trackBarVideo.BackColor = System.Drawing.Color.White;
            this.trackBarVideo.BackgroundImage = null;
            this.trackBarVideo.ButtonAccentColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.trackBarVideo.ButtonBorderColor = System.Drawing.Color.Black;
            this.trackBarVideo.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(160)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.trackBarVideo.ButtonCornerRadius = ((uint)(2u));
            this.trackBarVideo.ButtonSize = new System.Drawing.Size(24, 12);
            this.trackBarVideo.ButtonStyle = MediaSlider.MediaSlider.ButtonType.GlassInline;
            this.trackBarVideo.ContextMenuStrip = null;
            this.trackBarVideo.Enabled = false;
            this.trackBarVideo.FirstHighlightedFrame = -1;
            this.trackBarVideo.FlyOutLetterNumber = -1;
            this.trackBarVideo.HighlightColor = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(101)))), ((int)(((byte)(188)))));
            this.trackBarVideo.InitializeAlways = true;
            this.trackBarVideo.LargeChange = 30;
            this.trackBarVideo.LastHighlightedFrame = -1;
            this.trackBarVideo.Location = new System.Drawing.Point(4, 7);
            this.trackBarVideo.Margin = new System.Windows.Forms.Padding(0);
            this.trackBarVideo.Maximum = 10;
            this.trackBarVideo.Minimum = 0;
            this.trackBarVideo.Name = "trackBarVideo";
            this.trackBarVideo.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.trackBarVideo.ShowButtonOnHover = false;
            this.trackBarVideo.ShowHighlight = true;
            this.trackBarVideo.Size = new System.Drawing.Size(656, 49);
            this.trackBarVideo.SliderFlyOut = MediaSlider.MediaSlider.FlyOutStyle.Persistant;
            this.trackBarVideo.SmallChange = 1;
            this.trackBarVideo.SmoothScrolling = false;
            this.trackBarVideo.TabIndex = 3;
            this.trackBarVideo.TickColor = System.Drawing.Color.DarkGray;
            this.trackBarVideo.TickStyle = System.Windows.Forms.TickStyle.BottomRight;
            this.trackBarVideo.TickType = MediaSlider.MediaSlider.TickMode.LargeStepped;
            this.trackBarVideo.TrackBorderColor = System.Drawing.Color.DimGray;
            this.trackBarVideo.TrackDepth = 5;
            this.trackBarVideo.TrackFillColor = System.Drawing.Color.Transparent;
            this.trackBarVideo.TrackProgressColor = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(101)))), ((int)(((byte)(188)))));
            this.trackBarVideo.TrackShadow = true;
            this.trackBarVideo.TrackShadowColor = System.Drawing.Color.Transparent;
            this.trackBarVideo.TrackStyle = MediaSlider.MediaSlider.TrackType.Value;
            this.trackBarVideo.Value = 0;
            this.trackBarVideo.ValueChanged += new MediaSlider.MediaSlider.ValueChangedDelegate(this.trackBarVideo_ValueChanged);
            this.trackBarVideo.MouseDown += new System.Windows.Forms.MouseEventHandler(this.trackBarVideo_MouseDown);
            this.trackBarVideo.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.trackBarVideo_KeyDown);
            // 
            // buttonPlay
            // 
            this.buttonPlay.Enabled = false;
            this.buttonPlay.Location = new System.Drawing.Point(6, 59);
            this.buttonPlay.Name = "buttonPlay";
            this.buttonPlay.Size = new System.Drawing.Size(432, 23);
            this.buttonPlay.TabIndex = 2;
            this.buttonPlay.Text = "Play";
            this.buttonPlay.UseVisualStyleBackColor = true;
            this.buttonPlay.Click += new System.EventHandler(this.buttonPlay_Click);
            // 
            // listBoxAvailableTakes
            // 
            this.listBoxAvailableTakes.FormattingEnabled = true;
            this.listBoxAvailableTakes.Location = new System.Drawing.Point(466, 59);
            this.listBoxAvailableTakes.Name = "listBoxAvailableTakes";
            this.listBoxAvailableTakes.Size = new System.Drawing.Size(194, 108);
            this.listBoxAvailableTakes.TabIndex = 1;
            this.listBoxAvailableTakes.SelectedIndexChanged += new System.EventHandler(this.listBoxAvailableTakes_SelectedIndexChanged);
            // 
            // buttonDisplayShortcuts
            // 
            this.buttonDisplayShortcuts.Location = new System.Drawing.Point(483, 700);
            this.buttonDisplayShortcuts.Name = "buttonDisplayShortcuts";
            this.buttonDisplayShortcuts.Size = new System.Drawing.Size(204, 23);
            this.buttonDisplayShortcuts.TabIndex = 2;
            this.buttonDisplayShortcuts.Text = "Key Shortcuts & About";
            this.buttonDisplayShortcuts.UseMnemonic = false;
            this.buttonDisplayShortcuts.UseVisualStyleBackColor = true;
            this.buttonDisplayShortcuts.Click += new System.EventHandler(this.buttonDisplayShortcuts_Click);
            // 
            // labelProjectDirectory
            // 
            this.labelProjectDirectory.AutoSize = true;
            this.labelProjectDirectory.Location = new System.Drawing.Point(13, 704);
            this.labelProjectDirectory.Name = "labelProjectDirectory";
            this.labelProjectDirectory.Size = new System.Drawing.Size(88, 13);
            this.labelProjectDirectory.TabIndex = 3;
            this.labelProjectDirectory.Text = "Project Directory:";
            // 
            // buttonChangeProjectDirectory
            // 
            this.buttonChangeProjectDirectory.Location = new System.Drawing.Point(380, 700);
            this.buttonChangeProjectDirectory.Name = "buttonChangeProjectDirectory";
            this.buttonChangeProjectDirectory.Size = new System.Drawing.Size(75, 23);
            this.buttonChangeProjectDirectory.TabIndex = 4;
            this.buttonChangeProjectDirectory.Text = "Change";
            this.buttonChangeProjectDirectory.UseVisualStyleBackColor = true;
            this.buttonChangeProjectDirectory.Click += new System.EventHandler(this.buttonChangeProjectDirectory_Click);
            // 
            // textBoxProjectDirectory
            // 
            this.textBoxProjectDirectory.Location = new System.Drawing.Point(107, 701);
            this.textBoxProjectDirectory.Name = "textBoxProjectDirectory";
            this.textBoxProjectDirectory.ReadOnly = true;
            this.textBoxProjectDirectory.Size = new System.Drawing.Size(267, 20);
            this.textBoxProjectDirectory.TabIndex = 5;
            // 
            // folderBrowserDialogProjectDirectory
            // 
            this.folderBrowserDialogProjectDirectory.Description = "Select the project directory. It contains subfolders with the data for each captu" +
                "re. Select an empty folder to start recording new data.";
            this.folderBrowserDialogProjectDirectory.RootFolder = System.Environment.SpecialFolder.MyComputer;
            // 
            // fileSystemWatcherProjectDir
            // 
            this.fileSystemWatcherProjectDir.EnableRaisingEvents = true;
            this.fileSystemWatcherProjectDir.SynchronizingObject = this;
            // 
            // MainWindow
            // 
            this.AcceptButton = this.buttonStartCapturing;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(705, 730);
            this.Controls.Add(this.textBoxProjectDirectory);
            this.Controls.Add(this.buttonChangeProjectDirectory);
            this.Controls.Add(this.labelProjectDirectory);
            this.Controls.Add(this.buttonDisplayShortcuts);
            this.Controls.Add(this.tabControlVideoSources);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "MainWindow";
            this.Text = "Kinect Annotation and Evaluation Tool";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainWindow_FormClosing);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.MainWindow_KeyUp);
            this.tabControlVideoSources.ResumeLayout(false);
            this.tabPageCapture.ResumeLayout(false);
            this.tabPageCapture.PerformLayout();
            this.tabPageExport.ResumeLayout(false);
            this.tabPageExport.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcherProjectDir)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControlVideoSources;
        private System.Windows.Forms.TabPage tabPageCapture;
        private System.Windows.Forms.TabPage tabPageExport;
        private System.Windows.Forms.Button buttonDisplayShortcuts;
        private System.Windows.Forms.Label labelProjectDirectory;
        private System.Windows.Forms.Button buttonChangeProjectDirectory;
        private System.Windows.Forms.TextBox textBoxProjectDirectory;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialogProjectDirectory;
        private System.Windows.Forms.Button buttonStartCapturing;
        private System.Windows.Forms.Label labelCaptureSubfolderContent;
        private System.Windows.Forms.Label labelCaptureSubfolder;
        private System.Windows.Forms.ListBox listBoxAvailableTakes;
        private System.IO.FileSystemWatcher fileSystemWatcherProjectDir;
        private System.Windows.Forms.Button buttonPlay;
        private MediaSlider.MediaSlider trackBarVideo;
        private System.Windows.Forms.Button buttonMarkFrame;
        private System.Windows.Forms.Button buttonResetMarkers;
        private System.Windows.Forms.Button buttonExport;
        private System.Windows.Forms.TextBox textBoxExport;
        private System.Windows.Forms.Button buttonEvaluation;

    }
}

