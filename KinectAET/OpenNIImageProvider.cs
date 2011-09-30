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


#define PARALELLIZED

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using OpenNI;
using System.Windows.Forms;


namespace KinectAET
{
    public abstract class OpenNIImageProvider
    {
        #region Variables
        #region General preferences
        internal static readonly String ONI_FILE_NAME = "KinectRawData.oni";
        internal static readonly String USER_ANNOTATION_FILENAME = "KinectUserPosition.xml";
        internal static readonly String MAP_FILE_NAME = "KinectUserData.raw";


        // The color codes for fancy display
        protected readonly Color[] COLORS = { Color.Red, Color.Blue, Color.ForestGreen,
                                   Color.Yellow, Color.Orange, Color.Purple,
                                   Color.White, Color.Turquoise };
        protected readonly Color[] ANTICOLORS = { Color.Green, Color.Orange, Color.Red,
                                       Color.Purple, Color.Blue, Color.Yellow,
                                       Color.Black, Color.Orange };
        protected readonly int NCOLORS = 7;
        protected Font font_label = new Font("Arial", 15);
        #endregion

        #region Private and Protected
        // The needed OpenNI nodes, capabilities and properties.
        protected Context context;
        protected DepthGenerator depthGenerator;
        protected ImageGenerator imageGenerator;
        protected const int VIDEO_WIDTH = 640, VIDEO_HEIGHT = 480;

        // The per-image updatable information.
        private System.Object HARDWARELOCK = new System.Object();
        private System.Object USERLOCK = new System.Object();
        private System.Object _IMAGELOCK = new System.Object();
        protected readonly Bitmap bitmap = new Bitmap(VIDEO_WIDTH, VIDEO_HEIGHT,
                            System.Drawing.Imaging.PixelFormat.Format24bppRgb);
        private readonly Rectangle rect =
                                new Rectangle(0, 0, VIDEO_WIDTH, VIDEO_HEIGHT);
        protected readonly ImageMetaData imageMD = new ImageMetaData();
        protected readonly DepthMetaData depthMD = new DepthMetaData();
        #endregion

        #region Public Interface
        /// <summary>
        /// Obtain a lock on the image by grouping critical passages in
        /// a code block surrounded by lock(ImageLock).
        /// </summary>
        public object ImageLock
        {
            get
            {
                return _IMAGELOCK;
            }
        }
        /// <summary>
        /// The current available image from the device. For efficiency
        /// reasons, an updated image will be painted to the same resource,
        /// so you must lock the image while working on the image, by using
        /// the <see cref="ImageLock"/>-property.
        /// </summary>
        public Bitmap Image
        {
            get { return bitmap; }
        }
        #endregion
        #endregion

        #region Events and Triggers
        #region Events
        // The interface for getting new image data.
        /// <summary>
        /// This event is triggered when the <see cref="Image"/>-property was
        /// updated.
        /// </summary>
        public event Action<bool> NewImageDataAvailable;
        /// <summary>
        /// This event is triggered, when the kinect starts generating data.
        /// </summary>
        public event Action StartedGenerating;
        /// <summary>
        /// This event is triggered, when the kinect stopped generating data.
        /// </summary>
        public event Action StoppedGenerating;
        /// <summary>
        /// Is triggered if an error occured of the device. Please subscribe
        /// to this event to be informed about errors.
        /// The current error message is offered as string to the method.
        /// </summary>
        public event Action<object, String> ErrorOccured;
        #endregion

        #region Triggers
        /// <summary>
        /// Triggers the ErrorOccured-event.
        /// </summary>
        protected void onErrorOccured(object sender, String message)
        {
            if (ErrorOccured != null)
            {
                ErrorOccured(sender, message);
            }
        }
        /// <summary>
        /// Triggers the NewImageDataAvailable-event.
        /// </summary>
        protected void onNewImageDataAvailable(bool handled)
        {
            if (NewImageDataAvailable != null)
                NewImageDataAvailable(handled);
        }
        /// <summary>
        /// Triggers the StartedGenerating-event.
        /// </summary>
        protected void onStartedGenerating()
        {
            if (StartedGenerating != null)
            {
                StartedGenerating();
            }
        }
        /// <summary>
        /// Triggers the StoppedGenerating-event.
        /// </summary>
        protected void onStoppedGenerating()
        {
            if (StoppedGenerating != null)
                StoppedGenerating();
        }
        #endregion
        #endregion

        #region Visualization
        #region Image Drawing
        /// <summary>
        /// Draw the image from the image sensor without modifications to the
        /// <see cref="Image"/>-property. This method is significantly faster
        /// than the one with these additions.
        /// </summary>
        protected unsafe void drawImageWithoutHighlightAndBackgroundSubtraction(ImageMetaData imageMD)
        {
            BitmapData data = bitmap.LockBits(rect,
                    ImageLockMode.ReadWrite,
                    System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            int stride = data.Stride;

            // set pixels
#if PARALELLIZED
            // Otherwise Parallelization does not work.
            bitmap.UnlockBits(data);

            Parallel.For(0, imageMD.YRes, (y) =>
            {
                byte* pDest = (byte*)data.Scan0.ToPointer() + y * stride;
                byte* pImage = (byte*)imageGenerator.ImageMapPtr.ToPointer() + y * stride;
                for (int x = 0; x < imageMD.XRes; ++x, pDest += 3, pImage += 3)
                {
                    pDest[2] = pImage[0];
                    pDest[1] = pImage[1];
                    pDest[0] = pImage[2];
                }
            });
#else
            try
            {
                byte* pImage = (byte*)imageGenerator.ImageMapPtr.ToPointer();
                byte* pDest;

                for (int y = 0; y < imageMD.YRes; ++y)
                {
                    pDest = (byte*)data.Scan0.ToPointer() + y * stride;
                    for (int x = 0; x < imageMD.XRes; ++x, pDest += 3, pImage += 3)
                    {
                        pDest[2] = pImage[0];
                        pDest[1] = pImage[1];
                        pDest[0] = pImage[2];
                    }
                }
            }
            finally
            {
                bitmap.UnlockBits(data);
            }
#endif
        }

        /// <summary>
        /// Draw the image from the image with optional background subtraction
        /// or user markers to the <see cref="Image"/>-property.
        /// </summary>
        protected unsafe void drawImageWithHighlightAndBackgroundSubtraction(
            ImageMetaData imageMD, ushort[] userInformationMap,
            bool drawBackground, bool drawHighlight, List<int> background_users)
        {
            fixed(ushort* userInformation = userInformationMap)
            {
                drawImageWithHighlightAndBackgroundSubtraction(imageMD, userInformation,
                    drawBackground, drawHighlight, background_users);
            }
        }

        /// <summary>
        /// Draw the image from the image with optional background subtraction
        /// or user markers to the <see cref="Image"/>-property.
        /// </summary>
        protected unsafe void drawImageWithHighlightAndBackgroundSubtraction(
            ImageMetaData imageMD, ushort* userInformation,
            bool drawBackground, bool drawHighlight, List<int> background_users)
        {
            BitmapData data = bitmap.LockBits(rect,
                    ImageLockMode.ReadWrite,
                    System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            int stride = data.Stride;
                // set pixels
#if !PARALELLIZED
            try
            {
                ushort label;
                byte* pImage = (byte*)imageGenerator.ImageMapPtr.ToPointer();
                byte* pDest;
                ushort* pLabels = userInformation;
                for (int y = 0; y < imageMD.YRes; ++y)
                {
                    pDest = (byte*)data.Scan0.ToPointer() + y * stride;
                    for (int x = 0; x < imageMD.XRes; ++x, ++pLabels, pDest += 3, pImage += 3)
                    {
                        pDest[0] = pDest[1] = pDest[2] = 0;

                        label = *pLabels;
                        if (drawBackground || *pLabels != 0 && !background_users.Contains(*pLabels))
                        {
                            Color labelColor = Color.White;
                            if (label != 0 && drawHighlight)
                            {
                                labelColor = COLORS[label % NCOLORS];
                            }

                            pDest[2] = (byte)(pImage[0] * (labelColor.B / 255.0));
                            pDest[1] = (byte)(pImage[1] * (labelColor.G / 255.0));
                            pDest[0] = (byte)(pImage[2] * (labelColor.R / 255.0));
                        }
                    }
                }
            }
            finally
            {
                bitmap.UnlockBits(data);
            }
#else
            bitmap.UnlockBits(data);
            Parallel.For(0, imageMD.YRes, (y) =>
                {
                    ushort label;
                    byte* pImage = (byte*)imageGenerator.ImageMapPtr.ToPointer() + y * stride;
                    ushort* pLabels = userInformation + y * imageMD.XRes;
                    byte* pDest = (byte*)data.Scan0.ToPointer() + y * stride;

                    for (int x = 0; x < imageMD.XRes; ++x, ++pLabels, pDest += 3, pImage += 3)
                    {
                        pDest[0] = pDest[1] = pDest[2] = 0;

                        label = *pLabels;
                        if (drawBackground || *pLabels != 0 && !background_users.Contains(*pLabels))
                        {
                            Color labelColor = Color.White;
                            if (label != 0 && drawHighlight)
                            {
                                labelColor = COLORS[label % NCOLORS];
                            }

                            pDest[2] = (byte)(pImage[0] * (labelColor.B / 256.0));
                            pDest[1] = (byte)(pImage[1] * (labelColor.G / 256.0));
                            pDest[0] = (byte)(pImage[2] * (labelColor.R / 256.0));
                        }
                    }
                });
#endif
        }

        /// <summary>
        /// Draw the image from the depth sensor without optional background subtraction
        /// or user markers to the <see cref="Image"/>-property.
        /// </summary>
        protected unsafe void drawDepthWithoutHighlightAndBackgroundSubtraction(
            DepthMetaData depthMD, int[] histogram)
        {
            // Create a depth histogram.
            CalcHist(depthMD, histogram);

            BitmapData data = bitmap.LockBits(rect, ImageLockMode.ReadWrite,
                        System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            double depthMax = (float)depthGenerator.DeviceMaxDepth;

#if PARALELLIZED
            // Otherwise Parallelization does not work.
            bitmap.UnlockBits(data);

            Parallel.For(0, depthMD.YRes, (y) =>
            {
                ushort* pDepth = (ushort*)this.depthGenerator.DepthMapPtr.ToPointer() + y * depthMD.XRes;
                byte* pDest = (byte*)data.Scan0.ToPointer() + y * data.Stride;
                for (int x = 0; x < depthMD.XRes; ++x, pDest += 3, pDepth++)
                {
                    pDest[0] = pDest[1] = pDest[2] = 0;

                    //byte pixel = (byte)((*pDepth) / depthMax * 255.0);
                    byte pixel = (byte)histogram[*pDepth];
                    pDest[2] = pixel;
                    pDest[1] = pixel;
                    pDest[0] = pixel;
                }
            });
#else
            try
            {
                byte pixel;
                // set pixels
                for (int y = 0; y < depthMD.YRes; ++y)
                {
                    byte* pDest = (byte*)data.Scan0.ToPointer() + y * data.Stride;
                    for (int x = 0; x < depthMD.XRes; ++x, pDest += 3, pDepth++)
                    {
                        pDest[0] = pDest[1] = pDest[2] = 0;

                        //pixel = ((*pDepth) / depthMax * 255.0);
                        pixel = (byte)histogram[*pDepth];
                        pDest[2] = pixel;
                        pDest[1] = pixel;
                        pDest[0] = pixel;
                    }
                }
            }
            finally
            { bitmap.UnlockBits(data); }
#endif
        }

        /// <summary>
        /// Draw the image from the depth sensor with optional background subtraction
        /// or user markers to the <see cref="Image"/>-property.
        /// </summary>
        /// <param name="background_users">Users to regard as background.</param>
        protected unsafe void drawDepthWithHighlightAndBackgroundSubtraction(
            DepthMetaData depthMD, ushort[] userInformationMap,
            bool drawBackground, bool drawHighlight, int[] histogram,
            List<int> background_users)
        {
            fixed (ushort* userInformation = userInformationMap)
            {
                drawDepthWithHighlightAndBackgroundSubtraction(
                    depthMD, userInformation, drawBackground, drawHighlight,
                    histogram, background_users);
            }
        }

        /// <summary>
        /// Draw the image from the depth sensor with optional background subtraction
        /// or user markers to the <see cref="Image"/>-property.
        /// </summary>
        /// <param name="background_users">Users to regard as background.</param>
        protected unsafe void drawDepthWithHighlightAndBackgroundSubtraction(
            DepthMetaData depthMD, ushort* userInformation,
            bool drawBackground, bool drawHighlight, int[] histogram,
            List<int> background_users)
        {
            // Create a depth histogram.
            CalcHist(depthMD, histogram);

            BitmapData data = bitmap.LockBits(rect, ImageLockMode.ReadWrite,
                        System.Drawing.Imaging.PixelFormat.Format24bppRgb);

#if PARALELLIZED
            double depthMax = (float)depthGenerator.DeviceMaxDepth;
            // Otherwise Parallelization does not work.
            bitmap.UnlockBits(data);

            Parallel.For(0, depthMD.YRes, (y) =>
            {
                ushort* pLabels = userInformation + y * depthMD.XRes;
                byte* pDest = (byte*)data.Scan0.ToPointer() + y * data.Stride;
                ushort* pDepth = (ushort*)this.depthGenerator.DepthMapPtr.ToPointer() + y * depthMD.XRes;
                for (int x = 0; x < depthMD.XRes; ++x, ++pLabels, pDest += 3, pDepth++)
                {
                    pDest[0] = pDest[1] = pDest[2] = 0;

                    ushort label = *pLabels;
                    if (drawBackground ||
                        *pLabels != 0 && !background_users.Contains(*pLabels))
                    {
                        Color labelColor = Color.White;
                        if (label != 0 && drawHighlight)
                        {
                            labelColor = COLORS[label % NCOLORS];
                        }

                        double pixel = (byte)histogram[*pDepth];
                        pDest[2] = (byte)(pixel * (labelColor.B / 256.0));
                        pDest[1] = (byte)(pixel * (labelColor.G / 256.0));
                        pDest[0] = (byte)(pixel * (labelColor.R / 256.0));
                    }
                }
            });
#else
            try
            {
                ushort* pDepth = (ushort*)this.depthGenerator.DepthMapPtr.ToPointer();
                ushort* pLabels = userInformation;
                double pixel;
                Color labelColor;
                ushort label;
                double depthMax = (float)depthGenerator.DeviceMaxDepth;

                // set pixels
                for (int y = 0; y < depthMD.YRes; ++y)
                {
                    byte* pDest = (byte*)data.Scan0.ToPointer() + y * data.Stride;
                    for (int x = 0; x < depthMD.XRes; ++x, ++pLabels, pDest += 3, pDepth++)
                    {
                        pDest[0] = pDest[1] = pDest[2] = 0;

                        label = *pLabels;
                        if (drawBackground ||
                            *pLabels != 0 && !background_users.Contains(*pLabels))
                        {
                            labelColor = Color.White;
                            if (label != 0 && drawHighlight)
                            {
                                labelColor = COLORS[label % NCOLORS];
                            }

                            //pixel = ((*pDepth) / depthMax * 255.0);
                            pixel = (byte)histogram[*pDepth];
                            pDest[2] = (byte)(pixel * (labelColor.B / 256.0));
                            pDest[1] = (byte)(pixel * (labelColor.G / 256.0));
                            pDest[0] = (byte)(pixel * (labelColor.R / 256.0));
                        }
                    }
                }
            }
            finally
            {
                bitmap.UnlockBits(data);
            }
#endif
        }

        /// <summary>
        /// Draws the skeleton in the current image for the specified user.
        /// </summary>
        protected void DrawSkeleton(Graphics g, Color color, 
            Dictionary<SkeletonJoint, SkeletonJointPosition> joints,
            double confidenceThreshold)
        {
            DrawLine(g, color, joints, SkeletonJoint.Head, SkeletonJoint.Neck, confidenceThreshold);

            DrawLine(g, color, joints, SkeletonJoint.Neck, SkeletonJoint.LeftShoulder, confidenceThreshold);
            DrawLine(g, color, joints, SkeletonJoint.LeftShoulder, SkeletonJoint.LeftElbow, confidenceThreshold);
            DrawLine(g, color, joints, SkeletonJoint.LeftElbow, SkeletonJoint.LeftHand, confidenceThreshold);

            DrawLine(g, color, joints, SkeletonJoint.Neck, SkeletonJoint.RightShoulder, confidenceThreshold);
            DrawLine(g, color, joints, SkeletonJoint.RightShoulder, SkeletonJoint.RightElbow, confidenceThreshold);
            DrawLine(g, color, joints, SkeletonJoint.RightElbow, SkeletonJoint.RightHand, confidenceThreshold);

            DrawLine(g, color, joints, SkeletonJoint.LeftHip, SkeletonJoint.RightShoulder, confidenceThreshold);
            DrawLine(g, color, joints, SkeletonJoint.RightHip, SkeletonJoint.LeftShoulder, confidenceThreshold);
            DrawLine(g, color, joints, SkeletonJoint.LeftHip, SkeletonJoint.Waist, confidenceThreshold);
            DrawLine(g, color, joints, SkeletonJoint.Waist, SkeletonJoint.RightHip, confidenceThreshold);

            DrawLine(g, color, joints, SkeletonJoint.LeftHip, SkeletonJoint.LeftKnee, confidenceThreshold);
            DrawLine(g, color, joints, SkeletonJoint.LeftKnee, SkeletonJoint.LeftFoot, confidenceThreshold);

            DrawLine(g, color, joints, SkeletonJoint.RightHip, SkeletonJoint.RightKnee, confidenceThreshold);
            DrawLine(g, color, joints, SkeletonJoint.RightKnee, SkeletonJoint.RightFoot, confidenceThreshold);
        }

        /// <summary>
        /// Loads all 2D joint positions for a specific user.
        /// </summary>
        protected void GetJoints(SkeletonCapability source,
            int user, Dictionary<SkeletonJoint, SkeletonJointPosition> target)
        {
            for (int i = 0; i < JointDictionary.JointTranslationList.Count; i++)
            {
                GetJoint(source, user, JointDictionary.JointTranslationList[i].joint_type, target);
            }
        }

        /// <summary>
        /// Loads all 3D joint positions for a specific user.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown if the target dictionary
        /// does not contain 3d data.</exception>
        protected void GetJoints3D(SkeletonCapability source,
            int user, JointDictionary target)
        {
            if (!target.Is3DData)
                throw new ArgumentException("Trying to load 3d data to a non 3D JointDictionary.");

            for (int i = 0; i < JointDictionary.JointTranslationList.Count; i++)
            {
                GetJoint3D(source, user, JointDictionary.JointTranslationList[i].joint_type, target);
            }
        }

        #endregion

        #region Helper methods
        /// <summary>
        /// Calculates a special cumulative histogram that nicely transports
        /// depth visualization.
        /// </summary>
        private unsafe void CalcHist(DepthMetaData depthMD, int[] histogram)
        {
            // reset
            for (int i = 0; i < histogram.Length; ++i)
                histogram[i] = 0;

            ushort* pDepth = (ushort*)depthMD.DepthMapPtr.ToPointer();

            int points = 0;
            for (int y = 0; y < depthMD.YRes; ++y)
            {
                for (int x = 0; x < depthMD.XRes; ++x, ++pDepth)
                {
                    ushort depthVal = *pDepth;
                    if (depthVal != 0)
                    {
                        histogram[depthVal]++;
                        points++;
                    }
                }
            }

            for (int i = 1; i < histogram.Length; i++)
            {
                histogram[i] += histogram[i - 1];
            }

            if (points > 0)
            {
                for (int i = 1; i < histogram.Length; i++)
                {
                    histogram[i] = (int)(256 * (1.0f - (histogram[i] / (float)points)));
                }
            }
        }

        /// <summary>
        /// Connects the to provided joints with each other.
        /// </summary>
        private void DrawLine(Graphics g, Color color,
            Dictionary<SkeletonJoint, SkeletonJointPosition> dict,
            SkeletonJoint j1, SkeletonJoint j2,
            double confidenceThreshold)
        {
            if (!(dict.ContainsKey(j1) && dict.ContainsKey(j2)))
                return;

            Point3D pos1 = dict[j1].Position;
            Point3D pos2 = dict[j2].Position;
            int marker_rect_half_width = 3;
            int marker_rect_half_height = 3;
            bool draw_line = true;
            Brush marker_brush;

            // Draw the threshold markers
            if (!(pos1.X.Equals(float.NaN) || pos1.Y.Equals(float.NaN)))
            {
                if (dict[j1].Confidence < confidenceThreshold)
                    marker_brush = Brushes.Red;
                else
                    marker_brush = Brushes.Green;

                g.FillEllipse(marker_brush,
                    new Rectangle((int)pos1.X - marker_rect_half_width,
                                    (int)pos1.Y - marker_rect_half_height,
                                    marker_rect_half_width * 2 + 1,
                                    marker_rect_half_height * 2 + 1));
            }
            else draw_line = false;

            if (!(pos2.X.Equals(float.NaN) || pos2.Y.Equals(float.NaN)))
            {
                if (dict[j2].Confidence < confidenceThreshold)
                    marker_brush = Brushes.Red;
                else
                    marker_brush = Brushes.Green;

                g.FillEllipse(marker_brush,
                    new Rectangle((int)pos2.X - marker_rect_half_width,
                                    (int)pos2.Y - marker_rect_half_height,
                                    marker_rect_half_width * 2 + 1,
                                    marker_rect_half_height * 2 + 1));
            }
            else draw_line = false;

            if (draw_line)
            {
                using (Pen pen = new Pen(color))
                {
                    g.DrawLine(pen,
                                new Point((int)pos1.X, (int)pos1.Y),
                                new Point((int)pos2.X, (int)pos2.Y));
                }
            }
        }

        /// <summary>
        /// Loads the 2D data for a specific skeleton joint.
        /// </summary>
        private void GetJoint(SkeletonCapability source,
                        int user, SkeletonJoint joint,
                       Dictionary<SkeletonJoint, SkeletonJointPosition> target)
        {
            SkeletonJointPosition pos;
            if (joint == SkeletonJoint.Waist)
            {
                // Calculate the joint position as arithmetic mean of right
                // and left hip joints, as it is not possible to poll it
                // directly.

                pos = new SkeletonJointPosition();

                SkeletonJointPosition posLeft = source.GetSkeletonJointPosition(user, SkeletonJoint.LeftHip);
                SkeletonJointPosition posRight = source.GetSkeletonJointPosition(user, SkeletonJoint.RightHip);

                if (posLeft.Position.Z == 0 || posRight.Position.Z == 0)
                {
                    pos.Confidence = 0;
                    pos.Position = new Point3D(
                        (posLeft.Position.X + posRight.Position.X) / 2,
                        (posLeft.Position.Y + posRight.Position.Y) / 2,
                        0);
                }
                else
                {
                    pos.Confidence = Math.Min(posLeft.Confidence, posRight.Confidence);
                    pos.Position = depthGenerator.ConvertRealWorldToProjective(
                        new Point3D(
                        (posLeft.Position.X + posRight.Position.X) / 2,
                        (posLeft.Position.Y + posRight.Position.Y) / 2,
                        (posLeft.Position.Z + posRight.Position.Z) / 2));
                }
            }
            else
            {
                pos = source.GetSkeletonJointPosition(user, joint);
                if (pos.Position.Z == 0)
                {
                    pos.Confidence = 0;
                }
                else
                {
                    pos.Position = depthGenerator.ConvertRealWorldToProjective(pos.Position);
                }
            }

            target[joint] = pos;
        }

        /// <summary>
        /// Loads the 3D data for a specific skeleton joint.
        /// </summary>
        private void GetJoint3D(SkeletonCapability source,
                        int user, SkeletonJoint joint,
                       JointDictionary target)
        {
            SkeletonJointPosition pos;
            if (joint == SkeletonJoint.Waist)
            {
                // Calculate the joint position as arithmetic mean of right
                // and left hip joints, as it is not possible to poll it
                // directly.

                pos = new SkeletonJointPosition();

                SkeletonJointPosition posLeft = source.GetSkeletonJointPosition(user, SkeletonJoint.LeftHip);
                SkeletonJointPosition posRight = source.GetSkeletonJointPosition(user, SkeletonJoint.RightHip);

                if (posLeft.Position.Z == 0 || posRight.Position.Z == 0)
                {
                    pos.Confidence = 0;
                    pos.Position = new Point3D(
                        (posLeft.Position.X + posRight.Position.X) / 2,
                        (posLeft.Position.Y + posRight.Position.Y) / 2,
                        0);
                }
                else
                {
                    pos.Confidence = Math.Min(posLeft.Confidence, posRight.Confidence);
                    pos.Position = new Point3D(
                        (posLeft.Position.X + posRight.Position.X) / 2,
                        (posLeft.Position.Y + posRight.Position.Y) / 2,
                        (posLeft.Position.Z + posRight.Position.Z) / 2);
                }
            }
            else
            {
                pos = source.GetSkeletonJointPosition(user, joint);
                if (pos.Position.Z == 0)
                {
                    pos.Confidence = 0;
                }
            }
            target[joint] = pos;
        }

        #endregion
        #endregion

        #region Generation Control
        /// <summary>
        /// Cleans up resources and shuts down the generating process. After
        /// calling this method, no further video can be polled.
        /// </summary>
        public abstract void Shutdown();
        #endregion

        #region Helper methods

        /// <summary>
        /// Converts a JointDictionary with 3D data to the corresponding one
        /// with 2D data. Note that the Z-Coordinate is left unchanged, though
        /// it has no meaning in the resulting 2D space and can be seen as 0.
        /// </summary>
        public JointDictionary Convert3Dto2D(JointDictionary source, DepthGenerator generator)
        {
            if (!source.Is3DData)
                return new JointDictionary(source);
            else
            {
                JointDictionary ret = new JointDictionary(false);

                foreach (SkeletonJoint joint in source.Keys)
                {
                    SkeletonJointPosition pos = new SkeletonJointPosition();
                    pos.Confidence = source[joint].Confidence;
                    pos.Position = generator.ConvertRealWorldToProjective(source[joint].Position);
                    ret.Add(joint, pos);
                }
                return ret;
            }
        }

        /// <summary>
        /// Saves the labelmap with the provided stream.
        /// </summary>
        protected unsafe void saveUserDataToFileStream(BinaryWriter userInformationWriter, UserGenerator userGenerator)
        {
            ushort* pLabels = (ushort*)userGenerator.GetUserPixels(0).LabelMapPtr.ToPointer();

            // At one position, only three bits of the 16 actual ushort bits
            // are used to indicate the user id. So by combining four of these
            // values, it is possible to write three bytes to the stream.
            // This might be implemented for a future release.
            
            for (int y = 0; y < imageMD.YRes; ++y)
            {
                for (int x = 0; x < imageMD.XRes; ++x)
                {
                    userInformationWriter.Write(*pLabels++);
                }
            }
        }

        /// <summary>
        /// Loads the labelmap from the given stream.
        /// </summary>
        /// <returns>The labelmap as array.</returns>
        /// <exception cref="Exception">Might occur if reading fails.</exception>
        protected ushort[] loadUserDataFromFileStream(
            BinaryReader userInformationReader, int pixels)
        {
            ushort[] userMap = new ushort[pixels];

            for (int i = 0; i < pixels; ++i)
            {
                    userMap[i] = userInformationReader.ReadUInt16();
            }
            return userMap;
        }

        /// <summary>
        /// Tries to create a folder in the containing folder with a name
        /// starting with the given prefix an with a higher number after the
        /// last separator in the prefix string, than the highest one present
        /// in the containing folder. Returns the DirectoryInfo for this folder.
        /// </summary>
        /// <exception>Thrown if such a folder couldnt be created or found.
        /// </exception>
        internal static DirectoryInfo GetNewFolderWithHigherIndex(DirectoryInfo containing,
                                            String prefix, String last_separator)
        {
            IEnumerable<DirectoryInfo> existingDirs =
                    containing.EnumerateDirectories(prefix + "*");
            int initial = 1;
            int number;
            foreach (DirectoryInfo dir in existingDirs)
            {
                try
                {
                    number = Int32.Parse(dir.Name.Substring(dir.Name.LastIndexOf(last_separator) + 1));
                }
                catch (Exception)
                { continue; }

                if (number >= initial)
                {
                    initial = number + 1;
                }
            }
            String newDirName = Path.Combine(containing.FullName, prefix + initial);
            if (Directory.Exists(newDirName))
            {
                throw new Exception("Could not initialize a folder in the project directory to store the captured data.");
            }

            return Directory.CreateDirectory(newDirName);
        }
        #endregion
    }
}
