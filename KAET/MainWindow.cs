/*
 * Copyright 2011 Christoph Lassner.
 * Christoph.Lassner@googlemail.com
 * All rights reserved.
 * 
 * This file is part of the Kinect Annotation and Evaluation Tool.
 * 
 * The Kinect Annotation and Evaluation Tool is free software: 
 * you can redistribute it and/or modify it under the terms of
 * the GNU General Public License as published by the Free Software
 * Foundation, either version 3 of the License, or (at your option)
 * any later version.
 * 
 * The Kinect Annotation and Evaluation Tool is distributed
 * in the hope that it will be useful, but WITHOUT ANY WARRANTY;
 * without even the implied warranty of MERCHANTABILITY or
 * FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public
 * License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with the Kinect Annotation and Evaluation Tool.
 * If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Windows.Forms;
using System.Xml.Serialization;
using KAET.Properties;

namespace KAET
{
    public partial class MainWindow : Form
    {
        #region Variables
        #region Readonly flags
        private static readonly String BUTTON_START_CAPTURING_START_TEXT = "Start Capturing";
        private static readonly String BUTTON_START_CAPTURING_STOP_TEXT = "Stop Capturing";
        private static readonly String BUTTON_PLAY_PLAY_TEXT = "Play";
        private static readonly String BUTTON_PLAY_PAUSE_TEXT = "Pause";
        public static readonly String EXPORT_DIR_PREFIX = "Export-";
        #endregion

        
        private readonly OpenNILiveStreamController kinect;
        private readonly OpenNIImageDisplay videoDisplay;

        private readonly XmlSerializer movementDataSerializer =
                                    new XmlSerializer(typeof(ImageDictionary));

        private readonly FormExportProgress exportForm = new FormExportProgress();
        //private readonly StatisticsDisplayForm statisticsForm = new StatisticsDisplayForm();

        private string projectDir;
        /// <summary>
        /// The current project directory. Updating will also reflect in the
        /// text box displaying the project directory.
        /// </summary>
        private String projectDirectory
        {
            get { return projectDir;  }
            set {
                projectDir = value;
                textBoxProjectDirectory.Text = value;
            }
        }

        private readonly Evaluation evaluationForm;
        #endregion

        #region Constructor
        public MainWindow()
        {
            // Initialize the main controls
            InitializeComponent();

            // Add and initilize the image display
            videoDisplay = new OpenNIImageDisplay();
            Controls.Add(videoDisplay);
            videoDisplay.Size = new Size(640, 480);
            videoDisplay.Location = new Point(30, 7);

            try
            {
                kinect = new OpenNILiveStreamController();
                kinect.StartedRecording += kinect_StartedRecording;
                kinect.StoppedRecording += kinect_StoppedRecording;
            }
            catch (Exception)
            {
                kinect = null;
            }

            // Initialize tab pages and events
            tabControlVideoSources.SelectedTab = tabPageExport;

            // Initialize project directory
            // Try to load from settings
            if (Settings.Default.ProjectDirectory != null &&
                Settings.Default.ProjectDirectory != "")
            {
                projectDirectory = Settings.Default.ProjectDirectory;
            }
            else
            {
                projectDirectory =
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }

            // Set evaluation form
            evaluationForm = new Evaluation(videoDisplay);
            evaluationForm.VisibleChanged += new EventHandler(evaluationForm_VisibleChanged);
            buttonEvaluation.Text = BUTTON_EVALUATION_SHOW_TEXT;
            evaluationForm.EvaluationRangeChanged += new Action<int, int>(evaluationForm_EvaluationRangeChanged);

            // Initialize File system watcher
            fileSystemWatcherProjectDir.Path = projectDirectory;

            // Initialize Event system.
            fileSystemWatcherProjectDir.Changed += fileSystemWatcherProjectDir_Changed;
            videoDisplay.ErrorOccured += videoImageControl_ErrorOccured;
            tabControlVideoSources.SelectedIndexChanged += tabControlVideoSources_SelectedIndexChanged;

            // - for the TrackBar
            videoDisplay.SourceChanged += trackBarVideoImageSourceChanged;
            videoDisplay.NewRecordingFrameDisplayed += trackBarVideoUpdateValue;

            // - for the PlayButton
            videoDisplay.SourceChanged += buttonPlayImageSourceChanged;
            videoDisplay.StartedPlaying += buttonPlayStartedPlayingHandler;
            videoDisplay.StoppedPlaying += buttonPlayStoppedPlayingHandler;

            // - for the Mark Frame button
            videoDisplay.SourceChanged += buttonMarkFrameImageSourceChanged;
            evaluationForm.VisibleChanged += buttonMarkFrameEvaluationFormVisibleChanged;

            // - for the evaluation button
            videoDisplay.SourceChanged += buttonEvaluationUpdateEnabledHandler;
            
            // - for the export and reset button
            trackBarVideo.MarkerValueChanged += buttonExportUpdateEnabledHandler;
            evaluationForm.VisibleChanged += buttonExportUpdateEnabledHandler;
            trackBarVideo.MarkerValueChanged += buttonResetMarkersUpdateEnabledHandler;
            evaluationForm.VisibleChanged += buttonResetMarkersUpdateEnabledHandler;

            // Initialize project dir display
            updateListBoxProjectDir();

            // Initialize the export form
            exportForm.buttonCancelExport.Click += cancelCurrentExport;
        }
        #endregion

        #region Event system
        #region Events concerning the entire form
        /// <summary>
        /// Check wether the kinect was initialized properly and cancel the
        /// action if necessary.
        /// </summary>
        private void tabControlVideoSources_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (e.Action == TabControlAction.Selecting &&
                e.TabPage == tabPageCapture &&
                kinect == null)
            {
                e.Cancel = true;
                showException("The Kinect could not be initialized for capture.");
            }
        }
        private void videoImageControl_ErrorOccured(object arg1, string arg2)
        {
            showException("An OpenNI error occured: \n" + arg2);
        }

        private void listBoxAvailableTakes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (videoDisplay.Source != null)
            {
                videoDisplay.Source.Shutdown();
                videoDisplay.Source = null;
            }

            if (listBoxAvailableTakes.SelectedItem != null)
            {
                // Read new directory
                String subDir = Path.Combine(projectDirectory,
                             (String)listBoxAvailableTakes.SelectedItem);
                String oniFileName = Path.Combine(subDir, OpenNIImageProvider.ONI_FILE_NAME);
                String userAnnotationFileName = Path.Combine(subDir, OpenNIImageProvider.USER_ANNOTATION_FILENAME);
                String backgroundUserMapFileName = Path.Combine(subDir, OpenNIImageProvider.MAP_FILE_NAME);
                try
                {
                    videoDisplay.Source = new OpenNIRecordingController(oniFileName,
                        backgroundUserMapFileName, userAnnotationFileName);
                    ((OpenNIRecordingController)videoDisplay.Source).RequestUpdate(true);
                }
                catch (Exception ex)
                {
                    showException(ex.Message);
                    listBoxAvailableTakes.ClearSelected();
                }
            }
        }

        private void tabControlVideoSources_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControlVideoSources.SelectedTab == tabPageCapture)
            {
                listBoxAvailableTakes.ClearSelected();
                videoDisplay.Source = kinect;
                kinect.StartGenerating();
                AcceptButton = buttonStartCapturing;
            }
            else
            {
                kinect.StopGenerating();
                videoDisplay.Source = null;
                AcceptButton = buttonPlay;
            }
        }

        void evaluationForm_EvaluationRangeChanged(int arg1, int arg2)
        {
            trackBarVideo.FirstHighlightedFrame = arg1;
            trackBarVideo.LastHighlightedFrame = arg2;
        }

        #region System Events
        /// <summary>
        /// Handle the file system event.
        /// </summary>
        private void fileSystemWatcherProjectDir_Changed(object sender, FileSystemEventArgs e)
        {
            if(listBoxAvailableTakes.Items.Contains(e.Name) &&
                e.ChangeType != WatcherChangeTypes.Deleted &&
                e.ChangeType != WatcherChangeTypes.Renamed)
                return;

            updateListBoxProjectDir();
        }

        /// <summary>
        /// Handle key events.
        /// </summary>
        private void MainWindow_KeyUp(object sender, KeyEventArgs e)
        {
            if (tabControlVideoSources.SelectedTab == tabPageCapture &&
                            kinect != null)
            {
                switch (e.KeyData)
                {
                    case Keys.D:
                        kinect.ImageOrDepth = false;
                        break;
                    case Keys.I:
                        kinect.ImageOrDepth = true;
                        break;
                    case Keys.L:
                        kinect.DrawUserInformation = !kinect.DrawUserInformation;
                        break;
                    case Keys.S:
                        kinect.DrawSkeletonMesh = !kinect.DrawSkeletonMesh;
                        break;
                    case Keys.B:
                        kinect.DrawBackground = !kinect.DrawBackground;
                        break;
                    case Keys.A:
                        kinect.DrawSensorData = !kinect.DrawSensorData;
                        break;
                    case Keys.H:
                        kinect.DrawUserHighlight = !kinect.DrawUserHighlight;
                        break;
                }
            }
            OpenNIRecordingController playback = videoDisplay.Source as OpenNIRecordingController;
            if (tabControlVideoSources.SelectedTab == tabPageExport &&
                playback != null)
            {
                switch (e.KeyData)
                {
                    case Keys.D:
                        playback.ImageOrDepth = false;
                        playback.RequestUpdate(true);
                        e.SuppressKeyPress = true;
                        break;
                    case Keys.I:
                        playback.ImageOrDepth = true;
                        playback.RequestUpdate(true);
                        e.SuppressKeyPress = true;
                        break;
                    case Keys.S:
                        playback.DrawSkeletonMesh = !playback.DrawSkeletonMesh;
                        playback.RequestUpdate(true);
                        e.SuppressKeyPress = true;
                        //e.Handled = true;
                        break;
                    case Keys.B:
                        playback.DrawBackground = !playback.DrawBackground;
                        playback.RequestUpdate(true);
                        e.SuppressKeyPress = true;
                        break;
                    case Keys.H:
                        playback.DrawUserHighlight = !playback.DrawUserHighlight;
                        playback.RequestUpdate(true);
                        e.SuppressKeyPress = true;
                        break;
                    case Keys.L:
                        playback.DrawUserInformation = !playback.DrawUserInformation;
                        playback.RequestUpdate(true);
                        e.SuppressKeyPress = true;
                        break;
                    case Keys.A:
                        playback.DrawSensorData = !playback.DrawSensorData;
                        playback.RequestUpdate(true);
                        e.SuppressKeyPress = true;
                        break;
                    case Keys.M:
                        buttonMarkFrame.PerformClick();
                        e.SuppressKeyPress = true;
                        break;
                    case Keys.E:
                        buttonExport.PerformClick();
                        e.SuppressKeyPress = true;
                        break;
                    case Keys.R:
                        buttonResetMarkers.PerformClick();
                        e.SuppressKeyPress = true;
                        break;
                    case Keys.P:
                        buttonPlay.PerformClick();
                        e.SuppressKeyPress = true;
                        break;
                    case Keys.Left:
                    case Keys.Right:
                    case Keys.PageDown:
                    case Keys.PageUp:
                        if(! trackBarVideo.Focused)
                            trackBarVideo.Focus();
                        break;
                }
            }
        }

        /// <summary>
        /// Clean up necessary things before closing.
        /// </summary>
        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Clean up.
            if (videoDisplay.Source != null)
            {
                OpenNIImageProvider src = videoDisplay.Source;
                videoDisplay.Source = null;
                src.Shutdown();
            }

            // Save the settings.
            try
            {
                Settings.Default.ProjectDirectory = projectDirectory;
                Settings.Default.Save();
            }
            catch
            { }
        }
        #endregion
        #endregion

        #region trackBarVideo Handlers

        void evaluationForm_VisibleChanged(object sender, EventArgs e)
        {
            trackBarVideo.FirstHighlightedFrame = -1;
            trackBarVideo.LastHighlightedFrame = -1;

            if (evaluationForm.Visible)
                buttonEvaluation.Text = BUTTON_EVALUATION_HIDE_TEXT;
            else
                buttonEvaluation.Text = BUTTON_EVALUATION_SHOW_TEXT;
        }

        private static readonly String BUTTON_EVALUATION_SHOW_TEXT = "Show Evaluation Info";
        private static readonly String BUTTON_EVALUATION_HIDE_TEXT = "Hide Evaluation Info";
        /// <summary>
        /// Resets the Value to 0, resets the marker (no marker there), resets
        /// the value limits (0,10) and set enabled to false.
        /// </summary>
        private void trackBarVideoImageSourceChanged(object sender, EventArgs a)
        {
            trackBarVideo.Value = 0;

            trackBarVideo.FirstHighlightedFrame = -1;
            trackBarVideo.LastHighlightedFrame = -1;

            trackBarVideo.Minimum = 0;
            if(videoDisplay.Source == null ||
                videoDisplay.Source is OpenNILiveStreamController)
            {
            trackBarVideo.Maximum = 10;
            trackBarVideo.Enabled = false;
            }
            else
            {
                trackBarVideo.Maximum =
                    (videoDisplay.Source as OpenNIRecordingController).NumberOfFrames;
                    trackBarVideo.Enabled = true;
            }
        }

        /// <summary>
        /// Updates the value to the current frame position of the
        /// video source.
        /// </summary>
        private void trackBarVideoUpdateValue(object sender, bool handled)
        {
            OpenNIRecordingController src = videoDisplay.Source as OpenNIRecordingController;
            if (src != null && !handled)
            {
                trackBarVideo.Value = src.CurrentFrame;
            }
        }

        private void trackBarVideo_MouseDown(object sender, MouseEventArgs e)
        {
            OpenNIRecordingController video_src = videoDisplay.Source as OpenNIRecordingController;

            if (video_src != null)
            {
                video_src.StopPlayback();
            }
        }

        private void trackBarVideo_KeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            OpenNIRecordingController video_src = videoDisplay.Source as OpenNIRecordingController;

            // The p key is used for playback control and should hence not
            // change that state.
            if (video_src != null && e.KeyData != Keys.P)
            {
                video_src.StopPlayback();
            }
        }

        private void trackBarVideo_ValueChanged(object sender, EventArgs e)
        {
            if (e is KeyEventArgs || e is MouseEventArgs)
            {
                OpenNIRecordingController video_src = videoDisplay.Source as OpenNIRecordingController;

                if (video_src != null)
                {
                    video_src.SeekFrame(trackBarVideo.Value);
                    video_src.RequestUpdate(true);
                }
            }
        }
        #endregion

        #region buttonExport Handlers
        /// <summary>
        /// Updates the button's enabled value according to marker presence.
        /// </summary>
        private void buttonExportUpdateEnabledHandler(object sender, EventArgs a)
        {
            if (trackBarVideo.FirstHighlightedFrame >= 0 && trackBarVideo.FirstHighlightedFrame <= trackBarVideo.Maximum &&
                trackBarVideo.LastHighlightedFrame >= 0 && trackBarVideo.LastHighlightedFrame <= trackBarVideo.Maximum &&
                evaluationForm.Visible == false)
            {
                buttonExport.Enabled = true;
            }
            else
            {
                buttonExport.Enabled = false;
            }
        }
        #endregion

        #region buttonResetMarkers Handlers
        private void buttonResetMarkers_Click(object sender, EventArgs e)
        {
            trackBarVideo.FirstHighlightedFrame = -1;
            trackBarVideo.LastHighlightedFrame = -1;
        }
        /// <summary>
        /// Updates the button enabled state dependant of whether a marker is
        /// set or not.
        /// </summary>
        private void buttonResetMarkersUpdateEnabledHandler(object sender, EventArgs e)
        {
            if ((trackBarVideo.FirstHighlightedFrame >= 0 && trackBarVideo.FirstHighlightedFrame <= trackBarVideo.Maximum ||
                trackBarVideo.LastHighlightedFrame >= 0 && trackBarVideo.LastHighlightedFrame <= trackBarVideo.Maximum) &&
                evaluationForm.Visible == false)
            {
                buttonResetMarkers.Enabled = true;
            }
            else
            {
                buttonResetMarkers.Enabled = false;
            }
        }
        #endregion

        #region buttonMarkFrame Handlers
        private void buttonMarkFrame_Click(object sender, EventArgs e)
        {
            if (trackBarVideo.FirstHighlightedFrame == -1)
            {
                // Set the first value
                trackBarVideo.FirstHighlightedFrame = trackBarVideo.Value;
            }
            else
            {
                if (trackBarVideo.LastHighlightedFrame == -1)
                {
                    // Set the second value
                    if (trackBarVideo.Value > trackBarVideo.FirstHighlightedFrame)
                    {
                        trackBarVideo.LastHighlightedFrame = trackBarVideo.Value;
                    }
                    else
                    {
                        trackBarVideo.FirstHighlightedFrame = trackBarVideo.FirstHighlightedFrame;
                        trackBarVideo.LastHighlightedFrame = trackBarVideo.Value;
                    }
                }
                else
                {
                    // Move the closest marker end to current position
                    if (Math.Abs(trackBarVideo.LastHighlightedFrame - trackBarVideo.Value) <=
                        Math.Abs(trackBarVideo.FirstHighlightedFrame - trackBarVideo.Value))
                        trackBarVideo.LastHighlightedFrame = trackBarVideo.Value;
                    else
                        trackBarVideo.FirstHighlightedFrame = trackBarVideo.Value;
                }
            }
        }
        private void buttonMarkFrameImageSourceChanged(object sender, EventArgs a)
        {
            if (videoDisplay.Source != null &&
                videoDisplay.Source is OpenNIRecordingController &&
                evaluationForm.Visible == false)
                buttonMarkFrame.Enabled = true;
            else
                buttonMarkFrame.Enabled = false;
        }
        private void buttonMarkFrameEvaluationFormVisibleChanged(object sender, EventArgs a)
        {
            if (videoDisplay.Source != null &&
                videoDisplay.Source is OpenNIRecordingController &&
                evaluationForm.Visible == false)
                buttonMarkFrame.Enabled = true;
            else
                buttonMarkFrame.Enabled = false;
        }
        #endregion

        #region buttonPlay Handlers
        private void buttonPlay_Click(object sender, EventArgs e)
        {
            OpenNIRecordingController video_src = videoDisplay.Source as OpenNIRecordingController;

            if (video_src != null)
            {
                if (buttonPlay.Text.Equals(BUTTON_PLAY_PLAY_TEXT))
                {
                    video_src.StartPlayback();
                }
                else
                {
                    video_src.StopPlayback();
                }
            }
        }

        private void buttonPlayImageSourceChanged(object sender, EventArgs a)
        {
            buttonPlay.Text = BUTTON_PLAY_PLAY_TEXT;
            if(videoDisplay.Source == null ||
                ! (videoDisplay.Source is OpenNIRecordingController))
                buttonPlay.Enabled = false;
            else
                buttonPlay.Enabled = true;
        }

        private void buttonPlayStartedPlayingHandler(object sender, EventArgs a)
        {
            if (videoDisplay.Source != null &&
                (videoDisplay.Source is OpenNIRecordingController))
                buttonPlay.Text = BUTTON_PLAY_PAUSE_TEXT;
        }

        private void buttonPlayStoppedPlayingHandler(object sender, EventArgs a)
        {
            buttonPlay.Text = BUTTON_PLAY_PLAY_TEXT;
        }
        #endregion

        #region buttonStartCapturing Handlers
        private void lazyInvoke(MethodInvoker to_invoke)
        {
            BeginInvoke(to_invoke);
        }

        private void kinect_StoppedRecording()
        {
            lazyInvoke((MethodInvoker)delegate
            {
                buttonStartCapturing.Text = BUTTON_START_CAPTURING_START_TEXT;
            });
        }

        private void kinect_StartedRecording()
        {
            lazyInvoke((MethodInvoker)delegate { buttonStartCapturing.Text = BUTTON_START_CAPTURING_STOP_TEXT; });
        }

        private void buttonStartCapturing_Click(object sender, EventArgs e)
        {
            if (buttonStartCapturing.Text.Equals(BUTTON_START_CAPTURING_START_TEXT))
            {
                try
                {
                    DirectoryInfo containing = new DirectoryInfo(this.projectDirectory);
                    String prefix = DateTime.Now.ToString("yyyy-MM-dd-Take-");
                    DirectoryInfo captureFolder = OpenNIImageProvider.
                                GetNewFolderWithHigherIndex(containing, prefix, "-");

                    labelCaptureSubfolderContent.Text = captureFolder.Name;
                    kinect.StartRecording(captureFolder, true);
                }
                catch (Exception exc)
                {
                    showException(exc.Message);
                }
            }
            else
            {
                kinect.StopRecording();
            }
        }
        //private void buttonStartCapturingStoppedPlayingHandler(object sender, EventArgs e)
        //{
        //    buttonStartCapturing.Enabled = false;
        //}
        #endregion

        #region buttonDisplayShortcuts Handlers
        private void buttonDisplayShortcuts_Click(object sender, EventArgs e)
        {
            new KeyShortcutsAboutDialog().ShowDialog(this);
        }
        #endregion

        #region buttonChangeProjectDirectory Handlers
        private void buttonChangeProjectDirectory_Click(object sender, EventArgs e)
        {
            // Set the current directory as selected
            DirectoryInfo pathInfo = new DirectoryInfo(projectDirectory);
            folderBrowserDialogProjectDirectory.SelectedPath = pathInfo.FullName;
            DialogResult dialogAnswer =
                folderBrowserDialogProjectDirectory.ShowDialog();

            // Show the dialog and wait for the result
            if (dialogAnswer == System.Windows.Forms.DialogResult.OK &&
                !projectDirectory.Equals(folderBrowserDialogProjectDirectory.SelectedPath))
            {
                // If the user selected ok and selected a new path
                projectDirectory = folderBrowserDialogProjectDirectory.SelectedPath;
                // Set the filesystem watcher
                fileSystemWatcherProjectDir.Path = projectDirectory;
                updateListBoxProjectDir();
            }
        }
        #endregion

        #region Button Evaluation Handlers
        private void buttonEvaluation_Click(object sender, EventArgs e)
        {
            if (buttonEvaluation.Text.Equals(BUTTON_EVALUATION_HIDE_TEXT))
            {
                evaluationForm.Close();
            }
            else
            {
                evaluationForm.DesktopLocation = new Point(
                    DesktopLocation.X + Size.Width, DesktopLocation.Y);

                evaluationForm.Show();
            }
            buttonEvaluationUpdateEnabledHandler(sender, e);
        }

        private void buttonEvaluationUpdateEnabledHandler(object sender, EventArgs a)
        {
            if ((videoDisplay.Source == null ||
                    !(videoDisplay.Source is OpenNIRecordingController)) &&
                evaluationForm.Visible == false)
                buttonEvaluation.Enabled = false;
            else
                buttonEvaluation.Enabled = true;
        }
        #endregion
        #endregion

        #region Helpers
        /// <summary>
        /// Display a messagebox to inform the user about the exception.
        /// </summary>
        private void showException(String message)
        {
            MessageBox.Show(message, "An Error occured",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Fills the project directory list box with values.
        /// </summary>
        private void updateListBoxProjectDir()
        {
            DirectoryInfo dirInfo = new DirectoryInfo(projectDirectory);
            IEnumerable<DirectoryInfo> subDirs = dirInfo.EnumerateDirectories("*-Take-*");

            List<String> sortList = new List<String>();
            foreach (DirectoryInfo dir in subDirs)
                sortList.Add(dir.Name);
            // Sort the list numerically
            sortList.Sort(NaturalSort.NatCompare);

            // Reset the listbox and the currently displayed data
            listBoxAvailableTakes.ClearSelected();
            listBoxAvailableTakes.Items.Clear();

            // Display the new data
            foreach(String directory in sortList)
                listBoxAvailableTakes.Items.Add(directory);
        }
        #endregion

        #region Export
        /// <summary>
        /// Initializes the export.
        /// </summary>
        private void buttonExport_Click(object sender, EventArgs e)
        {
            OpenNIRecordingController exportVideo = videoDisplay.Source as OpenNIRecordingController;
            if (exportVideo == null) return;

            // Stop playback before configuration.
            exportVideo.StopPlayback();

            // Select frames to export
            List<int> export_frames = new List<int>(
                Math.Abs(trackBarVideo.LastHighlightedFrame - trackBarVideo.FirstHighlightedFrame));

            for (int i = trackBarVideo.FirstHighlightedFrame; i <= trackBarVideo.LastHighlightedFrame; i++)
                export_frames.Add(i);

            Dictionary<int,int> statistics = exportVideo.GetUserStatistics(export_frames);

            // Show export configuration dialog.
            ExportConfiguration configuration_form = new ExportConfiguration(
                                                        statistics, export_frames);
            configuration_form.ImageSensorData = exportVideo.ImageOrDepth;
            configuration_form.DrawBackground = exportVideo.DrawBackground;
            configuration_form.DrawHighlight = exportVideo.DrawUserHighlight;
            configuration_form.DrawLabels = exportVideo.DrawUserInformation;
            configuration_form.DrawSkeleton = exportVideo.DrawSkeletonMesh;

            configuration_form.ShowDialog(this);
            if (configuration_form.DialogResult == DialogResult.Cancel)
                return;

            // Set paralellization event handlers.
            exportVideo.ExportMadeProgress += exportMadeProgress;
            exportVideo.ExportFailed += exportFailed;

            // Check whether the user selected to do a batch export.
            if (configuration_form.DoBatchExport)
            {
                exportVideo.ExportFinished += batchExportPart1Finished;
                // Configure for the first batch export part and ignore
                // the corresponding user preferences.
                configuration_form.ImageSensorData = true;
                configuration_form.DrawBackground = true;
                configuration_form.DrawSkeleton = false;
                configuration_form.DrawHighlight = false;
                configuration_form.DrawLabels = false;
            }
            else
            {
                exportVideo.ExportFinished += exportFinished;
            }

            // Display progress bar.
            exportForm.progressBar.Value = 0;
            exportForm.textBoxProgress.Text = "Initializing export...";
            exportForm.Show(this);

            // Prepare export directory.
            FileInfo recording = new FileInfo(exportVideo.RecordingFilename);
            DirectoryInfo recordDir = recording.Directory;
            DirectoryInfo exportDir = OpenNIImageProvider.
                GetNewFolderWithHigherIndex(recordDir, EXPORT_DIR_PREFIX, "-");

            // Begin exporting.
            exportVideo.ExportFrames(
                export_frames,
                configuration_form.SelectedUsers,
                configuration_form.AnnotationIn2D,
                configuration_form.AnnotationIn3D,
                configuration_form.ExportRelativePathNames,
                configuration_form.ImageSensorData,
                configuration_form.DrawBackground,
                configuration_form.DrawSkeleton,
                configuration_form.DrawHighlight,
                configuration_form.DrawLabels,
                exportDir);
        }

        /// <summary>
        /// Cancels the running export task and hides the progress bar.
        /// </summary>
        internal void cancelCurrentExport(object sender, EventArgs a)
        {
            OpenNIRecordingController exportVideo = videoDisplay.Source as OpenNIRecordingController;
            if (exportVideo == null) return;

            exportVideo.CancelExport();
            exportVideo.ResetExportEventHandlers();
            exportForm.Hide();
        }

        /// <summary>
        /// Reconfigures the exporting parameters and starts the second part
        /// of the batch export.
        /// </summary>
        private void batchExportPart1Finished(
            OpenNIRecordingController sender,
            DirectoryInfo exportDir, List<int> frames, List<int> userIDs,
            ImageDictionary exportedFiles2D,
            ImageDictionary exportedFiles3D, bool ExportRelativePathNames,
            String export_filename_suffix)
        {
            // Report success.
            lazyInvoke((MethodInvoker)delegate
            {
                exportForm.textBoxProgress.Text =
                    "Exporting standard images finished (Batch export part 1)." + 
                    "Generating data index file...";
            });


            try
            {
                if (exportedFiles2D != null)
                {
                    using (FileStream write_to = File.OpenWrite(
                        Path.Combine(exportDir.FullName, "Export2D" + export_filename_suffix + ".xml")))
                    {
                        movementDataSerializer.Serialize(write_to, exportedFiles2D);
                    }
                }
                if (exportedFiles3D != null)
                {
                    using (FileStream write_to = File.OpenWrite(
                        Path.Combine(exportDir.FullName, "Export3D" + export_filename_suffix + ".xml")))
                    {
                        movementDataSerializer.Serialize(write_to, exportedFiles3D);
                    }
                }
            }
            catch (Exception e)
            {
                showException("Error creating data index file: \n" +
                    e.Message);
            }

            // Configure for the second export.
            // Check whether the user selected to do a batch export.
            OpenNIRecordingController exportVideo = sender;
            exportVideo.ExportFinished -= batchExportPart1Finished;
            exportVideo.ExportFinished += batchExportPart2Finished;
            
            // Reset progress bar.
            lazyInvoke((MethodInvoker)delegate
                {
                    exportForm.progressBar.Value = 0;
                    exportForm.textBoxProgress.Text = "Initializing batch export part 2...";
                });

            // Begin exporting.
            exportVideo.ExportFrames(
                frames,
                userIDs,
                (exportedFiles2D != null),
                (exportedFiles3D != null),
                ExportRelativePathNames,
                true, true, true, false, false,
                exportDir);
        }

        /// <summary>
        /// Reconfigures the exporting parameters and starts the third part
        /// of the batch export.
        /// </summary>
        private void batchExportPart2Finished(
            OpenNIRecordingController sender,
            DirectoryInfo exportDir, List<int> frames, List<int> userIDs,
            ImageDictionary exportedFiles2D,
            ImageDictionary exportedFiles3D, bool ExportRelativePathNames,
            String export_filename_suffix)
        {
            // Report success.
            lazyInvoke((MethodInvoker)delegate
            {
                exportForm.textBoxProgress.Text =
                    "Exporting images with skeleton finished (Batch export part 2)." +
                    "Generating data index file...";
            });


            try
            {
                if (exportedFiles2D != null)
                {
                    using (FileStream write_to = File.OpenWrite(
                        Path.Combine(exportDir.FullName, "Export2D" + export_filename_suffix + ".xml")))
                    {
                        movementDataSerializer.Serialize(write_to, exportedFiles2D);
                    }
                }
                if (exportedFiles3D != null)
                {
                    using (FileStream write_to = File.OpenWrite(
                        Path.Combine(exportDir.FullName, "Export3D" + export_filename_suffix + ".xml")))
                    {
                        movementDataSerializer.Serialize(write_to, exportedFiles3D);
                    }
                }
            }
            catch (Exception e)
            {
                showException("Error creating data index file: \n" +
                    e.Message);
            }

            // Configure for the second export.
            // Check whether the user selected to do a batch export.
            OpenNIRecordingController exportVideo = sender;
            exportVideo.ExportFinished -= batchExportPart2Finished;
            exportVideo.ExportFinished += exportFinished;

            // Reset progress bar.
            lazyInvoke((MethodInvoker)delegate
                 {
                     exportForm.progressBar.Value = 0;
                     exportForm.textBoxProgress.Text = "Initializing batch export part 3...";
                 });

            // Begin exporting.
            exportVideo.ExportFrames(
                frames,
                userIDs,
                (exportedFiles2D != null),
                (exportedFiles3D != null),
                ExportRelativePathNames,
                true, false, false, false, false,
                exportDir);
        }

        /// <summary>
        /// Reports success and tries to generate data index file.
        /// </summary>
        private void exportFinished(
            OpenNIRecordingController sender,
            DirectoryInfo exportDir, List<int> frames, List<int> userIDs,
            ImageDictionary exportedFiles2D,
            ImageDictionary exportedFiles3D, bool ExportRelativePathNames,
            String export_filename_suffix)
        {
            // Report success.
            lazyInvoke((MethodInvoker)delegate
            {
                exportForm.textBoxProgress.Text =
                    "Exporting images finished. Generating data index file...";
            });


            try
            {
                if (exportedFiles2D != null)
                {
                    using (FileStream write_to = File.OpenWrite(
                        Path.Combine(exportDir.FullName, "Export2D" + export_filename_suffix + ".xml")))
                    {
                        movementDataSerializer.Serialize(write_to, exportedFiles2D);
                    }
                }
                if (exportedFiles3D != null)
                {
                    using (FileStream write_to = File.OpenWrite(
                        Path.Combine(exportDir.FullName, "Export3D" + export_filename_suffix + ".xml")))
                    {
                        movementDataSerializer.Serialize(write_to, exportedFiles3D);
                    }
                }
            }
            catch (Exception e)
            {
                showException("Error creating data index file: \n" +
                    e.Message);
            }

            // Report success.
            lazyInvoke((MethodInvoker)delegate
            {
                exportForm.Hide();
                textBoxExport.Text = "Last export successful to folder " + exportDir.FullName;
            });

            sender.ResetExportEventHandlers();
        }

        /// <summary>
        /// Reports failure.
        /// </summary>
        void exportFailed(object sender, string message)
        {
            lazyInvoke((MethodInvoker)delegate
            {
                exportForm.Hide();
                showException("There was an error exporting: " + message);
            });
            ((OpenNIRecordingController)sender).ResetExportEventHandlers();
        }

        /// <summary>
        /// Updates the formExport progressbar and text.
        /// </summary>
        void exportMadeProgress(double arg1, int arg2, int arg3)
        {
            lazyInvoke((MethodInvoker)delegate
            {
                if (exportForm != null)
                {
                    exportForm.progressBar.Value = (int)arg1;
                    exportForm.textBoxProgress.Text = "Exported " + arg2 +
                        " of " + arg3 + " image files.";
                }
            });
        }
        #endregion
    }
}
