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
using System.Drawing;
using System.Windows.Forms;

namespace KAET
{
    /// <summary>
    /// This control allows rendering of the video image outputs of an Kinect
    /// or Kinect emulated source. It will render as black if the source
    /// is set to null. It does neither start or stop the generation process
    /// of the KinectControl object.
    /// </summary>
    public partial class OpenNIImageDisplay : Control
    {
        #region Variables
        private OpenNIImageProvider imageSource = null;
        private Action startedHandler;
        private Action stoppedHandler;
        private Action<bool> newImageDataHandler;

        /// <summary>
        /// This KinectControl is the source for the images. If set to null,
        /// this control renders as black square.
        /// </summary>
        public OpenNIImageProvider Source
        {
            get { return imageSource; }
            set
            {
                if (imageSource != null)
                {
                    imageSource.StartedGenerating -= startedHandler;
                    imageSource.StoppedGenerating -= stoppedHandler;
                    imageSource.NewImageDataAvailable -= newImageDataHandler;
                    imageSource.ErrorOccured -= errorHandler;
                }
                imageSource = value;
                if (value != null)
                {
                    imageSource.StartedGenerating += startedHandler;
                    imageSource.StoppedGenerating += stoppedHandler;
                    imageSource.NewImageDataAvailable += newImageDataHandler;
                    imageSource.ErrorOccured += errorHandler;
                }
                Invalidate();
                onSourceChanged();
            }
        }
        /// <summary>
        /// This event is raised, when the image source changed.
        /// </summary>
        public event EventHandler SourceChanged;
        /// <summary>
        /// Raises the corresponding event.
        /// </summary>
        private void onSourceChanged()
        {
            if (SourceChanged != null)
            {
                SourceChanged(this, new EventArgs());
            }
        }
        /// <summary>
        /// This event is raised, when an error occured.
        /// </summary>
        public event Action<object, String> ErrorOccured;
        /// <summary>
        /// Raises the corresponding event.
        /// </summary>
        private void onErrorOccured(object sender, String message)
        {
            if (ErrorOccured != null)
            {
                ErrorOccured(sender, message);
            }
        }
        /// <summary>
        /// This event is raised, when the playing images stopped.
        /// </summary>
        public event EventHandler StoppedPlaying;
        /// <summary>
        /// Raises the corresponding event.
        /// </summary>
        private void onStoppedPlaying()
        {
            if (StoppedPlaying != null)
            {
                StoppedPlaying(this, new EventArgs());
            }
        }
        /// <summary>
        /// This event is raised, when the playing images started.
        /// </summary>
        public event EventHandler StartedPlaying;
        /// <summary>
        /// Raises the corresponding event.
        /// </summary>
        private void onStartedPlaying()
        {
            if (StartedPlaying != null)
            {
                StartedPlaying(this, new EventArgs());
            }
        }
        /// <summary>
        /// Raised when a new frame is displayed, but only when playing a
        /// recording.
        /// </summary>
        public event Action<object, bool> NewRecordingFrameDisplayed;
        /// <summary>
        /// Raises the corresponding event.
        /// </summary>
        private void onNewRecordingFrameDisplayed(bool handled)
        {
            if (NewRecordingFrameDisplayed != null)
                NewRecordingFrameDisplayed(this, handled);
        }
        #endregion

        #region Constructor
        public OpenNIImageDisplay()
        {
            InitializeComponent();

            startedHandler = handleStartedGenerating;
            stoppedHandler = handleStoppedGenerating;
            newImageDataHandler = handleNewImageDataAvailable;
        }
        #endregion

        #region Event handlers

        protected void handleStartedGenerating()
        {
            BeginInvoke((MethodInvoker)delegate
            {
                onStartedPlaying();
            });
        }

        protected void handleStoppedGenerating() {
            BeginInvoke((MethodInvoker)delegate
            {
                onStoppedPlaying();
            });
        }

        protected void handleNewImageDataAvailable(bool handled) 
        {
            Invalidate();

            if (Source is OpenNIRecordingController)
                BeginInvoke((MethodInvoker)delegate
                {
                    onNewRecordingFrameDisplayed(handled);
                });
        }

        protected void errorHandler(object sender, String message)
        {
            BeginInvoke((MethodInvoker)delegate
            {
                onErrorOccured(sender, message);
            });
        }
        #endregion

        #region Paint methods
        protected override void OnPaintBackground(PaintEventArgs e)
        {        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (imageSource != null)
            {
                lock (imageSource.ImageLock)
                {
                    e.Graphics.DrawImage(imageSource.Image,
                        0, 0,
                        Size.Width,
                        Size.Height);
                }
            }
            else
            {
                e.Graphics.FillRectangle(Brushes.Black,
                    new Rectangle(0, 0, Size.Width, Size.Height));
            }
        }
        #endregion
    }
}
