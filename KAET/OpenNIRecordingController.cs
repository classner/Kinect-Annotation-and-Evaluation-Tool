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
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using OpenNI;
using System.Xml.Serialization;
using System.Linq;


namespace KAET
{
    /// <summary>
    /// A class to interface with an OpenNI recording stream.
    /// If you do not need the class any more, you should clean it up using
    /// the <see cref="Shutdown"/> method to release any handles.
    /// </summary>
    public class OpenNIRecordingController : OpenNIImageProvider
    {
        #region Variables
        #region Private
        private Player player;

        // The thread and the different control flags.
        private Thread readerThread;
        private Thread exportThread;
        private readonly object EXPORT_LOCK = new object();
        private readonly object RUN_CONTROL_LOCK = new object();
        private readonly int[] histogram;
        private readonly String recording_filename;
        private BinaryReader _userInformationReader;

        private List<int> _usersToIgnore = new List<int>();

        private bool shouldRun = false;
        private bool shouldDrawBackground = true;
        private bool shouldPrintID = true;
        private bool shouldDrawHighlight = false;
        private bool shouldDrawSkeleton = true;
        private bool switchImageOrDepth = true;
        private bool shouldDrawPixels = true;
        private readonly ImageDictionary _userLocationInformation;

        private readonly XmlSerializer movementDataSerializer =
                            new XmlSerializer(typeof(ImageDictionary));
        #endregion

        #region Public interface
        private ImageDictionary _additionalSkeletonInfo = null;
        /// <summary>
        /// Add ImageDictionaries with additional position information to be
        /// painted on the images. The IDs of the skeletons to paint can be
        /// set with the <see cref="AdditionalSkeletonIDs"/> property.
        /// </summary>
        public ImageDictionary AdditionalSkeletonInformation
        {
            get { return _additionalSkeletonInfo; }
            set { _additionalSkeletonInfo = value; }
        }
        private List<int> _additionalSkeletonIDs = new List<int>();
        /// <summary>
        /// Set the IDs of the additional skeletons to paint. The
        /// ImageDictionary with the location information can be set with the
        /// <see cref="AdditionalSkeletonInformation"/> property.
        /// </summary>
        public List<int> AdditionalSkeletonIDs
        {
            get { return _additionalSkeletonIDs; }
            set { _additionalSkeletonIDs = value; }
        }

        /// <summary>
        /// Converts a JointDictionary with 3D data to the corresponding one
        /// with 2D data. Note that the Z-Coordinate is left unchanged, though
        /// it has no meaning in the resulting 2D space and can be seen as 0.
        /// </summary>
        public JointDictionary Convert3Dto2D(JointDictionary source)
        {
            return base.Convert3Dto2D(source, depthGenerator);
        }

        private double _confidenceThreshold = 0.0;
        /// <summary>
        /// Set a confidence threshold for the skeleton joints.
        /// </summary>
        public double ConfidenceThreshold
        {
            get { return _confidenceThreshold; }
            set { _confidenceThreshold = value; }
        }

        /// <summary>
        /// Calculates how often the user with given id occurs in the given
        /// frames. If the argument frames is null, the entire video will
        /// be iterated.
        /// </summary>
        public Dictionary<int, int> GetUserStatistics(List<int> frames)
        {
            return UserLocationInformation.GetUserStatistics(frames);
        }

        /// <summary>
        /// Gets the filename of the played recording.
        /// </summary>
        public String RecordingFilename
        {
            get { return recording_filename; }
        }

        /// <summary>
        /// Gets the currently displayed frame number.
        /// </summary>
        public int CurrentFrame
        {
            get {
                try
                {
                    if (switchImageOrDepth)
                        return player.TellFrame(imageGenerator);
                    else
                        return player.TellFrame(depthGenerator);
                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }
        /// <summary>
        /// Gets the total number of frames.
        /// </summary>
        public int NumberOfFrames
        {
            get
            {
                if (switchImageOrDepth)
                    return player.GetNumFrames(imageGenerator);
                else
                    return player.GetNumFrames(depthGenerator);
            }
        }
        /// <summary>
        /// Gets or sets whether to draw the image background.
        /// </summary>
        public bool DrawBackground
        {
            get { return shouldDrawBackground; }
            set {
                lock (EXPORT_LOCK)
                {
                    shouldDrawBackground = value;
                }
            }
        }
        /// <summary>
        /// The list of users to ignore as foreground.
        /// </summary>
        public List<int> UsersToIgnore
        {
            get { return _usersToIgnore; }
        }
        /// <summary>
        /// Gets or set whether to label found users.
        /// </summary>
        public bool DrawUserInformation
        {
            get { return shouldPrintID; }
            set {
                lock (EXPORT_LOCK)
                {
                    shouldPrintID = value;
                }
            }
        }
        /// <summary>
        /// Gets or sets whether users should be marked with colors
        /// when detected.
        /// </summary>
        public bool DrawUserHighlight
        {
            get { return shouldDrawHighlight; }
            set {
                lock (EXPORT_LOCK)
                {
                    shouldDrawHighlight = value;
                }
            }
        }
        /// <summary>
        /// A dictionary which associates frames with UserIDs and
        /// location information.
        /// </summary>
        public ImageDictionary UserLocationInformation
        {
            get { return _userLocationInformation;  }
        }
        /// <summary>
        /// Set the User Information Reader. It reads a binary encoded file
        /// with labeled foreground and background images.
        /// </summary>
        public BinaryReader UserInformationReader
        {
            set { _userInformationReader = value; }
        }
        /// <summary>
        /// Gets or set whether the image or depth image should be rendered.
        /// </summary>
        public bool ImageOrDepth
        {
            get { return switchImageOrDepth; }
            set {
                lock (EXPORT_LOCK)
                {
                    switchImageOrDepth = value;
                }
            }
        }
        /// <summary>
        /// Gets or sets whether any of the selected sensor data should
        /// be drawn.
        /// </summary>
        public bool DrawSensorData {
            get { return shouldDrawPixels; }
            set
            {
                lock (EXPORT_LOCK)
                {
                    shouldDrawPixels = value;
                }
            }
        }
        /// <summary>
        /// Gets or sets whether the user skeleton should be drawn when
        /// detected.
        /// </summary>
        public bool DrawSkeletonMesh
        {
            get { return shouldDrawSkeleton; }
            set
            {
                lock (EXPORT_LOCK)
                {
                    shouldDrawSkeleton = value;
                }
            }
        }
        #endregion
        #endregion

        #region Constructors and Destructors
        /// <summary>
        /// Creates a new control instance with a recording as data
        /// source. The current image data can be obtained by the
        /// <see cref="Image"/>-property. The
        /// <see cref="NewImageDataAvailable"/> event informs about when the
        /// data is updated, the <see cref="ErrorOccured"/>-event about
        /// errors.
        /// </summary>
        /// <exception cref="System.Exception">Thrown if the stream could not
        /// be initialized properly.</exception>
        public OpenNIRecordingController(String recording_filename,
            String movementDataFileName, String userAnnotationFilename)
        {
            // Create a new context and the data-generating nodes.
            context = new Context();
            context.OpenFileRecording(recording_filename);
            this.recording_filename = recording_filename;

            _userInformationReader = new BinaryReader(
                       new FileStream(movementDataFileName, FileMode.Open));

            using (FileStream annotation_stream = File.OpenRead(userAnnotationFilename))
            {
                _userLocationInformation = (ImageDictionary)movementDataSerializer.Deserialize(annotation_stream);
            }

            // Image
            imageGenerator = (ImageGenerator)context.FindExistingNode(NodeType.Image);

            // Depth
            depthGenerator = (DepthGenerator)context.FindExistingNode(NodeType.Depth);
            histogram = new int[depthGenerator.DeviceMaxDepth];

            // Player
            player = (Player)context.FindExistingNode(NodeType.Player);
            player.PlaybackSpeed = 1.0;

            if (depthGenerator == null || imageGenerator == null || player == null)
            {
                throw new Exception("Could not initialize recording stream.");
            }
            
            // Error handling
            context.ErrorStateChanged += context_ErrorStateChanged;
            context.StartGeneratingAll();
        }

        ~OpenNIRecordingController()
        {
            if(_userInformationReader != null)
                _userInformationReader.Close();
        }
        #endregion

        #region Events
        #region OpenNI event handlers
        private void context_ErrorStateChanged(object sender, ErrorStateEventArgs e)
        {
            onErrorOccured(sender, e.CurrentError);
            StopPlayback();
        }
        #endregion
        #endregion

        #region Generation control
        /// <summary>
        /// Starts generating images and populating the
        /// <see cref="Image"/>-property. Receive updates on when the image
        /// was updated by subscribing to the
        /// <see cref="NewImageDataAvailable"/>-Event.
        /// </summary>
        public void StartPlayback()
        {
            lock (RUN_CONTROL_LOCK)
            {
                if (shouldRun == false)
                {
                    shouldRun = true;
                    readerThread = new Thread(ReaderThread);
                    readerThread.Start();
                }

                onStartedGenerating();
            }
        }

        /// <summary>
        /// Stops generating images.
        /// </summary>
        public void StopPlayback()
        {
            lock (RUN_CONTROL_LOCK)
            {
                if (shouldRun)
                {
                    shouldRun = false;
                    readerThread.Join();
                    onStoppedGenerating();
                }
            }
        }

        /// <summary>
        /// Cleans up resources and shuts down the generating process. After
        /// calling this method, no further video can be polled.
        /// </summary>
        public override void Shutdown()
        {
            StopPlayback();
            if(_userInformationReader != null)
                _userInformationReader.Close();

            context.Release();
        }

        /// <summary>
        /// Seeks the frame with the given number. It is not displayed if
        /// the nodes are not currently generating. You can request an
        /// update in this case by calling <see cref="RequestUpdate"/>.
        /// </summary>
        public void SeekFrame(int frameIndex)
        {
            player.SeekToFrame(imageGenerator, frameIndex, PlayerSeekOrigin.Set);
        }

        /// <summary>
        /// Updates the image data. Only has an effect if the nodes are not
        /// currently generating.
        /// </summary>
        /// <param name="handled">Whether the image update should be regarded
        /// as handled. The NewImageDataAvailable event will be raised
        /// with this value.</param>
        public void RequestUpdate(bool handled)
        {
            lock (RUN_CONTROL_LOCK)
            {
                if (!shouldRun)
                {
                    if (CurrentFrame > 0)
                    {
                        SeekFrame(CurrentFrame);
                    }
                    pollAndProcessOneFrame(handled);
                }
            }
        }
        #endregion

        #region Data visualization
        /// <summary>
        /// The thread task to read from the device.
        /// </summary>
        private void ReaderThread()
        {
            while (this.shouldRun)
            {
                pollAndProcessOneFrame(false);
            }
        }

        /// <summary>
        /// Wait for one new frame and process it.
        /// </summary>
        private void pollAndProcessOneFrame(bool handled)
        {
            try
            {
                context.WaitAndUpdateAll();
            }
            catch (Exception)
            {
                onErrorOccured(this, "Data could not be read from device.");
                if (shouldRun)
                    shouldRun = false;
                return;
            }

            processOneFrame(handled);
        }

        /// <summary>
        /// Process the current available data from the image or depth generator.
        /// </summary>
        /// <param name="handled">The processing will trigger the NewImageDataAvailable
        /// event with this parameter.</param>
        private void processOneFrame(bool handled)
        {
            if (imageGenerator.GetMetaData() == null)
                return;
            lock (ImageLock)
            {
                if (shouldDrawPixels)
                {
                    if (switchImageOrDepth)
                    {
                        imageGenerator.GetMetaData(imageMD);
                        if ((!shouldDrawBackground || shouldDrawHighlight) &&
                            _userInformationReader != null)
                        {
                            long position = Math.Max(CurrentFrame - 1, 0) * imageMD.XRes * imageMD.YRes * 2;
                            if (position < _userInformationReader.BaseStream.Length - imageMD.XRes * imageMD.YRes * 2)
                            {
                                _userInformationReader.BaseStream.Seek(position, SeekOrigin.Begin);
                                ushort[] userInfo =
                                    loadUserDataFromFileStream(_userInformationReader, imageMD.XRes * imageMD.YRes);

                                drawImageWithHighlightAndBackgroundSubtraction(
                                    imageMD, userInfo, shouldDrawBackground, shouldDrawHighlight,
                                    UsersToIgnore);
                            }
                            else
                            {
                                drawImageWithoutHighlightAndBackgroundSubtraction(imageMD);
                            }
                        }
                        else
                        {
                            drawImageWithoutHighlightAndBackgroundSubtraction(imageMD);
                        }
                    }
                    else
                    {
                        depthGenerator.GetMetaData(depthMD);

                        if ((!shouldDrawBackground || shouldDrawHighlight) &&
                            _userInformationReader != null)
                        {
                            long position = (Math.Max(CurrentFrame - 1, 0)) * imageMD.XRes * imageMD.YRes * 2;
                            if (position < _userInformationReader.BaseStream.Length - imageMD.XRes * imageMD.YRes * 2)
                            {
                                _userInformationReader.BaseStream.Seek(position, SeekOrigin.Begin);
                                ushort[] userInfo =
                                    loadUserDataFromFileStream(_userInformationReader, imageMD.XRes * imageMD.YRes);

                                drawDepthWithHighlightAndBackgroundSubtraction(
                                    depthMD, userInfo,
                                    shouldDrawBackground, shouldDrawHighlight,
                                    histogram, _usersToIgnore);
                            }
                            else
                            {
                                drawDepthWithoutHighlightAndBackgroundSubtraction(depthMD, histogram);
                            }
                        }
                        else
                        {
                            drawDepthWithoutHighlightAndBackgroundSubtraction(depthMD, histogram);
                        }
                    }
                }
                else
                {
                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        // In this case, the image must be cleared
                        g.FillRectangle(Brushes.Black,
                            new Rectangle(0, 0, imageMD.XRes, imageMD.YRes));
                    }
                }

                drawSkeletonAndLabels();
            }

            onNewImageDataAvailable(handled);
        }

        /// <summary>
        /// Draws the skeleton if necessary.
        /// </summary>
        private void drawSkeletonAndLabels()
        {
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                if (UserLocationInformation.ContainsKey(CurrentFrame))
                {
                    foreach (int user in UserLocationInformation[CurrentFrame].Keys)
                    {
                        JointDictionary twoDPositions =
                            Convert3Dto2D(UserLocationInformation[CurrentFrame][user], depthGenerator);

                        if (shouldDrawSkeleton)
                        {
                            DrawSkeleton(g, ANTICOLORS[user % NCOLORS],
                                twoDPositions, ConfidenceThreshold);
                        }

                        if (shouldPrintID)
                        {
                            Point3D com = twoDPositions[SkeletonJoint.Torso].Position;
                            string label = "User " + user.ToString();
                            using(Brush brush = new SolidBrush(ANTICOLORS[user % NCOLORS]))
                            {
                            g.DrawString(label, font_label,
                                brush,
                                com.X - TextRenderer.MeasureText(label, font_label).Width / 2,
                                com.Y);
                            }
                        }
                    }

                    if (AdditionalSkeletonIDs != null &&
                        AdditionalSkeletonInformation != null &&
                        shouldDrawSkeleton)
                    {
                        foreach (int user in AdditionalSkeletonIDs)
                        {
                            if (AdditionalSkeletonInformation.ContainsKey(CurrentFrame) &&
                                AdditionalSkeletonInformation[CurrentFrame].ContainsKey(user))
                            {
                                JointDictionary twoDPositions;
                                if (UserLocationInformation.Is3DData)
                                    twoDPositions =
                          Convert3Dto2D(AdditionalSkeletonInformation[CurrentFrame][user], depthGenerator);
                                else
                                    twoDPositions = AdditionalSkeletonInformation[CurrentFrame][user];

                                DrawSkeleton(g, COLORS[user % NCOLORS],
                                    twoDPositions, 0.0);
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region Data export
        /// <summary>
        /// Removes all subscriber methods from the export events.
        /// </summary>
        public void ResetExportEventHandlers()
        {
            ExportMadeProgress = null;
            ExportFailed = null;
            ExportFinished = null;
        }

        /// <summary>
        /// An event to report export progress. First argument is the reached
        /// percentage, second last finished object, last number of objects to
        /// go.
        /// </summary>
        public event Action<double, int, int> ExportMadeProgress;
        /// <summary>
        /// Raises the corresponding event.
        /// </summary>
        private void onExportMadeProgress(double progress, int current, int max)
        {
            if (ExportMadeProgress != null)
                ExportMadeProgress(progress, current, max);
        }
        /// <summary>
        /// This event is raised when the export process failed. The message
        /// contains the error message.
        /// </summary>
        public event Action<object, String> ExportFailed;
        /// <summary>
        /// Raises the corresponding event.
        /// </summary>
        /// <param name="message">The error message.</param>
        private void onExportFailed(String message)
        {
            if (ExportFailed != null)
                ExportFailed(this, message);
        }
        /// <summary>
        /// An event to report the successful, finished export to the
        /// specified Directory.
        /// </summary>
        public event Action<OpenNIRecordingController,
            DirectoryInfo, List<int>,
            List<int>, ImageDictionary, ImageDictionary,
            bool, String> ExportFinished;
        /// <summary>
        /// Raises the corresponding event.
        /// </summary>
        private void onExportFinished(object parameter)
        {
            object[] parameters = parameter as object[];
            OpenNIRecordingController sender = parameters[0] as OpenNIRecordingController;
            DirectoryInfo exportDir = parameters[1] as DirectoryInfo;
            List<int> frames = parameters[2] as List<int>;
            List<int> userIDs = parameters[3] as List<int>;
            ImageDictionary exported_file_information2D = parameters[4] as ImageDictionary;
            ImageDictionary exported_file_information3D = parameters[5] as ImageDictionary;
            bool ExportRelativePathNames = (bool)parameters[6];
            String export_filename_suffix = parameters[7] as String;


            if (exportThread != null && exportThread.IsAlive)
            {
                exportThread.Join();
            }

            if (ExportFinished != null)
                ExportFinished(sender, exportDir, frames, userIDs,
                    exported_file_information2D,
                    exported_file_information3D,
                    ExportRelativePathNames, export_filename_suffix);
        }
        private bool export_cancelled;

        /// <summary>
        /// Initialize the export process for the given frames. Exporting will
        /// be done asynchronously. The corresponding events are
        /// <see cref="ExportMadeProgress"/>, <see cref="ExportFailed"/> and
        /// <see cref="ExportFinished"/>. Will do nothing if an export is
        /// already running. The frames are exported according to the current
        /// display configuration. Only the images are exported with the given
        /// configuration.
        /// An Image Dictionary with the collected content from the entire
        /// export is returned as result and can be serialized as
        /// annotation reference.
        /// Currently only complete lists with numbers from the first to the
        /// last contained frame should be exported, as the statistics class only
        /// supports such data.
        /// </summary>
        /// <param name="frames">The frame numbers of the frames to export.
        /// </param>
        /// <param name="userIDs">The userIDs of the users who will be
        /// regarded as foreground.</param>
        /// <returns>Whether exporting was started.</returns>
        public bool ExportFrames(List<int> frames, List<int> userIDs,
            bool AnnotationsIn2D, bool AnnotationsIn3D,
            bool ExportRelativePathNames,
            bool ImageOrDepth,
            bool DrawBackground, bool DrawSkeleton,
            bool DrawHighlights, bool DrawLabels,
            DirectoryInfo export_directory)
        {
            if (exportThread != null && exportThread.IsAlive)
                return false;

            export_cancelled = false;
            exportThread = new Thread(doExport);
            exportThread.Start(new object[]{
                frames, userIDs, AnnotationsIn2D, AnnotationsIn3D,
                ExportRelativePathNames,
                ImageOrDepth,
                DrawBackground, DrawSkeleton, DrawHighlights,
                DrawLabels, export_directory});

            return true;
        }

        /// <summary>
        /// Cancels the current export and waits for the export thread to cancel.
        /// </summary>
        public void CancelExport()
        {
            export_cancelled = true;
            if (exportThread != null && exportThread.IsAlive)
                exportThread.Join();
        }

        /// <summary>
        /// The export logic.
        /// </summary>
        /// <param name="parameter">An object[] with the real parameters.
        /// Have a look at <see cref="ExportFrames"/> for a usage
        /// example.</param>
        private void doExport(object parameter)
        {
            object[] parameters = parameter as object[];
            if (parameters == null)
                return;

            // Export directory
            DirectoryInfo exportDir = parameters[10] as DirectoryInfo;
            if (exportDir == null) return;

            // Frame list
            List<int> frames = parameters[0] as List<int>;
            if (frames == null || frames.Count == 0)
                return;
            frames.Sort();

            // User list
            List<int> userIDs = parameters[1] as List<int>;
            if (userIDs == null)
                return;

            // Ignore List
            List<int> formerIgnoreList = new List<int>(UsersToIgnore);
            UsersToIgnore.Clear();
            List<int> occuringUsers = GetUserStatistics(frames).Keys.ToList<int>();
            foreach (int user in occuringUsers)
            {
                if (!userIDs.Contains(user))
                {
                    UsersToIgnore.Add(user);
                }
            }

            // The rest of parameters
            bool AnnotationsIn2D = (bool)parameters[2];
            bool AnnotationsIn3D = (bool)parameters[3];
            bool ExportRelativePathNames = (bool)parameters[4];
            bool ImageOrDepth = (bool)parameters[5];
            bool DrawBackground = (bool)parameters[6];
            bool DrawSkeleton = (bool)parameters[7];
            bool DrawHighlights = (bool)parameters[8];
            bool DrawLabels = (bool)parameters[9];

            // Number format
            int max = frames[frames.Count - 1];
            int characters;
            if (max == 0)
                characters = 1;
            else
                characters = (int)Math.Ceiling(Math.Log10(max));
            String exportFormatString = "d" + characters;

            // The export xml files.
            ImageDictionary exportedIndex2D = new ImageDictionary(false);
            exportedIndex2D.XmlExportFrameFileList = new Dictionary<int, string>();
            exportedIndex2D.XmlOmitPoints = (userIDs.Count == 0);

            ImageDictionary exportedIndex3D = new ImageDictionary(true);
            exportedIndex3D.XmlExportFrameFileList = new Dictionary<int, string>();
            exportedIndex3D.XmlOmitPoints = (userIDs.Count == 0);

            String filename;
            String export_filename_suffix = "";
            lock (RUN_CONTROL_LOCK)
            {
                try
                {
                    StopPlayback();

                    lock (EXPORT_LOCK)
                    {
                        // Configure for export
                        DrawSkeletonMesh = DrawSkeleton;
                        DrawSensorData = true;
                        this.ImageOrDepth = ImageOrDepth;
                        this.DrawBackground = DrawBackground;
                        this.DrawUserHighlight = DrawHighlights;
                        this.DrawUserInformation = DrawLabels;

                        if (DrawSkeleton) export_filename_suffix += "s";
                        if (!DrawSensorData) export_filename_suffix += "a";
                        if (!ImageOrDepth) export_filename_suffix += "d";
                        if (!DrawBackground) export_filename_suffix += "b";
                        if (DrawUserHighlight) export_filename_suffix += "h";
                        if (DrawUserInformation) export_filename_suffix += "l";

                        if (export_filename_suffix.Length > 0)
                            export_filename_suffix = "-" + export_filename_suffix;

                        for (int frameIndex = 0; frameIndex < frames.Count; frameIndex++)
                        {
                            SeekFrame(frames[frameIndex]);
                            RequestUpdate(false);
                            String relative_filename = frames[frameIndex].ToString(exportFormatString) +
                                                        export_filename_suffix + ".png";
                            filename = Path.Combine(exportDir.FullName,
                                                    relative_filename);


                            lock (ImageLock)
                            {
                                bitmap.Save(filename, ImageFormat.Png);
                            }

                            // Get the user positions in this frame
                            Dictionary<int, JointDictionary> frame_positions2D = 
                                new Dictionary<int,JointDictionary>();
                            Dictionary<int, JointDictionary> frame_positions3D =
                                new Dictionary<int, JointDictionary>();
                            if (UserLocationInformation.ContainsKey(frames[frameIndex]))
                            {
                                foreach (int user in UserLocationInformation[frames[frameIndex]].Keys)
                                {
                                    if (userIDs.Contains(user))
                                    {
                                        frame_positions3D.Add(user, new JointDictionary(
                                            UserLocationInformation[frames[frameIndex]][user]));
                                        frame_positions2D.Add(user, new JointDictionary(
                                            Convert3Dto2D(UserLocationInformation[frames[frameIndex]][user], depthGenerator)));
                                    }
                                }
                            }
                            exportedIndex2D.Add(frames[frameIndex], frame_positions2D);
                            if (!ExportRelativePathNames)
                            {
                                exportedIndex2D.XmlExportFrameFileList.Add(
                                    frames[frameIndex], filename);
                            }
                            else
                            {
                                exportedIndex2D.XmlExportFrameFileList.Add(
                                    frames[frameIndex], relative_filename);
                            }
                            exportedIndex3D.Add(frames[frameIndex], frame_positions3D);
                            if (!ExportRelativePathNames)
                            {
                                exportedIndex3D.XmlExportFrameFileList.Add(
                                    frames[frameIndex], filename);
                            }
                            else
                            {
                                exportedIndex3D.XmlExportFrameFileList.Add(
                                    frames[frameIndex], relative_filename);
                            }


                            onExportMadeProgress(
                                (float)(frameIndex + 1) / (float)frames.Count * 100, frameIndex + 1, frames.Count);
                            if (export_cancelled)
                                return;
                        }
                    }
                }
                catch (Exception e)
                {
                    onExportFailed(e.Message);
                    return;
                }

                // Reset ignore list
                UsersToIgnore.Clear();
                UsersToIgnore.AddRange(formerIgnoreList);

                if (!AnnotationsIn2D) exportedIndex2D = null;
                if (!AnnotationsIn3D) exportedIndex3D = null;
            }

            // Report success.
            Thread reporter = new Thread(onExportFinished);
            reporter.Start(new object[]{this, exportDir, frames, userIDs,
                    exportedIndex2D, exportedIndex3D, ExportRelativePathNames,
                    export_filename_suffix});
        }

        #endregion
    }
}
