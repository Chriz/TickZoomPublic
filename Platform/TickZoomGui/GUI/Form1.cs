#region Copyright
/*
 * Software: TickZoom Trading Platform
 * Copyright 2009 M. Wayne Walter
 * 
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, see <http://www.tickzoom.org/wiki/Licenses>
 * or write to Free Software Foundation, Inc., 51 Franklin Street,
 * Fifth Floor, Boston, MA  02110-1301, USA.
 * 
 */
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.Odbc;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

using TickZoom.Api;
using TickZoom.Common;
using TickZoom.GUI;
//using TickZoom.Provider;
using TickZoom.TickUtil;

namespace TickZoom
{
    
    public partial class Form1 : Form
    {
        // The progress of the task in percentage
        private static int PercentProgress;
        // The delegate which we will call from the thread to update the form
        // When to pause
//        bool goPause = false;
	    public delegate void UpdateProgressDelegate(string fileName, Int64 BytesRead, Int64 TotalBytes);
	    public delegate Chart CreateChartDelegate();
   		DateTime startTime = new DateTime(2003,1,1); 
   		DateTime endTime = DateTime.Now;
// 		ProviderProxy provider = null;
		Log log;
		
        Interval initialInterval;
        Interval intervalDefault;
		Interval intervalEngine;
		Interval intervalChartDisplay;
		Interval intervalChartBar;
		Interval intervalChartUpdate;
		string storageFolder;
		string tickZoomEngine;
        bool isInitialized = false;
		
		public DateTime EndTime {
			get { return endTime; }
			set { endTime = value; }
		}
        Thread thread_ProcessMessages;
   		
        public Form1()
        {
        	log = Factory.Log.GetLogger(typeof(Form1));
//	        initialInterval = Intervals.Day1;
	        intervalDefault = initialInterval;
			intervalEngine = initialInterval;
			intervalChartDisplay = initialInterval;
			intervalChartBar = initialInterval;
			intervalChartUpdate = initialInterval;
            InitializeComponent();
            defaultCombo.DataSource = Enum.GetValues(typeof(BarUnit));
            engineBarsCombo.DataSource = Enum.GetValues(typeof(BarUnit));
            engineBarsCombo2.DataSource = Enum.GetValues(typeof(BarUnit));
            chartDisplayCombo.DataSource = Enum.GetValues(typeof(BarUnit));
            chartBarsCombo.DataSource = Enum.GetValues(typeof(BarUnit));
            chartBarsCombo2.DataSource = Enum.GetValues(typeof(BarUnit));
            chartUpdateCombo.DataSource = Enum.GetValues(typeof(BarUnit));
            IntervalDefaults();
            LoadIntervals();
            IntervalDefaults();
			storageFolder = Factory.Settings["AppDataFolder"];
       		tickZoomEngine = Factory.Settings["TickZoomEngine"];
       		txtSymbol.Text = Factory.Settings["Symbol"];
            Array units = Enum.GetValues(typeof(BarUnit));
            DateTime availableDate = new DateTime(1800,1,1);
            startTimePicker.Value = startTimePicker.MinDate = availableDate;
            startTimePicker.MaxDate = DateTime.Now;
            
            endTimePicker.MinDate = availableDate;
            endTimePicker.Value = endTimePicker.MaxDate = DateTime.Now.AddDays(1).AddSeconds(-1);
       		string startTimeStr = Factory.Settings["startTime"];
       		string EndTimeStr = Factory.Settings["EndTime"];
       		if( startTimeStr != null) {
       			startTime = DateTime.Parse(startTimeStr);
       			startTimePicker.Value = startTime;
       		} else {
       			startTime = availableDate;
       		}
       		if( EndTimeStr != null) {
       			EndTime = DateTime.Parse(EndTimeStr).AddDays(1).AddSeconds(-1);
       			if( EndTime > endTimePicker.MaxDate) {
       				endTimePicker.Value = endTimePicker.MaxDate;
       			} else {
       				endTimePicker.Value = EndTime;
       			}
       		} else {
       			EndTime = DateTime.Now;
       		}
            thread_ProcessMessages = new Thread(new ThreadStart(ProcessMessages));
            thread_ProcessMessages.Name = "ProcessMessages";
            thread_ProcessMessages.Priority = ThreadPriority.Lowest;
            thread_ProcessMessages.IsBackground = true;
            thread_ProcessMessages.Start();
            isInitialized = true;
        }
        
        public void HandlePlugins() {
    		modelLoaderBox.Enabled = true;
        	
			Plugins plugins = Plugins.Instance;
			List<ModelLoader> loaders = plugins.GetLoaders();
			List<string> modelLoaderList = new List<string>();
			for( int i=0; i<loaders.Count; i++) {
				if( loaders[i].IsVisibleInGUI) {
					modelLoaderList.Add(loaders[i].Name);
				}
			}
			modelLoaderBox.Items.AddRange(modelLoaderList.ToArray());
			string value = Factory.Settings["ModelLoader"];
			modelLoaderBox.SelectedItem = value;
        }

		private void TryUpdate(BackgroundWorker bw) {
			if( Factory.AutoUpdate(bw)) {
				ConfigurationSettings.AppSettings["AutoUpdate"] = "false";
				SaveAutoUpdate();
				log.Notice("AutoUpdate succesful. Restart unnecessary.");
			}
        	CheckForEngineInvoke();
		}
		
   		delegate void SetTextCallback(string msg);
   		
        private void Echo(string msg)
        {
            if (logOutput.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(Echo);
                try {
                	this.Invoke(d, new object[] { msg });
                } catch( ObjectDisposedException) {
                	// Any error here can't be logged.
                }
            }
            else
            {
                logOutput.Text += msg + "\r\n";
                logOutput.SelectionStart = logOutput.Text.Length;
                logOutput.ScrollToCaret();
            }
        }
        
        private void ProcessMessages()
        {
        	try {
	            while (!stopMessages)
	            {
	            	if(  log.HasLine) {
		            	try {
			            	Echo(log.ReadLine());
			   			} catch( CollectionTerminatedException) {
			   				break;
		            	}
	            	}
	            	Thread.Sleep(1);
	            }
			} catch( Exception ex) {
				log.Error("ERROR: Thread had an exception:",ex);
			}
        }
   
        List<PortfolioDoc> portfolioDocs = new List<PortfolioDoc>();
        private void btnOptimize_Click(object sender, EventArgs e)
        {
        	RunCommand(0);
        }

        public void RunCommand(int command) {
        	if( !isEngineLoaded) return;
            if (commandWorker.IsBusy)
            {
                MessageBox.Show("A task is already running. Please either the stop the current task or await for its completion before starting a new one.", "Test in progress", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
	            Save();
	            
            	commandWorker.RunWorkerAsync(command);
            }
        }
        
        public void btnStartRun_Click(object sender, System.EventArgs e)
        {
        	RunCommand(1);
        }
        
        public void ShowChartInvoke()
        {
        	try {
	        	Invoke(new MethodInvoker(ShowChart));
        	} catch( Exception ex) {
        		log.Error(ex.Message, ex);
        	}
        }

        public void ShowChart()
        {
        	try {
        		for( int i=portfolioDocs.Count-1; i>=0; i--) {
        			PortfolioDoc portfolioDoc = portfolioDocs[i];
        			if( portfolioDoc.ChartControl.IsDisposed) {
        				portfolioDocs.RemoveAt(i);
        			} else {
        				if( portfolioDoc.ChartControl.IsValid) {
		        			portfolioDoc.Show();
    	    			}
        			}
        		}
        	} catch( Exception ex) {
        		log.Error(ex.Message, ex);
        	}
        }
        
        public Chart CreateChartInvoke()
        {
        	Chart chart = null;
        	try {
        		chart =  (Chart) Invoke(new CreateChartDelegate(CreateChart));
        	} catch( Exception ex) {
        		log.Error(ex.Message, ex);
        	}
        	return chart;
        }
        
        public void CheckForEngineInvoke()
        {
        	try {
        		Invoke(new Action(CheckForEngine));
        	} catch( Exception ex) {
        		log.Error(ex.Message, ex);
        	}
        }
        
        public Chart CreateChart() {
        	Chart chart = null;
        	try {
        		PortfolioDoc portfolioDoc = new PortfolioDoc();
        		portfolioDocs.Add( portfolioDoc);
        		chart = portfolioDoc.ChartControl;
        	} catch( Exception ex) {
        		log.Error(ex.Message, ex);
        	}
        	return chart;
        }
        
        public void FlushChartsInvoke()
        {
        	try {
        		Invoke(new MethodInvoker(FlushCharts));
        	} catch( Exception ex) {
        		log.Error(ex.Message, ex);
        	}
        }
        
        public void FlushCharts() {
        	try {
        		for( int i=portfolioDocs.Count-1; i>=0; i--) {
        			if( portfolioDocs[i].Visible == false) {
        				portfolioDocs.RemoveAt(i);
        			}
        		}
        	} catch( Exception ex) {
        		log.Error(ex.Message, ex);
        	}
        }
        
        public void CloseCharts() {
        	try {
        		for( int i=portfolioDocs.Count-1; i>=0; i--) {
        			portfolioDocs[i].Close();
        			portfolioDocs.RemoveAt(i);
        		}
        	} catch( Exception ex) {
        		log.Error(ex.Message, ex);
        	}
        }
        
        public void RealTimeButtonClick(object sender, System.EventArgs e)
        {
        	RunCommand(2);
        }
        
        public void btnStop_Click(object sender, EventArgs e)
        {
        	StopProcess();
            // Set the progress bar back to 0 and the label
            prgExecute.Value = 0;
            lblProgress.Text = "Execute Stopped";
        }

        void StartTimePickerCloseUp(object sender, EventArgs e)
        {
        	if( startTimePicker.Value >= EndTime ) {
        		startTimePicker.Value = startTime;
			    DialogResult dr = MessageBox.Show("Start date must be less than end date.", "Continue", MessageBoxButtons.OK);
        	} else {
        		startTime = startTimePicker.Value;
        		Save();
        	}        	
        }

        void EndTimePickerCloseUp(object sender, System.EventArgs e)
        {
        	if( endTimePicker.Value <= startTime ) {
        		endTimePicker.Value = EndTime;
			    DialogResult dr = MessageBox.Show("End date must be greater than start date.", "Continue", MessageBoxButtons.OK);
        	} else {
        		endTime = endTimePicker.Value;
        		// Move the EndTime till the last second of the day.
//        		endTime = endTime.AddDays(1).AddSeconds(-1);
        		Save();
        	}
        }
        
        public void Save() {
			// Get the configuration file.
			Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
			
			// Add an entry to appSettings.
			int appStgCnt = ConfigurationManager.AppSettings.Count;
			
			SaveSetting(config,"startTime",startTime.ToLongDateString());
			SaveSetting(config,"EndTime",EndTime.ToLongDateString());
			SaveSetting(config,"Symbol",txtSymbol.Text);
			SaveSetting(config,"UseModelLoader",modelLoaderBox.Enabled ? "true" : "false");
			SaveSetting(config,"ModelLoader",modelLoaderBox.Text);
			SaveSetting(config,"AutoUpdate",Factory.Settings["AutoUpdate"]);
			
			SaveIntervals(config);
			
			// Save the configuration file.
			config.Save(ConfigurationSaveMode.Modified);
			
			// Force a reload of the changed section.
			ConfigurationManager.RefreshSection("appSettings");
        }
        
        public void SaveAutoUpdate() {
			// Get the configuration file.
			Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
			
			// Add an entry to appSettings.
			int appStgCnt = ConfigurationManager.AppSettings.Count;
			
			SaveSetting(config,"AutoUpdate",Factory.Settings["AutoUpdate"]);
			
			config.Save(ConfigurationSaveMode.Modified);
			
			// Force a reload of the changed section.
			ConfigurationManager.RefreshSection("appSettings");
        }
        
        private void SaveSetting(Configuration config, string key, string value) {
        	config.AppSettings.Settings.Remove(key);
			config.AppSettings.Settings.Add(key,value);
        }
        
        void ProcessWorkerDoWork(object sender, DoWorkEventArgs e)
        {
        	BackgroundWorker bw = sender as BackgroundWorker;
            int arg = (int)e.Argument;
            if( log.IsTraceEnabled) log.Trace("Started thread to do work.");
            if( Thread.CurrentThread.Name == null) {
            	Thread.CurrentThread.Name = "ProcessWorker";
            }
            switch( arg) {
            	case 0:
		        	if( !isEngineLoaded) return;
            		Optimize(bw);
            		break;
            	case 1:
		        	if( !isEngineLoaded) return;
            		StartHistorical(bw);
            		break;
            	case 2:
		        	if( !isEngineLoaded) return;
            		RealTime(bw);
            		break;
            	case 3:
		        	if( !isEngineLoaded) return;
            		GeneticOptimize(bw);
            		break;
            	case 4:
            		TryUpdate(bw);
            		break;
            }
        }
        
        public void Optimize(BackgroundWorker bw)
        {
        	Starter starter = new OptimizeStarter();
        	starter.ProjectProperties.Starter.EndTime = (TimeStamp) endTime;
			SetupStarter(starter,bw);
        }
        
        public void GeneticOptimize(BackgroundWorker bw)
        {
        	Starter starter = new GeneticStarter();
        	starter.ProjectProperties.Starter.EndTime = (TimeStamp) endTime;
			SetupStarter(starter,bw);
        }
        
        public void StartHistorical(BackgroundWorker bw)
        {
			int breakAtBar = 0;
    		if( breakAtBarText.Text.Length > 0) {
    			breakAtBar = System.Convert.ToInt32(breakAtBarText.Text,10);
    		}
    		int replaySpeed = 0;
    		if( replaySpeedTextBox.Text.Length > 0) {
    			replaySpeed = System.Convert.ToInt32(replaySpeedTextBox.Text,10);
    		}
    		Starter starter = new HistoricalStarter();
    		starter.ProjectProperties.Engine.TickReplaySpeed = replaySpeed;
			starter.ProjectProperties.Engine.BreakAtBar = breakAtBar;
        	starter.ProjectProperties.Starter.EndTime = (TimeStamp) endTime;
    		SetupStarter(starter,bw);
        }
    		
        private void SetupStarter( Starter starter, BackgroundWorker bw) {
        	FlushChartsInvoke();
    		starter.ProjectProperties.Starter.StartTime = (TimeStamp) startTime;
    		starter.BackgroundWorker = bw;
    		starter.ShowChartCallback = new ShowChartCallback(ShowChartInvoke);
    		starter.CreateChartCallback = new CreateChartCallback(CreateChartInvoke);
	    	starter.ProjectProperties.Chart.ChartType = chartType;
	    	starter.ProjectProperties.Starter.Symbols = txtSymbol.Text;
			starter.ProjectProperties.Starter.IntervalDefault = intervalDefault;
			if( defaultOnly.Checked) {
				starter.ProjectProperties.Chart.IntervalChartDisplay = intervalDefault;
				starter.ProjectProperties.Chart.IntervalChartBar = intervalDefault;
				starter.ProjectProperties.Chart.IntervalChartUpdate = intervalDefault;
			} else {
				starter.ProjectProperties.Chart.IntervalChartDisplay = intervalChartDisplay;
				starter.ProjectProperties.Chart.IntervalChartBar = intervalChartBar;
				starter.ProjectProperties.Chart.IntervalChartUpdate = intervalChartUpdate;
			}
			if( intervalChartDisplay.BarUnit == BarUnit.Default) {
				starter.ProjectProperties.Chart.IntervalChartDisplay = intervalDefault;
			}
			if( intervalChartBar.BarUnit == BarUnit.Default) {
				starter.ProjectProperties.Chart.IntervalChartBar = intervalDefault;
			}
			if( intervalChartUpdate.BarUnit == BarUnit.Default) {
				starter.ProjectProperties.Chart.IntervalChartUpdate = intervalDefault;
			}
			starter.Run(Plugins.Instance.GetLoader(modelLoaderText));
        }
        
		public void RealTime(BackgroundWorker bw)
		{
        	Starter starter = new RealTimeStarter();
    		SetupStarter(starter,bw);
		}
		
		void ProcessWorkerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
        	Progress progress = (Progress) e.UserState;
            // Calculate the task progress in percentages
            if( progress.Final > 0) {
            	PercentProgress = Convert.ToInt32((progress.Current * 100) / progress.Final);
            } else {
            	PercentProgress = 0;
            }
            // Make progress on the progress bar
            prgExecute.Value = Math.Min(100,PercentProgress);
            // Display the current progress on the form
            lblProgress.Text = progress.Text + ": " + progress.Current + " out of " + progress.Final + " (" + PercentProgress + "%)";
        }
        
		private Exception taskException;
		
		public void Catch() {
			if( taskException != null) {
				throw new ApplicationException(taskException.Message,taskException);
			}
		}
		
        void ProcessWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
        	taskException = e.Error;
			if(taskException !=null)
			{ 
				if( taskException.InnerException is TickZoomException) {
					log.Error(taskException.InnerException.Message,taskException.InnerException);
				} else {
					log.Error(taskException.Message,taskException);
				}
			}
        }
        
        void StopProcess() {
        	commandWorker.CancelAsync();
        	if( isEngineLoaded) {
	        	Api.Factory.Engine.TickEngine.Close();
        	}
        	
			TickReader.CloseAll();
        }

        void Terminate() {
            stopMessages = true;
            while( thread_ProcessMessages.IsAlive) {
            	Application.DoEvents();
            }
        	StopProcess();
        	CloseCharts();
            commandWorker.CancelAsync();
            Api.Factory.Engine.TickEngine.Close();
            Factory.Engine.Release();
            Factory.Provider.Release();
            TickReader.CloseAll();
        }
        
        private bool stopMessages = false;
        
        void Form1FormClosing(object sender, FormClosingEventArgs e)
        {
        	Terminate();
        }
        
        
        void Form1Load(object sender, EventArgs e)
        {
        }
        
        void DefaultOnlyClick(object sender, EventArgs e)
        {
        	UpdateCheckBoxes();
        }
        
		private void IntervalsUpdate() {
			intervalDefault = Factory.Engine.DefineInterval((BarUnit)defaultCombo.SelectedValue, Convert.ToDouble(defaultBox.Text));
			intervalEngine = Factory.Engine.DefineInterval((BarUnit)engineBarsCombo.SelectedValue, Convert.ToDouble(engineBarsBox.Text));
			if( engineRollingCheckBox.Checked) {
				intervalEngine= Factory.Engine.DefineInterval((BarUnit)engineBarsCombo.SelectedValue, Convert.ToDouble(engineBarsBox.Text),
											(BarUnit)engineBarsCombo2.SelectedValue, Convert.ToDouble(engineBarsBox2.Text));
			} else {
				intervalChartDisplay = Factory.Engine.DefineInterval((BarUnit)chartDisplayCombo.SelectedValue, Convert.ToDouble(chartDisplayBox.Text));
			}
			if( chartBarsCheckBox.Checked ) {
				intervalChartBar = Factory.Engine.DefineInterval((BarUnit)chartBarsCombo.SelectedValue, Convert.ToDouble(chartBarsBox.Text),
			                                      (BarUnit)chartBarsCombo2.SelectedValue, Convert.ToDouble(chartBarsBox2.Text));
			} else {
				intervalChartBar = Factory.Engine.DefineInterval((BarUnit)chartBarsCombo.SelectedValue, Convert.ToDouble(chartBarsBox.Text));
			}
			intervalChartUpdate = Factory.Engine.DefineInterval((BarUnit)chartUpdateCombo.SelectedValue, Convert.ToDouble(chartUpdateBox.Text));
        }

		private void IntervalDefaults() {
			if( defaultBox.Text.Length == 0) { defaultBox.Text = "1"; }
			if( engineBarsBox.Text.Length == 0) { engineBarsBox.Text = "1"; }
			if( engineBarsBox2.Text.Length == 0) { engineBarsBox2.Text = "1"; }
			if( chartDisplayBox.Text.Length == 0) { chartDisplayBox.Text = "1"; }
			if( chartBarsBox.Text.Length == 0) { chartBarsBox.Text = "1"; }
			if( chartBarsBox2.Text.Length == 0) { chartBarsBox2.Text = "1"; }
			if( chartUpdateBox.Text.Length == 0) { chartUpdateBox.Text = "1"; }
			if( defaultCombo.Text == "None" || defaultCombo.Text.Length == 0) { 
				defaultCombo.Text = "Hour";
			}
			if( engineBarsCombo.Text == "None" || engineBarsCombo.Text.Length == 0 ) { 
				engineBarsCombo.Text = "Hour";
			}
			if( engineBarsCombo2.Text == "None" || engineBarsCombo2.Text.Length == 0 ) { 
				engineBarsCombo2.Text = "Hour";
			}
			if( chartDisplayCombo.Text == "None" || chartDisplayCombo.Text.Length == 0 ) { 
				chartDisplayCombo.Text = "Hour";
			}
			if( chartBarsCombo.Text == "None" || chartBarsCombo.Text.Length == 0 ) { 
				chartBarsCombo.Text = "Hour";
			}
			if( chartBarsCombo2.Text == "None" || chartBarsCombo2.Text.Length == 0 ) { 
				chartBarsCombo2.Text = "Hour";
			}
			if( chartUpdateCombo.Text == "None" || chartUpdateCombo.Text.Length == 0 ) { 
				chartUpdateCombo.Text = "Hour";
			}
//           	IntervalsUpdate();
//           	UpdateCheckBoxes();
		}
        
        void CopyDefaultIntervals() {
       		engineBarsBox.Text = defaultBox.Text;
       		engineBarsBox2.Text = defaultBox.Text;
       		engineBarsCombo.Text = defaultCombo.Text;
       		engineBarsCombo2.Text = defaultCombo.Text;
       		chartDisplayBox.Text = defaultBox.Text;
       		chartDisplayCombo.Text = defaultCombo.Text;
       		chartBarsBox.Text = defaultBox.Text;
       		chartBarsCombo.Text = defaultCombo.Text;
       		chartBarsBox2.Text = defaultBox.Text;
       		chartBarsCombo2.Text = defaultCombo.Text;
       		chartUpdateBox.Text = defaultBox.Text;
       		chartUpdateCombo.Text = defaultCombo.Text;
        }
		
		void SaveIntervals(Configuration config) {
			SaveSetting(config,"defaultBox",defaultBox.Text);
			SaveSetting(config,"defaultCombo",defaultCombo.Text);
			SaveSetting(config,"engineBarsBox",engineBarsBox.Text);
			SaveSetting(config,"engineBarsBox2",engineBarsBox2.Text);
			SaveSetting(config,"engineBarsCombo",engineBarsCombo.Text);
			SaveSetting(config,"engineBarsCombo2",engineBarsCombo2.Text);
			SaveSetting(config,"chartDisplayBox",chartDisplayBox.Text);
			SaveSetting(config,"chartDisplayCombo",chartDisplayCombo.Text);
			SaveSetting(config,"chartBarsBox",chartBarsBox.Text);
			SaveSetting(config,"chartBarsCombo",chartBarsCombo.Text);
			SaveSetting(config,"chartBarsBox2",chartBarsBox2.Text);
			SaveSetting(config,"chartBarsCombo2",chartBarsCombo2.Text);
			SaveSetting(config,"chartUpdateBox",chartUpdateBox.Text);
			SaveSetting(config,"chartUpdateCombo",chartUpdateCombo.Text);
        }

		void LoadIntervals() {
			defaultBox.Text = CheckNull(Factory.Settings["defaultBox"]);
			defaultCombo.Text = CheckNull(Factory.Settings["defaultCombo"]);
			engineBarsBox.Text = CheckNull(Factory.Settings["engineBarsBox"]);
			engineBarsBox2.Text = CheckNull(Factory.Settings["engineBarsBox2"]);
			engineBarsCombo.Text = CheckNull(Factory.Settings["engineBarsCombo"]);
			engineBarsCombo2.Text = CheckNull(Factory.Settings["engineBarsCombo2"]);
			chartDisplayBox.Text = CheckNull(Factory.Settings["chartDisplayBox"]);
			chartDisplayCombo.Text = CheckNull(Factory.Settings["chartDisplayCombo"]);
			chartBarsBox.Text = CheckNull(Factory.Settings["chartBarsBox"]);
			chartBarsCombo.Text = CheckNull(Factory.Settings["chartBarsCombo"]);
			chartBarsBox2.Text = CheckNull(Factory.Settings["chartBarsBox2"]);
			chartBarsCombo2.Text = CheckNull(Factory.Settings["chartBarsCombo2"]);
			chartUpdateBox.Text = CheckNull(Factory.Settings["chartUpdateBox"]);
			chartUpdateCombo.Text = CheckNull(Factory.Settings["chartUpdateCombo"]);
        }
		
		string CheckNull(string str) {
			if( str == null) { 
				return "";
			} else {
				return str;
			}
		}
        
        void CopyDebugCheckBoxClick(object sender, EventArgs e)
        {
        	CheckBox cb = (CheckBox) sender;
        	cb.Checked = false;
        	CopyDefaultIntervals();
        }
        
        void IntervalChange(object sender, EventArgs e)
        {
            if( isInitialized) IntervalsUpdate();
        }
        
        void EngineRollingCheckBoxClick(object sender, EventArgs e)
        {
        	UpdateCheckBoxes();
        }
        
        void UpdateCheckBoxes() {
       		engineBarsBox.Enabled = !defaultOnly.Checked;
       		engineBarsBox2.Enabled = !defaultOnly.Checked;
       		engineBarsCombo.Enabled = !defaultOnly.Checked;
       		engineBarsCombo2.Enabled = !defaultOnly.Checked;
       		chartDisplayBox.Enabled = !defaultOnly.Checked;
       		chartDisplayCombo.Enabled = !defaultOnly.Checked;
       		chartBarsBox.Enabled = !defaultOnly.Checked;
       		chartBarsCombo.Enabled = !defaultOnly.Checked;
       		chartBarsBox2.Enabled = !defaultOnly.Checked;
       		chartBarsCombo2.Enabled = !defaultOnly.Checked;
       		chartUpdateBox.Enabled = !defaultOnly.Checked;
       		chartUpdateCombo.Enabled = !defaultOnly.Checked;
       		
       		// Set rolling last.
       		engineBarsBox2.Enabled = engineRollingCheckBox.Checked;
       		engineBarsCombo2.Enabled = engineRollingCheckBox.Checked;
       		
       		// Set rolling last.
       		chartBarsBox2.Enabled = chartBarsCheckBox.Checked;
       		chartBarsCombo2.Enabled = chartBarsCheckBox.Checked;
        }
        
        ChartType chartType = ChartType.Bar;
        void ChartRadioClick(object sender, EventArgs e)
        {
        	if( barChartRadio.Checked) {
        		chartType = ChartType.Bar;		
        	} else {
        		chartType = ChartType.Time;		
        	}
        }
        
        void BtnGeneticClick(object sender, EventArgs e)
        {
        	RunCommand(3);
        }
        
        string modelLoaderText = null;
        void ModelLoaderBoxSelectedIndexChanged(object sender, EventArgs e)
        {
        	modelLoaderText = modelLoaderBox.Text;
        }
        
        private bool isEngineLoaded = false;
        
		public bool IsEngineLoaded {
			get { return isEngineLoaded; }
		}
        
        void Form1Shown(object sender, EventArgs e)
        {
   			string autoUpdateFlag = Factory.Settings["AutoUpdate"];
   			string runUpdate = Factory.Settings["RunUpdate"];
   			if( "true".Equals(autoUpdateFlag) && "true".Equals(runUpdate)) {
	           	commandWorker.RunWorkerAsync(4);
   		    } else {
				log.Notice("To enable AutoUpdate, set AutoUpdate and RunUpdate to 'true'");
				CheckForEngineInvoke();
   			}
        }
        
        public void CheckForEngine() {
   			try {
	   			TickEngine engine = Factory.Engine.TickEngine;
	   			isEngineLoaded = true;
   			} catch( Exception ex) {
   				string msg = "Engine load failed, see log for further detail: " + ex.Message;
   				log.Info(msg,ex);
   			}
   			if( isEngineLoaded) {
	        	initialInterval = Factory.Engine.DefineInterval(BarUnit.Day,1);
				IntervalsUpdate();
	            HandlePlugins();
        	}
    	}
        
		public List<PortfolioDoc> PortfolioDocs {
			get { return portfolioDocs; }
		}
        
		public System.Windows.Forms.TextBox LogOutput {
			get { return logOutput; }
		}
   }
}
