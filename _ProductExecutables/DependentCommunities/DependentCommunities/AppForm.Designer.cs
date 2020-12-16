namespace DependencyZones
{
    partial class AppForm
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
            this.PathTextBox = new System.Windows.Forms.TextBox();
            this.ReadFileButton = new System.Windows.Forms.Button();
            this.DetectZonesButton = new System.Windows.Forms.Button();
            this.OpenFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.GdfCheckBox = new System.Windows.Forms.CheckBox();
            this.OuterZoneCheckBox = new System.Windows.Forms.CheckBox();
            this.GdfMinNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.ZonesProgressBar = new System.Windows.Forms.ProgressBar();
            this.DepEdgeCheckBox = new System.Windows.Forms.CheckBox();
            this.SuperSubCheckBox = new System.Windows.Forms.CheckBox();
            this.ProgressLabel = new System.Windows.Forms.Label();
            this.ErdosRenyiModelButton = new System.Windows.Forms.Button();
            this.BarabasiAlbertModelButton = new System.Windows.Forms.Button();
            this.TriadicClosureModelButton = new System.Windows.Forms.Button();
            this.VerticesTextBox = new System.Windows.Forms.TextBox();
            this.ParameterTextBox = new System.Windows.Forms.TextBox();
            this.SaveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.LineLabel1 = new System.Windows.Forms.Label();
            this.NetworkTypeComboBox = new System.Windows.Forms.ComboBox();
            this.OverlapTextBox = new System.Windows.Forms.TextBox();
            this.GdfOverlapButton = new System.Windows.Forms.Button();
            this.LineLabel3 = new System.Windows.Forms.Label();
            this.CommunityDetectionCheckBox = new System.Windows.Forms.CheckBox();
            this.ZxCCheckBox = new System.Windows.Forms.CheckBox();
            this.LargeScaleCheckBox = new System.Windows.Forms.CheckBox();
            this.GroundTruthButton = new System.Windows.Forms.Button();
            this.GroundTruthTextBox = new System.Windows.Forms.TextBox();
            this.UkazkaButton = new System.Windows.Forms.Button();
            this.ProminencyFilterComboBox = new System.Windows.Forms.ComboBox();
            this.GroupingStrategyComboBox = new System.Windows.Forms.ComboBox();
            this.ProminencyFilterLabel = new System.Windows.Forms.Label();
            this.GroupingStrategyLabel = new System.Windows.Forms.Label();
            this.LineLabel2 = new System.Windows.Forms.Label();
            this.NetDepLabel = new System.Windows.Forms.Label();
            this.MinSizeLabel = new System.Windows.Forms.Label();
            this.MaxSizeLabel = new System.Windows.Forms.Label();
            this.NetDepNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.MinSizeNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.MaxSizeNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.DuplicityFilterLabel = new System.Windows.Forms.Label();
            this.DuplicityFilterComboBox = new System.Windows.Forms.ComboBox();
            this.DefaultSettingButton = new System.Windows.Forms.Button();
            this.DependencyClosureButton = new System.Windows.Forms.Button();
            this.GroupDepLabel = new System.Windows.Forms.Label();
            this.GroupDepNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.P = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.GdfMinNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NetDepNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MinSizeNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MaxSizeNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.GroupDepNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // PathTextBox
            // 
            this.PathTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.PathTextBox.Location = new System.Drawing.Point(18, 18);
            this.PathTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.PathTextBox.Name = "PathTextBox";
            this.PathTextBox.ReadOnly = true;
            this.PathTextBox.Size = new System.Drawing.Size(436, 26);
            this.PathTextBox.TabIndex = 0;
            // 
            // ReadFileButton
            // 
            this.ReadFileButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.ReadFileButton.Location = new System.Drawing.Point(470, 16);
            this.ReadFileButton.Margin = new System.Windows.Forms.Padding(2);
            this.ReadFileButton.Name = "ReadFileButton";
            this.ReadFileButton.Size = new System.Drawing.Size(123, 30);
            this.ReadFileButton.TabIndex = 1;
            this.ReadFileButton.Text = "Read file";
            this.ReadFileButton.UseVisualStyleBackColor = true;
            this.ReadFileButton.Click += new System.EventHandler(this.ReadFileButton_Click);
            // 
            // DetectZonesButton
            // 
            this.DetectZonesButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 19.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.DetectZonesButton.Location = new System.Drawing.Point(158, 268);
            this.DetectZonesButton.Margin = new System.Windows.Forms.Padding(2);
            this.DetectZonesButton.Name = "DetectZonesButton";
            this.DetectZonesButton.Size = new System.Drawing.Size(296, 71);
            this.DetectZonesButton.TabIndex = 28;
            this.DetectZonesButton.Text = "Detect Ego-zones";
            this.DetectZonesButton.UseVisualStyleBackColor = true;
            this.DetectZonesButton.Click += new System.EventHandler(this.DetectZonesButton_Click);
            // 
            // GdfCheckBox
            // 
            this.GdfCheckBox.AutoSize = true;
            this.GdfCheckBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.GdfCheckBox.Location = new System.Drawing.Point(475, 265);
            this.GdfCheckBox.Margin = new System.Windows.Forms.Padding(1);
            this.GdfCheckBox.Name = "GdfCheckBox";
            this.GdfCheckBox.Size = new System.Drawing.Size(63, 24);
            this.GdfCheckBox.TabIndex = 29;
            this.GdfCheckBox.Text = "GDF";
            this.GdfCheckBox.UseVisualStyleBackColor = true;
            this.GdfCheckBox.CheckedChanged += new System.EventHandler(this.GdfCheckBox_CheckedChanged);
            // 
            // OuterZoneCheckBox
            // 
            this.OuterZoneCheckBox.AutoSize = true;
            this.OuterZoneCheckBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.OuterZoneCheckBox.Location = new System.Drawing.Point(20, 296);
            this.OuterZoneCheckBox.Margin = new System.Windows.Forms.Padding(1);
            this.OuterZoneCheckBox.Name = "OuterZoneCheckBox";
            this.OuterZoneCheckBox.Size = new System.Drawing.Size(107, 24);
            this.OuterZoneCheckBox.TabIndex = 26;
            this.OuterZoneCheckBox.Text = "Outer zone";
            this.OuterZoneCheckBox.UseVisualStyleBackColor = true;
            // 
            // GdfMinNumericUpDown
            // 
            this.GdfMinNumericUpDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.GdfMinNumericUpDown.Location = new System.Drawing.Point(543, 265);
            this.GdfMinNumericUpDown.Margin = new System.Windows.Forms.Padding(2);
            this.GdfMinNumericUpDown.Name = "GdfMinNumericUpDown";
            this.GdfMinNumericUpDown.Size = new System.Drawing.Size(50, 26);
            this.GdfMinNumericUpDown.TabIndex = 30;
            this.GdfMinNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // ZonesProgressBar
            // 
            this.ZonesProgressBar.Location = new System.Drawing.Point(18, 363);
            this.ZonesProgressBar.Margin = new System.Windows.Forms.Padding(2);
            this.ZonesProgressBar.Name = "ZonesProgressBar";
            this.ZonesProgressBar.Size = new System.Drawing.Size(579, 20);
            this.ZonesProgressBar.TabIndex = 33;
            // 
            // DepEdgeCheckBox
            // 
            this.DepEdgeCheckBox.AutoSize = true;
            this.DepEdgeCheckBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.DepEdgeCheckBox.Location = new System.Drawing.Point(475, 296);
            this.DepEdgeCheckBox.Margin = new System.Windows.Forms.Padding(2);
            this.DepEdgeCheckBox.Name = "DepEdgeCheckBox";
            this.DepEdgeCheckBox.Size = new System.Drawing.Size(112, 24);
            this.DepEdgeCheckBox.TabIndex = 31;
            this.DepEdgeCheckBox.Text = "Dep. output";
            this.DepEdgeCheckBox.UseVisualStyleBackColor = true;
            // 
            // SuperSubCheckBox
            // 
            this.SuperSubCheckBox.AutoSize = true;
            this.SuperSubCheckBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.SuperSubCheckBox.Location = new System.Drawing.Point(475, 327);
            this.SuperSubCheckBox.Margin = new System.Windows.Forms.Padding(2);
            this.SuperSubCheckBox.Name = "SuperSubCheckBox";
            this.SuperSubCheckBox.Size = new System.Drawing.Size(109, 24);
            this.SuperSubCheckBox.TabIndex = 32;
            this.SuperSubCheckBox.Text = "Super / sub";
            this.SuperSubCheckBox.UseVisualStyleBackColor = true;
            // 
            // ProgressLabel
            // 
            this.ProgressLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.ProgressLabel.Location = new System.Drawing.Point(18, 384);
            this.ProgressLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.ProgressLabel.Name = "ProgressLabel";
            this.ProgressLabel.Size = new System.Drawing.Size(579, 29);
            this.ProgressLabel.TabIndex = 34;
            this.ProgressLabel.Text = "Nothing to write";
            this.ProgressLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ProgressLabel.Click += new System.EventHandler(this.ProgressLabel_Click);
            // 
            // ErdosRenyiModelButton
            // 
            this.ErdosRenyiModelButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.ErdosRenyiModelButton.Location = new System.Drawing.Point(158, 52);
            this.ErdosRenyiModelButton.Margin = new System.Windows.Forms.Padding(2);
            this.ErdosRenyiModelButton.Name = "ErdosRenyiModelButton";
            this.ErdosRenyiModelButton.Size = new System.Drawing.Size(100, 26);
            this.ErdosRenyiModelButton.TabIndex = 4;
            this.ErdosRenyiModelButton.Text = "E-R model";
            this.ErdosRenyiModelButton.UseVisualStyleBackColor = true;
            this.ErdosRenyiModelButton.Click += new System.EventHandler(this.ErdosRenyiModelButton_Click);
            // 
            // BarabasiAlbertModelButton
            // 
            this.BarabasiAlbertModelButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.BarabasiAlbertModelButton.Location = new System.Drawing.Point(261, 52);
            this.BarabasiAlbertModelButton.Margin = new System.Windows.Forms.Padding(2);
            this.BarabasiAlbertModelButton.Name = "BarabasiAlbertModelButton";
            this.BarabasiAlbertModelButton.Size = new System.Drawing.Size(95, 26);
            this.BarabasiAlbertModelButton.TabIndex = 5;
            this.BarabasiAlbertModelButton.Text = "B-A model";
            this.BarabasiAlbertModelButton.UseVisualStyleBackColor = true;
            this.BarabasiAlbertModelButton.Click += new System.EventHandler(this.BarabasiAlbertModelButton_Click);
            // 
            // TriadicClosureModelButton
            // 
            this.TriadicClosureModelButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.TriadicClosureModelButton.Location = new System.Drawing.Point(358, 52);
            this.TriadicClosureModelButton.Margin = new System.Windows.Forms.Padding(2);
            this.TriadicClosureModelButton.Name = "TriadicClosureModelButton";
            this.TriadicClosureModelButton.Size = new System.Drawing.Size(94, 26);
            this.TriadicClosureModelButton.TabIndex = 6;
            this.TriadicClosureModelButton.Text = "T-C model";
            this.TriadicClosureModelButton.UseVisualStyleBackColor = true;
            this.TriadicClosureModelButton.Click += new System.EventHandler(this.BianconiTriadicClosureModelButton_Click);
            // 
            // VerticesTextBox
            // 
            this.VerticesTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.VerticesTextBox.Location = new System.Drawing.Point(19, 54);
            this.VerticesTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.VerticesTextBox.Name = "VerticesTextBox";
            this.VerticesTextBox.Size = new System.Drawing.Size(58, 26);
            this.VerticesTextBox.TabIndex = 2;
            this.VerticesTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // ParameterTextBox
            // 
            this.ParameterTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.ParameterTextBox.Location = new System.Drawing.Point(83, 54);
            this.ParameterTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.ParameterTextBox.Name = "ParameterTextBox";
            this.ParameterTextBox.Size = new System.Drawing.Size(68, 26);
            this.ParameterTextBox.TabIndex = 3;
            this.ParameterTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // LineLabel1
            // 
            this.LineLabel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.LineLabel1.Location = new System.Drawing.Point(18, 91);
            this.LineLabel1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.LineLabel1.Name = "LineLabel1";
            this.LineLabel1.Size = new System.Drawing.Size(579, 2);
            this.LineLabel1.TabIndex = 8;
            // 
            // NetworkTypeComboBox
            // 
            this.NetworkTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.NetworkTypeComboBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.NetworkTypeComboBox.FormattingEnabled = true;
            this.NetworkTypeComboBox.Items.AddRange(new object[] {
            "Component",
            "No outliers",
            "Everything"});
            this.NetworkTypeComboBox.Location = new System.Drawing.Point(18, 265);
            this.NetworkTypeComboBox.Margin = new System.Windows.Forms.Padding(1);
            this.NetworkTypeComboBox.Name = "NetworkTypeComboBox";
            this.NetworkTypeComboBox.Size = new System.Drawing.Size(120, 28);
            this.NetworkTypeComboBox.TabIndex = 25;
            // 
            // OverlapTextBox
            // 
            this.OverlapTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.OverlapTextBox.Location = new System.Drawing.Point(376, 478);
            this.OverlapTextBox.Name = "OverlapTextBox";
            this.OverlapTextBox.Size = new System.Drawing.Size(76, 26);
            this.OverlapTextBox.TabIndex = 40;
            this.OverlapTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // GdfOverlapButton
            // 
            this.GdfOverlapButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.GdfOverlapButton.Location = new System.Drawing.Point(470, 478);
            this.GdfOverlapButton.Name = "GdfOverlapButton";
            this.GdfOverlapButton.Size = new System.Drawing.Size(123, 30);
            this.GdfOverlapButton.TabIndex = 41;
            this.GdfOverlapButton.Text = "GDF overlap";
            this.GdfOverlapButton.UseVisualStyleBackColor = true;
            this.GdfOverlapButton.Click += new System.EventHandler(this.GdfOverlapButton_Click);
            // 
            // LineLabel3
            // 
            this.LineLabel3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.LineLabel3.Location = new System.Drawing.Point(18, 422);
            this.LineLabel3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.LineLabel3.Name = "LineLabel3";
            this.LineLabel3.Size = new System.Drawing.Size(579, 2);
            this.LineLabel3.TabIndex = 35;
            // 
            // CommunityDetectionCheckBox
            // 
            this.CommunityDetectionCheckBox.AutoSize = true;
            this.CommunityDetectionCheckBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.CommunityDetectionCheckBox.Location = new System.Drawing.Point(21, 480);
            this.CommunityDetectionCheckBox.Margin = new System.Windows.Forms.Padding(1);
            this.CommunityDetectionCheckBox.Name = "CommunityDetectionCheckBox";
            this.CommunityDetectionCheckBox.Size = new System.Drawing.Size(246, 24);
            this.CommunityDetectionCheckBox.TabIndex = 38;
            this.CommunityDetectionCheckBox.Text = "Community detection (Louvain)";
            this.CommunityDetectionCheckBox.UseVisualStyleBackColor = true;
            // 
            // ZxCCheckBox
            // 
            this.ZxCCheckBox.AutoSize = true;
            this.ZxCCheckBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.ZxCCheckBox.Location = new System.Drawing.Point(308, 480);
            this.ZxCCheckBox.Margin = new System.Windows.Forms.Padding(1);
            this.ZxCCheckBox.Name = "ZxCCheckBox";
            this.ZxCCheckBox.Size = new System.Drawing.Size(64, 24);
            this.ZxCCheckBox.TabIndex = 39;
            this.ZxCCheckBox.Text = "Z x C";
            this.ZxCCheckBox.UseVisualStyleBackColor = true;
            // 
            // LargeScaleCheckBox
            // 
            this.LargeScaleCheckBox.AutoSize = true;
            this.LargeScaleCheckBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.LargeScaleCheckBox.Location = new System.Drawing.Point(20, 327);
            this.LargeScaleCheckBox.Margin = new System.Windows.Forms.Padding(2);
            this.LargeScaleCheckBox.Name = "LargeScaleCheckBox";
            this.LargeScaleCheckBox.Size = new System.Drawing.Size(111, 24);
            this.LargeScaleCheckBox.TabIndex = 27;
            this.LargeScaleCheckBox.Text = "Large-scale";
            this.LargeScaleCheckBox.UseVisualStyleBackColor = true;
            // 
            // GroundTruthButton
            // 
            this.GroundTruthButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.GroundTruthButton.Location = new System.Drawing.Point(470, 436);
            this.GroundTruthButton.Name = "GroundTruthButton";
            this.GroundTruthButton.Size = new System.Drawing.Size(123, 31);
            this.GroundTruthButton.TabIndex = 37;
            this.GroundTruthButton.Text = "Ground-truth";
            this.GroundTruthButton.UseVisualStyleBackColor = true;
            this.GroundTruthButton.Click += new System.EventHandler(this.GroundTruthButton_Click);
            // 
            // GroundTruthTextBox
            // 
            this.GroundTruthTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.GroundTruthTextBox.Location = new System.Drawing.Point(18, 439);
            this.GroundTruthTextBox.Name = "GroundTruthTextBox";
            this.GroundTruthTextBox.ReadOnly = true;
            this.GroundTruthTextBox.Size = new System.Drawing.Size(434, 26);
            this.GroundTruthTextBox.TabIndex = 36;
            this.GroundTruthTextBox.DoubleClick += new System.EventHandler(this.GroundTruthTextBox_DoubleClick);
            // 
            // UkazkaButton
            // 
            this.UkazkaButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.UkazkaButton.Location = new System.Drawing.Point(470, 51);
            this.UkazkaButton.Name = "UkazkaButton";
            this.UkazkaButton.Size = new System.Drawing.Size(123, 29);
            this.UkazkaButton.TabIndex = 7;
            this.UkazkaButton.Text = "Ukázka";
            this.UkazkaButton.UseVisualStyleBackColor = true;
            this.UkazkaButton.Click += new System.EventHandler(this.UkazkaButton_Click);
            // 
            // ProminencyFilterComboBox
            // 
            this.ProminencyFilterComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ProminencyFilterComboBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.ProminencyFilterComboBox.FormattingEnabled = true;
            this.ProminencyFilterComboBox.Location = new System.Drawing.Point(158, 102);
            this.ProminencyFilterComboBox.Margin = new System.Windows.Forms.Padding(1);
            this.ProminencyFilterComboBox.Name = "ProminencyFilterComboBox";
            this.ProminencyFilterComboBox.Size = new System.Drawing.Size(296, 28);
            this.ProminencyFilterComboBox.TabIndex = 10;
            // 
            // GroupingStrategyComboBox
            // 
            this.GroupingStrategyComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.GroupingStrategyComboBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.GroupingStrategyComboBox.FormattingEnabled = true;
            this.GroupingStrategyComboBox.Location = new System.Drawing.Point(158, 171);
            this.GroupingStrategyComboBox.Margin = new System.Windows.Forms.Padding(1);
            this.GroupingStrategyComboBox.Name = "GroupingStrategyComboBox";
            this.GroupingStrategyComboBox.Size = new System.Drawing.Size(296, 28);
            this.GroupingStrategyComboBox.TabIndex = 14;
            // 
            // ProminencyFilterLabel
            // 
            this.ProminencyFilterLabel.AutoSize = true;
            this.ProminencyFilterLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.ProminencyFilterLabel.Location = new System.Drawing.Point(18, 105);
            this.ProminencyFilterLabel.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.ProminencyFilterLabel.Name = "ProminencyFilterLabel";
            this.ProminencyFilterLabel.Size = new System.Drawing.Size(125, 20);
            this.ProminencyFilterLabel.TabIndex = 9;
            this.ProminencyFilterLabel.Text = "Prominency filter";
            // 
            // GroupingStrategyLabel
            // 
            this.GroupingStrategyLabel.AutoSize = true;
            this.GroupingStrategyLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.GroupingStrategyLabel.Location = new System.Drawing.Point(20, 174);
            this.GroupingStrategyLabel.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.GroupingStrategyLabel.Name = "GroupingStrategyLabel";
            this.GroupingStrategyLabel.Size = new System.Drawing.Size(136, 20);
            this.GroupingStrategyLabel.TabIndex = 13;
            this.GroupingStrategyLabel.Text = "Grouping strategy";
            // 
            // LineLabel2
            // 
            this.LineLabel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.LineLabel2.Location = new System.Drawing.Point(18, 252);
            this.LineLabel2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.LineLabel2.Name = "LineLabel2";
            this.LineLabel2.Size = new System.Drawing.Size(579, 2);
            this.LineLabel2.TabIndex = 24;
            // 
            // NetDepLabel
            // 
            this.NetDepLabel.AutoSize = true;
            this.NetDepLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.NetDepLabel.Location = new System.Drawing.Point(18, 216);
            this.NetDepLabel.Name = "NetDepLabel";
            this.NetDepLabel.Size = new System.Drawing.Size(68, 20);
            this.NetDepLabel.TabIndex = 16;
            this.NetDepLabel.Text = "Net Dep";
            // 
            // MinSizeLabel
            // 
            this.MinSizeLabel.AutoSize = true;
            this.MinSizeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.MinSizeLabel.Location = new System.Drawing.Point(330, 216);
            this.MinSizeLabel.Name = "MinSizeLabel";
            this.MinSizeLabel.Size = new System.Drawing.Size(66, 20);
            this.MinSizeLabel.TabIndex = 20;
            this.MinSizeLabel.Text = "Min size";
            // 
            // MaxSizeLabel
            // 
            this.MaxSizeLabel.AutoSize = true;
            this.MaxSizeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.MaxSizeLabel.Location = new System.Drawing.Point(466, 216);
            this.MaxSizeLabel.Name = "MaxSizeLabel";
            this.MaxSizeLabel.Size = new System.Drawing.Size(70, 20);
            this.MaxSizeLabel.TabIndex = 22;
            this.MaxSizeLabel.Text = "Max size";
            // 
            // NetDepNumericUpDown
            // 
            this.NetDepNumericUpDown.DecimalPlaces = 2;
            this.NetDepNumericUpDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.NetDepNumericUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.NetDepNumericUpDown.Location = new System.Drawing.Point(89, 214);
            this.NetDepNumericUpDown.Maximum = new decimal(new int[] {
            9,
            0,
            0,
            65536});
            this.NetDepNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.NetDepNumericUpDown.Name = "NetDepNumericUpDown";
            this.NetDepNumericUpDown.Size = new System.Drawing.Size(59, 26);
            this.NetDepNumericUpDown.TabIndex = 17;
            this.NetDepNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.NetDepNumericUpDown.Value = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            // 
            // MinSizeNumericUpDown
            // 
            this.MinSizeNumericUpDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.MinSizeNumericUpDown.Location = new System.Drawing.Point(402, 214);
            this.MinSizeNumericUpDown.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.MinSizeNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.MinSizeNumericUpDown.Name = "MinSizeNumericUpDown";
            this.MinSizeNumericUpDown.Size = new System.Drawing.Size(50, 26);
            this.MinSizeNumericUpDown.TabIndex = 21;
            this.MinSizeNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.MinSizeNumericUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // MaxSizeNumericUpDown
            // 
            this.MaxSizeNumericUpDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.MaxSizeNumericUpDown.Location = new System.Drawing.Point(543, 214);
            this.MaxSizeNumericUpDown.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.MaxSizeNumericUpDown.Name = "MaxSizeNumericUpDown";
            this.MaxSizeNumericUpDown.Size = new System.Drawing.Size(50, 26);
            this.MaxSizeNumericUpDown.TabIndex = 23;
            this.MaxSizeNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // DuplicityFilterLabel
            // 
            this.DuplicityFilterLabel.AutoSize = true;
            this.DuplicityFilterLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.DuplicityFilterLabel.Location = new System.Drawing.Point(18, 139);
            this.DuplicityFilterLabel.Name = "DuplicityFilterLabel";
            this.DuplicityFilterLabel.Size = new System.Drawing.Size(102, 20);
            this.DuplicityFilterLabel.TabIndex = 11;
            this.DuplicityFilterLabel.Text = "Duplicity filter";
            // 
            // DuplicityFilterComboBox
            // 
            this.DuplicityFilterComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.DuplicityFilterComboBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.DuplicityFilterComboBox.FormattingEnabled = true;
            this.DuplicityFilterComboBox.Location = new System.Drawing.Point(158, 136);
            this.DuplicityFilterComboBox.Name = "DuplicityFilterComboBox";
            this.DuplicityFilterComboBox.Size = new System.Drawing.Size(296, 28);
            this.DuplicityFilterComboBox.TabIndex = 12;
            // 
            // DefaultSettingButton
            // 
            this.DefaultSettingButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.DefaultSettingButton.Location = new System.Drawing.Point(470, 102);
            this.DefaultSettingButton.Name = "DefaultSettingButton";
            this.DefaultSettingButton.Size = new System.Drawing.Size(123, 97);
            this.DefaultSettingButton.TabIndex = 15;
            this.DefaultSettingButton.Text = "Default setting";
            this.DefaultSettingButton.UseVisualStyleBackColor = true;
            this.DefaultSettingButton.Click += new System.EventHandler(this.DefaultSettingButton_Click);
            // 
            // DependencyClosureButton
            // 
            this.DependencyClosureButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.DependencyClosureButton.Location = new System.Drawing.Point(18, 515);
            this.DependencyClosureButton.Name = "DependencyClosureButton";
            this.DependencyClosureButton.Size = new System.Drawing.Size(575, 31);
            this.DependencyClosureButton.TabIndex = 42;
            this.DependencyClosureButton.Text = "Dependency closure";
            this.DependencyClosureButton.UseVisualStyleBackColor = true;
            this.DependencyClosureButton.Click += new System.EventHandler(this.DependencyClosureButton_Click);
            // 
            // GroupDepLabel
            // 
            this.GroupDepLabel.AutoSize = true;
            this.GroupDepLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.GroupDepLabel.Location = new System.Drawing.Point(154, 216);
            this.GroupDepLabel.Name = "GroupDepLabel";
            this.GroupDepLabel.Size = new System.Drawing.Size(85, 20);
            this.GroupDepLabel.TabIndex = 18;
            this.GroupDepLabel.Text = "Group dep";
            // 
            // GroupDepNumericUpDown
            // 
            this.GroupDepNumericUpDown.DecimalPlaces = 2;
            this.GroupDepNumericUpDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.GroupDepNumericUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.GroupDepNumericUpDown.Location = new System.Drawing.Point(245, 214);
            this.GroupDepNumericUpDown.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.GroupDepNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.GroupDepNumericUpDown.Name = "GroupDepNumericUpDown";
            this.GroupDepNumericUpDown.Size = new System.Drawing.Size(59, 26);
            this.GroupDepNumericUpDown.TabIndex = 19;
            this.GroupDepNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // P
            // 
            this.P.Location = new System.Drawing.Point(18, 552);
            this.P.Margin = new System.Windows.Forms.Padding(2);
            this.P.Name = "P";
            this.P.Size = new System.Drawing.Size(575, 33);
            this.P.TabIndex = 43;
            this.P.Text = "Triad Evolution";
            this.P.UseVisualStyleBackColor = true;
            this.P.Click += new System.EventHandler(this.button1_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(18, 589);
            this.button1.Margin = new System.Windows.Forms.Padding(2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(575, 33);
            this.button1.TabIndex = 44;
            this.button1.Text = "Triad Evolution / steps";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(19, 627);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(565, 33);
            this.button2.TabIndex = 45;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // AppForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(610, 739);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.P);
            this.Controls.Add(this.DependencyClosureButton);
            this.Controls.Add(this.DefaultSettingButton);
            this.Controls.Add(this.DuplicityFilterComboBox);
            this.Controls.Add(this.DuplicityFilterLabel);
            this.Controls.Add(this.MaxSizeNumericUpDown);
            this.Controls.Add(this.MinSizeNumericUpDown);
            this.Controls.Add(this.GroupDepNumericUpDown);
            this.Controls.Add(this.NetDepNumericUpDown);
            this.Controls.Add(this.MaxSizeLabel);
            this.Controls.Add(this.MinSizeLabel);
            this.Controls.Add(this.GroupDepLabel);
            this.Controls.Add(this.NetDepLabel);
            this.Controls.Add(this.GroupingStrategyLabel);
            this.Controls.Add(this.ProminencyFilterLabel);
            this.Controls.Add(this.GroupingStrategyComboBox);
            this.Controls.Add(this.ProminencyFilterComboBox);
            this.Controls.Add(this.UkazkaButton);
            this.Controls.Add(this.GroundTruthTextBox);
            this.Controls.Add(this.GroundTruthButton);
            this.Controls.Add(this.ZxCCheckBox);
            this.Controls.Add(this.CommunityDetectionCheckBox);
            this.Controls.Add(this.GdfOverlapButton);
            this.Controls.Add(this.OverlapTextBox);
            this.Controls.Add(this.NetworkTypeComboBox);
            this.Controls.Add(this.LineLabel3);
            this.Controls.Add(this.LineLabel2);
            this.Controls.Add(this.LineLabel1);
            this.Controls.Add(this.ParameterTextBox);
            this.Controls.Add(this.VerticesTextBox);
            this.Controls.Add(this.TriadicClosureModelButton);
            this.Controls.Add(this.BarabasiAlbertModelButton);
            this.Controls.Add(this.ErdosRenyiModelButton);
            this.Controls.Add(this.GdfMinNumericUpDown);
            this.Controls.Add(this.ProgressLabel);
            this.Controls.Add(this.SuperSubCheckBox);
            this.Controls.Add(this.DepEdgeCheckBox);
            this.Controls.Add(this.LargeScaleCheckBox);
            this.Controls.Add(this.ZonesProgressBar);
            this.Controls.Add(this.OuterZoneCheckBox);
            this.Controls.Add(this.DetectZonesButton);
            this.Controls.Add(this.ReadFileButton);
            this.Controls.Add(this.PathTextBox);
            this.Controls.Add(this.GdfCheckBox);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.Name = "AppForm";
            this.Text = "Ego-zones detector";
            this.Load += new System.EventHandler(this.AppForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.GdfMinNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NetDepNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MinSizeNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MaxSizeNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.GroupDepNumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox PathTextBox;
        private System.Windows.Forms.Button ReadFileButton;
        private System.Windows.Forms.Button DetectZonesButton;
        private System.Windows.Forms.OpenFileDialog OpenFileDialog;
        private System.Windows.Forms.CheckBox GdfCheckBox;
        private System.Windows.Forms.CheckBox OuterZoneCheckBox;
        private System.Windows.Forms.NumericUpDown GdfMinNumericUpDown;
        private System.Windows.Forms.ProgressBar ZonesProgressBar;
        private System.Windows.Forms.CheckBox DepEdgeCheckBox;
        private System.Windows.Forms.CheckBox SuperSubCheckBox;
        private System.Windows.Forms.Label ProgressLabel;
        private System.Windows.Forms.Button ErdosRenyiModelButton;
        private System.Windows.Forms.Button BarabasiAlbertModelButton;
        private System.Windows.Forms.Button TriadicClosureModelButton;
        private System.Windows.Forms.TextBox VerticesTextBox;
        private System.Windows.Forms.TextBox ParameterTextBox;
        private System.Windows.Forms.SaveFileDialog SaveFileDialog;
        private System.Windows.Forms.Label LineLabel1;
        private System.Windows.Forms.ComboBox NetworkTypeComboBox;
        private System.Windows.Forms.TextBox OverlapTextBox;
        private System.Windows.Forms.Button GdfOverlapButton;
        private System.Windows.Forms.Label LineLabel3;
        private System.Windows.Forms.CheckBox CommunityDetectionCheckBox;
        private System.Windows.Forms.CheckBox ZxCCheckBox;
        private System.Windows.Forms.CheckBox LargeScaleCheckBox;
        private System.Windows.Forms.Button GroundTruthButton;
        private System.Windows.Forms.TextBox GroundTruthTextBox;
        private System.Windows.Forms.Button UkazkaButton;
        private System.Windows.Forms.ComboBox ProminencyFilterComboBox;
        private System.Windows.Forms.ComboBox GroupingStrategyComboBox;
        private System.Windows.Forms.Label ProminencyFilterLabel;
        private System.Windows.Forms.Label GroupingStrategyLabel;
        private System.Windows.Forms.Label LineLabel2;
        private System.Windows.Forms.Label NetDepLabel;
        private System.Windows.Forms.Label MinSizeLabel;
        private System.Windows.Forms.Label MaxSizeLabel;
        private System.Windows.Forms.NumericUpDown NetDepNumericUpDown;
        private System.Windows.Forms.NumericUpDown MinSizeNumericUpDown;
        private System.Windows.Forms.NumericUpDown MaxSizeNumericUpDown;
        private System.Windows.Forms.Label DuplicityFilterLabel;
        private System.Windows.Forms.ComboBox DuplicityFilterComboBox;
        private System.Windows.Forms.Button DefaultSettingButton;
        private System.Windows.Forms.Button DependencyClosureButton;
        private System.Windows.Forms.Label GroupDepLabel;
        private System.Windows.Forms.NumericUpDown GroupDepNumericUpDown;
        private System.Windows.Forms.Button P;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
    }
}

