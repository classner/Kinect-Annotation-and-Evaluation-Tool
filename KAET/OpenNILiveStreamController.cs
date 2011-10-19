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
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Serialization;
using OpenNI;


namespace KAET
{
    /// <summary>
    /// A class to interface with an OpenNI livestream device.
    /// </summary>
    public class OpenNILiveStreamController : OpenNIImageProvider
    {
        #region Variables
        #region Private
        // The needed OpenNI nodes, capabilities and properties.
        // Functionality not yet implemented. Makes no difference.
        private readonly bool COMPRESS_USER_DATA = false;
        private readonly UserGenerator userGenerator;
        private Recorder recorder;
        private readonly SkeletonCapability skeletonCapability;
        private readonly PoseDetectionCapability poseDetectionCapability;
        private readonly string calibPose;
        private readonly List<int> tracked_users = new List<int>();
        BinaryWriter userInformationWriter;

        // The per-image updatable information.
        private readonly object HARDWARELOCK = new object();
        private Dictionary<SkeletonJoint, SkeletonJointPosition> joints =
                        new Dictionary<SkeletonJoint, SkeletonJointPosition>();
        private ImageDictionary movementData;
        private String movementDataFileName;
        private readonly int[] histogram;

        // The thread and the different control flags.
        private Thread readerThread;
        private readonly object RUN_CONTROL_LOCK = new object();
        private bool recording;
        private bool shouldRun;
        private bool shouldDrawPixels = true;
        private bool shouldDrawBackground = true;
        private bool shouldPrintID = true;
        private bool shouldPrintState = true;
        private bool shouldDrawSkeleton = true;
        private bool shouldDrawHighlight = false;
        private bool switchImageOrDepth = true;
        #endregion

        #region Public interface
        /// <summary>
        /// Returns a list with the ids of the currently tracked users.
        /// </summary>
        public List<int> TrackedUsers
        {
            get
            {
                return tracked_users;
            }
        }
        /// <summary>
        /// This event is triggered whenever a user is lost for tracking.
        /// </summary>
        public event Action<int> LostTracking;
        /// <summary>
        /// This event is triggered whenever a new users is started to be tracked.
        /// </summary>
        public event Action<int> StartedTracking;
        /// <summary>
        /// Gets or set whether the image or depth image should be rendered.
        /// </summary>
        public bool ImageOrDepth
        {
            get { return switchImageOrDepth; }
            set { switchImageOrDepth = value; }
        }
        /// <summary>
        /// Gets or sets whether any of the selected sensor data should
        /// be drawn.
        /// </summary>
        public bool DrawSensorData {
            get { return shouldDrawPixels; }
            set { shouldDrawPixels = value; }
        }
        /// <summary>
        /// Gets or sets whether to draw the image background.
        /// </summary>
        public bool DrawBackground
        {
            get { return shouldDrawBackground; }
            set { shouldDrawBackground = value; }
        }
        /// <summary>
        /// Gets or set whether to label found users.
        /// </summary>
        public bool DrawUserInformation
        {
            get { return shouldPrintID || shouldPrintState; }
            set { shouldPrintID = value; shouldPrintState = value; }
        }
        /// <summary>
        /// Gets or sets whether the user skeleton should be drawn when
        /// detected.
        /// </summary>
        public bool DrawSkeletonMesh
        {
            get { return shouldDrawSkeleton; }
            set { shouldDrawSkeleton = value; }
        }
        /// <summary>
        /// Gets or sets whether users should be marked with colors
        /// when detected.
        /// </summary>
        public bool DrawUserHighlight
        {
            get { return shouldDrawHighlight; }
            set { shouldDrawHighlight = value; }
        }
        /// <summary>
        /// This event is triggered when a recording is started.
        /// </summary>
        public event Action StartedRecording;
        /// <summary>
        /// This event is triggered when recording is stopped.
        /// </summary>
        public event Action StoppedRecording;
        #endregion
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new control instance with a live device as data
        /// source. The current image data can be obtained by the
        /// <see cref="Image"/>-property. The
        /// <see cref="NewImageDataAvailable"/> event informs about when the
        /// data is updated, the <see cref="ErrorOccured"/>-event about
        /// errors.
        /// Start the generation-process by calling
        /// <see cref="StartGenerating"/> and stop it by calling
        /// <see cref="StopGenerating"/>.
        /// </summary>
        /// <exception cref="System.Exception">Thrown if the device could not
        /// be initialized properly.</exception>
        public OpenNILiveStreamController()
        {
            // Create a new context and the data-generating nodes.
            context = new Context();
            context.GlobalMirror = false;

            // Image
            imageGenerator = new ImageGenerator(context);
            MapOutputMode mapMode = new MapOutputMode();
            mapMode.FPS = 30;
            mapMode.XRes = VIDEO_WIDTH;
            mapMode.YRes = VIDEO_HEIGHT;
            imageGenerator.MapOutputMode = mapMode;
            imageGenerator.PixelFormat = OpenNI.PixelFormat.RGB24;

            // Depth
            depthGenerator = new DepthGenerator(context);
            depthGenerator.AlternativeViewpointCapability.SetViewpoint(imageGenerator);
            histogram = new int[depthGenerator.DeviceMaxDepth];
            if (depthGenerator == null || imageGenerator == null)
            {
                throw new Exception("Could not initialize kinect device.");
            }

            // User generator
            userGenerator = new UserGenerator(context);
            skeletonCapability = userGenerator.SkeletonCapability;
            poseDetectionCapability = userGenerator.PoseDetectionCapability;
            calibPose = skeletonCapability.CalibrationPose;

            userGenerator.NewUser += userGenerator_NewUser;
            userGenerator.LostUser += userGenerator_LostUser;
            poseDetectionCapability.PoseDetected +=
                poseDetectionCapability_PoseDetected;
            skeletonCapability.CalibrationEnd +=
                skeletonCapability_CalibrationEnd;
            skeletonCapability.SetSkeletonProfile(SkeletonProfile.All);

            // Error handling
            context.ErrorStateChanged += context_ErrorStateChanged;
        }

        #endregion

        #region Events
        #region OpenNI event handlers
        private void userGenerator_LostUser(object sender, UserLostEventArgs e)
        {
            if (tracked_users.Contains(e.ID))
            {
                tracked_users.Remove(e.ID);
                onLostTracking(e.ID);
            }
        }

        private void skeletonCapability_CalibrationEnd(object sender, CalibrationEndEventArgs e)
        {
            if (e.Success)
            {
                skeletonCapability.StartTracking(e.ID);

                tracked_users.Add(e.ID);
                onStartedTracking(e.ID);
            }
            else
            {
                poseDetectionCapability.StartPoseDetection(calibPose, e.ID);
            }
        }

        private void poseDetectionCapability_PoseDetected(object sender, PoseDetectedEventArgs e)
        {
            poseDetectionCapability.StopPoseDetection(e.ID);
            skeletonCapability.RequestCalibration(e.ID, true);
        }

        private void userGenerator_NewUser(object sender, NewUserEventArgs e)
        {
            poseDetectionCapability.StartPoseDetection(this.calibPose, e.ID);
        }

        private void context_ErrorStateChanged(object sender, ErrorStateEventArgs e)
        {
            onErrorOccured(sender, e.CurrentError);
            StopGenerating();
        }
        #endregion

        #region Event Triggers
        /// <summary>
        /// Triggers the StartedTracking event.
        /// </summary>
        protected void onStartedTracking(int id)
        {
            if (StartedTracking != null)
            {
                StartedTracking(id);
            }
        }
        /// <summary>
        /// Triggers the LostTracking event.
        /// </summary>
        protected void onLostTracking(int id)
        {
            if (LostTracking != null)
            {
                LostTracking(id);
            }
        }
        /// <summary>
        /// Triggers the StartedRecording event.
        /// </summary>
        private void onStartedRecording()
        {
            if (StartedRecording != null)
            {
                StartedRecording();
            }
        }
        /// <summary>
        /// Triggers the StartedTracking event.
        /// </summary>
        private void onStoppedRecording()
        {
            if (StoppedRecording != null)
            {
                StoppedRecording();
            }
        }
        #endregion
        #endregion

        #region Generation control
        #region Recording
        /// <summary>
        /// Starts to record the current stream. The stream must have been
        /// initialized when calling this method by using the method
        /// <see cref="StartGenerating"/>.
        /// </summary>
        /// <param name="captureFolder">The folder to create to recording in.</param>
        /// <param name="SaveUserPosition">Whether to record the user position
        /// in a separate file.</param>
        /// <exception cref="Exception">Thrown if initialization of the recording
        /// fails.</exception>
        internal void StartRecording(DirectoryInfo captureFolder, bool SaveUserPosition)
        {
            lock (RUN_CONTROL_LOCK)
            {
                if (!shouldRun || recording)
                {
                    return;               
                }
                else
                {
                    // Change display options to get possibly smooth
                    // recording.W
                    shouldDrawBackground = true;
                    shouldDrawHighlight = false;
                    shouldDrawPixels = false;
                    shouldDrawSkeleton = true;

                    // Setup datastructures for recording.
                    movementData = new ImageDictionary(true);
                    if (SaveUserPosition)
                    {
                        Stream file_stream = new FileStream(
                                Path.Combine(captureFolder.FullName, MAP_FILE_NAME), FileMode.Create);
                        Stream write_stream;
                        if (COMPRESS_USER_DATA)
                        {
                            write_stream = file_stream;
                        }
                        else
                        {
                            write_stream = file_stream;
                        }
                        // A writer for the user data file.
                        userInformationWriter = new BinaryWriter(write_stream);
                    }
                    

                    lock (HARDWARELOCK)
                    {
                        recorder = new Recorder(context);
                        recorder.SetDestination(RecordMedium.File,
                            Path.Combine(captureFolder.FullName, ONI_FILE_NAME));
                        recorder.AddNodeToRecording(imageGenerator);
                        recorder.AddNodeToRecording(depthGenerator);
                    }

                    FileInfo movementDataFileInfo = new FileInfo(
                        Path.Combine(captureFolder.FullName,
                                            USER_ANNOTATION_FILENAME));
                    movementDataFileName = movementDataFileInfo.FullName;
                }
                recording = true;
                onStartedRecording();
            }
        }

        /// <summary>
        /// Stops recording if it is currently active.
        /// </summary>
        /// <exception cref="Exception">Various exceptions could be thrown
        /// while trying to serialize the movement data.</exception>
        internal void StopRecording()
        {
            lock (RUN_CONTROL_LOCK)
            {
                if (!recording) return;
                else
                {
                    lock (HARDWARELOCK)
                    {
                        recording = false;
                        recorder.RemoveNodeFromRecording(imageGenerator);
                        recorder.RemoveNodeFromRecording(depthGenerator);
                        recorder.Dispose();

                        if (userInformationWriter != null)
                        {
                            userInformationWriter.Flush();
                            userInformationWriter.Close();
                        }
                    }

                    XmlSerializer serializer = new XmlSerializer(typeof(ImageDictionary));
                    using (FileStream movementDataStream = File.OpenWrite(movementDataFileName))
                    {
                        try
                        {
                            serializer.Serialize(movementDataStream, movementData);
                            movementDataStream.Flush();
                        }
                        finally
                        { movementDataStream.Close(); }
                    }
                }
                onStoppedRecording();
            }
        }
        #endregion

        /// <summary>
        /// Starts generating images and populating the
        /// <see cref="Image"/>-property. Receive updates on when the image
        /// was updated by subscribing to the
        /// <see cref="NewImageDataAvailable"/>-Event.
        /// </summary>
        public void StartGenerating()
        {
            lock (RUN_CONTROL_LOCK)
            {
                if (shouldRun == false)
                {
                    try
                    {
                        shouldRun = true;
                        context.StartGeneratingAll();
                    }
                    catch (Exception e)
                    {
                        shouldRun = false;
                        onErrorOccured(this, e.Message);
                        return;
                    }
                    readerThread = new Thread(ReaderThread);
                    readerThread.Start();
                }

                onStartedGenerating();
            }
        }

        /// <summary>
        /// Stops generating images.
        /// </summary>
        public void StopGenerating()
        {
            bool stopRecording = false;

            // Check what to do
            lock (RUN_CONTROL_LOCK)
            {
                if (recording)
                {
                    stopRecording = true;
                }
            }
            // Avoid deadlock
            if(stopRecording)
               StopRecording();

            lock (RUN_CONTROL_LOCK)
            {
                if (shouldRun)
                {
                    shouldRun = false;
                    readerThread.Join();
                    try
                    {
                        context.StopGeneratingAll();
                    }
                    catch (Exception e)
                    {
                        onErrorOccured(this, e.Message);
                    }
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
            StopGenerating();
            // Produces an error for whatever reason.
            // TODO Check
            //context.Shutdown();
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
                pollAndProcessOneFrame();
            }
        }

        /// <summary>
        /// Wait for one new frame and process it.
        /// </summary>
        private void pollAndProcessOneFrame()
        {
            try
            {
                lock (HARDWARELOCK)
                {
                    context.WaitAndUpdateAll();
                }
            }
            catch (Exception)
            {
                onErrorOccured(this, "Data could not be read from device.");
                StopGenerating();
                return;
            }

            if (switchImageOrDepth)
                loadImageDataToBitmap();
            else
                loadDepthDataToBitmap();

            lock (HARDWARELOCK)
            {
                if (recording && userInformationWriter != null)
                    saveUserDataToFileStream(userInformationWriter, userGenerator);
            }

            onNewImageDataAvailable(false);
        }

        /// <summary>
        /// Processes the current image sensor data and loads it to the
        /// <see cref="Image"/>-property.
        /// </summary>
        private unsafe void loadImageDataToBitmap()
        {
            lock (ImageLock)
            {
                imageGenerator.GetMetaData(imageMD);

                if (shouldDrawPixels)
                {
                    // If highlights should be drawn or background subtracted
                    // a more difficult (and slower) drawing function is
                    // necessary.
                    if (shouldDrawHighlight || !shouldDrawBackground)
                    {
                        drawImageWithHighlightAndBackgroundSubtraction(
                            imageMD, (ushort*)userGenerator.GetUserPixels(0).LabelMapPtr.ToPointer(),
                            shouldDrawBackground, shouldDrawHighlight, new List<int>());
                    }
                    else
                    {
                        drawImageWithoutHighlightAndBackgroundSubtraction(imageMD);
                    }
                }
                else
                {
                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        // In this case, the image must be cleared
                        if (!shouldDrawPixels)
                        {
                            g.FillRectangle(Brushes.Black,
                                new Rectangle(0, 0, imageMD.XRes, imageMD.YRes));
                        }
                    }
                }
                drawAndLogUserInformation();
            }
        }

        /// <summary>
        /// Processes the current depth sensor data and loads it to the
        /// <see cref="Image"/>-property.
        /// </summary>
        private unsafe void loadDepthDataToBitmap()
        {
            lock (ImageLock)
            {
                // Get the data.
                depthGenerator.GetMetaData(depthMD);
                drawDepthWithHighlightAndBackgroundSubtraction(depthMD,
                    (ushort*)userGenerator.GetUserPixels(0).LabelMapPtr.ToPointer(),
                    shouldDrawBackground, shouldDrawHighlight,
                    histogram, new List<int>());
                drawAndLogUserInformation();
            }
        }

        /// <summary>
        /// Draws the currently available user information
        /// (ID, tracking status) on the Image. If recording, this method
        /// also writes the user joints to the log-datastructure.
        /// </summary>
        private void drawAndLogUserInformation()
        {
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                int[] users = this.userGenerator.GetUsers();
                if(recording)
                    movementData.Add(movementData.Keys.Count,
                                    new Dictionary<int,JointDictionary>());

                foreach (int user in users)
                {
                    if (shouldPrintID)
                    {
                        Point3D com = this.userGenerator.GetCoM(user);
                        com = depthGenerator.ConvertRealWorldToProjective(com);

                        string label = "";
                        if (!this.shouldPrintState)
                            label += user;
                        else if (skeletonCapability.IsTracking(user))
                            label += user + " - Tracking";
                        else if (skeletonCapability.IsCalibrating(user))
                            label += user + " - Calibrating...";
                        else
                            label += user + " - Looking for pose";

                        using (Brush brush = new SolidBrush(ANTICOLORS[user % NCOLORS]))
                        {
                            g.DrawString(label, font_label,
                                brush,
                                com.X - TextRenderer.MeasureText(label, font_label).Width / 2,
                                com.Y);
                        }
                    }
                    if ((shouldDrawSkeleton || recording) &&
                            skeletonCapability.IsTracking(user))
                    {
                        GetJoints(skeletonCapability, user, joints);
                        if (recording)
                        {
                            JointDictionary threeDPositions = new JointDictionary(true);
                            GetJoints3D(skeletonCapability, user, threeDPositions);
                            movementData[movementData.Keys.Count-1].
                                Add(user, threeDPositions);
                        }

                        if (shouldDrawSkeleton)
                            DrawSkeleton(g, ANTICOLORS[user % NCOLORS], joints, 0.5f);
                    }
                }
            }
        }
        #endregion
    }
}
