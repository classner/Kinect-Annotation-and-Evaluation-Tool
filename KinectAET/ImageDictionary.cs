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
using System.IO;
using System.Xml.Serialization;
using System.Collections;
using System.Linq;

namespace KinectAET
{
    [XmlRoot("annotationlist"), Serializable]
    public class ImageDictionary :
        Dictionary<int, Dictionary<int, JointDictionary>>,
        IXmlSerializable
    {
        /// <summary>
        /// Wether this dictionary contains 3d data.
        /// </summary>
        public bool Is3DData
        {
            get { return _is3DData; }
        }

        /// <summary>
        /// Calculates how often the user with given id occurs in the given
        /// frames. If the argument frames is null, the entire video will
        /// be iterated.
        /// </summary>
        public Dictionary<int, int> GetUserStatistics(List<int> frames)
        {
            Dictionary<int, int> res = new Dictionary<int, int>();

            if (frames == null)
                frames = Keys.ToList<int>();

            foreach (int frame in Keys)
            {
                if (frames.Contains(frame))
                {
                    foreach (int userID in this[frame].Keys)
                    {
                        if (!res.ContainsKey(userID))
                        { res.Add(userID, 0); }

                        res[userID]++;
                    }
                }
            }

            return res;
        }

        #region Constructors

        /// <summary>
        /// Creates an ImageDictionary for the specified data type.
        /// </summary>
        /// <param name="contains3DData">Wether the points stored in this
        /// dictionary are 3d points.</param>
        public ImageDictionary(bool contains3DData) : base()
        {
            _is3DData = contains3DData;
        }

        private ImageDictionary()
            : base()
        { }
        #endregion

        /// <summary>
        /// Adds the given user_positions for the specified frame. They must
        /// contain data in an matching format.
        /// </summary>
        /// <exception cref="FormatException">Thrown if the point formats don't
        /// match.</exception>
        public new void Add(int frame, Dictionary<int, JointDictionary> user_positions)
        {
            foreach(JointDictionary user_position in user_positions.Values)
            {
                if (!user_position.Is3DData == Is3DData)
                    throw new FormatException("The user positions must be specified in a matching format.");
            }

            base.Add(frame, user_positions);
        }

        #region IXmlSerializable Members
        #region Configuration
        private readonly Object SERIALIZE_LOCK = new Object();

        private bool exportFrameNumbersAsFilenames = false;
        private String filenamePrefix = null;
        private String filenameSuffix = null;
        private List<int> xmlExportFrames = null;
        private bool ommitPoints = false;
        private Dictionary<int, string> xmlFrameFileList = null;
        private bool _is3DData;

        /// <summary>
        /// A list of frames to export with a file that should be set as
        /// file property. Overrides the <see cref="XmlExportFrameNumbersAsFilenames"/>
        /// property and corresponding properties.
        /// </summary>
        public Dictionary<int, String> XmlExportFrameFileList
        {
            get { return xmlFrameFileList; }
            set
            {
                lock (SERIALIZE_LOCK) { xmlFrameFileList = value; }
            }
        }
        /// <summary>
        /// A list with frames to export.
        /// </summary>
        public List<int> XmlExportFrames
        {
            get { return xmlExportFrames; }
            set { lock (SERIALIZE_LOCK) xmlExportFrames = value; }
        }
        /// <summary>
        /// Whether for XmlSerialization the dictionary key (the frame number)
        /// will not exported as number but as a filename. The properties
        /// <see cref="XmlExportFilenamePrefix"/> and 
        /// <see cref="XmlExportFilenameSuffix"/> must be set, otherwise
        /// an exception is thrown when exporting.
        /// </summary>
        public bool XmlExportFrameNumbersAsFilenames
        {
            get { return exportFrameNumbersAsFilenames; }
            set { lock(SERIALIZE_LOCK) exportFrameNumbersAsFilenames = value; }
        }
        /// <summary>
        /// If set to true, only the keys (frame numbers) are exported according
        /// to the other settings. The points will not be serialized.
        /// </summary>
        public bool XmlOmitPoints
        {
            get { return ommitPoints; }
            set { lock (SERIALIZE_LOCK) ommitPoints = value; }
        }
        /// <summary>
        /// A suffix to add before the frame number to the file name.
        /// </summary>
        public String XmlExportFilenamePrefix
        {
            get { return filenamePrefix; }
            set { lock (SERIALIZE_LOCK) filenamePrefix = value; }
        }
        /// <summary>
        /// A suffix to add after the frame number to the file name.
        /// </summary>
        public String XmlExportFilenameSuffix
        {
            get { return filenameSuffix; }
            set { lock (SERIALIZE_LOCK) filenameSuffix = value; }
        }
        #endregion
        

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            // Initialization
            XmlSerializer valueSerializer = new XmlSerializer(typeof(JointDictionary));
            _is3DData = Boolean.Parse(reader.GetAttribute(__3DDataAttributeName));

            bool wasEmpty = reader.IsEmptyElement;
            reader.Read();

            if (wasEmpty)
                return;

            while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
            {
                reader.ReadStartElement("annotation");

                    int key;
                    if (reader.Name.Equals("frame-number"))
                    {
                        reader.Read();
                        key = reader.ReadContentAsInt();
                    }
                    else
                    {
                        reader.Read();
                        reader.ReadStartElement("name");
                        FileInfo info = new FileInfo(reader.ReadContentAsString());
                        String to_parse = info.Name.Remove(info.Name.IndexOf('.'));

                        // Remove optional suffixes after a dash
                        if (to_parse.IndexOf("-") != -1)
                        {
                            to_parse = to_parse.Remove(to_parse.IndexOf("-"));
                        }

                        try
                        {
                            key = Int32.Parse(to_parse);
                        }
                        catch (Exception)
                        {
                            throw new Exception("The xml-file must only contain file references to files with numbers as names.");
                        }
                        reader.ReadEndElement();
                    }
                    reader.ReadEndElement();

                    Dictionary<int, JointDictionary> user_information =
                                    new Dictionary<int, JointDictionary>();
                    while (reader.IsStartElement())
                    {
                        int user = Int32.Parse(reader.GetAttribute("UserID"));
                        reader.ReadStartElement("annorect");
                        JointDictionary joints = (JointDictionary)valueSerializer.Deserialize(reader);
                        if (joints.Is3DData != Is3DData)
                            throw new Exception("Inconsisten 2D/3D data format.");
                        reader.ReadEndElement();

                        // For compatibility...
                        if (reader.Name.Equals("x1"))
                        {
                            reader.Skip();
                            reader.Skip();
                            reader.Skip();
                            reader.Skip();
                        }
                        reader.ReadEndElement();
                        user_information.Add(user, joints);
                    }

                    this.Add(key, user_information);


                reader.ReadEndElement();
                reader.MoveToContent();
            }
            reader.ReadEndElement();
        }
        private static readonly String __3DDataAttributeName = "Is3DData";
        public void WriteXml(System.Xml.XmlWriter writer)
        {
            lock (SERIALIZE_LOCK)
            {
                // Initialization
                if (XmlExportFrameNumbersAsFilenames &&
                    (XmlExportFilenamePrefix == null ||
                     XmlExportFilenameSuffix == null) &&
                    XmlExportFrameFileList == null)
                {
                    throw new InvalidOperationException(
                        "Trying to serialize skeleton to xml for a file without providing file information.");
                }
                String exportFormatString = "d";
                if (XmlExportFrameNumbersAsFilenames && XmlExportFrames != null &&
                    XmlExportFrameFileList == null)
                {
                    XmlExportFrames.Sort();
                    int max = XmlExportFrames[XmlExportFrames.Count - 1];
                    int characters = (int)Math.Ceiling(Math.Log10(max));
                    exportFormatString = "d" + characters;
                }

                XmlSerializer valueSerializer = new XmlSerializer(typeof(JointDictionary));


                // Export
                // Write the data type.
                writer.WriteAttributeString(__3DDataAttributeName, Is3DData.ToString());

                // Write the data.
                foreach (int key in this.Keys)
                {
                    if (XmlExportFrames == null && XmlExportFrameFileList == null ||
                        (XmlExportFrames != null && XmlExportFrames.Contains(key)) ||
                        (XmlExportFrameFileList != null && XmlExportFrameFileList.ContainsKey(key)))
                    {
                        writer.WriteStartElement("annotation");

                        if (XmlExportFrameNumbersAsFilenames ||
                            XmlExportFrameFileList != null)
                        {
                            writer.WriteStartElement("image");
                            writer.WriteStartElement("name");
                            if (XmlExportFrameFileList != null)
                            {
                                writer.WriteString(XmlExportFrameFileList[key]);
                            }
                            else
                            {
                                writer.WriteString(
                                    Path.Combine(XmlExportFilenamePrefix,
                                    key.ToString(exportFormatString) + XmlExportFilenameSuffix));
                            }
                            writer.WriteEndElement();
                            writer.WriteEndElement();
                        }
                        else
                        {
                            writer.WriteStartElement("frame-number");
                            writer.WriteString(key.ToString());
                            writer.WriteEndElement();
                        }

                        if (!XmlOmitPoints)
                        {
                            foreach (int user in this[key].Keys)
                            {
                                writer.WriteStartElement("annorect");
                                writer.WriteAttributeString("UserID", user.ToString());

                                JointDictionary joints = this[key][user];
                                valueSerializer.Serialize(writer, joints);
                                writer.WriteEndElement();
                            }
                        }

                        writer.WriteEndElement();
                    }
                }
            }
        }
        #endregion
    }
}
