namespace KinectAET
{
    partial class Evaluation
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Title title1 = new System.Windows.Forms.DataVisualization.Charting.Title();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Evaluation));
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chartCumulativeConfidence = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.mediaSliderConfidenceThreshold = new MediaSlider.MediaSlider();
            this.groupBoxValues = new System.Windows.Forms.GroupBox();
            this.textBoxNumberOfPoints = new System.Windows.Forms.TextBox();
            this.labelNumberOfPoints = new System.Windows.Forms.Label();
            this.textBoxFrameValue = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxFrameMean = new System.Windows.Forms.TextBox();
            this.textBoxGlobalMean = new System.Windows.Forms.TextBox();
            this.labelMeanGlobal = new System.Windows.Forms.Label();
            this.labelMeanLocal = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.buttonBodyPartsNone = new System.Windows.Forms.Button();
            this.buttonBodyPartsAll = new System.Windows.Forms.Button();
            this.checkedListBoxBodyParts = new System.Windows.Forms.CheckedListBox();
            this.groupBoxDataSource = new System.Windows.Forms.GroupBox();
            this.listBoxUserTruth = new System.Windows.Forms.ListBox();
            this.listBoxUserResults = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.listBoxResultDataSets = new System.Windows.Forms.ListBox();
            this.chartErrors = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartCumulativeConfidence)).BeginInit();
            this.groupBoxValues.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBoxDataSource.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartErrors)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 378);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(769, 239);
            this.panel1.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.panel2.Controls.Add(this.groupBox1);
            this.panel2.Controls.Add(this.groupBoxValues);
            this.panel2.Controls.Add(this.groupBox2);
            this.panel2.Controls.Add(this.groupBoxDataSource);
            this.panel2.Location = new System.Drawing.Point(11, 6);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(747, 221);
            this.panel2.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chartCumulativeConfidence);
            this.groupBox1.Controls.Add(this.mediaSliderConfidenceThreshold);
            this.groupBox1.Location = new System.Drawing.Point(447, 72);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(288, 146);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Confidence and Threshold";
            // 
            // chartCumulativeConfidence
            // 
            this.chartCumulativeConfidence.BackColor = System.Drawing.SystemColors.Control;
            chartArea1.AxisX.IsMarginVisible = false;
            chartArea1.AxisX.LabelAutoFitMaxFontSize = 7;
            chartArea1.AxisX.LabelAutoFitMinFontSize = 5;
            chartArea1.AxisX.Maximum = 1D;
            chartArea1.AxisX.Minimum = 0D;
            chartArea1.AxisY.Enabled = System.Windows.Forms.DataVisualization.Charting.AxisEnabled.False;
            chartArea1.Name = "ChartArea1";
            this.chartCumulativeConfidence.ChartAreas.Add(chartArea1);
            this.chartCumulativeConfidence.Location = new System.Drawing.Point(8, 18);
            this.chartCumulativeConfidence.Name = "chartCumulativeConfidence";
            this.chartCumulativeConfidence.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Grayscale;
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.StackedArea;
            series1.Name = "Cumulative Confidence";
            this.chartCumulativeConfidence.Series.Add(series1);
            this.chartCumulativeConfidence.Size = new System.Drawing.Size(269, 79);
            this.chartCumulativeConfidence.TabIndex = 4;
            this.chartCumulativeConfidence.Text = "Cumulative Confidence";
            // 
            // mediaSliderConfidenceThreshold
            // 
            this.mediaSliderConfidenceThreshold.Animated = false;
            this.mediaSliderConfidenceThreshold.AnimationSize = 0.2F;
            this.mediaSliderConfidenceThreshold.AnimationSpeed = MediaSlider.MediaSlider.AnimateSpeed.Normal;
            this.mediaSliderConfidenceThreshold.AutoScrollMargin = new System.Drawing.Size(0, 0);
            this.mediaSliderConfidenceThreshold.AutoScrollMinSize = new System.Drawing.Size(0, 0);
            this.mediaSliderConfidenceThreshold.BackColor = System.Drawing.SystemColors.Control;
            this.mediaSliderConfidenceThreshold.BackgroundImage = null;
            this.mediaSliderConfidenceThreshold.ButtonAccentColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.mediaSliderConfidenceThreshold.ButtonBorderColor = System.Drawing.Color.Black;
            this.mediaSliderConfidenceThreshold.ButtonColor = System.Drawing.Color.FromArgb(((int)(((byte)(160)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.mediaSliderConfidenceThreshold.ButtonCornerRadius = ((uint)(2u));
            this.mediaSliderConfidenceThreshold.ButtonSize = new System.Drawing.Size(24, 12);
            this.mediaSliderConfidenceThreshold.ButtonStyle = MediaSlider.MediaSlider.ButtonType.GlassInline;
            this.mediaSliderConfidenceThreshold.ContextMenuStrip = null;
            this.mediaSliderConfidenceThreshold.Enabled = false;
            this.mediaSliderConfidenceThreshold.FirstHighlightedFrame = -1;
            this.mediaSliderConfidenceThreshold.FlyOutLetterNumber = 4;
            this.mediaSliderConfidenceThreshold.HighlightColor = System.Drawing.Color.Red;
            this.mediaSliderConfidenceThreshold.InitializeAlways = true;
            this.mediaSliderConfidenceThreshold.LargeChange = 5;
            this.mediaSliderConfidenceThreshold.LastHighlightedFrame = -1;
            this.mediaSliderConfidenceThreshold.Location = new System.Drawing.Point(3, 100);
            this.mediaSliderConfidenceThreshold.Margin = new System.Windows.Forms.Padding(0);
            this.mediaSliderConfidenceThreshold.Maximum = 20;
            this.mediaSliderConfidenceThreshold.Minimum = 0;
            this.mediaSliderConfidenceThreshold.Name = "mediaSliderConfidenceThreshold";
            this.mediaSliderConfidenceThreshold.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.mediaSliderConfidenceThreshold.ShowButtonOnHover = false;
            this.mediaSliderConfidenceThreshold.ShowHighlight = false;
            this.mediaSliderConfidenceThreshold.Size = new System.Drawing.Size(282, 38);
            this.mediaSliderConfidenceThreshold.SliderFlyOut = MediaSlider.MediaSlider.FlyOutStyle.Persistant;
            this.mediaSliderConfidenceThreshold.SmallChange = 1;
            this.mediaSliderConfidenceThreshold.SmoothScrolling = false;
            this.mediaSliderConfidenceThreshold.TabIndex = 5;
            this.mediaSliderConfidenceThreshold.TickColor = System.Drawing.Color.DarkGray;
            this.mediaSliderConfidenceThreshold.TickStyle = System.Windows.Forms.TickStyle.None;
            this.mediaSliderConfidenceThreshold.TickType = MediaSlider.MediaSlider.TickMode.LargeStepped;
            this.mediaSliderConfidenceThreshold.TrackBorderColor = System.Drawing.Color.DimGray;
            this.mediaSliderConfidenceThreshold.TrackDepth = 5;
            this.mediaSliderConfidenceThreshold.TrackFillColor = System.Drawing.Color.Transparent;
            this.mediaSliderConfidenceThreshold.TrackProgressColor = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(101)))), ((int)(((byte)(188)))));
            this.mediaSliderConfidenceThreshold.TrackShadow = false;
            this.mediaSliderConfidenceThreshold.TrackShadowColor = System.Drawing.Color.DarkGray;
            this.mediaSliderConfidenceThreshold.TrackStyle = MediaSlider.MediaSlider.TrackType.Value;
            this.mediaSliderConfidenceThreshold.Value = 0;
            this.mediaSliderConfidenceThreshold.FlyOutInfo += new MediaSlider.MediaSlider.FlyOutInfoDelegate(this.mediaSliderConfidenceThreshold_FlyOutInfo);
            this.mediaSliderConfidenceThreshold.ValueChanged += new MediaSlider.MediaSlider.ValueChangedDelegate(this.mediaSliderConfidenceThreshold_ValueChanged);
            // 
            // groupBoxValues
            // 
            this.groupBoxValues.Controls.Add(this.textBoxNumberOfPoints);
            this.groupBoxValues.Controls.Add(this.labelNumberOfPoints);
            this.groupBoxValues.Controls.Add(this.textBoxFrameValue);
            this.groupBoxValues.Controls.Add(this.label3);
            this.groupBoxValues.Controls.Add(this.textBoxFrameMean);
            this.groupBoxValues.Controls.Add(this.textBoxGlobalMean);
            this.groupBoxValues.Controls.Add(this.labelMeanGlobal);
            this.groupBoxValues.Controls.Add(this.labelMeanLocal);
            this.groupBoxValues.Location = new System.Drawing.Point(447, 4);
            this.groupBoxValues.Name = "groupBoxValues";
            this.groupBoxValues.Size = new System.Drawing.Size(288, 62);
            this.groupBoxValues.TabIndex = 7;
            this.groupBoxValues.TabStop = false;
            this.groupBoxValues.Text = "Values";
            // 
            // textBoxNumberOfPoints
            // 
            this.textBoxNumberOfPoints.Location = new System.Drawing.Point(220, 9);
            this.textBoxNumberOfPoints.Name = "textBoxNumberOfPoints";
            this.textBoxNumberOfPoints.ReadOnly = true;
            this.textBoxNumberOfPoints.Size = new System.Drawing.Size(62, 20);
            this.textBoxNumberOfPoints.TabIndex = 9;
            // 
            // labelNumberOfPoints
            // 
            this.labelNumberOfPoints.AutoSize = true;
            this.labelNumberOfPoints.Location = new System.Drawing.Point(152, 16);
            this.labelNumberOfPoints.Name = "labelNumberOfPoints";
            this.labelNumberOfPoints.Size = new System.Drawing.Size(61, 13);
            this.labelNumberOfPoints.TabIndex = 8;
            this.labelNumberOfPoints.Text = "# of Points:";
            // 
            // textBoxFrameValue
            // 
            this.textBoxFrameValue.Location = new System.Drawing.Point(220, 35);
            this.textBoxFrameValue.Name = "textBoxFrameValue";
            this.textBoxFrameValue.ReadOnly = true;
            this.textBoxFrameValue.Size = new System.Drawing.Size(62, 20);
            this.textBoxFrameValue.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(152, 38);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Frame Value:";
            // 
            // textBoxFrameMean
            // 
            this.textBoxFrameMean.Location = new System.Drawing.Point(82, 35);
            this.textBoxFrameMean.Name = "textBoxFrameMean";
            this.textBoxFrameMean.ReadOnly = true;
            this.textBoxFrameMean.Size = new System.Drawing.Size(62, 20);
            this.textBoxFrameMean.TabIndex = 5;
            // 
            // textBoxGlobalMean
            // 
            this.textBoxGlobalMean.Location = new System.Drawing.Point(82, 13);
            this.textBoxGlobalMean.Name = "textBoxGlobalMean";
            this.textBoxGlobalMean.ReadOnly = true;
            this.textBoxGlobalMean.Size = new System.Drawing.Size(62, 20);
            this.textBoxGlobalMean.TabIndex = 4;
            // 
            // labelMeanGlobal
            // 
            this.labelMeanGlobal.AutoSize = true;
            this.labelMeanGlobal.Location = new System.Drawing.Point(6, 16);
            this.labelMeanGlobal.Name = "labelMeanGlobal";
            this.labelMeanGlobal.Size = new System.Drawing.Size(70, 13);
            this.labelMeanGlobal.TabIndex = 2;
            this.labelMeanGlobal.Text = "Global Mean:";
            // 
            // labelMeanLocal
            // 
            this.labelMeanLocal.AutoSize = true;
            this.labelMeanLocal.Location = new System.Drawing.Point(6, 38);
            this.labelMeanLocal.Name = "labelMeanLocal";
            this.labelMeanLocal.Size = new System.Drawing.Size(69, 13);
            this.labelMeanLocal.TabIndex = 3;
            this.labelMeanLocal.Text = "Frame Mean:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.buttonBodyPartsNone);
            this.groupBox2.Controls.Add(this.buttonBodyPartsAll);
            this.groupBox2.Controls.Add(this.checkedListBoxBodyParts);
            this.groupBox2.Location = new System.Drawing.Point(274, 4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(167, 214);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Select Body Parts to Compare";
            // 
            // buttonBodyPartsNone
            // 
            this.buttonBodyPartsNone.Enabled = false;
            this.buttonBodyPartsNone.Location = new System.Drawing.Point(90, 183);
            this.buttonBodyPartsNone.Name = "buttonBodyPartsNone";
            this.buttonBodyPartsNone.Size = new System.Drawing.Size(66, 23);
            this.buttonBodyPartsNone.TabIndex = 2;
            this.buttonBodyPartsNone.Text = "None";
            this.buttonBodyPartsNone.UseVisualStyleBackColor = true;
            this.buttonBodyPartsNone.Click += new System.EventHandler(this.buttonBodyPartsNone_Click);
            // 
            // buttonBodyPartsAll
            // 
            this.buttonBodyPartsAll.Enabled = false;
            this.buttonBodyPartsAll.Location = new System.Drawing.Point(7, 183);
            this.buttonBodyPartsAll.Name = "buttonBodyPartsAll";
            this.buttonBodyPartsAll.Size = new System.Drawing.Size(66, 23);
            this.buttonBodyPartsAll.TabIndex = 1;
            this.buttonBodyPartsAll.Text = "All";
            this.buttonBodyPartsAll.UseVisualStyleBackColor = true;
            this.buttonBodyPartsAll.Click += new System.EventHandler(this.buttonBodyPartsAll_Click);
            // 
            // checkedListBoxBodyParts
            // 
            this.checkedListBoxBodyParts.CheckOnClick = true;
            this.checkedListBoxBodyParts.Enabled = false;
            this.checkedListBoxBodyParts.FormattingEnabled = true;
            this.checkedListBoxBodyParts.Location = new System.Drawing.Point(7, 19);
            this.checkedListBoxBodyParts.Name = "checkedListBoxBodyParts";
            this.checkedListBoxBodyParts.Size = new System.Drawing.Size(149, 154);
            this.checkedListBoxBodyParts.TabIndex = 0;
            this.checkedListBoxBodyParts.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.checkedListBoxBodyParts_ItemCheck);
            // 
            // groupBoxDataSource
            // 
            this.groupBoxDataSource.Controls.Add(this.listBoxUserTruth);
            this.groupBoxDataSource.Controls.Add(this.listBoxUserResults);
            this.groupBoxDataSource.Controls.Add(this.label2);
            this.groupBoxDataSource.Controls.Add(this.label1);
            this.groupBoxDataSource.Controls.Add(this.listBoxResultDataSets);
            this.groupBoxDataSource.Location = new System.Drawing.Point(4, 4);
            this.groupBoxDataSource.Name = "groupBoxDataSource";
            this.groupBoxDataSource.Size = new System.Drawing.Size(264, 214);
            this.groupBoxDataSource.TabIndex = 0;
            this.groupBoxDataSource.TabStop = false;
            this.groupBoxDataSource.Text = "Configure Data Source";
            // 
            // listBoxUserTruth
            // 
            this.listBoxUserTruth.Enabled = false;
            this.listBoxUserTruth.FormattingEnabled = true;
            this.listBoxUserTruth.Location = new System.Drawing.Point(146, 113);
            this.listBoxUserTruth.Name = "listBoxUserTruth";
            this.listBoxUserTruth.Size = new System.Drawing.Size(112, 95);
            this.listBoxUserTruth.TabIndex = 5;
            this.listBoxUserTruth.SelectedIndexChanged += new System.EventHandler(this.listBoxUser_SelectedIndexChanged);
            // 
            // listBoxUserResults
            // 
            this.listBoxUserResults.Enabled = false;
            this.listBoxUserResults.FormattingEnabled = true;
            this.listBoxUserResults.Location = new System.Drawing.Point(6, 113);
            this.listBoxUserResults.Name = "listBoxUserResults";
            this.listBoxUserResults.Size = new System.Drawing.Size(112, 95);
            this.listBoxUserResults.TabIndex = 4;
            this.listBoxUserResults.SelectedIndexChanged += new System.EventHandler(this.listBoxUser_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(144, 95);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "User ground truth";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 95);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "User Results";
            // 
            // listBoxResultDataSets
            // 
            this.listBoxResultDataSets.Enabled = false;
            this.listBoxResultDataSets.FormattingEnabled = true;
            this.listBoxResultDataSets.Location = new System.Drawing.Point(6, 19);
            this.listBoxResultDataSets.Name = "listBoxResultDataSets";
            this.listBoxResultDataSets.Size = new System.Drawing.Size(252, 69);
            this.listBoxResultDataSets.TabIndex = 1;
            this.listBoxResultDataSets.SelectedIndexChanged += new System.EventHandler(this.listBoxResultDataSets_SelectedIndexChanged);
            // 
            // chartErrors
            // 
            this.chartErrors.BackColor = System.Drawing.SystemColors.Control;
            chartArea2.Name = "ChartAreaErrors";
            this.chartErrors.ChartAreas.Add(chartArea2);
            this.chartErrors.Dock = System.Windows.Forms.DockStyle.Fill;
            legend1.DockedToChartArea = "ChartAreaErrors";
            legend1.IsDockedInsideChartArea = false;
            legend1.Name = "LegendAll";
            this.chartErrors.Legends.Add(legend1);
            this.chartErrors.Location = new System.Drawing.Point(0, 0);
            this.chartErrors.Name = "chartErrors";
            this.chartErrors.Size = new System.Drawing.Size(769, 378);
            this.chartErrors.TabIndex = 1;
            this.chartErrors.Text = "Error Chart";
            title1.DockedToChartArea = "ChartAreaErrors";
            title1.Name = "Error Value";
            this.chartErrors.Titles.Add(title1);
            this.chartErrors.Paint += new System.Windows.Forms.PaintEventHandler(this.chartErrors_Paint);
            // 
            // Evaluation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(769, 617);
            this.Controls.Add(this.chartErrors);
            this.Controls.Add(this.panel1);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(785, 655);
            this.Name = "Evaluation";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Evaluation";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Evaluation_FormClosing);
            this.VisibleChanged += new System.EventHandler(this.Evaluation_VisibleChanged);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chartCumulativeConfidence)).EndInit();
            this.groupBoxValues.ResumeLayout(false);
            this.groupBoxValues.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBoxDataSource.ResumeLayout(false);
            this.groupBoxDataSource.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartErrors)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartErrors;
        private System.Windows.Forms.Panel panel2;
        private MediaSlider.MediaSlider mediaSliderConfidenceThreshold;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartCumulativeConfidence;
        private System.Windows.Forms.Label labelMeanLocal;
        private System.Windows.Forms.Label labelMeanGlobal;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button buttonBodyPartsNone;
        private System.Windows.Forms.Button buttonBodyPartsAll;
        private System.Windows.Forms.CheckedListBox checkedListBoxBodyParts;
        private System.Windows.Forms.GroupBox groupBoxDataSource;
        private System.Windows.Forms.ListBox listBoxUserTruth;
        private System.Windows.Forms.ListBox listBoxUserResults;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox listBoxResultDataSets;
        private System.Windows.Forms.GroupBox groupBoxValues;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textBoxFrameMean;
        private System.Windows.Forms.TextBox textBoxGlobalMean;
        private System.Windows.Forms.TextBox textBoxFrameValue;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label labelNumberOfPoints;
        private System.Windows.Forms.TextBox textBoxNumberOfPoints;
    }
}