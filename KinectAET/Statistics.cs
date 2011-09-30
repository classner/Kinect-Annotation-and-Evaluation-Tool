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
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using MathNet.Numerics.Statistics;
using MathNet.Numerics.Distributions;
using OpenNI;

namespace KinectAET
{
    internal class Statistics : INotifyPropertyChanged
    {
        #region Structs and Enums and Types
        private struct DifferenceData
        {
            public double Difference;
            public double Confidence;
            public bool inTruthData;
            public bool inResultData;
            public int frame;
        }
        public enum MarkerType
        {
            FalsePositive, FalseNegative, NotAvailable, Normal
        }

        public delegate JointDictionary To2DConverter(JointDictionary from3D);
        #endregion

        #region Variables
        #region Backing Variables for Public bindings
        private double _globalMean = 0.0;
        private double _frameMean = 0.0;
        private double _frameValue = 0.0;
        private int _numberOfPoints = 0;
        #endregion

        #region Common
        private bool isInBatchMode = false;
        private Dictionary<SkeletonJoint, List<DifferenceData>> errors;

        private ImageDictionary resultDict;
        private ImageDictionary truthDict;
        private bool work_in_3D;
        #endregion
        #endregion

        #region Properties
        public double GlobalMean
        {
            get { return _globalMean; }
            private set
            {
                bool changed = false;
                if (value != _globalMean)
                    changed = true;
                _globalMean = value;
                if (changed)
                    onPropertyChanged("GlobalMean");
            }
        }

        public double FrameMean
        {
            get { return _frameMean; }
            private set
            {
                bool changed = false;
                if (value != _frameMean)
                    changed = true;
                _frameMean = value;
                if (changed)
                    onPropertyChanged("FrameMean");
            }
        }

        public double FrameValue
        {
            get { return _frameValue; }
            private set
            {
                bool changed = false;
                if (value != _frameValue)
                    changed = true;
                _frameValue = value;
                if (changed)
                    onPropertyChanged("FrameValue");
            }
        }
        public int NumberOfPoints
        {
            get { return _numberOfPoints; }
            private set
            {
                bool changed = false;
                if (value != _numberOfPoints)
                    changed = true;
                _numberOfPoints = value;
                if (changed)
                    onPropertyChanged("NumberOfPoints");
            }
        }
        #endregion

        #region Events
        private void onClearErrorPoints()
        {
            if (ClearErrorPoints != null)
                ClearErrorPoints();
        }
        public event Action ClearErrorPoints;
        private void onAddErrorPoint(SkeletonJoint for_series, int x, double y, MarkerType mark_as)
        {
            if(AddErrorPoint != null)
                AddErrorPoint(for_series, x, y, mark_as);
        }
        public event Action<SkeletonJoint, int, double, MarkerType> AddErrorPoint;
        private void onClearCumulativeConfidencePoints()
        {
            if (ClearCumulativeConfidencePoints != null)
                ClearCumulativeConfidencePoints();
        }
        public event Action ClearCumulativeConfidencePoints;
        private void onAddCumulativeConfidencePoint(double x, double y)
        {
            if (AddCumulativeConfidencePoint != null)
                AddCumulativeConfidencePoint(x, y);
        }
        public event Action<double, double> AddCumulativeConfidencePoint;

        private void onPropertyChanged(String name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void onFinalizeErrorUpdate()
        {
            if (FinalizeErrorUpdate != null)
                FinalizeErrorUpdate();
        }
        public event Action FinalizeErrorUpdate;
        private void onFinalizeConfidenceUpdate()
        {
            if (FinalizeConfidenceUpdate != null)
                FinalizeConfidenceUpdate();
        }
        public event Action FinalizeConfidenceUpdate;
        private void onFrameUpdated(int number)
        {
            if (FrameUpdated != null)
                FrameUpdated(number);
        }
        public event Action<int> FrameUpdated;
        #endregion

        #region Constructor
        public Statistics()
        {        }
        #endregion

        #region Calculation
        /// <summary>
        /// Sets the data for the statistics object. The statistic values
        /// are calculated in this method. The results and truth dictionaries
        /// must contain at least a key for every frame between minimal and
        /// maximal frame number.
        /// </summary>
        /// <param name="truth">The ground truth dictionary.</param>
        /// <param name="results">The result dictionary.</param>
        /// <param name="truth_id">The id of the user to compare to as truth in
        /// the truth data ids.</param>
        /// <param name="results_id">The id of the result user to compare with 
        /// in the result data ids.</param>
        /// <param name="converter">A function to convert 3d data to 2d data
        /// if necessary.</param>
        /// <exception cref="Exception">Thrown if the frames are not continuous
        /// from first to last.</exception>
        internal void SetData(ImageDictionary truth,
            ImageDictionary results, int truth_id, int results_id,
            To2DConverter converter)
        {
            // Reset.
            GlobalMean = 0.0;
            FrameMean = 0.0;
            FrameValue = 0.0;
            NumberOfPoints = 0;
            first_frame = int.MaxValue;
            last_frame = -1;

            isInBatchMode = false;
            errors = new Dictionary<SkeletonJoint, List<DifferenceData>>();
            foreach (JointTranslationTuple translation in JointDictionary.JointTranslationList)
                  errors.Add(translation.joint_type, new List<DifferenceData>());
            resultDict = results;
            truthDict = truth;
            work_in_3D = results.Is3DData;
            if (work_in_3D && !truth.Is3DData)
                throw new ArgumentException("Trying to compare data with insufficient information.");

            // Check converter availability if necessary
            if (!resultDict.Is3DData && truth.Is3DData && converter == null)
                    throw new ArgumentException("Must convert data to 2D, but no converter was specified.");

            // Start calculating data.
            foreach (int frame in results.Keys)
            {
                if (frame < first_frame)
                    first_frame = frame;
                if (frame > last_frame)
                    last_frame = frame;

                // Convert the data if necessary
                JointDictionary frame_data_truth = new JointDictionary(work_in_3D);
                JointDictionary frame_data_results = new JointDictionary(work_in_3D);
                bool user_in_truth = false;
                bool user_in_results = false;

                if (results[frame].ContainsKey(results_id))
                {
                    user_in_results = true;
                    if (results.Is3DData && !work_in_3D)
                        frame_data_results = converter(results[frame][results_id]);
                    else
                        frame_data_results = results[frame][results_id];
                }
                if (truth[frame].ContainsKey(truth_id))
                {
                    user_in_truth = true;
                    if (truth.Is3DData && !work_in_3D)
                        frame_data_truth = converter(truth[frame][truth_id]);
                    else
                        frame_data_truth = truth[frame][truth_id];
                }

                foreach (SkeletonJoint joint in errors.Keys)
                {
                    DifferenceData data = new DifferenceData();
                    data.frame = frame;
                    data.Difference = double.NaN;
                    Point3D resultsPoint = new Point3D(), truthPoint = new Point3D();

                    if (user_in_results && frame_data_results.ContainsKey(joint))
                    {
                        data.inResultData = true;
                        resultsPoint = frame_data_results[joint].Position;
                    }
                    if (user_in_truth && frame_data_truth.ContainsKey(joint))
                    {
                        data.inTruthData = true;
                        data.Confidence = frame_data_truth[joint].Confidence;
                        truthPoint = frame_data_truth[joint].Position;
                    }

                    if (data.inTruthData && data.inResultData)
                    {
                        if (work_in_3D)
                            data.Difference = Math.Sqrt(
                                Math.Pow(truthPoint.X - resultsPoint.X, 2.0) +
                                Math.Pow(truthPoint.Y - resultsPoint.Y, 2.0) +
                                Math.Pow(truthPoint.Z - resultsPoint.Z, 2.0));
                        else
                            data.Difference = Math.Sqrt(
                                 Math.Pow(truthPoint.X - resultsPoint.X, 2.0) +
                                 Math.Pow(truthPoint.Y - resultsPoint.Y, 2.0));
                    }

                    int position = 0;
                    List<DifferenceData> to_insert_in = errors[joint];
                    for (int i = 0; i < to_insert_in.Count; i++)
                    {
                        if (frame > to_insert_in[i].frame) break;
                        else position++;
                    }

                    errors[joint].Add(data);
                }
            }


            // Check that list is complete!!!
            int range = last_frame - first_frame + 1;
            bool failed = false;
            foreach (SkeletonJoint joint in errors.Keys)
            {
                if (errors[joint].Count != range)
                {
                    failed = true;
                    break;
                }
            }
            if (failed)
            {
                errors.Clear();
                throw new Exception("The algorithm results did not contain every " +
                "frame from the first to the last.");
            }
        }

        int first_frame;
        int last_frame;
        internal void Calculate(List<SkeletonJoint> enabledJoints,
            double confidenceThreshold, bool updatePoints,
            int frame)
        {
            List<double> Values = new List<double>();
            List<int> cumulativeConfidenceList = Enumerable.Repeat<int>(0, 21).ToList();
            List<double> frameValues = new List<double>();
            Dictionary<int, MarkerType> fill_list = new Dictionary<int, MarkerType>();
            List<double> joint_values = new List<double>();

            bool update_these_points;
            bool is_enabled;

            if (errors == null || isInBatchMode)
                return;

            if (updatePoints)
                onClearErrorPoints();

            foreach (SkeletonJoint joint in errors.Keys)
            {
                is_enabled = enabledJoints.Contains(joint);
                update_these_points = updatePoints && is_enabled;
                fill_list.Clear();
                joint_values.Clear();

                if(!is_enabled && !updatePoints)
                    continue;

                foreach (DifferenceData difference in errors[joint])
                {
                    if(is_enabled)
                        cumulativeConfidenceList[(int)Math.Floor((difference.Confidence) * 100.0 / 5.0)]++;

                    if (difference.Confidence < confidenceThreshold ||
                        !difference.inTruthData)
                    {
                        if (difference.inResultData)
                        {
                            if (updatePoints)
                                fill_list.Add(difference.frame, MarkerType.FalsePositive);
                        }
                        else
                            if (updatePoints)
                                fill_list.Add(difference.frame, MarkerType.NotAvailable);
                    }
                    else
                    {
                        if (difference.inResultData)
                        {
                            if (updatePoints)
                            {
                                onAddErrorPoint(joint, difference.frame, difference.Difference,
                                    MarkerType.Normal);
                                joint_values.Add(difference.Difference);
                            }
                            if (is_enabled)
                            {
                                Values.Add(difference.Difference);
                                if (difference.frame == frame)
                                    frameValues.Add(difference.Difference);
                            }
                        }
                        else
                        {
                            if (updatePoints)
                                fill_list.Add(difference.frame, MarkerType.FalseNegative);
                        }
                    }
                }

                // Fill the empty points.
                double fill_value = 0.0;
                if (joint_values.Count > 0)
                {
                    fill_value = joint_values.Median();
                }
                foreach (KeyValuePair<int, MarkerType> fill_data in fill_list)
                {
                    onAddErrorPoint(joint, fill_data.Key, fill_value,
                        fill_data.Value);
                }
            }

            GlobalMean = Values.Mean();
            FrameMean = frameValues.Mean();
            NumberOfPoints = Values.Count;
            FrameValue = frameValues.Sum();
            onFinalizeErrorUpdate();

            // Cumulative Confidence
            for (int i = cumulativeConfidenceList.Count - 1; i > 0; i--)
            {
                cumulativeConfidenceList[i - 1] = cumulativeConfidenceList[i - 1] + cumulativeConfidenceList[i];
            }

            onClearCumulativeConfidencePoints();
            for (int i = 0; i < 21; i++)
                onAddCumulativeConfidencePoint(i * 5.0 / 100.0, cumulativeConfidenceList[i]);
            onFinalizeConfidenceUpdate();
            }

        internal void StartBatchPhase()
        {
            isInBatchMode = true;
        }

        internal void EndBatchPhase(List<SkeletonJoint> list,
            double confidenceThreshold, bool updatePoints,
            int frame)
        {
            isInBatchMode = false;
            Calculate(list, confidenceThreshold, updatePoints, frame);
        }

        public void UpdateFrame(int currentFrame, List<SkeletonJoint> activeJoints)
        {
            if (errors == null)
                return;

            List<double> t = new List<double>();
            int index = currentFrame - first_frame;
            if (index < 0 || currentFrame > last_frame)
            {
                FrameMean = 0.0;
                FrameValue = 0.0;
                onFrameUpdated(0);
                return;
            }
            else
            {
                foreach (SkeletonJoint joint in activeJoints)
                {
                    double diff = errors[joint][index].Difference;
                    if(! diff.Equals(double.NaN))
                        t.Add(diff);
                }
            }
            
            if (t.Count > 0)
            {
                FrameMean = t.Mean();
                FrameValue = t.Sum();
            }
            else
            {
                FrameMean = 0.0;
                FrameValue = 0.0;
            }

            onFrameUpdated(currentFrame);
        }
        #endregion
    }

}
