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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace KAET
{
    public partial class ExportConfiguration : Form
    {
        #region Properties
        private readonly String USER_PREFIX = "User ";

        /// <summary>
        /// Gets the selected users from the dialog.
        /// </summary>
        public List<int> SelectedUsers
        {
            get
            {
                List<int> ret = new List<int>();
                foreach (String user in checkedListBoxUserSelection.CheckedItems)
                {
                    String to_parse = user.Substring(USER_PREFIX.Length);
                    to_parse = to_parse.Substring(0, to_parse.IndexOf(" "));
                    int userid = Int32.Parse(to_parse);
                    if (!ret.Contains(userid))
                        ret.Add(userid);
                }
                return ret;
            }
        }
        /// <summary>
        /// Whether the user selected to get annotations in 2D.
        /// </summary>
        public bool AnnotationIn2D
        {
            get { return checkBox2D.Checked; }
            set
            {  checkBox2D.Checked = value;    }
        }
        /// <summary>
        /// Whether the user selected to get annotations in 3D.
        /// </summary>
        public bool AnnotationIn3D
        {
            get { return checkBox3D.Checked; }
            set
            { checkBox3D.Checked = value; }
        }
        /// <summary>
        /// Whether the user selected to get relative path names.
        /// </summary>
        public bool ExportRelativePathNames
        {
            get { return checkBoxRelativePath.Checked; }
            set { checkBoxRelativePath.Checked = value; }
        }
        /// <summary>
        /// Whether the user selected to get image sensor data exported.
        /// </summary>
        public bool ImageSensorData
        {
            get { return radioButtonImage.Checked; }
            set
            {
                if (value)
                    radioButtonImage.Checked = true;
                else
                    radioButtonDepth.Checked = true;
            }
        }
        /// <summary>
        /// Whether the user selected to draw the background.
        /// </summary>
        public bool DrawBackground
        {
            get { return checkBoxExportBackground.Checked; }
            set { checkBoxExportBackground.Checked = value; }
        }
        /// <summary>
        /// Whether the user selected to get the skeleton drawn.
        /// </summary>
        public bool DrawSkeleton
        {
            get { return checkBoxDrawSkeleton.Checked; }
            set { checkBoxDrawSkeleton.Checked = value; }
        }
        /// <summary>
        /// Whether the user selected to get the highlights drawn.
        /// </summary>
        public bool DrawHighlight
        {
            get { return checkBoxDrawHighlight.Checked; }
            set { checkBoxDrawHighlight.Checked = value; }
        }
        /// <summary>
        /// Whether the user selected to get user labels drawn.
        /// </summary>
        public bool DrawLabels
        {
            get { return checkBoxUserLabel.Checked; }
            set { checkBoxUserLabel.Checked = value; }
        }

        private bool _doBatchExport = false;
        /// <summary>
        /// Whether to do a batch export of the most common image types.
        /// </summary>
        public bool DoBatchExport
        {
            get { return _doBatchExport; }
        }
        #endregion

        /// <summary>
        /// Initializes the export configuration dialog with the given parameters.
        /// </summary>
        public ExportConfiguration(Dictionary<int,int> UserStatistics, List<int> frames)
        {
            InitializeComponent();

            // Initialize frame status.
            labelFramesSelected.Text = frames.Count +
                (frames.Count == 1 ? " Frame" : " Frames") +
                " selected for export.";

            // Initialize user selection box.
            foreach (int user in UserStatistics.Keys)
            {
                checkedListBoxUserSelection.Items.Add(USER_PREFIX + user +
                                " (in " + UserStatistics[user] +
                                (frames.Count == 1 ? " Frame" : " Frames") + ")");
            }
        }

        private void buttonBatchExport_Click(object sender, EventArgs e)
        {
            _doBatchExport = true;
            this.AcceptButton.PerformClick();
        }
    }
}
