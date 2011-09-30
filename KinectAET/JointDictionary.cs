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
using System.Globalization;
using System.Xml.Serialization;
using OpenNI;
using System.Linq;

namespace KinectAET
{
    /// <summary>
    /// Represents one set of equivalent translations for the same tuple.
    /// </summary>
    public struct JointTranslationTuple
    {
        public String name;
        public SkeletonJoint joint_type;
        public int export_id;

        public JointTranslationTuple(String name, SkeletonJoint joint_type, int export_id)
        {
            this.name = name; this.joint_type = joint_type; this.export_id = export_id;
        }
    }

    [XmlRoot("annopoints"), Serializable]
    public class JointDictionary :
        Dictionary<SkeletonJoint, SkeletonJointPosition>,
        IXmlSerializable
    {
        #region Properties
        private bool _is3DData;

        /// <summary>
        /// Wether this dictionary contains 3d data.
        /// </summary>
        public bool Is3DData
        {
            get { return _is3DData; }
        }
        #endregion

        #region Joint Conversion
        /// <summary>
        /// A list of possible translations for different tuple type representations.
        /// Changes reflect in serialization of tuples from JointDictionaries!
        /// </summary>
        public static readonly List<JointTranslationTuple> JointTranslationList =
            new List<JointTranslationTuple>(){
                new JointTranslationTuple("Right Foot", SkeletonJoint.LeftFoot, 0),
                new JointTranslationTuple("Right Knee", SkeletonJoint.LeftKnee, 1),
                new JointTranslationTuple("Right Hip", SkeletonJoint.LeftHip, 2),
                new JointTranslationTuple("Left Hip", SkeletonJoint.RightHip, 3),
                new JointTranslationTuple("Left Knee", SkeletonJoint.RightKnee, 4),
                new JointTranslationTuple("Left Foot", SkeletonJoint.RightFoot, 5),
                new JointTranslationTuple("Right Hand", SkeletonJoint.LeftHand, 6),
                new JointTranslationTuple("Right Elbow", SkeletonJoint.LeftElbow, 7),
                new JointTranslationTuple("Right Shoulder", SkeletonJoint.LeftShoulder,8),
                new JointTranslationTuple("Left Shoulder", SkeletonJoint.RightShoulder, 9),
                new JointTranslationTuple("Left Elbow", SkeletonJoint.RightElbow, 10),
                new JointTranslationTuple("Left Hand", SkeletonJoint.RightHand, 11),
                new JointTranslationTuple("Neck", SkeletonJoint.Neck, 12),
                new JointTranslationTuple("Head", SkeletonJoint.Head, 13),
                new JointTranslationTuple("Torso", SkeletonJoint.Torso, 15),
                new JointTranslationTuple("Waist", SkeletonJoint.Waist, 14)
            };
        /// <summary>
        /// Gets the <see cref="JointTranslationTuple"/> for the given name.
        /// </summary>
        /// <exception cref="Exception">Thrown if no fitting tuple is found.
        /// </exception>
        public static JointTranslationTuple GetTranslation(String name)
        {
            return JointTranslationList.Single(
                          (translationTuple) => translationTuple.name == name);
        }
        /// <summary>
        /// Gets the <see cref="JointTranslationTuple"/> for the given joint.
        /// </summary>
        /// <exception cref="Exception">Thrown if no fitting tuple is found.
        /// </exception>
        public static JointTranslationTuple GetTranslation(SkeletonJoint joint)
        {
            return JointTranslationList.Single(
                          (translationTuple) => translationTuple.joint_type == joint);
        }
        /// <summary>
        /// Gets the <see cref="JointTranslationTuple"/> for the give export id.
        /// </summary>
        /// <exception cref="Exception">Thrown if no fitting tuple is found.
        /// </exception>
        public static JointTranslationTuple GetTranslation(int export_id)
        {
            return JointTranslationList.Single(
                          (translationTuple) => translationTuple.export_id == export_id);
        }
        #endregion

        #region Constructors
        public JointDictionary(bool contains3DData) : base()
        {
            _is3DData = contains3DData;
        }

        public JointDictionary(JointDictionary original)
            : base(original)
        {
            _is3DData = original.Is3DData;
        }

        private JointDictionary()
            : base()
        { }
        #endregion

        #region IXmlSerializable Members
        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            bool wasEmpty = reader.IsEmptyElement;
            bool first = true;
            reader.Read();

            if (wasEmpty)
                return;

            while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
            {
                reader.Read();
                if (reader.IsEmptyElement)
                {
                    reader.Skip();
                    reader.Skip();
                    reader.Skip();
                    reader.Skip();
                    reader.ReadEndElement();
                    reader.MoveToContent();
                    continue;
                }
                reader.ReadStartElement("id");
                int keyValue = reader.ReadContentAsInt();
                SkeletonJoint key;
                try
                {
                    key = GetTranslation(keyValue).joint_type;
                }
                catch (Exception) { key = SkeletonJoint.Invalid; }
                reader.ReadEndElement();

                float confidence = 0.0f;
                if (reader.Name.Equals("confidence"))
                {
                    reader.Read();
                    confidence = reader.ReadContentAsFloat();
                    reader.ReadEndElement();
                }

                reader.ReadStartElement("x");
                float x = reader.ReadContentAsFloat();
                reader.ReadEndElement();
                reader.ReadStartElement("y");
                float y = reader.ReadContentAsFloat();
                reader.ReadEndElement();

                float z = 0.0f;
                if (first)
                {
                    if (reader.Name.Equals("z"))
                    {
                        _is3DData = true;
                    }
                    else
                    {
                        _is3DData = false;
                    }
                    first = false;
                }
                if (reader.Name.Equals("z"))
                {
                    reader.ReadStartElement("z");
                    z = reader.ReadContentAsFloat();
                    reader.ReadEndElement();

                    if (!_is3DData) throw new Exception("Inconsistent 2D/3D data format.");
                }
                else
                {
                    if (_is3DData) throw new Exception("Inconsistent 2D/3D data format.");
                }

                SkeletonJointPosition value = new SkeletonJointPosition();
                value.Confidence = confidence;
                value.Position = new Point3D(x, y, z);

                if (key != SkeletonJoint.Invalid)
                    this.Add(key, value);
                reader.ReadEndElement();
                reader.MoveToContent();
            }
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            foreach (SkeletonJoint key in this.Keys)
            {
                String idString = null;
                try
                {
                    idString = GetTranslation(key).export_id.ToString();
                }
                catch (Exception) { }
                if (idString == null) continue;

                writer.WriteStartElement("point");

                writer.WriteStartElement("id");
                writer.WriteValue(idString);
                writer.WriteEndElement();

                writer.WriteStartElement("confidence");
                writer.WriteValue(this[key].Confidence.ToString("0.0",CultureInfo.InvariantCulture.NumberFormat));
                writer.WriteEndElement();

                writer.WriteStartElement("x");
                writer.WriteValue(this[key].Position.X);
                writer.WriteEndElement();

                writer.WriteStartElement("y");
                writer.WriteValue(this[key].Position.Y);
                writer.WriteEndElement();

                if (Is3DData)
                {
                    writer.WriteStartElement("z");
                    writer.WriteValue(this[key].Position.Z);
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }
        }
        #endregion
    }
}
