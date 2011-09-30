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
using System.Globalization;
using System.IO;
using System.Xml.Serialization;
using OpenNI;
using System.Windows.Forms.DataVisualization.Charting;

namespace KinectAET
{
    public partial class Evaluation : Form
    {
        #region Variables
        private static readonly CultureInfo NUMBER_CULTURE =
            CultureInfo.CreateSpecificCulture("en-US");
         
        private readonly OpenNIImageDisplay videoDataSource;
        private ImageDictionary resultDictionary;
        private DirectoryInfo current_take_dir;
        private Statistics statistics;
        private static readonly String USER_STRING = "User ";

        private readonly XmlSerializer movementDataSerializer =
                            new XmlSerializer(typeof(ImageDictionary));

        private double mediaSliderValueFactor = 5.0 / 100.0;
        private List<SkeletonJoint> activeJoints = new List<SkeletonJoint>();
        private Dictionary<SkeletonJoint, List<Annotation>> annotations =
            new Dictionary<SkeletonJoint, List<Annotation>>();

        private int min, max;

        // Annotation colors
        private Color markerColorFalsePositive = Color.Red;
        private Color markerColorFalseNegative = Color.Red;
        private Color markerColorNA = Color.Blue;

        // Variables for fast frame position indicator change-hack.
        private VerticalLineAnnotation frame_indicator;
        private List<int> xPositions = new List<int>();
        private int ymax = 0;
        private int ynull = 0;
        private Bitmap former_image;
        private int former_pos;
        private bool former_image_invalid = false;
        #endregion

        #region Event system
        #region Statistic event handlers
        internal void statistics_FrameUpdated(int obj)
        {
            FrameUpdated(obj, true);
        }

        private void clearErrorPoints()
        {
            foreach (Series series in chartErrors.Series)
            {
                series.Points.Clear();

                List<Annotation> remove_list = annotations[JointDictionary.GetTranslation(series.Name).joint_type];
                foreach (Annotation annotation in remove_list)
                {
                    chartErrors.Annotations.Remove(annotation);
                    annotation.Dispose();
                }
                remove_list.Clear();
            }
        }

        private void clearCumulativeConfidencePoints()
        {
            chartCumulativeConfidence.Series[0].Points.Clear();
        }

        private void addErrorPoint(SkeletonJoint series, int x, double y, Statistics.MarkerType markerType)
        {
            String seriesName = JointDictionary.GetTranslation(series).name;
            DataPoint newPoint = new DataPoint(x, y);

            if (markerType == Statistics.MarkerType.FalseNegative)
            {
                EllipseAnnotation marker = new EllipseAnnotation();
                marker.AnchorDataPoint = newPoint;
                marker.AnchorAlignment = ContentAlignment.MiddleCenter;
                marker.Width = 0.8;
                marker.Height = 1.6;
                marker.BackColor = markerColorFalsePositive;
                marker.LineColor = Color.Black;
                marker.LineWidth = 1;

                AnnotationSmartLabelStyle markerLocation = new AnnotationSmartLabelStyle();
                markerLocation.IsMarkerOverlappingAllowed = true;
                markerLocation.IsOverlappedHidden = false;
                markerLocation.MaxMovingDistance = 0;
                markerLocation.MinMovingDistance = 0;
                marker.SmartLabelStyle = markerLocation;
                chartErrors.Annotations.Add(marker);
                annotations[series].Add(marker);
            }
            if (markerType == Statistics.MarkerType.FalsePositive)
            {
                RectangleAnnotation marker = new RectangleAnnotation();
                marker.AnchorDataPoint = newPoint;
                marker.AnchorAlignment = ContentAlignment.MiddleCenter;
                marker.Width = 0.8;
                marker.Height = 1.6;
                marker.BackColor = markerColorFalseNegative;
                marker.LineColor = Color.Black;
                marker.LineWidth = 1;

                AnnotationSmartLabelStyle markerLocation = new AnnotationSmartLabelStyle();
                markerLocation.IsMarkerOverlappingAllowed = true;
                markerLocation.IsOverlappedHidden = false;
                markerLocation.MaxMovingDistance = 0;
                markerLocation.MinMovingDistance = 0;
                marker.SmartLabelStyle = markerLocation;
                chartErrors.Annotations.Add(marker);
                annotations[series].Add(marker);
            }
            if (markerType == Statistics.MarkerType.NotAvailable)
            {
                EllipseAnnotation marker = new EllipseAnnotation();
                marker.AnchorDataPoint = newPoint;
                marker.AnchorAlignment = ContentAlignment.MiddleCenter;
                marker.Width = 0.8;
                marker.Height = 1.6;
                marker.BackColor = markerColorNA;
                marker.LineColor = Color.Black;
                marker.LineWidth = 1;

                AnnotationSmartLabelStyle markerLocation = new AnnotationSmartLabelStyle();
                markerLocation.IsMarkerOverlappingAllowed = true;
                markerLocation.IsOverlappedHidden = false;
                markerLocation.MaxMovingDistance = 0;
                markerLocation.MinMovingDistance = 0;
                marker.SmartLabelStyle = markerLocation;
                chartErrors.Annotations.Add(marker);
                annotations[series].Add(marker);
            }
            Series insertionSeries = chartErrors.Series[seriesName];
            int insertion_index = 0;
            for (int index = 0; index < insertionSeries.Points.Count; index++)
            {
                if (insertionSeries.Points[index].XValue > x) break;
                else insertion_index++;
            }

            chartErrors.Series[seriesName].Points.Insert(insertion_index, newPoint);
        }

        private void addConfidencePoint(double x, double y)
        {
            chartCumulativeConfidence.Series[0].Points.Add(new DataPoint(x, y));
        }

        private void finalizeMainPointUpdate()
        {
            chartErrors.ChartAreas[0].RecalculateAxesScale();
        }

        private void finalizeCumulativeConfidenceUpdate()
        {
            chartCumulativeConfidence.ChartAreas[0].RecalculateAxesScale();
        }
        #endregion

        #region UI component handlers
        private void listBoxUser_SelectedIndexChanged(object sender, EventArgs e)
        {
            OpenNIRecordingController source = (videoDataSource.Source as OpenNIRecordingController);

            if ((sender as ListBox) != null &&
                (sender as ListBox) == listBoxUserResults &&
                source != null)
            {
                source.AdditionalSkeletonIDs.Clear();

                if (listBoxUserResults.SelectedItem != null)
                {
                    int uid_results = parseUserID((String)listBoxUserResults.SelectedItem);
                    source.AdditionalSkeletonIDs.Add(uid_results);
                }
                source.RequestUpdate(true);
            }

            if (listBoxUserResults.SelectedItem != null &&
                listBoxUserTruth.SelectedItem != null)
            {
                // Parse the User IDs from the items.
                int uid_truth = parseUserID((String)listBoxUserTruth.SelectedItem);
                int uid_results = parseUserID((String)listBoxUserResults.SelectedItem);

                // Put the data into the statistics object.
                statistics.SetData(
                    source.UserLocationInformation,
                    resultDictionary, uid_truth, uid_results,
                    new Statistics.To2DConverter(source.Convert3Dto2D));

                // Initialize
                checkedListBoxBodyParts.Enabled = true;
                buttonBodyPartsAll.Enabled = true;
                buttonBodyPartsNone.Enabled = true;
                mediaSliderConfidenceThreshold.Enabled = true;
                selectAllBodyJoints();
                statistics.Calculate(getActiveJoints(),
                    mediaSliderConfidenceThreshold.Value * mediaSliderValueFactor,
                    true, source.CurrentFrame);
            }
            else
            {
                clearBodyJointList();
                checkedListBoxBodyParts.Enabled = false;
                buttonBodyPartsAll.Enabled = false;
                buttonBodyPartsNone.Enabled = false;
                mediaSliderConfidenceThreshold.Enabled = false;
                mediaSliderConfidenceThreshold.Value = 0;
            }
        }

        private void buttonBodyPartsAll_Click(object sender, EventArgs e)
        {
            selectAllBodyJoints();
        }

        private void buttonBodyPartsNone_Click(object sender, EventArgs e)
        {
            clearBodyJointList();
        }

        private void checkedListBoxBodyParts_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            SkeletonJoint changedJoint = JointDictionary.GetTranslation((String)checkedListBoxBodyParts.Items[e.Index]).joint_type;
            chartErrors.Series[(String)checkedListBoxBodyParts.Items[e.Index]].Enabled = (e.NewValue == CheckState.Checked);
            foreach (Annotation annotation in annotations[changedJoint])
            {
                annotation.Visible = e.NewValue == CheckState.Checked;
            }

            if (activeJoints.Contains(changedJoint))
            {
                if (e.NewValue != CheckState.Checked)
                    activeJoints.Remove(changedJoint);
            }
            else
            {
                if (e.NewValue == CheckState.Checked)
                    activeJoints.Add(changedJoint);
            }

            OpenNIRecordingController video = (videoDataSource.Source as OpenNIRecordingController);
            if (video != null)
                statistics.Calculate(getActiveJoints(),
                    (double)mediaSliderConfidenceThreshold.Value * mediaSliderValueFactor,
                    false, video.CurrentFrame);
        }

        private void mediaSliderConfidenceThreshold_ValueChanged(object sender, EventArgs e)
        {
            OpenNIRecordingController video = (videoDataSource.Source as OpenNIRecordingController);
            if (video != null)
            {
                video.ConfidenceThreshold = (double)mediaSliderConfidenceThreshold.Value * mediaSliderValueFactor;
                video.RequestUpdate(true);
            }

            List<SkeletonJoint> enabledJoints = getActiveJoints();

            statistics.Calculate(enabledJoints,
                (double)mediaSliderConfidenceThreshold.Value * mediaSliderValueFactor,
                true, video.CurrentFrame);
        }

        private void Evaluation_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                listBoxResultDataSets.ClearSelected();
                listBoxResultDataSets.Items.Clear();
                listBoxResultDataSets.Enabled = false;

                listBoxUserTruth.ClearSelected();
                listBoxUserTruth.Items.Clear();
                listBoxUserTruth.Enabled = false;
                Hide();
            }
        }

        private void listBoxResultDataSets_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBoxUserResults.ClearSelected();
            listBoxUserResults.Items.Clear();
            onEvaluationRangeChanged(-1, -1);
            OpenNIRecordingController source = (videoDataSource.Source as OpenNIRecordingController);
            if (source != null)
            {
                source.AdditionalSkeletonInformation = null;
                source.AdditionalSkeletonIDs.Clear();
                source.RequestUpdate(true);
            }

            if (listBoxResultDataSets.SelectedItem == null)
            {
                listBoxUserResults.Enabled = false;
            }
            else
            {
                String resultfilename = Path.Combine(current_take_dir.FullName, (String)listBoxResultDataSets.SelectedItem);
                try
                {
                    using (FileStream file = File.Open(resultfilename, FileMode.Open))
                    {
                        resultDictionary = (ImageDictionary)movementDataSerializer.Deserialize(file);
                    }
                    Dictionary<int, int> userActivities = resultDictionary.GetUserStatistics(null);
                    foreach (int userID in userActivities.Keys)
                    {
                        listBoxUserResults.Items.Add("User " + userID);
                    }

                    if (source != null)
                    {
                        source.AdditionalSkeletonInformation = resultDictionary;
                        onEvaluationRangeChanged(resultDictionary.Keys.Min(),
                                                    resultDictionary.Keys.Max());
                    }

                    listBoxUserResults.Enabled = true;
                }
                catch (Exception ex)
                {
                    listBoxUserResults.ClearSelected();
                    listBoxUserResults.Items.Clear();
                    listBoxUserResults.Enabled = false;
                    MessageBox.Show(this,
                        "An Error occured loading the results file:\n" + ex.Message,
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void Evaluation_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
            {
                reloadSourceList();
                reloadUserTruthList();
            }
        }
        #endregion

        public event Action<int, int> EvaluationRangeChanged;
        private void onEvaluationRangeChanged(int start, int end)
        {
            if (EvaluationRangeChanged != null)
                EvaluationRangeChanged(start, end);
        }

        void videoDataSource_SourceChanged(object sender, EventArgs e)
        {
            if (!Visible)
                return;

            if (videoDataSource.Source == null ||
                videoDataSource.Source is OpenNILiveStreamController)
                Close();
            else
                reloadSourceList();
            reloadUserTruthList();
        }
        #endregion

        #region Constructor
        public Evaluation(OpenNIImageDisplay videoControl)
        {
            InitializeComponent();

            // Initialize Statistics
            statistics = new Statistics();
            statistics.ClearCumulativeConfidencePoints += clearCumulativeConfidencePoints;
            statistics.AddCumulativeConfidencePoint += addConfidencePoint;
            statistics.ClearErrorPoints += clearErrorPoints;
            statistics.AddErrorPoint += addErrorPoint;
            statistics.FinalizeErrorUpdate += finalizeMainPointUpdate;
            statistics.FinalizeConfidenceUpdate += finalizeCumulativeConfidenceUpdate;
            statistics.FrameUpdated += statistics_FrameUpdated;
            videoControl.NewRecordingFrameDisplayed += UpdateFrame;

            // Add data Bindings
            textBoxGlobalMean.DataBindings.Add("Text", statistics, "GlobalMean",
                true, DataSourceUpdateMode.Never, 0, "0.000",
                NUMBER_CULTURE);

            textBoxFrameMean.DataBindings.Add("Text", statistics, "FrameMean",
                true, DataSourceUpdateMode.Never, 0, "0.000",
                NUMBER_CULTURE);

            textBoxFrameValue.DataBindings.Add("Text", statistics, "FrameValue",
                true, DataSourceUpdateMode.Never, 0, "0.000",
                NUMBER_CULTURE);

            textBoxNumberOfPoints.DataBindings.Add("Text", statistics, "NumberOfPoints",
                true, DataSourceUpdateMode.Never, 0, "0",
                NUMBER_CULTURE);

            // Initialize list box and error chart area
            foreach (JointTranslationTuple translation in JointDictionary.JointTranslationList)
            {
                checkedListBoxBodyParts.Items.Add(translation.name);
                chartErrors.Series.Add(translation.name);
                chartErrors.Series[translation.name].Enabled = false;
                chartErrors.Series[translation.name].ChartType = SeriesChartType.StackedArea;
                chartErrors.Series[translation.name].ToolTip = translation.name;
                annotations[translation.joint_type] = new List<Annotation>();
            }

            // Add always present legend items
            LegendItem legendItemMarkerFalsePositive = new LegendItem();
            legendItemMarkerFalsePositive.Name = "False Positive";
            legendItemMarkerFalsePositive.MarkerStyle = MarkerStyle.Square;
            legendItemMarkerFalsePositive.MarkerSize = 7;
            legendItemMarkerFalsePositive.ImageStyle = LegendImageStyle.Marker;
            legendItemMarkerFalsePositive.MarkerColor = markerColorFalseNegative;
            chartErrors.Legends[0].CustomItems.Add(legendItemMarkerFalsePositive);

            LegendItem legendItemMarkerFalseNegative = new LegendItem();
            legendItemMarkerFalseNegative.Name = "False Negative";
            legendItemMarkerFalseNegative.MarkerStyle = MarkerStyle.Circle;
            legendItemMarkerFalseNegative.ImageStyle = LegendImageStyle.Marker;
            legendItemMarkerFalseNegative.MarkerColor = markerColorFalsePositive;
            legendItemMarkerFalseNegative.MarkerSize = 7;
            chartErrors.Legends[0].CustomItems.Add(legendItemMarkerFalseNegative);

            LegendItem legendItemMarkerNA = new LegendItem();
            legendItemMarkerNA.Name = "No data available";
            legendItemMarkerNA.MarkerStyle = MarkerStyle.Circle;
            legendItemMarkerNA.ImageStyle = LegendImageStyle.Marker;
            legendItemMarkerNA.MarkerColor = markerColorNA;
            legendItemMarkerNA.MarkerSize = 7;
            chartErrors.Legends[0].CustomItems.Add(legendItemMarkerNA);

            // Add the line annotation marking the displayed frame.
            frame_indicator = new VerticalLineAnnotation();
            frame_indicator.AxisX = chartErrors.ChartAreas[0].AxisX;
            frame_indicator.AxisY = chartErrors.ChartAreas[0].AxisY;
            frame_indicator.X = 0;
            frame_indicator.Height = chartErrors.Height;
            frame_indicator.LineColor = Color.Red;
            frame_indicator.LineWidth = 1;
            frame_indicator.LineDashStyle = ChartDashStyle.Solid;
            frame_indicator.Y = frame_indicator.AxisY.Minimum;
            chartErrors.Annotations.Add(frame_indicator);

            // Initialize the video source the line indicator is synced with.
            videoDataSource = videoControl;
            videoDataSource.SourceChanged += new EventHandler(videoDataSource_SourceChanged);
        }
        #endregion

        #region Selected frame change methods
        internal void FrameUpdated(int frame_position, bool resetFormerPlace)
        {
            if (xPositions.Count == 0)
                return;

            if (frame_position < min || frame_position > max)
                frame_position = min;

            using (Graphics g = chartErrors.CreateGraphics())
            {
                FrameUpdated(frame_position, resetFormerPlace, g);
            }
        }
        internal void FrameUpdated(int frame_position, bool resetFormerPlace, Graphics g)
        {
            if (xPositions.Count == 0)
                return;

            if (frame_position < min || frame_position > max)
                frame_position = min;

            Point setup_point;

            Size drawing_size = new Size(1, ynull - ymax);
            if (drawing_size.Height <= 0) return;

            if (resetFormerPlace && former_image != null)
            {
                setup_point = new Point(xPositions[former_pos - min], ymax);
                g.DrawImage(former_image, setup_point);
                former_image.Dispose();
            }

            int pos_x = xPositions[frame_position - min];
            setup_point = new Point(pos_x, ymax);

            former_image = new Bitmap(drawing_size.Width, drawing_size.Height, g);
            Graphics memory_graphics = Graphics.FromImage(former_image);
            memory_graphics.CopyFromScreen(PointToScreen(setup_point), new Point(0, 0), drawing_size);

            if (former_pos == frame_position) former_image_invalid = true;
            else
            {
                if (former_image_invalid)
                    chartErrors.Invalidate();

                former_image_invalid = false;
            }
            former_pos = frame_position;
            g.FillRectangle(Brushes.Black,
                new Rectangle(setup_point, drawing_size));
        }
        private void UpdateFrame(object sender, bool handled)
        {
            OpenNIRecordingController video = videoDataSource.Source as OpenNIRecordingController;
            if (video != null)
            {
                statistics.UpdateFrame(video.CurrentFrame, getActiveJoints());
            }
        }

        private void chartErrors_Paint(object sender, PaintEventArgs e)
        {
            ChartArea area = chartErrors.ChartAreas[0];
            min = (int)area.AxisX.Minimum;
            max = (int)area.AxisX.Maximum;
            xPositions.Clear();
            xPositions.Add((int)Math.Ceiling(area.AxisX.ValueToPixelPosition(0)) - 1);
            for (int i = min + 1; i <= max; i++)
            {
                xPositions.Add((int)Math.Ceiling(area.AxisX.ValueToPixelPosition(i)));
            }


            ynull = (int)area.AxisY.ValueToPixelPosition(0);
            ymax = (int)area.AxisY.ValueToPixelPosition(
                                        area.AxisY.Maximum);
            // Dispose the former image, as it is not valid any more and also
            // not needed.
            if (former_image != null)
            {
                former_image.Dispose();
                former_image = null;
            }

            OpenNIRecordingController rec = videoDataSource.Source as OpenNIRecordingController;
            if (rec != null)
            {
                int CurrentFrame = rec.CurrentFrame;
                FrameUpdated(CurrentFrame, false, e.Graphics);
            }
        }
        #endregion

        #region Helpers

        private void clearBodyJointList()
        {
            statistics.StartBatchPhase();
            for (int index = 0; index < checkedListBoxBodyParts.Items.Count; index++)
            {
                checkedListBoxBodyParts.SetItemChecked(index, false);
            }
            OpenNIRecordingController video = (videoDataSource.Source as OpenNIRecordingController);
            if (video != null)
                statistics.EndBatchPhase(getActiveJoints(),
                    mediaSliderConfidenceThreshold.Value * mediaSliderValueFactor,
                    false, video.CurrentFrame);
        }

        private void selectAllBodyJoints()
        {
            statistics.StartBatchPhase();
            for (int index = 0; index < checkedListBoxBodyParts.Items.Count; index++)
            {
                checkedListBoxBodyParts.SetItemChecked(index, true);
            }
            statistics.EndBatchPhase(getActiveJoints(),
                mediaSliderConfidenceThreshold.Value * mediaSliderValueFactor,
                false, ((videoDataSource.Source as OpenNIRecordingController).CurrentFrame));
        }

        private List<SkeletonJoint> getActiveJoints()
        {
            return activeJoints;
        }
        private void reloadSourceList()
        {
            listBoxResultDataSets.ClearSelected();
            listBoxResultDataSets.Items.Clear();

            if (videoDataSource.Source != null &&
                videoDataSource.Source is OpenNIRecordingController)
            {
                FileInfo recording = new FileInfo((videoDataSource.Source as OpenNIRecordingController).RecordingFilename);
                current_take_dir = recording.Directory;
                try
                {
                    foreach (DirectoryInfo subdir in current_take_dir.GetDirectories(MainWindow.EXPORT_DIR_PREFIX + "*"))
                    {
                        foreach (FileInfo file in subdir.GetFiles("Results-*.xml"))
                        {
                            listBoxResultDataSets.Items.Add(Path.Combine(subdir.Name, file.Name));
                        }
                    }
                    listBoxResultDataSets.Enabled = true;
                }
                catch (Exception)
                {
                    listBoxResultDataSets.Items.Clear();
                }
            }
            else
            {
                listBoxResultDataSets.Enabled = false;
            }
        }
        private void reloadUserTruthList()
        {
            listBoxUserTruth.ClearSelected();
            listBoxUserTruth.Items.Clear();

            if (videoDataSource.Source != null &&
                videoDataSource.Source is OpenNIRecordingController)
            {
                Dictionary<int, int> user_stats = (videoDataSource.Source as OpenNIRecordingController).GetUserStatistics(null);
                foreach (int userID in user_stats.Keys)
                {
                    listBoxUserTruth.Items.Add(USER_STRING + userID);
                }

                listBoxUserTruth.Enabled = true;
            }
            else
            {
                listBoxUserTruth.Enabled = false;
            }
        }
        private void mediaSliderConfidenceThreshold_FlyOutInfo(ref string data)
        {
            data = (((double)mediaSliderConfidenceThreshold.Value) * mediaSliderValueFactor).ToString("0.00", NUMBER_CULTURE);
        }
        private int parseUserID(String item)
        {
            return Int32.Parse(item.Substring(USER_STRING.Length));
        }
        #endregion
    }
}
