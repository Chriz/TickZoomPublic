namespace TickZoom
{
    partial class Form1
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
                commandWorker.CancelAsync();
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
        	this.lblSymbol = new System.Windows.Forms.Label();
        	this.txtSymbol = new System.Windows.Forms.TextBox();
        	this.btnOptimize = new System.Windows.Forms.Button();
        	this.prgExecute = new System.Windows.Forms.ProgressBar();
        	this.btnStop = new System.Windows.Forms.Button();
        	this.lblProgress = new System.Windows.Forms.Label();
        	this.btnRun = new System.Windows.Forms.Button();
        	this.startTimePicker = new System.Windows.Forms.DateTimePicker();
        	this.endTimePicker = new System.Windows.Forms.DateTimePicker();
        	this.startLabel = new System.Windows.Forms.Label();
        	this.endLabel = new System.Windows.Forms.Label();
        	this.commandWorker = new System.ComponentModel.BackgroundWorker();
        	this.liveButton = new System.Windows.Forms.Button();
        	this.logOutput = new System.Windows.Forms.TextBox();
        	this.breakAtBarText = new System.Windows.Forms.TextBox();
        	this.label1 = new System.Windows.Forms.Label();
        	this.replaySpeedTextBox = new System.Windows.Forms.TextBox();
        	this.label2 = new System.Windows.Forms.Label();
        	this.intervalEngineTxt = new System.Windows.Forms.Label();
        	this.engineBarsCombo = new System.Windows.Forms.ComboBox();
        	this.intervals = new System.Windows.Forms.GroupBox();
        	this.chartBarsCheckBox = new System.Windows.Forms.CheckBox();
        	this.chartBarsBox2 = new System.Windows.Forms.TextBox();
        	this.chartBarsCombo2 = new System.Windows.Forms.ComboBox();
        	this.engineRollingCheckBox = new System.Windows.Forms.CheckBox();
        	this.engineBarsBox2 = new System.Windows.Forms.TextBox();
        	this.engineBarsCombo2 = new System.Windows.Forms.ComboBox();
        	this.label7 = new System.Windows.Forms.Label();
        	this.timeChartRadio = new System.Windows.Forms.RadioButton();
        	this.barChartRadio = new System.Windows.Forms.RadioButton();
        	this.label3 = new System.Windows.Forms.Label();
        	this.label4 = new System.Windows.Forms.Label();
        	this.copyDebugCheckBox = new System.Windows.Forms.CheckBox();
        	this.chartUpdateBox = new System.Windows.Forms.TextBox();
        	this.chartBarsBox = new System.Windows.Forms.TextBox();
        	this.chartDisplayBox = new System.Windows.Forms.TextBox();
        	this.engineBarsBox = new System.Windows.Forms.TextBox();
        	this.defaultBox = new System.Windows.Forms.TextBox();
        	this.defaultOnly = new System.Windows.Forms.CheckBox();
        	this.chartUpdateCombo = new System.Windows.Forms.ComboBox();
        	this.chartUpdateTxt = new System.Windows.Forms.Label();
        	this.chartBarsCombo = new System.Windows.Forms.ComboBox();
        	this.chartDisplayCombo = new System.Windows.Forms.ComboBox();
        	this.defaultTxt = new System.Windows.Forms.Label();
        	this.defaultCombo = new System.Windows.Forms.ComboBox();
        	this.chartBarsTxt = new System.Windows.Forms.Label();
        	this.chartTxt = new System.Windows.Forms.Label();
        	this.timeFrameTxt = new System.Windows.Forms.Label();
        	this.periodTxt = new System.Windows.Forms.Label();
        	this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
        	this.btnGenetic = new System.Windows.Forms.Button();
        	this.modelLoaderBox = new System.Windows.Forms.ComboBox();
        	this.groupBox1 = new System.Windows.Forms.GroupBox();
        	this.label5 = new System.Windows.Forms.Label();
        	this.intervals.SuspendLayout();
        	this.groupBox1.SuspendLayout();
        	this.SuspendLayout();
        	// 
        	// lblSymbol
        	// 
        	this.lblSymbol.AutoSize = true;
        	this.lblSymbol.Location = new System.Drawing.Point(13, 16);
        	this.lblSymbol.Name = "lblSymbol";
        	this.lblSymbol.Size = new System.Drawing.Size(41, 13);
        	this.lblSymbol.TabIndex = 0;
        	this.lblSymbol.Text = "Symbol";
        	// 
        	// txtSymbol
        	// 
        	this.txtSymbol.Location = new System.Drawing.Point(78, 13);
        	this.txtSymbol.Name = "txtSymbol";
        	this.txtSymbol.Size = new System.Drawing.Size(110, 20);
        	this.txtSymbol.TabIndex = 4;
        	this.txtSymbol.Text = "USD/JPY";
        	// 
        	// btnOptimize
        	// 
        	this.btnOptimize.Location = new System.Drawing.Point(110, 118);
        	this.btnOptimize.Name = "btnOptimize";
        	this.btnOptimize.Size = new System.Drawing.Size(78, 23);
        	this.btnOptimize.TabIndex = 6;
        	this.btnOptimize.Text = "Optimize";
        	this.btnOptimize.UseVisualStyleBackColor = true;
        	this.btnOptimize.Click += new System.EventHandler(this.btnOptimize_Click);
        	// 
        	// prgExecute
        	// 
        	this.prgExecute.Location = new System.Drawing.Point(12, 169);
        	this.prgExecute.Name = "prgExecute";
        	this.prgExecute.Size = new System.Drawing.Size(475, 23);
        	this.prgExecute.TabIndex = 8;
        	// 
        	// btnStop
        	// 
        	this.btnStop.Location = new System.Drawing.Point(413, 118);
        	this.btnStop.Name = "btnStop";
        	this.btnStop.Size = new System.Drawing.Size(75, 23);
        	this.btnStop.TabIndex = 8;
        	this.btnStop.Text = "Stop";
        	this.btnStop.UseVisualStyleBackColor = true;
        	this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
        	// 
        	// lblProgress
        	// 
        	this.lblProgress.AutoSize = true;
        	this.lblProgress.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
        	this.lblProgress.Location = new System.Drawing.Point(12, 149);
        	this.lblProgress.Name = "lblProgress";
        	this.lblProgress.Size = new System.Drawing.Size(72, 13);
        	this.lblProgress.TabIndex = 8;
        	this.lblProgress.Text = "Awaiting Start";
        	// 
        	// btnRun
        	// 
        	this.btnRun.Location = new System.Drawing.Point(13, 118);
        	this.btnRun.Name = "btnRun";
        	this.btnRun.Size = new System.Drawing.Size(75, 23);
        	this.btnRun.TabIndex = 5;
        	this.btnRun.Text = "Start Run";
        	this.btnRun.UseVisualStyleBackColor = true;
        	this.btnRun.Click += new System.EventHandler(this.btnStartRun_Click);
        	// 
        	// startTimePicker
        	// 
        	this.startTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
        	this.startTimePicker.Location = new System.Drawing.Point(78, 51);
        	this.startTimePicker.Name = "startTimePicker";
        	this.startTimePicker.Size = new System.Drawing.Size(110, 20);
        	this.startTimePicker.TabIndex = 1;
        	this.startTimePicker.CloseUp += new System.EventHandler(this.StartTimePickerCloseUp);
        	// 
        	// endTimePicker
        	// 
        	this.endTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
        	this.endTimePicker.Location = new System.Drawing.Point(78, 91);
        	this.endTimePicker.Name = "endTimePicker";
        	this.endTimePicker.Size = new System.Drawing.Size(110, 20);
        	this.endTimePicker.TabIndex = 2;
        	this.endTimePicker.CloseUp += new System.EventHandler(this.EndTimePickerCloseUp);
        	// 
        	// startLabel
        	// 
        	this.startLabel.Location = new System.Drawing.Point(12, 55);
        	this.startLabel.Name = "startLabel";
        	this.startLabel.Size = new System.Drawing.Size(60, 16);
        	this.startLabel.TabIndex = 14;
        	this.startLabel.Text = "Start Date";
        	// 
        	// endLabel
        	// 
        	this.endLabel.Location = new System.Drawing.Point(12, 95);
        	this.endLabel.Name = "endLabel";
        	this.endLabel.Size = new System.Drawing.Size(60, 16);
        	this.endLabel.TabIndex = 15;
        	this.endLabel.Text = "End Date";
        	// 
        	// processWorker
        	// 
        	this.commandWorker.WorkerReportsProgress = true;
        	this.commandWorker.WorkerSupportsCancellation = true;
        	this.commandWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.ProcessWorkerDoWork);
        	this.commandWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.ProcessWorkerRunWorkerCompleted);
        	this.commandWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.ProcessWorkerProgressChanged);
        	// 
        	// liveButton
        	// 
        	this.liveButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.liveButton.Location = new System.Drawing.Point(318, 118);
        	this.liveButton.Name = "liveButton";
        	this.liveButton.Size = new System.Drawing.Size(74, 23);
        	this.liveButton.TabIndex = 16;
        	this.liveButton.Text = "Real Time";
        	this.liveButton.UseVisualStyleBackColor = true;
        	this.liveButton.Click += new System.EventHandler(this.RealTimeButtonClick);
        	// 
        	// output
        	// 
        	this.logOutput.Location = new System.Drawing.Point(12, 198);
        	this.logOutput.Multiline = true;
        	this.logOutput.Name = "output";
        	this.logOutput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
        	this.logOutput.Size = new System.Drawing.Size(475, 161);
        	this.logOutput.TabIndex = 17;
        	// 
        	// breakAtBarText
        	// 
        	this.breakAtBarText.Location = new System.Drawing.Point(428, 92);
        	this.breakAtBarText.Name = "breakAtBarText";
        	this.breakAtBarText.Size = new System.Drawing.Size(60, 20);
        	this.breakAtBarText.TabIndex = 18;
        	// 
        	// label1
        	// 
        	this.label1.Location = new System.Drawing.Point(354, 95);
        	this.label1.Name = "label1";
        	this.label1.Size = new System.Drawing.Size(68, 16);
        	this.label1.TabIndex = 19;
        	this.label1.Text = "Break at bar";
        	// 
        	// replaySpeedTextBox
        	// 
        	this.replaySpeedTextBox.Location = new System.Drawing.Point(280, 92);
        	this.replaySpeedTextBox.Name = "replaySpeedTextBox";
        	this.replaySpeedTextBox.Size = new System.Drawing.Size(58, 20);
        	this.replaySpeedTextBox.TabIndex = 20;
        	// 
        	// label2
        	// 
        	this.label2.Location = new System.Drawing.Point(194, 95);
        	this.label2.Name = "label2";
        	this.label2.Size = new System.Drawing.Size(80, 23);
        	this.label2.TabIndex = 21;
        	this.label2.Text = "Replay Speed";
        	// 
        	// intervalEngineTxt
        	// 
        	this.intervalEngineTxt.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.intervalEngineTxt.Location = new System.Drawing.Point(6, 109);
        	this.intervalEngineTxt.Name = "intervalEngineTxt";
        	this.intervalEngineTxt.Size = new System.Drawing.Size(80, 15);
        	this.intervalEngineTxt.TabIndex = 22;
        	this.intervalEngineTxt.Text = "Engine Bars";
        	// 
        	// engineBarsCombo
        	// 
        	this.engineBarsCombo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.engineBarsCombo.FormattingEnabled = true;
        	this.engineBarsCombo.Location = new System.Drawing.Point(145, 105);
        	this.engineBarsCombo.Name = "engineBarsCombo";
        	this.engineBarsCombo.Size = new System.Drawing.Size(121, 21);
        	this.engineBarsCombo.TabIndex = 23;
        	this.engineBarsCombo.SelectedIndexChanged += new System.EventHandler(this.IntervalChange);
        	// 
        	// intervals
        	// 
        	this.intervals.Controls.Add(this.chartBarsCheckBox);
        	this.intervals.Controls.Add(this.chartBarsBox2);
        	this.intervals.Controls.Add(this.chartBarsCombo2);
        	this.intervals.Controls.Add(this.engineRollingCheckBox);
        	this.intervals.Controls.Add(this.engineBarsBox2);
        	this.intervals.Controls.Add(this.engineBarsCombo2);
        	this.intervals.Controls.Add(this.label7);
        	this.intervals.Controls.Add(this.timeChartRadio);
        	this.intervals.Controls.Add(this.barChartRadio);
        	this.intervals.Controls.Add(this.label3);
        	this.intervals.Controls.Add(this.label4);
        	this.intervals.Controls.Add(this.copyDebugCheckBox);
        	this.intervals.Controls.Add(this.chartUpdateBox);
        	this.intervals.Controls.Add(this.chartBarsBox);
        	this.intervals.Controls.Add(this.chartDisplayBox);
        	this.intervals.Controls.Add(this.engineBarsBox);
        	this.intervals.Controls.Add(this.defaultBox);
        	this.intervals.Controls.Add(this.defaultOnly);
        	this.intervals.Controls.Add(this.chartUpdateCombo);
        	this.intervals.Controls.Add(this.chartUpdateTxt);
        	this.intervals.Controls.Add(this.chartBarsCombo);
        	this.intervals.Controls.Add(this.chartDisplayCombo);
        	this.intervals.Controls.Add(this.defaultTxt);
        	this.intervals.Controls.Add(this.defaultCombo);
        	this.intervals.Controls.Add(this.chartBarsTxt);
        	this.intervals.Controls.Add(this.chartTxt);
        	this.intervals.Controls.Add(this.timeFrameTxt);
        	this.intervals.Controls.Add(this.periodTxt);
        	this.intervals.Controls.Add(this.intervalEngineTxt);
        	this.intervals.Controls.Add(this.engineBarsCombo);
        	this.intervals.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.intervals.Location = new System.Drawing.Point(494, 10);
        	this.intervals.Name = "intervals";
        	this.intervals.Size = new System.Drawing.Size(276, 349);
        	this.intervals.TabIndex = 25;
        	this.intervals.TabStop = false;
        	this.intervals.Text = "Intervals";
        	// 
        	// chartBarsCheckBox
        	// 
        	this.chartBarsCheckBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.chartBarsCheckBox.Location = new System.Drawing.Point(19, 190);
        	this.chartBarsCheckBox.Name = "chartBarsCheckBox";
        	this.chartBarsCheckBox.Size = new System.Drawing.Size(59, 24);
        	this.chartBarsCheckBox.TabIndex = 59;
        	this.chartBarsCheckBox.Text = "Rolling";
        	this.chartBarsCheckBox.UseVisualStyleBackColor = true;
        	this.chartBarsCheckBox.Click += new System.EventHandler(this.ChartBarsCheckBoxClick);
        	// 
        	// chartBarsBox2
        	// 
        	this.chartBarsBox2.Location = new System.Drawing.Point(92, 193);
        	this.chartBarsBox2.Name = "chartBarsBox2";
        	this.chartBarsBox2.Size = new System.Drawing.Size(46, 20);
        	this.chartBarsBox2.TabIndex = 58;
        	this.chartBarsBox2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
        	// 
        	// chartBarsCombo2
        	// 
        	this.chartBarsCombo2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.chartBarsCombo2.FormattingEnabled = true;
        	this.chartBarsCombo2.Location = new System.Drawing.Point(145, 193);
        	this.chartBarsCombo2.Name = "chartBarsCombo2";
        	this.chartBarsCombo2.Size = new System.Drawing.Size(121, 21);
        	this.chartBarsCombo2.TabIndex = 57;
        	// 
        	// engineRollingCheckBox
        	// 
        	this.engineRollingCheckBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.engineRollingCheckBox.Location = new System.Drawing.Point(19, 128);
        	this.engineRollingCheckBox.Name = "engineRollingCheckBox";
        	this.engineRollingCheckBox.Size = new System.Drawing.Size(59, 24);
        	this.engineRollingCheckBox.TabIndex = 56;
        	this.engineRollingCheckBox.Text = "Rolling";
        	this.engineRollingCheckBox.UseVisualStyleBackColor = true;
        	this.engineRollingCheckBox.Click += new System.EventHandler(this.EngineRollingCheckBoxClick);
        	// 
        	// engineBarsBox2
        	// 
        	this.engineBarsBox2.Location = new System.Drawing.Point(92, 131);
        	this.engineBarsBox2.Name = "engineBarsBox2";
        	this.engineBarsBox2.Size = new System.Drawing.Size(46, 20);
        	this.engineBarsBox2.TabIndex = 55;
        	this.engineBarsBox2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
        	// 
        	// engineBarsCombo2
        	// 
        	this.engineBarsCombo2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.engineBarsCombo2.FormattingEnabled = true;
        	this.engineBarsCombo2.Location = new System.Drawing.Point(145, 131);
        	this.engineBarsCombo2.Name = "engineBarsCombo2";
        	this.engineBarsCombo2.Size = new System.Drawing.Size(121, 21);
        	this.engineBarsCombo2.TabIndex = 54;
        	// 
        	// label7
        	// 
        	this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.label7.Location = new System.Drawing.Point(6, 287);
        	this.label7.Name = "label7";
        	this.label7.Size = new System.Drawing.Size(80, 20);
        	this.label7.TabIndex = 52;
        	this.label7.Text = "Chart Type";
        	// 
        	// timeChartRadio
        	// 
        	this.timeChartRadio.Location = new System.Drawing.Point(168, 283);
        	this.timeChartRadio.Name = "timeChartRadio";
        	this.timeChartRadio.Size = new System.Drawing.Size(104, 24);
        	this.timeChartRadio.TabIndex = 51;
        	this.timeChartRadio.Text = "Time Chart";
        	this.timeChartRadio.UseVisualStyleBackColor = true;
        	this.timeChartRadio.Click += new System.EventHandler(this.ChartRadioClick);
        	// 
        	// barChartRadio
        	// 
        	this.barChartRadio.Checked = true;
        	this.barChartRadio.Location = new System.Drawing.Point(92, 283);
        	this.barChartRadio.Name = "barChartRadio";
        	this.barChartRadio.Size = new System.Drawing.Size(70, 24);
        	this.barChartRadio.TabIndex = 50;
        	this.barChartRadio.TabStop = true;
        	this.barChartRadio.Text = "Bar Chart";
        	this.barChartRadio.UseVisualStyleBackColor = true;
        	this.barChartRadio.Click += new System.EventHandler(this.ChartRadioClick);
        	// 
        	// label3
        	// 
        	this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.label3.Location = new System.Drawing.Point(145, 87);
        	this.label3.Name = "label3";
        	this.label3.Size = new System.Drawing.Size(63, 14);
        	this.label3.TabIndex = 47;
        	this.label3.Text = "Time Frame";
        	// 
        	// label4
        	// 
        	this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.label4.Location = new System.Drawing.Point(92, 88);
        	this.label4.Name = "label4";
        	this.label4.Size = new System.Drawing.Size(46, 14);
        	this.label4.TabIndex = 46;
        	this.label4.Text = "Period";
        	// 
        	// copyDebugCheckBox
        	// 
        	this.copyDebugCheckBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.copyDebugCheckBox.Location = new System.Drawing.Point(156, 59);
        	this.copyDebugCheckBox.Name = "copyDebugCheckBox";
        	this.copyDebugCheckBox.Size = new System.Drawing.Size(89, 24);
        	this.copyDebugCheckBox.TabIndex = 45;
        	this.copyDebugCheckBox.Text = "Copy default";
        	this.copyDebugCheckBox.UseVisualStyleBackColor = true;
        	this.copyDebugCheckBox.Click += new System.EventHandler(this.CopyDebugCheckBoxClick);
        	// 
        	// chartUpdateBox
        	// 
        	this.chartUpdateBox.Location = new System.Drawing.Point(92, 257);
        	this.chartUpdateBox.Name = "chartUpdateBox";
        	this.chartUpdateBox.Size = new System.Drawing.Size(46, 20);
        	this.chartUpdateBox.TabIndex = 44;
        	this.chartUpdateBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
        	this.chartUpdateBox.Leave += new System.EventHandler(this.IntervalChange);
        	// 
        	// chartBarsBox
        	// 
        	this.chartBarsBox.Location = new System.Drawing.Point(92, 167);
        	this.chartBarsBox.Name = "chartBarsBox";
        	this.chartBarsBox.Size = new System.Drawing.Size(46, 20);
        	this.chartBarsBox.TabIndex = 43;
        	this.chartBarsBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
        	this.chartBarsBox.Leave += new System.EventHandler(this.IntervalChange);
        	// 
        	// chartDisplayBox
        	// 
        	this.chartDisplayBox.Location = new System.Drawing.Point(92, 231);
        	this.chartDisplayBox.Name = "chartDisplayBox";
        	this.chartDisplayBox.Size = new System.Drawing.Size(46, 20);
        	this.chartDisplayBox.TabIndex = 42;
        	this.chartDisplayBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
        	this.chartDisplayBox.Leave += new System.EventHandler(this.IntervalChange);
        	// 
        	// engineBarsBox
        	// 
        	this.engineBarsBox.Location = new System.Drawing.Point(92, 105);
        	this.engineBarsBox.Name = "engineBarsBox";
        	this.engineBarsBox.Size = new System.Drawing.Size(46, 20);
        	this.engineBarsBox.TabIndex = 41;
        	this.engineBarsBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
        	this.engineBarsBox.Leave += new System.EventHandler(this.IntervalChange);
        	// 
        	// defaultBox
        	// 
        	this.defaultBox.Location = new System.Drawing.Point(92, 36);
        	this.defaultBox.Name = "defaultBox";
        	this.defaultBox.Size = new System.Drawing.Size(46, 20);
        	this.defaultBox.TabIndex = 40;
        	this.defaultBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
        	this.defaultBox.Leave += new System.EventHandler(this.IntervalChange);
        	// 
        	// defaultOnly
        	// 
        	this.defaultOnly.Checked = true;
        	this.defaultOnly.CheckState = System.Windows.Forms.CheckState.Checked;
        	this.defaultOnly.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.defaultOnly.Location = new System.Drawing.Point(27, 59);
        	this.defaultOnly.Name = "defaultOnly";
        	this.defaultOnly.Size = new System.Drawing.Size(123, 24);
        	this.defaultOnly.TabIndex = 39;
        	this.defaultOnly.Text = "Use this default only";
        	this.defaultOnly.UseVisualStyleBackColor = true;
        	this.defaultOnly.Click += new System.EventHandler(this.DefaultOnlyClick);
        	// 
        	// chartUpdateCombo
        	// 
        	this.chartUpdateCombo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.chartUpdateCombo.FormattingEnabled = true;
        	this.chartUpdateCombo.Location = new System.Drawing.Point(145, 256);
        	this.chartUpdateCombo.Name = "chartUpdateCombo";
        	this.chartUpdateCombo.Size = new System.Drawing.Size(121, 21);
        	this.chartUpdateCombo.TabIndex = 37;
        	this.chartUpdateCombo.SelectedIndexChanged += new System.EventHandler(this.IntervalChange);
        	// 
        	// chartUpdateTxt
        	// 
        	this.chartUpdateTxt.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.chartUpdateTxt.Location = new System.Drawing.Point(6, 260);
        	this.chartUpdateTxt.Name = "chartUpdateTxt";
        	this.chartUpdateTxt.Size = new System.Drawing.Size(80, 20);
        	this.chartUpdateTxt.TabIndex = 36;
        	this.chartUpdateTxt.Text = "Chart Update";
        	// 
        	// chartBarsCombo
        	// 
        	this.chartBarsCombo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.chartBarsCombo.FormattingEnabled = true;
        	this.chartBarsCombo.Location = new System.Drawing.Point(145, 166);
        	this.chartBarsCombo.Name = "chartBarsCombo";
        	this.chartBarsCombo.Size = new System.Drawing.Size(121, 21);
        	this.chartBarsCombo.TabIndex = 34;
        	this.chartBarsCombo.SelectedIndexChanged += new System.EventHandler(this.IntervalChange);
        	// 
        	// chartDisplayCombo
        	// 
        	this.chartDisplayCombo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.chartDisplayCombo.FormattingEnabled = true;
        	this.chartDisplayCombo.Location = new System.Drawing.Point(145, 231);
        	this.chartDisplayCombo.Name = "chartDisplayCombo";
        	this.chartDisplayCombo.Size = new System.Drawing.Size(121, 21);
        	this.chartDisplayCombo.TabIndex = 32;
        	this.chartDisplayCombo.SelectedIndexChanged += new System.EventHandler(this.IntervalChange);
        	// 
        	// defaultTxt
        	// 
        	this.defaultTxt.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.defaultTxt.Location = new System.Drawing.Point(6, 39);
        	this.defaultTxt.Name = "defaultTxt";
        	this.defaultTxt.Size = new System.Drawing.Size(80, 15);
        	this.defaultTxt.TabIndex = 29;
        	this.defaultTxt.Text = "Default";
        	// 
        	// defaultCombo
        	// 
        	this.defaultCombo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.defaultCombo.FormattingEnabled = true;
        	this.defaultCombo.Location = new System.Drawing.Point(145, 36);
        	this.defaultCombo.Name = "defaultCombo";
        	this.defaultCombo.Size = new System.Drawing.Size(121, 21);
        	this.defaultCombo.TabIndex = 30;
        	this.defaultCombo.SelectedIndexChanged += new System.EventHandler(this.IntervalChange);
        	// 
        	// chartBarsTxt
        	// 
        	this.chartBarsTxt.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.chartBarsTxt.Location = new System.Drawing.Point(6, 170);
        	this.chartBarsTxt.Name = "chartBarsTxt";
        	this.chartBarsTxt.Size = new System.Drawing.Size(80, 20);
        	this.chartBarsTxt.TabIndex = 28;
        	this.chartBarsTxt.Text = "Chart Bars";
        	// 
        	// chartTxt
        	// 
        	this.chartTxt.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.chartTxt.Location = new System.Drawing.Point(6, 235);
        	this.chartTxt.Name = "chartTxt";
        	this.chartTxt.Size = new System.Drawing.Size(80, 20);
        	this.chartTxt.TabIndex = 27;
        	this.chartTxt.Text = "Chart Display";
        	// 
        	// timeFrameTxt
        	// 
        	this.timeFrameTxt.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.timeFrameTxt.Location = new System.Drawing.Point(145, 19);
        	this.timeFrameTxt.Name = "timeFrameTxt";
        	this.timeFrameTxt.Size = new System.Drawing.Size(63, 14);
        	this.timeFrameTxt.TabIndex = 26;
        	this.timeFrameTxt.Text = "Time Frame";
        	// 
        	// periodTxt
        	// 
        	this.periodTxt.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        	this.periodTxt.Location = new System.Drawing.Point(92, 20);
        	this.periodTxt.Name = "periodTxt";
        	this.periodTxt.Size = new System.Drawing.Size(46, 14);
        	this.periodTxt.TabIndex = 25;
        	this.periodTxt.Text = "Period";
        	// 
        	// btnGenetic
        	// 
        	this.btnGenetic.Location = new System.Drawing.Point(214, 118);
        	this.btnGenetic.Name = "btnGenetic";
        	this.btnGenetic.Size = new System.Drawing.Size(81, 23);
        	this.btnGenetic.TabIndex = 26;
        	this.btnGenetic.Text = "Genetic";
        	this.btnGenetic.UseVisualStyleBackColor = true;
        	this.btnGenetic.Click += new System.EventHandler(this.BtnGeneticClick);
        	// 
        	// modelLoaderBox
        	// 
        	this.modelLoaderBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        	this.modelLoaderBox.FormattingEnabled = true;
        	this.modelLoaderBox.Location = new System.Drawing.Point(118, 13);
        	this.modelLoaderBox.MaxDropDownItems = 20;
        	this.modelLoaderBox.Name = "modelLoaderBox";
        	this.modelLoaderBox.Size = new System.Drawing.Size(165, 21);
        	this.modelLoaderBox.Sorted = true;
        	this.modelLoaderBox.TabIndex = 28;
        	this.modelLoaderBox.SelectedIndexChanged += new System.EventHandler(this.ModelLoaderBoxSelectedIndexChanged);
        	// 
        	// groupBox1
        	// 
        	this.groupBox1.Controls.Add(this.label5);
        	this.groupBox1.Controls.Add(this.modelLoaderBox);
        	this.groupBox1.Location = new System.Drawing.Point(194, 10);
        	this.groupBox1.Name = "groupBox1";
        	this.groupBox1.Size = new System.Drawing.Size(294, 47);
        	this.groupBox1.TabIndex = 30;
        	this.groupBox1.TabStop = false;
        	this.groupBox1.Text = "Load";
        	// 
        	// label5
        	// 
        	this.label5.Location = new System.Drawing.Point(36, 16);
        	this.label5.Name = "label5";
        	this.label5.Size = new System.Drawing.Size(76, 17);
        	this.label5.TabIndex = 29;
        	this.label5.Text = "Model Loader";
        	// 
        	// Form1
        	// 
        	this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        	this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        	this.ClientSize = new System.Drawing.Size(782, 371);
        	this.Controls.Add(this.btnGenetic);
        	this.Controls.Add(this.label2);
        	this.Controls.Add(this.replaySpeedTextBox);
        	this.Controls.Add(this.label1);
        	this.Controls.Add(this.breakAtBarText);
        	this.Controls.Add(this.logOutput);
        	this.Controls.Add(this.liveButton);
        	this.Controls.Add(this.endLabel);
        	this.Controls.Add(this.startLabel);
        	this.Controls.Add(this.endTimePicker);
        	this.Controls.Add(this.startTimePicker);
        	this.Controls.Add(this.btnRun);
        	this.Controls.Add(this.lblProgress);
        	this.Controls.Add(this.btnStop);
        	this.Controls.Add(this.prgExecute);
        	this.Controls.Add(this.btnOptimize);
        	this.Controls.Add(this.txtSymbol);
        	this.Controls.Add(this.lblSymbol);
        	this.Controls.Add(this.intervals);
        	this.Controls.Add(this.groupBox1);
        	this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
        	this.Name = "Form1";
        	this.Text = "TickZOOM";
        	this.Load += new System.EventHandler(this.Form1Load);
        	this.Shown += new System.EventHandler(this.Form1Shown);
        	this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1FormClosing);
        	this.intervals.ResumeLayout(false);
        	this.intervals.PerformLayout();
        	this.groupBox1.ResumeLayout(false);
        	this.ResumeLayout(false);
        	this.PerformLayout();
        }
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox modelLoaderBox;
        private System.Windows.Forms.Label lblSymbol;
        private System.Windows.Forms.TextBox txtSymbol;
        private System.Windows.Forms.Button btnGenetic;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.RadioButton timeChartRadio;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox engineBarsCombo2;
        private System.Windows.Forms.TextBox engineBarsBox2;
        private System.Windows.Forms.CheckBox engineRollingCheckBox;
        private System.Windows.Forms.ComboBox chartBarsCombo2;
        private System.Windows.Forms.TextBox chartBarsBox2;
        private System.Windows.Forms.CheckBox chartBarsCheckBox;
        private System.Windows.Forms.RadioButton barChartRadio;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox copyDebugCheckBox;
        private System.Windows.Forms.TextBox defaultBox;
        private System.Windows.Forms.TextBox engineBarsBox;
        private System.Windows.Forms.TextBox chartDisplayBox;
        private System.Windows.Forms.TextBox chartBarsBox;
        private System.Windows.Forms.TextBox chartUpdateBox;
        private System.Windows.Forms.ComboBox engineBarsCombo;
        private System.Windows.Forms.CheckBox defaultOnly;
        private System.Windows.Forms.ComboBox chartDisplayCombo;
        private System.Windows.Forms.ComboBox chartBarsCombo;
        private System.Windows.Forms.Label chartUpdateTxt;
        private System.Windows.Forms.ComboBox chartUpdateCombo;
        private System.Windows.Forms.ComboBox defaultCombo;
        private System.Windows.Forms.Label defaultTxt;
        private System.Windows.Forms.Label chartTxt;
        private System.Windows.Forms.Label chartBarsTxt;
        private System.Windows.Forms.Label periodTxt;
        private System.Windows.Forms.Label timeFrameTxt;
        private System.Windows.Forms.Label intervalEngineTxt;
        private System.Windows.Forms.GroupBox intervals;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox replaySpeedTextBox;
        private System.Windows.Forms.TextBox breakAtBarText;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox logOutput;
        private System.Windows.Forms.Button liveButton;
        private System.ComponentModel.BackgroundWorker commandWorker;
        private System.Windows.Forms.Label endLabel;
        private System.Windows.Forms.Label startLabel;
        private System.Windows.Forms.DateTimePicker startTimePicker;
        private System.Windows.Forms.DateTimePicker endTimePicker;
        private System.Windows.Forms.Button btnOptimize;
        private System.Windows.Forms.Button btnRun;

        #endregion

        private System.Windows.Forms.ProgressBar prgExecute;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Label lblProgress;
        

        
        void ChartBarsCheckBoxClick(object sender, System.EventArgs e)
        {
        	UpdateCheckBoxes();
        }
        
		public System.ComponentModel.BackgroundWorker ProcessWorker {
			get { return commandWorker; }
		}

		public System.Windows.Forms.TextBox TxtSymbol {
			get { return txtSymbol; }
		}
        
		public System.Windows.Forms.ComboBox DefaultCombo {
			get { return defaultCombo; }
		}
        
		public System.Windows.Forms.TextBox DefaultBox {
			get { return defaultBox; }
		}
    }
}

